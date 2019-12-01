﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_01
{
    static Vector2[,] mapPositionMatrix;
    static CreatureManager creatureManager;
    static Portal portal;
    static List<int[]> goldInds;
    static List<int[]> stoneInds;
    public static List<int[]> goldLocsNotPickedUp = new List<int[]>();
    public static List<GameObject> golds = new List<GameObject>();
    public static List<int> goldNotPickedUpInds = new List<int>();
    public static List<GameObject> goldsNotPickedUp = new List<GameObject>(); 

    public static float[,,] Init(Vector2[,] mapPositionMatrix, CreatureManager creatureManager) {
        Level_01.mapPositionMatrix = mapPositionMatrix;
        Level_01.creatureManager = creatureManager; 

        Sprite mapOutline = Resources.Load<Sprite>("terrains/levels/level_02");
        float[,,] mapMarkers = new float[64, 64,3];
        goldInds = new List<int[]>();
        stoneInds = new List<int[]>(); 
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
                else if (red == 255 && green == 242 && blue == 0) //gold
                {
                    goldInds.Add(new int[] { i, j });
                }
                else if (red == 185 && green == 122 && blue == 87)  //stone
                {
                    stoneInds.Add(new int[] { i, j });
                }
                else if (red == 0 && green == 162 && blue == 232) 
                {
                    Smelter.Init(new int[] { i, j }, mapPositionMatrix); 
                }
            }

        CreateGold(goldInds);
        CreateStone(stoneInds); 

        return mapMarkers; 

    }

    public static void RemoveGold(GameObject goldObject)
    {
        int goldIndex = goldsNotPickedUp.IndexOf(goldObject);
        Debug.Log("removing gold, index=" + goldIndex);

        goldNotPickedUpInds.RemoveAt(goldIndex);
        goldsNotPickedUp.RemoveAt(goldIndex);
        goldLocsNotPickedUp.RemoveAt(goldIndex); 
        goldObject.SetActive(false);
    }

    public static void Update(float deltaTime) {

        Portal.Update(deltaTime, creatureManager); 
    }

    public static void CheckUnveiled(int x, int y) {

        Portal.CheckUnveiled(x, y);
        CheckGoldUnveiled(x, y); 
    }

    public static void CheckGoldUnveiled(int x, int y) 
    {
        for (int i = 0; i < goldInds.Count; i++) 
        {
            if (x == goldInds[i][0] && y == goldInds[i][1]) //&& !goldsNotPickedUp.Contains(golds[i])
            {
                Main.audioSource.PlayOneShot(Main.goldFalling,10);
                goldsNotPickedUp.Add(golds[i]); 
                goldLocsNotPickedUp.Add(new int[]{x,y});
                goldNotPickedUpInds.Add(i); 
                
            }
        }
    }

    public static void CreateGold(List<int[]> goldInds) {

        for (int i = 0; i < goldInds.Count; i++) {
            int goldIndsX = goldInds[i][0];
            int goldIndsY = goldInds[i][1];

            Sprite goldSprite = Resources.Load<Sprite>("terrains/golds/gold_01");
            GameObject gold = new GameObject("gold: " + goldIndsX + "," + goldIndsY);
            gold.AddComponent<SpriteRenderer>();
            gold.GetComponent<SpriteRenderer>().sprite = goldSprite;
            Color tmp = gold.GetComponent<SpriteRenderer>().color;
            tmp.a = 0.85f;
            gold.GetComponent<SpriteRenderer>().color = tmp;
            gold.transform.position = new Vector3(
                mapPositionMatrix[goldIndsX, goldIndsY].x, mapPositionMatrix[goldIndsX, goldIndsY].y, -2.5f);

            golds.Add(gold); 
        }
    }

    public static void CreateStone(List<int[]> stoneInds)
    {
        for (int i = 0; i < stoneInds.Count; i++)
        {
            int stoneIndsX = stoneInds[i][0];
            int stoneIndsY = stoneInds[i][1];
            Sprite stoneSprite = Resources.Load<Sprite>("terrains/golds/stone_01");
            GameObject stone = new GameObject("stone: " + stoneIndsX + "," + stoneIndsY);
            stone.AddComponent<SpriteRenderer>();
            stone.GetComponent<SpriteRenderer>().sprite = stoneSprite;
            Color tmp = stone.GetComponent<SpriteRenderer>().color;
            tmp.a = 0.5f;
            stone.GetComponent<SpriteRenderer>().color = tmp; 
            stone.transform.position = new Vector3(
                mapPositionMatrix[stoneIndsX, stoneIndsY].x, mapPositionMatrix[stoneIndsX, stoneIndsY].y, -2.5f);
        }
    }




}
