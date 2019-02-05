using System;
using UnityEngine;

public class GridPoint
{
    public int X, Y;

    public GridPoint(int x, int y)
    {
        X = x; Y = y;
    }

    public GridPoint(float x, float y)
    {
        X = (int)x; Y = (int)y;
    }

    public GridPoint(Vector3 v3)
    {
        X = (int)v3.x; Y = (int)v3.y;
    }

}
