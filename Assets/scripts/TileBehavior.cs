using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 


public class TileBehavior
{
    GameObject strawObject;
    GameObject rockObject;

    Sprite[] strawTiles;
    Sprite[] rockTiles;

    Sprite[] crackSprites; 

    Sprite strawFull;
    Sprite rockFull;

    Bounds mapBounds;

    public Vector2[,] mapPositionMatrix;
    int[,] mapIndices;
    public bool[,] mapUnveiled;
    bool[,] mapTagged;
    Tile[,] tagTileObjects;
    GameObject[,] borderTileObjects;
    GameObject[,] unveiledTileObjects;

    public List<int[]> unveiledIndexList = new List<int[]>(); 

    public int[][] mapUnveiledJagged;
    int[][] mapUnveiledOrTaggedJagged; 

    Sprite topAlphaInterface;
    Sprite botAlphaInterface;
    Sprite leftAlphaInterface;
    Sprite rightAlphaInterface;
    Sprite topLeftAlphaInterface;
    Sprite topRightAlphaInterface;
    Sprite botLeftAlphaInterface;
    Sprite botRightAlphaInterface;
    Sprite leftRightAlphaInterface;
    Sprite topBotAlphaInterface;
    Sprite topBotLeftAlphaInterface;
    Sprite topBotRightAlphaInterface;
    Sprite leftRightTopAlphaInterface;
    Sprite leftRightBotAlphaInterface;
    Sprite leftRightTopBotAlphaInterface;

    //public List<int[]> accessibleTagged;
    //public List<int[]> inaccessibleTagged;

    public Dictionary<string, GameObject> accessibleTagged;
    public Dictionary<string, GameObject> inaccessibleTagged;

    List<string> accessibleKeys;
    List<string> inaccessibleKeys; 

    string currentAlphaMask; 

    Main main;
    CreatureManager creatureManager;
    
    //BaseGrid searchGrid; 

    public TileBehavior(Main main, CreatureManager creatureManager)
    {
        this.main = main;
        this.creatureManager = creatureManager;

     
        rockObject = new GameObject();
        strawTiles = Resources.LoadAll<Sprite>("terrains/cropstraw");
        rockTiles = Resources.LoadAll<Sprite>("terrains/croprock");

        rockFull = Resources.Load<Sprite>("terrains/croprock_full");
        rockObject.AddComponent<SpriteRenderer>();
        rockObject.GetComponent<SpriteRenderer>().sprite = rockFull;

        mapBounds = rockObject.GetComponent<SpriteRenderer>().sprite.bounds;

        accessibleTagged = new Dictionary<string, GameObject>();
        inaccessibleTagged = new Dictionary<string, GameObject>();

        accessibleKeys = new List<string>();
        inaccessibleKeys = new List<string>();

        unveiledIndexList = new List<int[]>(); 

        crackSprites = new Sprite[4]; 
        crackSprites[0] = Resources.Load<Sprite>("terrains/cracks/trans_crack_01");
        crackSprites[1] = Resources.Load<Sprite>("terrains/cracks/trans_crack_02");
        crackSprites[2] = Resources.Load<Sprite>("terrains/cracks/trans_crack_03");
        crackSprites[3] = Resources.Load<Sprite>("terrains/cracks/trans_crack_04");



        BuildMapPositionMatrix();
        InitAlphaInterfaceSprites();
       // ClearSpaceInMiddle();
        float[,,] unveilIndices = Level_01.Init(mapPositionMatrix, creatureManager);
        for (int i = 0; i < unveilIndices.GetLength(0); i++)
            for (int j = 0; j < unveilIndices.GetLength(0); j++)
            {
                if (unveilIndices[i, j,0] == 0)
                {
                 //   Debug.Log(mapPositionMatrix[i, j].x);
                 //   Debug.Log("min y" + mapPositionMatrix[0, 0].y);
                 //   Debug.Log("min x" + mapPositionMatrix[0, 0].x);

                    if (i>1 && j>1 && i<unveilIndices.GetLength(0)-2 && j<unveilIndices.GetLength(1)-2)
                        UnveilTile(new Vector3(mapPositionMatrix[i, j].x, mapPositionMatrix[i, j].y, -2));
                }
            }




    }




