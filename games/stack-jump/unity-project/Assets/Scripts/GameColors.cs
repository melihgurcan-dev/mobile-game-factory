using UnityEngine;

/// <summary>
/// Provides a cycling color palette for the stack blocks.
/// Colors become progressively more saturated as the stack grows.
/// </summary>
public static class GameColors
{
    // Flat UI color palette — cycles through as blocks stack
    private static readonly Color[] palette =
    {
        new Color(0.95f, 0.26f, 0.21f),  // Material Red 500
        new Color(1.00f, 0.60f, 0.00f),  // Material Orange 600
        new Color(1.00f, 0.92f, 0.23f),  // Material Yellow 600
        new Color(0.30f, 0.69f, 0.31f),  // Material Green 500
        new Color(0.13f, 0.59f, 0.95f),  // Material Blue 500
        new Color(0.61f, 0.15f, 0.69f),  // Material Purple 600
        new Color(0.00f, 0.74f, 0.83f),  // Material Cyan 500
        new Color(0.99f, 0.55f, 0.67f),  // Material Pink 300
        new Color(0.00f, 0.59f, 0.53f),  // Material Teal 600
        new Color(1.00f, 0.76f, 0.03f),  // Material Amber 500
    };

    public static Color GetColor(int index)
    {
        return palette[Mathf.Abs(index) % palette.Length];
    }

    /// <summary>
    /// Returns a slightly lighter version of the color — useful for perfect placement feedback.
    /// </summary>
    public static Color GetBrightColor(int index)
    {
        Color c = GetColor(index);
        return new Color(
            Mathf.Min(c.r + 0.15f, 1f),
            Mathf.Min(c.g + 0.15f, 1f),
            Mathf.Min(c.b + 0.15f, 1f)
        );
    }
}
