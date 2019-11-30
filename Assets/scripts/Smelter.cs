using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smelter 
{

    static List<GameObject> smelters = new List<GameObject>();
    static List<int[]> smelterLocations = new List<int[]>(); 
    static Vector2[,] mapPositionMatrix; 

    public static void Init(int[] smelterLoc, Vector2[,] mapPosMat) {
        mapPositionMatrix = mapPosMat;
        smelterLocations.Add(smelterLoc);

        int smelterX = smelterLoc[0];
        int smelterY = smelterLoc[1];

        Sprite portalSprite = Resources.Load<Sprite>("LevelItems/smelter");
        GameObject smelter = new GameObject("smelter_" + smelterLocations.Count + ": " + smelterX + "," + smelterY);
        smelter.AddComponent<SpriteRenderer>();
        smelter.GetComponent<SpriteRenderer>().sprite = portalSprite;
        smelter.transform.position = new Vector3(
        mapPositionMatrix[smelterX, smelterY].x, mapPositionMatrix[smelterX, smelterY].y, -2.5f);
        smelters.Add(smelter);

    }
}