    public void StartPath(int[] end)
    {

        List<Imp> idleImps = creatureManager.GetIdleImps();

        foreach (Imp imp in idleImps)
        {
            int[] start = GetTileIndex(imp.GetPosition());

            List<Vector2> path = new Astar(mapUnveiledJagged, start, end, "Euclidean").result;

            for (int i = 0; i < path.Count; i++)
            {
                GameObject gobj = unveiledTileObjects[(int)path[i].x, (int)path[i].y];
                gobj.GetComponent<SpriteRenderer>().color = new Color(0.8f, .6f, 0);
            }

            imp.SetupWalkingPath(path, mapPositionMatrix, "", this, tagTileObjects[end[0],end[1]]);
        }
    }

    public void StartPathToTagged(int[] end, string gameObjectName)
    {
        mapUnveiledJagged[end[1]][end[0]] = 0;

        List<Imp> idleImps = creatureManager.GetIdleImps();

        foreach(Imp imp in idleImps)
        { 
            int[] start = GetTileIndex(imp.GetPosition());
            List<Vector2> path = new Astar(mapUnveiledJagged, start, end, "Euclidean").result;

            if (imp.movingToToggled == false)
            {
                imp.InitiateTileMove();
                imp.SetupWalkingPath(path, mapPositionMatrix, gameObjectName, this, tagTileObjects[end[0], end[1]]);
            }        
        }
    }

    // check if any of the surrounding tiles are unveiled 
    public bool CheckIfTaggedAccessible(int[] taggedLocation)
    {
        int x = taggedLocation[0];
        int y = taggedLocation[1];

        if (mapUnveiled[x - 1, y] || mapUnveiled[x + 1, y] || mapUnveiled[x, y - 1] || mapUnveiled[x, y + 1])
            return true;
        else
            return false; 
    }

    public void ClearSpaceInMiddle()
    {

        for (float i = -1; i < 1; i+=0.1f)
            for (float j = -1; j < 1f; j+=0.1f)
            {
                UnveilTile(new Vector3(i,j,-2));
            }
    }

    private void InitAlphaInterfaceSprites()
    {
        topAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_toptex");
        botAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_bottex");
        leftAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_lefttex");
        rightAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_righttex");
        topLeftAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_toplefttex");
        topRightAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_toprighttex");
        botLeftAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_botlefttex");
        botRightAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_botrighttex");
        leftRightAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_leftrighttex");
        topBotAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_topbottex");
        topBotLeftAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_topbotlefttex");
        topBotRightAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_topbotrighttex");
        leftRightTopAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_leftrighttoptex");
        leftRightBotAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_leftrightbottex");
        leftRightTopBotAlphaInterface = Resources.Load<Sprite>("terrains/itfcs/itfc_leftrighttopbottex");
    }

    private void BuildMapPositionMatrix()
    {
        Vector2 spriteCenter = new Vector2(mapBounds.center.x, mapBounds.center.y);
        Vector2 spriteBounds = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

        int sqrtTiles = (int)Mathf.Sqrt(strawTiles.Length);

        mapPositionMatrix = new Vector2[sqrtTiles, sqrtTiles];
        mapIndices = new int[sqrtTiles, sqrtTiles];
        mapTagged = new bool[sqrtTiles, sqrtTiles];
        mapUnveiled = new bool[sqrtTiles, sqrtTiles];
        tagTileObjects = new Tile[sqrtTiles, sqrtTiles];
        borderTileObjects = new GameObject[sqrtTiles, sqrtTiles];
        mapUnveiledJagged = new int[sqrtTiles][];
        mapUnveiledOrTaggedJagged = new int[sqrtTiles][];

        unveiledTileObjects = new GameObject[sqrtTiles,sqrtTiles];

        for (int i = 0; i < mapUnveiledJagged.Length; i++)
        {
            mapUnveiledJagged[i] = new int[sqrtTiles];
            mapUnveiledOrTaggedJagged[i] = new int[sqrtTiles]; 
        }

        float tileWidth = (spriteBounds.x * 2) / sqrtTiles;
        float tileHeight = (spriteBounds.y * 2) / sqrtTiles;

        int jCount = 0;
        int iCount = 0;
        int tileCount = 0;
        for (float j = spriteBounds.y; j > -spriteBounds.y + tileHeight; j -= tileHeight)
        {
            for (float i = -spriteBounds.x; i < spriteBounds.x - tileWidth; i += tileWidth)
            {
                mapPositionMatrix[iCount, jCount] = new Vector2(i + tileWidth / 2, j - tileHeight / 2);
                mapIndices[iCount, jCount] = tileCount;
                mapTagged[iCount, jCount] = false;
                mapUnveiled[iCount, jCount] = false;
                mapUnveiledJagged[iCount][jCount] = 1;
                mapUnveiledOrTaggedJagged[iCount][jCount] = 1;

                iCount++;
                tileCount++;
            }
            iCount = 0;
            jCount++;
        }
    }

