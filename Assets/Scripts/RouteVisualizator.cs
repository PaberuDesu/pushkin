using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouteVisualizator : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GraphHierarchy graphHierarchy;

    public void ShowError(string message) => resultText.text = message;

    public void UpdateRoute(List<Place> currentRoute, float distance, bool isHotelNotRecommended)
    {
        for (int i = 0; i < currentRoute.Count - 1; i++)
        {
            int from = currentRoute[i].Id;
            int to = currentRoute[i + 1].Id;
            int min = Mathf.Min(from, to);
            int max = Mathf.Max(from, to);

            if (Graph.EdgeDictionary.TryGetValue((min, max), out GameObject line))
                SetLineState(line.transform, true);
        }
        SetAllVertexStates(currentRoute, true);

        resultText.text = $"Оптимальный маршрут:\n{string.Join(" → ", currentRoute.Select(place => place.Name))}\n\nОбщая протяженность: {distance:F2} км." +
            (isHotelNotRecommended ? "\n\nПри таком коротком маршруте гостиница может быть не нужна." : "");
    }

    public void ClearRoute()
    {
        while (graphHierarchy.EdgesRoutePack.childCount > 0)
            SetLineState(graphHierarchy.EdgesRoutePack.GetChild(0), false);
        SetAllVertexStates(Graph.Places as List<Place>, false);
    }

    private void SetAllVertexStates(List<Place> places, bool isOnPath)
    {
        foreach (Place place in places)
            place.markerRect.GetChild(0).GetComponent<Image>().color =
                VisualSettings.VertexColor(isOnPath);
    }

    private void SetLineState(Transform lineObjRect, bool isOnPath)
    {
        lineObjRect.SetParent(graphHierarchy.ParentForEdge(isOnPath));
        RectTransform lineRect = lineObjRect.GetChild(0) as RectTransform;
        lineRect.GetComponent<Image>().color = VisualSettings.EdgesColor(isOnPath);
        lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, isOnPath ? 5 : 2);
    }
}
