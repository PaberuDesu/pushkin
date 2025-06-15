using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WhiteAndBlackLists : MonoBehaviour
{
    [SerializeField] private TMP_Text limiterNote;
    [SerializeField] private GameObject settings;
    [SerializeField] private TMP_Dropdown dropdown;

    private readonly HashSet<Place> defaultPlaces = new();
    private readonly HashSet<Place> whiteList = new();
    private readonly HashSet<Place> blackList = new();
    private Place hotel;
    public int WhiteListCount => whiteList.Count;
    public int NotBlackListCount => whiteList.Union(defaultPlaces).ToList().Count;
    private List<int> Indices(HashSet<Place> list) => list.Select(p => p.Id).ToList();
    public List<int> WhiteIndices => Indices(whiteList);
    public List<int> DefaultIndices => Indices(defaultPlaces);

    private Place currentPlace;
    private int lastValue = 0;

    private void Start()
    {
        foreach (Place place in Graph.Places)
        {
            if (place.IsHotel) hotel = place;
            else AddToList(place, 0);
        }
        VisitHotel(false);
    }

    public void Initialize(Place place)
    {
        bool isHotel = place.IsHotel;
        settings.SetActive(!isHotel);
        if (isHotel) return;
        currentPlace = place;
        lastValue = whiteList.Contains(place) ? 1
            : (blackList.Contains(place) ? 2 : 0);
        dropdown.value = lastValue;
    }

    public void MoveToList(int listId)
    {
        if (listId == lastValue) return;
        if (!RemovePlaceFromAll()) return;
        lastValue = listId;
        AddToList(currentPlace, listId);
        int limit = NotBlackListCount;
        if (limit > 1)
            limiterNote.text = $"Введите число от 1 до {NotBlackListCount}";
        else if (limit == 1)
            limiterNote.text = $"Можно посетить только 1 место";
        else limiterNote.text = $"Все места исключены из маршрута";
    }

    public void VisitHotel(bool needTo)
    {
        if (needTo)
            hotel.markerRect.GetComponent<Image>().color = VisualSettings.WhiteListColor;
        else hotel.markerRect.GetComponent<Image>().color = VisualSettings.BlackListColor;
    }

    private void AddToList(Place place, int listId)
    {
        switch (listId)
        {
            case 1:
                whiteList.Add(place);
                place.markerRect.GetComponent<Image>().color = VisualSettings.WhiteListColor;
                break;
            case 2:
                blackList.Add(place);
                place.markerRect.GetComponent<Image>().color = VisualSettings.BlackListColor;
                break;
            default:
                defaultPlaces.Add(place);
                place.markerRect.GetComponent<Image>().color = VisualSettings.DefaultListColor;
                break;
        }
    }

    private bool RemovePlaceFromAll()
    {
        bool removedFromDefault = defaultPlaces.Remove(currentPlace);
        bool removedFromWhiteList = whiteList.Remove(currentPlace);
        bool removedFromBlackList = blackList.Remove(currentPlace);
        return removedFromDefault || removedFromWhiteList || removedFromBlackList;
    }
}