    public bool TagTile(Vector3 pointMain, int untaggingMode)
    { 

        int[] tileIndices = GetTileIndex(pointMain);

        bool alreadyTagged = mapTagged[tileIndices[0], tileIndices[1]]; 

        Vector2 mapPosition = mapPositionMatrix[tileIndices[0], tileIndices[1]];

        if (!alreadyTagged && untaggingMode != 1)
        {
            if (mapUnveiled[tileIndices[0], tileIndices[1]] == false)
            {

                Main.audioSource.PlayOneShot(Main.marble, 1f);


                GameObject taggedOject = ColorExistingTile(mapPosition, tileIndices[0], tileIndices[1]);
                mapUnveiledOrTaggedJagged[tileIndices[1]][tileIndices[0]] = 0;

                if (CheckIfTaggedAccessible(tileIndices))
                {
                    Debug.Log("Tile is accessible, finding path...");
                    accessibleTagged.Add(taggedOject.name, taggedOject);
                    accessibleKeys.Add(taggedOject.name); 
                    StartPathToTagged(tileIndices, taggedOject.name);
                }
                else
                {
                    inaccessibleTagged.Add(taggedOject.name, taggedOject);
                    inaccessibleKeys.Add(taggedOject.name); 
                    Debug.Log("Tile is inaccessible, storing for later...");
                }
            }
        }
        else if (alreadyTagged && untaggingMode != 2)
        {
            GameObject taggedOject = tagTileObjects[tileIndices[0], tileIndices[1]].baseObject;
            string objectTag = taggedOject.name;
            if (accessibleTagged.ContainsKey(objectTag))
            {
                accessibleTagged.Remove(objectTag);
                accessibleKeys.RemoveAt(accessibleKeys.IndexOf(objectTag));

                // main.Imp.CancelMovementToTagged(objectTag);            
                creatureManager.CancelImpMovements(objectTag); 
            }
            else if (inaccessibleTagged.ContainsKey(objectTag))
            {
                inaccessibleTagged.Remove(objectTag);
                inaccessibleKeys.RemoveAt(inaccessibleKeys.IndexOf(objectTag));

                //main.Imp.CancelMovementToTagged(objectTag);
                creatureManager.CancelImpMovements(objectTag);
            }
            main.DestroyGameObject(taggedOject);
            mapTagged[tileIndices[0], tileIndices[1]] = false;
        }

        return alreadyTagged; 
    }


    private int GetClosestEuclideanAccessible(Vector2 position)
    {
        int closestInd = -1;
        float minDist = 9999999; 
        for (int i = 0; i < accessibleKeys.Count; i++)
        {
            GameObject obj = accessibleTagged[accessibleKeys[i]];
            if (obj != null)
            {
                float dist = Vector2.Distance(obj.transform.position, position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestInd = i;
                }
            }
        }
        return closestInd; 
    }

    public void UpdateDestroyedAndRequestAnother(string tileName, Vector2 position)
    {
        if (accessibleTagged.ContainsKey(tileName))
        {
            UnveilTile(accessibleTagged[tileName].transform.position);

            main.DestroyGameObject(accessibleTagged[tileName]);
            accessibleTagged.Remove(tileName);
            accessibleKeys.RemoveAt(accessibleKeys.IndexOf(tileName));
            Main.audioSource.PlayOneShot(Main.wood, 1f);

            RedefineAccessibleTagged();
        }

        if (accessibleTagged.Count > 0)
        {

            int closestTileIndex = GetClosestEuclideanAccessible(position);
            if (closestTileIndex >= 0)
            {
                string tag = accessibleKeys[closestTileIndex];
                GameObject obj = accessibleTagged[tag];
                int[] tileIndex = GetTileIndex(obj.transform.position);
                StartPathToTagged(tileIndex, tag);
            }
        }

    }

    private void RedefineAccessibleTagged()
    {
        List<string> inaccessiblesToRemove = new List<string>(); 

        for(int i=0;i<inaccessibleKeys.Count;i++)
        {
            string s = inaccessibleKeys[i]; 
            GameObject obj = inaccessibleTagged[s]; 
            int[] tileIndex = GetTileIndex(obj.transform.position);

            if (CheckIfTaggedAccessible(tileIndex))
            {
                accessibleTagged.Add(s, obj);
                accessibleKeys.Add(s);
                inaccessiblesToRemove.Add(s); 
            }
        }

        foreach (string s in inaccessiblesToRemove)
        {
            inaccessibleTagged.Remove(s);
            inaccessibleKeys.Remove(s);
        }
    }


