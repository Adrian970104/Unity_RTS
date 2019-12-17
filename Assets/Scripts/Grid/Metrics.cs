using UnityEngine;

public static class Metrics
{
    public const float SideLength = 5f;

    public const int MaxGridSize = 9999;

    public const float GridCellTerrainGap = 0.1f;
    
    
    /**
     *             (0,sL)--------(sL,sL)
     *               1              2
     *               |              |
     *               |              |
     *               |              |
     *               0              3
     *             (0,0)----------(0,sL)
     */
    public static readonly Vector3[] Corners = {
        new Vector3(0f,         0f,    0f),
        new Vector3(0f,         0f,    SideLength),
        new Vector3(SideLength, 0f,    SideLength),
        new Vector3(SideLength, 0f,    0f)
    };
}
