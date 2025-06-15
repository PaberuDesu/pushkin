using System.Collections.Generic;
using UnityEngine;

public class Place
{
    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public bool IsHotel => Id == 10;

    private CoordinateValues Coordinates { get; }
    public Vector2 Position(Vector2 mapSize) => Coordinates.Convert(mapSize);

    public RectTransform markerRect;
    public List<Sprite> Images { get; private set; }

    public Place(int place_id, PlaceInfo info)
    {
        Id = place_id;
        Name = info.name;
        Description = info.description;
        Coordinates = new CoordinateValues(info.longitude, info.latitude);

        LoadImages();
    }

    private void LoadImages()
    {
        string resourcePath = $"Places/Place {Id}";

        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(resourcePath);

        if (loadedSprites != null && loadedSprites.Length > 0)
            Images = new(loadedSprites);
        else
        {
            Debug.LogWarning($"No images found for place {Id} at path: {resourcePath}");
            Images = new();
        }
    }

    public static (List<Place> places, float[,] distances) LoadFromJSON(TextAsset jsonFile)
    {
        List<Place> places = new();
        float[,] distances = new float[0,0];

        try
        {
            PlaceDataWrapper wrapper = JsonUtility.FromJson<PlaceDataWrapper>(jsonFile.text);

            for (int i = 0; i < wrapper.places.Count; i++)
                places.Add(new Place(i, wrapper.places[i]));

            int rowCount = wrapper.distances.Count;
            int colCount = wrapper.distances[0].row.Count;

            distances = new float[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                    distances[i, j] = wrapper.distances[i].row[j];
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parsing error: {e.Message}");
        }
        
        return (places, distances);
    }
}

internal class CoordinateValues
{
    private Vector2 position;

    private static readonly Vector2 min = new Vector2(28.843224059205046f, 57.01372724371663f);
    private static readonly Vector2 magnitude = new Vector2(0.13515415548085f, 0.07413042302159f);

    internal CoordinateValues(float lon, float lat) { position = new(lon, lat); }

    internal Vector2 Convert(Vector2 mapSize) => ((position - min) / magnitude - 0.5f * Vector2.one) * mapSize;
}

[System.Serializable]
public class PlaceInfo
{
    public string name;
    public string description;
    public float latitude;
    public float longitude;
}

[System.Serializable]
public class DistancesRow
{
    public List<float> row;
}

[System.Serializable]
public class PlaceDataWrapper
{
    public List<PlaceInfo> places;
    public List<DistancesRow> distances;
}