    private GameObject ColorExistingTile(Vector2 mapPosition, int xIndex, int yIndex)
    {
        GameObject gobj = new GameObject("tile: " + xIndex + "," + yIndex);
        gobj.AddComponent<SpriteRenderer>();
        gobj.GetComponent<SpriteRenderer>().sprite = rockTiles[mapIndices[xIndex, yIndex]];
        gobj.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
        gobj.transform.position = new Vector3(mapPosition.x, mapPosition.y, -1);

        mapTagged[xIndex, yIndex] = true;
        tagTileObjects[xIndex, yIndex] = new Tile(gobj, 4);

        return gobj; 
    }

    private void AddAlphaMask(Vector2 mapPosition, int[] tileIndices)
    {
        Sprite tileMask = SelectAlphaMask(tileIndices[0], tileIndices[1]);

        if (tileMask != null)
        {
            GameObject maskObject = new GameObject(currentAlphaMask);
            maskObject.AddComponent<SpriteRenderer>();
            maskObject.GetComponent<SpriteRenderer>().sprite = tileMask;
            maskObject.transform.position = new Vector3(mapPosition.x, mapPosition.y, -3);
            //maskObject.AddComponent<PolygonCollider2D>();
            //maskObject.GetComponent<MeshCollider>().convex = true; 
            borderTileObjects[tileIndices[0], tileIndices[1]] = maskObject;
        }
    }

    private void CheckSurroundingTileAlphaMask(Vector2 mapPosition, int[] tileIndices)
    {
        int[] above = new int[] { tileIndices[0], tileIndices[1] + 1 };
        int[] below = new int[] { tileIndices[0], tileIndices[1] - 1 };
        int[] left = new int[] { tileIndices[0] - 1, tileIndices[1] };
        int[] right = new int[] { tileIndices[0] + 1, tileIndices[1] };

        List<int[]> allTiles = new List<int[]> { above, below, left, right, tileIndices };

        for (int i = 0; i < allTiles.Count; i++)
        {
            int[] currentInds = allTiles[i];
            main.DestroyGameObject(borderTileObjects[currentInds[0], currentInds[1]]);
            Sprite newAlphaMask = SelectAlphaMask(currentInds[0], currentInds[1]);

            if (newAlphaMask != null)
            {
                GameObject maskObject = new GameObject(currentAlphaMask);
                maskObject.AddComponent<SpriteRenderer>();
                maskObject.GetComponent<SpriteRenderer>().sprite = newAlphaMask;
                maskObject.transform.position = new Vector3(mapPositionMatrix[currentInds[0], currentInds[1]].x, mapPositionMatrix[currentInds[0], currentInds[1]].y, -3);
                //maskObject.AddComponent<PolygonCollider2D>();
                //maskObject.GetComponent<MeshCollider>().convex = true;
                borderTileObjects[currentInds[0], currentInds[1]] = maskObject;
            }
        }
    }


