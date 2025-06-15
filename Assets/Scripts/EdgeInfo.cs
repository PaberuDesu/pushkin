using UnityEngine;

public class EdgeInfo : MonoBehaviour
{
    public int FromIndex { get; private set; }
    public int ToIndex { get; private set; }

    public void SetInfo(int from, int to)
    {
        FromIndex = from;
        ToIndex = to;
    }
}
