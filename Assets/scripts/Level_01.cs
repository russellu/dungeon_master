using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_01
{
    static Vector2[,] mapPositionMatrix;
    static CreatureManager creatureManager;
    static Portal portal; 

    public static float[,,] Init(Vector2[,] mapPositionMatrix, CreatureManager creatureManager) {
        Level_01.mapPositionMatrix = mapPositionMatrix;
        Level_01.creatureManager = creatureManager; 


        Sprite mapOutline = Resources.Load<Sprite>("terrains/levels/level_02");
        float[,,] mapMarkers = new float[64, 64,3];
        List<int[]> goldInds = new List<int[]>();
        int[] portalLocation; 
        for (int i = 0; i < 64; i++)
            for (int j = 0; j < 64; j++) 
            {
                Color current = mapOutline.texture.GetPixel(i, j);

                float red = (current.r * 255.0f);
                float green = (current.g * 255.0f);
                float blue = (current.b * 255.0f);
                mapMarkers[i,j,0] = red;
                mapMarkers[i, j, 1] = green;
                mapMarkers[i, j, 2] = blue;
                //gold 255 242 0
                //rock 185 122 87
                //enemy portal 237 28 36
                if (red == 34 && green == 177 && blue == 76) // good portal
                {
                    portalLocation = new int[] {i,j};
                    Portal.Init(0,portalLocation, mapPositionMatrix); 
                }
                if (red == 237 && green == 28 && blue == 36) // enemy portal
                {
                    portalLocation = new int[] { i, j };
                    Portal.Init(1, portalLocation, mapPositionMatrix);
                }
                else if (red == 255 && green == 242 && blue == 0)
                {
                    goldInds.Add(new int[] {i,j});
                }

            }

        CreateGold(goldInds);

        return mapMarkers; 

    }

    public static void Update(float deltaTime) {

        Portal.Update(deltaTime, creatureManager); 
    }

    public static void CheckUnveiled(int x, int y) {

        Portal.CheckUnveiled(x, y);


    }

    public static void CreateGold(List<int[]> goldInds) {

        for (int i = 0; i < goldInds.Count; i++) {
            int goldIndsX = goldInds[i][0];
            int goldIndsY = goldInds[i][1];

            Sprite goldSprite = Resources.Load<Sprite>("terrains/golds/gold_01");
            GameObject gold = new GameObject("gold: " + goldIndsX + "," + goldIndsY);
            gold.AddComponent<SpriteRenderer>();
            gold.GetComponent<SpriteRenderer>().sprite = goldSprite;
            gold.transform.position = new Vector3(
                mapPositionMatrix[goldIndsX, goldIndsY].x, mapPositionMatrix[goldIndsX, goldIndsY].y, -2.5f);
           // Debug.Log("creating Gold" + goldInds.Count + "goldx =" + goldIndsX + "goldy=" + goldIndsY + "posx=");

        }

    }






}
