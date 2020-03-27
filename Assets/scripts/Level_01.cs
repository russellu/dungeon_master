using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Level_01
{
    static Vector2[,] mapPositionMatrix;
    static CreatureManager creatureManager;
    static Portal portal;
    static List<int[]> goldInds;
    static List<int[]> stoneInds;
    public static Queue<int[]> goldLocsNotPickedUp = new Queue<int[]>();
    public static List<GameObject> golds = new List<GameObject>();
    public static Queue<int> goldNotPickedUpInds = new Queue<int>();
    public static Queue<GameObject> goldsNotPickedUp = new Queue<GameObject>();
    public static int gold = 0;


    public static float[,,] Init(Vector2[,] mapPositionMatrix, CreatureManager creatureManager) {

        Level_01.mapPositionMatrix = mapPositionMatrix;
        Level_01.creatureManager = creatureManager;

        Debug.Log("loading GRID_LVL_04"); 
        Sprite mapOutline = Resources.Load<Sprite>("terrains/levels/grid_lvl_04");
        Debug.Log("loaded GRID_LVL_04");

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
                mapMarkers[i,j, 0] = red;
                mapMarkers[i, j, 1] = green;
                mapMarkers[i, j, 2] = blue;

                //gold 255 242 0
                //rock 185 122 87
                //enemy portal 237 28 36
                if (red == 6 && green == 6 && blue == 6) // good portal
                {
                    portalLocation = new int[] {i,j};
                    Portal.Init(0,portalLocation, mapPositionMatrix); 
                }
                if (red == 237 && green == 28 && blue == 36) // enemy portal
                {
                    portalLocation = new int[] { i, j };
                    Portal.Init(1, portalLocation, mapPositionMatrix);
                }
                else if (red == 3 && green == 3 && blue == 3) //gold
                {
                    goldInds.Add(new int[] { i, j });
                }
                else if (red == 1 && green == 1 && blue == 1)  //stone
                {
                    Debug.Log("adding stone");
                    stoneInds.Add(new int[] { i, j });
                    mapMarkers[i, j, 0] = 1; 
                }
                else if (red == 5 && green == 5 && blue == 5)//smelter 
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

            goldObject.SetActive(false);
            Main.Destroy(goldObject);
     
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
            if (x == goldInds[i][0] && y == goldInds[i][1]) 
            {
                Main.audioSource.PlayOneShot(Main.goldFalling,10);
                goldsNotPickedUp.Enqueue(golds[i]);
                goldLocsNotPickedUp.Enqueue(new int[] { x, y });
                goldNotPickedUpInds.Enqueue(i); 

                
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
            tmp.a = 0.45f;
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
            tmp.a = 0.15f; tmp.r = 0.1f; 
            stone.GetComponent<SpriteRenderer>().color = tmp; 
            stone.transform.position = new Vector3(
                mapPositionMatrix[stoneIndsX, stoneIndsY].x, mapPositionMatrix[stoneIndsX, stoneIndsY].y, -2.5f);
        }
    }
}