    //assumes the player is not allowed to select edge tiles 
    private Sprite SelectAlphaMask(int xIndex, int yIndex)
    {
        Sprite tileMask = null;
     //   Debug.Log("yindex= " + yIndex + " xindex=" + xIndex); 
        bool above = !mapUnveiled[xIndex, yIndex - 1];
        bool below = !mapUnveiled[xIndex, yIndex + 1];
        bool left = !mapUnveiled[xIndex - 1, yIndex];
        bool right = !mapUnveiled[xIndex + 1, yIndex];

        if (mapUnveiled[xIndex, yIndex] == true)
            if (above && below && left && right) // if surrounded on all sides
            {
                tileMask = leftRightTopBotAlphaInterface;
                currentAlphaMask = "leftRightTopBotAlphaInterface";
            }
            else if (above && below && left)
            {
                tileMask = topBotLeftAlphaInterface;
                currentAlphaMask = "topBotLeftAlphaInterface";

            }
            else if (above && below && right)
            {
                tileMask = topBotRightAlphaInterface;
                currentAlphaMask = "topBotRightAlphaInterface";

            }
            else if (below && left && right)
            {
                tileMask = leftRightBotAlphaInterface;
                currentAlphaMask = "leftRightBotAlphaInterface";

            }
            else if (above && left && right)
            {
                tileMask = leftRightTopAlphaInterface;
                currentAlphaMask = "leftRightTopAlphaInterface";

            }
            else if (above && left)
            {
                tileMask = topLeftAlphaInterface;
                currentAlphaMask = "topLeftAlphaInterface";

            }
            else if (above && right)
            {
                tileMask = topRightAlphaInterface;
                currentAlphaMask = "topRightAlphaInterface";

            }
            else if (below && left)
            {
                tileMask = botLeftAlphaInterface;
                currentAlphaMask = "botLeftAlphaInterface";

            }
            else if (below && right)
            {
                tileMask = botRightAlphaInterface;
                currentAlphaMask = "botRightAlphaInterface";

            }
            else if (left && right)
            {
                tileMask = leftRightAlphaInterface;
                currentAlphaMask = "leftRightAlphaInterface";

            }
            else if (above && below)
            {
                tileMask = topBotAlphaInterface;
                currentAlphaMask = "topBotAlphaInterface";

            }
            else if (left)
            {
                tileMask = leftAlphaInterface;
                currentAlphaMask = "leftAlphaInterface";

            }
            else if (right)
            {
                tileMask = rightAlphaInterface;
                currentAlphaMask = "rightAlphaInterface";
            }
            else if (above)
            {
                tileMask = topAlphaInterface;
                currentAlphaMask = "topAlphaInterface";
            }
            else if (below)
            {
                tileMask = botAlphaInterface;
                currentAlphaMask = "botAlphaInterface";
            }

        return tileMask;
    }

    public void UnveilTile(Vector3 pointMain)
    {
        int[] tileIndices = GetTileIndex(pointMain);
      //  Debug.Log(tileIndices[0]+" "+tileIndices[1]);
        Vector2 mapPosition = mapPositionMatrix[tileIndices[0], tileIndices[1]];

        if (mapTagged[tileIndices[0], tileIndices[1]] == true)
        {
            main.DestroyGameObject(tagTileObjects[tileIndices[0], tileIndices[1]].baseObject);
            mapTagged[tileIndices[0], tileIndices[1]] = false; 
        }

        if (mapUnveiled[tileIndices[0], tileIndices[1]] == false)
        {
            mapUnveiled[tileIndices[0], tileIndices[1]] = true;
            mapUnveiledJagged[tileIndices[1]][tileIndices[0]] = 0;
            mapUnveiledOrTaggedJagged[tileIndices[1]][tileIndices[0]] = 0;

            CreateNewTile(mapPosition, tileIndices[0], tileIndices[1]);
            AddAlphaMask(mapPosition, tileIndices);
            CheckSurroundingTileAlphaMask(mapPosition, tileIndices);
        }
 


        Level_01.CheckUnveiled(tileIndices[0], tileIndices[1]);

        unveiledIndexList.Add(tileIndices); 

    }

    public void DegradeTileAlpha(string taggedTileName, int hitCount)
    {
        GameObject obj = accessibleTagged[taggedTileName];
        obj.GetComponent<SpriteRenderer>().sprite = crackSprites[hitCount];


    }

    public int[] GetTileIndex(Vector3 pointMain)
    {
        float x = pointMain.x;
        float y = pointMain.y;

        Vector2 spriteCenter = new Vector2(mapBounds.center.x, mapBounds.center.y);
        Vector2 spriteBounds = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

        float relX = (x + mapBounds.extents.x) / (mapBounds.extents.x * 2);
        float relY = 1 - (y + mapBounds.extents.y) / (mapBounds.extents.y * 2);

        int sqrtTiles = (int)Mathf.Sqrt(strawTiles.Length);

        int xIndex = (int)(relX * sqrtTiles);
        int yIndex = (int)(relY * sqrtTiles);

        int[] tileIndices = new int[] { xIndex, yIndex };

        return tileIndices;

    }

    private void CreateNewTile(Vector2 mapPosition, int xIndex, int yIndex)
    {
        GameObject gobj = new GameObject("tile: " + xIndex+ "," + yIndex);
        gobj.AddComponent<SpriteRenderer>();
        gobj.GetComponent<SpriteRenderer>().sprite = strawTiles[mapIndices[xIndex, yIndex]];
        gobj.transform.position = new Vector3(mapPosition.x, mapPosition.y, -2);

        unveiledTileObjects[xIndex, yIndex] = gobj; 
    }








}
