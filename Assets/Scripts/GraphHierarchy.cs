using UnityEngine;

public class GraphHierarchy : MonoBehaviour
{
    [SerializeField] private Transform edgesPack;
    [SerializeField] private Transform edgesRoutePack;
    [SerializeField] private Transform verticesPack;

    public Transform EdgesPack => edgesPack;
    public Transform EdgesRoutePack => edgesRoutePack;
    public Transform VerticesPack => verticesPack;

    public Transform ParentForEdge(bool isOnPath) => isOnPath ? edgesRoutePack : edgesPack;
}
