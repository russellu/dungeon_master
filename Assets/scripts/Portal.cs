using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal 
{
    static List<GameObject> portals = new List<GameObject>();
    public static List<int[]> portalLocations = new List<int[]>();
    static List<bool> portalOpens = new List<bool>(); 
    static List<float> nextCreatureTimes = new List<float>();
    static List<float> currentTimeCounts = new List<float>();
    static List<int> portalPolaritys = new List<int>(); 
    static Vector2[,] mapPositionMatrix;
    static string[] portalNames = new string[] { "LevelItems/portal", "LevelItems/enemy_portal" };
    static string[] openPortalNames = new string[] { "LevelItems/portal_open", "LevelItems/enemy_portal_open" };

    public static void Init(int polarity, int[] portalLoc, Vector2[,] mapPosMat) 
    {
        mapPositionMatrix = mapPosMat;
        portalLocations.Add(portalLoc);
        portalPolaritys.Add(polarity);
        portalOpens.Add(false);
        nextCreatureTimes.Add(5);
        currentTimeCounts.Add(0); 


        int portalX = portalLoc[0];
        int portalY = portalLoc[1];

        Sprite portalSprite = Resources.Load<Sprite>(portalNames[polarity]);
        GameObject portal = new GameObject("portal_" + portalLocations.Count + ": " + portalX + "," + portalY);
        portal.AddComponent<SpriteRenderer>();
        portal.GetComponent<SpriteRenderer>().sprite = portalSprite;
        portal.transform.position = new Vector3(
        mapPositionMatrix[portalX, portalY].x, mapPositionMatrix[portalX, portalY].y, -3.5f);
        portals.Add(portal); 


    }

    public static void Update(float deltaTime,CreatureManager creatureManager)
    {
        //Debug.Log("deltaTime = " + deltaTime);
        for (int i = 0; i < portals.Count; i++)
        {
            if (portalOpens[i])
            {
                currentTimeCounts[i] += deltaTime;
                if (currentTimeCounts[i] > nextCreatureTimes[i])
                {
                    currentTimeCounts[i] = 0;
                    Vector2 portalLoc = mapPositionMatrix[portalLocations[i][0], portalLocations[i][1]];
                    creatureManager.Spawn(portalLoc,portalPolaritys[i]);
                    Main.audioSource.PlayOneShot(Main.gruntSpawn, 1f);
                }
            }
        }
    }

    public static void CheckUnveiled(int x, int y)
    {
        for (int i = 0; i < portalLocations.Count; i++)
            if (portalLocations[i][0] == x && portalLocations[i][1] == y)
                UpdatePortalSprite(i);
    }

    public static void UpdatePortalSprite(int updateIndex)
    {
        //Debug.Log("updating portal sprite  ");

        int portalX = portalLocations[updateIndex][0];
        int portalY = portalLocations[updateIndex][1];
        Sprite portalSprite = Resources.Load<Sprite>(openPortalNames[portalPolaritys[updateIndex]]);
        portals[updateIndex].GetComponent<SpriteRenderer>().sprite = portalSprite;
        portals[updateIndex].transform.position = new Vector3(
            mapPositionMatrix[portalX, portalY].x, mapPositionMatrix[portalX, portalY].y, -3.5f);

        Main.audioSource.PlayOneShot(Main.portal, 1f);
        portalOpens[updateIndex] = true;

    }
}
