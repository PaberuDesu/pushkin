using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private GameObject markerPlacePrefab;
    [SerializeField] private GameObject markerHotelPrefab;
    [SerializeField] private GameObject linePrefab;
    [Space(20)]
    [SerializeField] private RectTransform map;
    [SerializeField] private GraphHierarchy graphHierarchy;
    [SerializeField] private Toggle nameShowToggle;
    [Space(20)]
    [SerializeField] TextAsset jsonFile;

    private static List<Place> places = new();
    public static IReadOnlyList<Place> Places => places;
    public static float[,] Distances { get; private set; }
    private static readonly Dictionary<(int, int), GameObject> edgeDictionary = new();
    public static IReadOnlyDictionary<(int, int), GameObject> EdgeDictionary => edgeDictionary;

    private void Awake()
    {
        (places, Distances) = Place.LoadFromJSON(jsonFile);
        CreateMarkers();
        CreateLines();
    }

    private void CreateMarkers()
    {
        foreach (Place place in places)
        {
            RectTransform pointRect = Instantiate(place.IsHotel ? markerHotelPrefab : markerPlacePrefab,
                graphHierarchy.VerticesPack).transform as RectTransform;
                
            pointRect.anchoredPosition = place.Position(map.rect.size);
            GameObject pointText = pointRect.GetChild(1).gameObject;
            pointText.GetComponent<TMP_Text>().text = place.Name;
            nameShowToggle.onValueChanged.AddListener((v) => { pointText.SetActive(v); });

            pointRect.GetChild(0).GetComponent<Image>().color = VisualSettings.DefaultVertexColor;

            place.markerRect = pointRect;
        }
    }

    private void CreateLines()
    {
        for (int fromIndex = 0; fromIndex < places.Count; fromIndex++)
            for (int toIndex = fromIndex + 1; toIndex < places.Count; toIndex++)
            {
                GameObject line = Instantiate(linePrefab, graphHierarchy.EdgesPack);
                line.transform.GetChild(0).GetComponent<Image>().color = VisualSettings.DefaultEdgeColor;

                EdgeInfo edgeInfo = line.AddComponent<EdgeInfo>();
                edgeInfo.SetInfo(fromIndex, toIndex);
                SetLineOnMap(line, edgeInfo, true);

                int min = Mathf.Min(fromIndex, toIndex);
                int max = Mathf.Max(fromIndex, toIndex);
                edgeDictionary[(min, max)] = line;
            }
    }

    private void SetLineOnMap(GameObject line, EdgeInfo edgeInfo, bool isNew)
    {
        Vector2 pos1 = places[edgeInfo.FromIndex].markerRect.anchoredPosition;
        Vector2 pos2 = places[edgeInfo.ToIndex].markerRect.anchoredPosition;

        RectTransform lineObjRect = line.transform as RectTransform;
        Vector2 dir = pos2 - pos1;

        lineObjRect.anchoredPosition = (pos1 + pos2) / 2;
        RectTransform lineRect = lineObjRect.GetChild(0) as RectTransform;
        lineRect.sizeDelta = new Vector2(dir.magnitude, isNew ? 2 : lineRect.sizeDelta.y);
        lineRect.localEulerAngles = Vector3.forward * Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineObjRect.GetChild(1).GetComponent<TMP_Text>().text = Distances[edgeInfo.FromIndex, edgeInfo.ToIndex].ToString();
    }

    public void Zoom(float rescaler)
    {
        foreach (Place place in places)
            place.markerRect.localScale = Vector2.one / Mathf.Pow(rescaler, 0.8f);
        foreach (var edge in edgeDictionary)
        {
            Transform lineObjRect = edge.Value.transform;
            lineObjRect.GetChild(0).localScale = new Vector2(1, 1 / rescaler);
            lineObjRect.GetChild(1).localScale = Vector2.one / rescaler;
        }
    }
}
