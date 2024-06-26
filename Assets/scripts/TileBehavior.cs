﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TileBehavior
{
    GameObject strawObject;
    GameObject rockObject;

    //Sprite[] strawTiles;
    //Sprite[] rockTiles;

    Sprite[] crackSprites;
    Sprite[] claimTileSprites;
    Sprite[] dirtSprites;
    Sprite[] rubbleSprites;
    Sprite[] claimSprites;

    Material rubbleMaterial;
    Material dirtMaterial;
    Material tileMaterial;

    Sprite strawFull;
    Sprite rockFull;

    Bounds mapBounds;

    public Vector2[,] mapPositionMatrix;
    int[,] mapIndices;
    public bool[,] mapUnveiled;
    bool[,] mapTagged;
    bool[,] toBeClaimed;
    Tile[,] tagTileObjects;
    GameObject[,] borderTileObjects;
    GameObject[,] unveiledTileObjects;
    public List<GameObject> rubble;
    public List<GameObject> rubbleClaiming;

    Sprite okSprite; 

    private List<Vector2> claimedLocations; 

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

    public Dictionary<string, GameObject> accessibleTagged;
    public Dictionary<string, GameObject> inaccessibleTagged;

    List<string> accessibleKeys;
    List<string> inaccessibleKeys;

    List<GameObject> claimedTiles;

    string currentAlphaMask;

    Main main;
    CreatureManager creatureManager;
    public LevelState levelState;

    float[,,] mapDesignTileValues;

    public TileBehavior(Main main, CreatureManager creatureManager)
    {
        this.main = main;
        this.creatureManager = creatureManager;
        levelState = new LevelState(main, this);

        Debug.Log("loading tile behavior grid_lvl");
        rockObject = new GameObject();

        rockFull = Resources.Load<Sprite>("terrains/gold_lvl_04_full");
        rockObject.AddComponent<SpriteRenderer>();
        rockObject.GetComponent<SpriteRenderer>().sprite = rockFull;
        Debug.Log("done loading tile behavior grid_lvl");
        mapBounds = rockObject.GetComponent<SpriteRenderer>().sprite.bounds;

        rubbleMaterial = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
        tileMaterial = new Material(Shader.Find("Mobile/Diffuse"));
        dirtMaterial = new Material(Shader.Find("Mobile/Diffuse"));


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

        claimTileSprites = Resources.LoadAll<Sprite>("LevelItems/tiles_small");
        dirtSprites = Resources.LoadAll<Sprite>("terrains/dirts");
        rubbleSprites = Resources.LoadAll<Sprite>("LevelItems/rubble2sprite");
        claimSprites = Resources.LoadAll<Sprite>("LevelItems/explode");

        BuildMapPositionMatrix();
        InitAlphaInterfaceSprites();

        mapDesignTileValues = Level_01.Init(mapPositionMatrix, creatureManager);
        for (int i = 0; i < mapDesignTileValues.GetLength(0); i++)
            for (int j = 0; j < mapDesignTileValues.GetLength(0); j++)
            {
                if (i > 1 && j > 1 && i < mapDesignTileValues.GetLength(0) - 2 && j < mapDesignTileValues.GetLength(1) - 2) //bound check
                    if (mapDesignTileValues[i, j, 0] == 4 || mapDesignTileValues[i, j, 0] == 5) // already unveiled by map design
                    {
                        UnveilTile(new Vector3(mapPositionMatrix[i, j].x, mapPositionMatrix[i, j].y, -2));
                    }
            }
    }

    public void StartPath(Imp imp, int[] end)
    {
        int[] start = GetTileIndex(imp.GetPosition());
        List<Vector2> path = new Astar(mapUnveiledJagged, start, end, "Euclidean").result;
        imp.SetupWalkingPath(path, mapPositionMatrix, "", this, tagTileObjects[end[0], end[1]]);

    }

    public void StartPathToTagged(int[] end, string gameObjectName)
    {
        mapUnveiledJagged[end[1]][end[0]] = 0;

        List<Imp> idleImps = creatureManager.GetIdleImps();

        foreach (Imp imp in idleImps)
        {
            int[] start = GetTileIndex(imp.GetPosition());
            List<Vector2> path = new Astar(mapUnveiledJagged, start, end, "Euclidean").result;

            if (imp.movingToToggled == false && path.Count > 0)
            {
                imp.InitiateTileMove();
                imp.SetupWalkingPath(path, mapPositionMatrix, gameObjectName, this, tagTileObjects[end[0], end[1]]);
            }
        }
        mapUnveiledJagged[end[1]][end[0]] = 1;
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

        for (float i = -1; i < 1; i += 0.1f)
            for (float j = -1; j < 1f; j += 0.1f)
            {
                UnveilTile(new Vector3(i, j, -2));
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
        claimedTiles = new List<GameObject>();

        Vector2 spriteCenter = new Vector2(mapBounds.center.x, mapBounds.center.y);
        Vector2 spriteBounds = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

        int sqrtTiles = 64;// (int)Mathf.Sqrt(strawTiles.Length);

        mapPositionMatrix = new Vector2[sqrtTiles, sqrtTiles];
        mapIndices = new int[sqrtTiles, sqrtTiles];
        mapTagged = new bool[sqrtTiles, sqrtTiles];
        toBeClaimed = new bool[sqrtTiles, sqrtTiles];
        mapUnveiled = new bool[sqrtTiles, sqrtTiles];
        tagTileObjects = new Tile[sqrtTiles, sqrtTiles];
        borderTileObjects = new GameObject[sqrtTiles, sqrtTiles];
        mapUnveiledJagged = new int[sqrtTiles][];
        mapUnveiledOrTaggedJagged = new int[sqrtTiles][];

        rubble = new List<GameObject>();
        rubbleClaiming = new List<GameObject>();

        okSprite = Resources.Load<Sprite>("LevelItems/ok_room");

        unveiledTileObjects = new GameObject[sqrtTiles, sqrtTiles];

        claimedLocations = new List<Vector2>(); 

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
                    //Debug.Log("Tile is accessible, finding path...");
                    accessibleTagged.Add(taggedOject.name, taggedOject);
                    accessibleKeys.Add(taggedOject.name);
                    StartPathToTagged(tileIndices, taggedOject.name);
                }
                else
                {
                    inaccessibleTagged.Add(taggedOject.name, taggedOject);
                    inaccessibleKeys.Add(taggedOject.name);
                    //
                    //Debug.Log("Tile is inaccessible, storing for later...");
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

    public bool UpdateDestroyedAndRequestAnother(string tileName, Vector2 position)
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
        else return false;
        return true;
    }

    private void RedefineAccessibleTagged()
    {
        List<string> inaccessiblesToRemove = new List<string>();

        for (int i = 0; i < inaccessibleKeys.Count; i++)
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

        gobj.GetComponent<SpriteRenderer>().sprite = claimTileSprites[3];
        gobj.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.2f);
        gobj.transform.position = new Vector3(mapPosition.x, mapPosition.y, -1);

        mapTagged[xIndex, yIndex] = true;

        float tileLife = 4;
        switch (mapDesignTileValues[xIndex, yIndex, 0])
        {
            case (1):
                tileLife = 1000;
                break;
            default:
                tileLife = 4;
                break;
        }

        tagTileObjects[xIndex, yIndex] = new Tile(gobj, tileLife);

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
            maskObject.transform.position = new Vector3(mapPosition.x, mapPosition.y, -3.25f);
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

                borderTileObjects[currentInds[0], currentInds[1]] = maskObject;
            }
        }
    }


    //assumes the player is not allowed to select edge tiles 
    private Sprite SelectAlphaMask(int xIndex, int yIndex)
    {
        Sprite tileMask = null;
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
        toBeClaimed[tileIndices[0], tileIndices[1]] = true;



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

        rubble.Add(CreateRubble(mapPosition, tileIndices[0], tileIndices[1]));

        unveiledIndexList.Add(tileIndices);
    }

    public void DegradeTileAlpha(string taggedTileName, int hitCount)
    {
        if (accessibleTagged.ContainsKey(taggedTileName))
        {
            GameObject obj = accessibleTagged[taggedTileName];
            if (hitCount < crackSprites.Length)
                obj.GetComponent<SpriteRenderer>().sprite = crackSprites[hitCount];
        }
    }

    public int[] GetTileIndex(Vector3 pointMain)
    {
        float x = pointMain.x;
        float y = pointMain.y;

        Vector2 spriteCenter = new Vector2(mapBounds.center.x, mapBounds.center.y);
        Vector2 spriteBounds = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

        float relX = (x + mapBounds.extents.x) / (mapBounds.extents.x * 2);
        float relY = 1 - (y + mapBounds.extents.y) / (mapBounds.extents.y * 2);

        int sqrtTiles = 64;// (int)Mathf.Sqrt(strawTiles.Length);

        int xIndex = (int)(relX * sqrtTiles);
        int yIndex = (int)(relY * sqrtTiles);

        int[] tileIndices = new int[] { xIndex, yIndex };

        return tileIndices;

    }

    private void CreateNewTile(Vector2 mapPosition, int xIndex, int yIndex)
    {
        GameObject gobj = new GameObject("tile: " + xIndex + "," + yIndex);
        gobj.AddComponent<SpriteRenderer>();
        //update claimed sprite
        int random = UnityEngine.Random.Range(0, 64);
        gobj.GetComponent<SpriteRenderer>().sprite = dirtSprites[random];

        gobj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        gobj.GetComponent<Renderer>().receiveShadows = true;

        gobj.GetComponent<Renderer>().material = dirtMaterial;

        gobj.transform.position = new Vector3(mapPosition.x, mapPosition.y, -2.5f);

        unveiledTileObjects[xIndex, yIndex] = gobj;
    }

    private GameObject CreateRubble(Vector2 mapPosition, int xIndex, int yIndex) {
        GameObject gobj = new GameObject("rubble: " + xIndex + "," + yIndex);
        gobj.AddComponent<SpriteRenderer>();
        gobj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        gobj.GetComponent<Renderer>().receiveShadows = true;
        gobj.GetComponent<Renderer>().material = rubbleMaterial;
        int random = UnityEngine.Random.Range(0, 9);
        gobj.GetComponent<SpriteRenderer>().sprite = rubbleSprites[random];
        gobj.transform.position = new Vector3(mapPosition.x, mapPosition.y, -2.5f);

        return gobj;

    }
    static int claimTileCount;
    public void ClaimTile(Imp claimingImp, GameObject currentRubble) {
        rubble.Remove(currentRubble);
        rubbleClaiming.Remove(currentRubble);
        claimingImp.DoClaim(currentRubble.transform.position);
        claimedLocations.Add(currentRubble.transform.position); 
        DestroyRubble(currentRubble);

    }

    public void FinishClaimTile(Vector2 tilePosition) {
        //Vector2 tilePosition = claimingImp.currentItemSeeking.transform.position; 

        main.StartTileAnimation(new SpriteAnimation(claimSprites, tilePosition));
        int[] inds = GetTileIndex(tilePosition);
        GameObject gobj = new GameObject("tile: " + inds[0] + "," + inds[1]);
        gobj.AddComponent<SpriteRenderer>();
        int random = UnityEngine.Random.Range(0, 9);
        gobj.GetComponent<SpriteRenderer>().sprite = claimTileSprites[random];
        gobj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        gobj.GetComponent<Renderer>().material = tileMaterial;
        gobj.GetComponent<Renderer>().receiveShadows = true;
        gobj.transform.position = new Vector3(mapPositionMatrix[inds[0], inds[1]].x, mapPositionMatrix[inds[0], inds[1]].y, -2.6f);
        claimedTiles.Add(gobj);
    }

    public void DestroyRubble(GameObject rubble) {
        main.DestroyGameObject(rubble);
    }

    public GameObject ClosestRubble(Vector2 impPosition) {
        float minDist = 99999999;
        GameObject closestRubble = null; 
        foreach (GameObject gobj in rubble) {
            float newDistx = Math.Abs(impPosition.x - gobj.transform.position.x);
            float newDisty = Math.Abs(impPosition.y - gobj.transform.position.y);
            if (newDistx + newDisty < minDist && !rubbleClaiming.Contains(gobj)) {
                minDist = newDistx + newDisty;
                closestRubble = gobj; 
            }
        }
        return closestRubble; 
    }

    public void MarkAvailableRoomTiles() {
        for (int i = 0; i < claimedLocations.Count; i++)
        {
            int[] inds = GetTileIndex(claimedLocations[i]);
            CreateNewTile(claimedLocations[i], inds[0], inds[1]);
            Debug.Log("CREATING NEW TILE");

            GameObject gobj = new GameObject("tile: ");
            gobj.AddComponent<SpriteRenderer>();
            gobj.GetComponent<SpriteRenderer>().sprite = okSprite;
            gobj.transform.position = new Vector3(mapPositionMatrix[inds[0], inds[1]].x, mapPositionMatrix[inds[0], inds[1]].y, -3.3f);
            
        }


    }

}
