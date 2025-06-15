using UnityEngine;

public static class VisualSettings
{
    private static readonly Color disabledEdgeColor = new Color(0.156f, 0.156f, 0.156f, 1f);
    private static readonly Color disabledVertexColor = new Color(0.388f, 0.624f, 0.537f, 1f);
    private static readonly Color enabledColor = new Color(1f, 0.749f, 0.627f, 1f);
    public static Color DefaultEdgeColor => disabledEdgeColor;
    public static Color DefaultVertexColor => disabledVertexColor;
    public static Color VertexColor(bool isOnPath) => isOnPath ? enabledColor : disabledVertexColor;
    public static Color EdgesColor(bool isOnPath) => isOnPath ? enabledColor : disabledEdgeColor;

    public static Color DefaultListColor { get; } = new Color(1.000f, 1.000f, 1.000f, 1.000f);
    public static Color WhiteListColor { get; } = new Color(0.129f, 1.000f, 0.200f, 1.000f);
    public static Color BlackListColor { get; } = new Color(0.894f, 0.325f, 0.325f, 1.000f);
}
