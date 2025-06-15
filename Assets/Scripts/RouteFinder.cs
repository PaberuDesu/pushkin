using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouteFinder : MonoBehaviour
{
    [SerializeField] private Toggle hotelToggle;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private WhiteAndBlackLists lists;
    [SerializeField] private RouteVisualizator visualizator;

    private readonly Dictionary<string, (List<int> route, float distance)> tspCache = new();

    public void CalculateOnSettingNumber()
    {
        if (SettingsManager.SearchOnSettingNumber) CalculateRoute();
    }
    public void CalculateOnSettingHotelUsing()
    {
        if (SettingsManager.SearchOnSettingHotelUsing) CalculateRoute();
    }
    public void CalculateOnChangingWhiteBlackLists()
    {
        if (SettingsManager.SearchOnChangingWhiteBlackLists) CalculateRoute();
    }

    public void CalculateRoute()
    {
        visualizator.ClearRoute();

        bool includeHotel = hotelToggle.isOn;
        int availableCount = lists.NotBlackListCount;

        // Обработка случая, когда нет доступных мест
        if (availableCount == 0)
        {
            visualizator.ShowError(includeHotel ?
                "Маршрут содержит только гостиницу" : "Нет доступных мест для посещения");
            return;
        }

        // Валидация
        if (!int.TryParse(inputField.text, out int k_user) || k_user < 1 || k_user > availableCount)
        {
            if (availableCount > 1)
                visualizator.ShowError($"Введите число от 1 до {availableCount}");
            else visualizator.ShowError($"Можно посетить только 1 место");
            return;
        }

        // Маршрут из одной точки
        if (k_user == 1 && !includeHotel)
        {
            visualizator.ShowError("Посетите любое место. Длина маршрута: 0 км.");
            return;
        }

        // Маршрут по белому списку
        bool useOnlyWhiteList = false;
        if (lists.WhiteListCount >= k_user)
        {
            useOnlyWhiteList = true;
            k_user = Mathf.Min(lists.WhiteListCount, k_user);
        }

        // Формирование базового набора индексов
        List<int> whiteIndices = lists.WhiteIndices;
        List<int> defaultIndices = lists.DefaultIndices;
        int neededFromDefault = useOnlyWhiteList ? 0 : k_user - lists.WhiteListCount;

        // Поиск оптимального маршрута
        float minDistance = float.MaxValue;
        IEnumerable<List<int>> combinations;

        if (useOnlyWhiteList)
            combinations = GetKCombinations(whiteIndices, k_user);
        else
            combinations = GetKCombinations(defaultIndices, neededFromDefault)
                .Select(comb => whiteIndices.Concat(comb).ToList());

        List<Place> currentRoute = new();

        foreach (List<int> currentCombination in combinations)
        {
            if (includeHotel)
                currentCombination.Add(10);

            string cacheKey = $"{string.Join(",", currentCombination.OrderBy(x => x))}";

            if (!tspCache.TryGetValue(cacheKey, out (List<int> route, float dist) result))
            {
                result = SolveTSP(currentCombination);
                tspCache[cacheKey] = result;
            }

            if (result.route != null && result.dist < minDistance)
            {
                currentRoute = result.route.Select(id => Graph.Places[id]).ToList();
                minDistance = result.dist;
            }
        }

        // Визуализация результатов
        bool isHotelNotRecommended = includeHotel && k_user <= 5;
        visualizator.UpdateRoute(currentRoute, minDistance, isHotelNotRecommended);
    }

    private (List<int> route, float distance) SolveTSP(List<int> subset)
    {
        int n = subset.Count;
        int fullMask = (1 << n) - 1;
        string cacheKey = $"{string.Join(",", subset.OrderBy(x => x))}";

        if (tspCache.TryGetValue(cacheKey, out var cached))
            return cached;

        float[,] dp = new float[fullMask + 1, n];
        int[,] parent = new int[fullMask + 1, n];

        for (int mask = 0; mask <= fullMask; mask++)
            for (int i = 0; i < n; i++)
            {
                dp[mask, i] = float.MaxValue;
                parent[mask, i] = -1;
            }

        for (int i = 0; i < n; i++)
            dp[1 << i, i] = 0;

        for (int mask = 0; mask <= fullMask; mask++)
            for (int i = 0; i < n; i++)
                if ((mask & (1 << i)) != 0)
                    for (int j = 0; j < n; j++)
                        if ((mask & (1 << j)) == 0)
                        {
                            int newMask = mask | (1 << j);
                            float newDist = dp[mask, i] + Graph.Distances[subset[i], subset[j]];

                            if (newDist < dp[newMask, j])
                            {
                                dp[newMask, j] = newDist;
                                parent[newMask, j] = i;
                            }
                        }

        float minDist = float.MaxValue;
        int endVertex = -1;

        for (int i = 0; i < n; i++)
            if (dp[fullMask, i] < minDist)
            {
                minDist = dp[fullMask, i];
                endVertex = i;
            }

        if (endVertex == -1) return (null, minDist);

        List<int> route = new();
        int cur = endVertex;
        int maskState = fullMask;

        while (maskState != 0)
        {
            route.Add(subset[cur]);
            int prev = parent[maskState, cur];
            maskState &= ~(1 << cur);
            cur = prev;
        }

        var result = (route, minDist);
        tspCache[cacheKey] = result;
        return result;
    }

    private IEnumerable<List<int>> GetKCombinations(List<int> list, int k)
    {
        if (k == 0)
        {
            yield return new List<int>();
            yield break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            int current = list[i];
            foreach (var comb in GetKCombinations(list.Skip(i + 1).ToList(), k - 1))
            {
                comb.Insert(0, current);
                yield return comb;
            }
        }
    }
}
