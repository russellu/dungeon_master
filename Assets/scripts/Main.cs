using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System; 


public class Main : MonoBehaviour
{
 
    Camera mainCamera;
    public static AudioSource audioSource;
    public static AudioClip marble;
    public static AudioClip tin;
    public static AudioClip wood;
    public static AudioClip stab;
    public static AudioClip portal;
    public static AudioClip gruntSpawn;

    TileBehavior tileBehavior;
    RoomPathFinder roomPathFinder;
    CreatureManager creatureManager; 


    Sprite circSprite;
    PhysicsMaterial2D physicsMaterial;
    GameObject circObject;
    Controls controls;

    float tapTime = 0f;

    bool alreadyTagged = false; 


    void Start()
    {
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        audioSource = mainCamera.GetComponent<AudioSource>();
        marble = Resources.Load<AudioClip>("audios/marble");
        tin = Resources.Load<AudioClip>("audios/tin");
        wood = Resources.Load<AudioClip>("audios/wood");
        stab = Resources.Load<AudioClip>("audios/stab");
        portal = Resources.Load<AudioClip>("audios/portal");
        gruntSpawn = Resources.Load<AudioClip>("audios/grunt_spawn");

        creatureManager = new CreatureManager();
        creatureManager.main = this; 

        tileBehavior = new TileBehavior(this, creatureManager);
        creatureManager.SetTileBehavior(tileBehavior);


        Physics2D.gravity = Vector2.zero;
        Physics2D.velocityThreshold = 0.001f;

        Physics2D.IgnoreLayerCollision(0, 1, true);

        physicsMaterial = new PhysicsMaterial2D();
        physicsMaterial.bounciness = 1;

        //controls = new Controls(this);

        /*
        circSprite = Resources.Load("players/circle", typeof(Sprite)) as Sprite;
        circObject = new GameObject("players/circ1");
        SpriteRenderer rend1 = circObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        rend1.sprite = circSprite;
        circObject.transform.position = new Vector3(0, 1,-3);
        circObject.AddComponent<Rigidbody2D>();
        circObject.AddComponent<PolygonCollider2D>();
        circObject.GetComponent<PolygonCollider2D>().sharedMaterial = physicsMaterial;
        */
        //tileBehavior.TestAStar();   
    }



    void Update()
    {

        /*
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 pointMain = mainCamera.ScreenToWorldPoint(pos);
            tileBehavior.UnveilTile(pointMain); 
        }
        */
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 pointMain = mainCamera.ScreenToWorldPoint(pos);
            alreadyTagged = tileBehavior.TagTile(pointMain, 0);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 pointMain = mainCamera.ScreenToWorldPoint(pos);
            if (alreadyTagged)
            {
                tileBehavior.TagTile(pointMain, 1);
            }
            else
            {
                tileBehavior.TagTile(pointMain, 2);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            mainCamera.orthographicSize += 0.5f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            mainCamera.orthographicSize -= 0.5f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
            mainCamera.transform.Translate(0, -.1f, 0);
        if (Input.GetKey(KeyCode.UpArrow))
            mainCamera.transform.Translate(0, 0.1f, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            mainCamera.transform.Translate(0.1f, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            mainCamera.transform.Translate(-0.1f, 0, 0);
        if(Input.GetKey(KeyCode.Equals))
            mainCamera.orthographicSize -= 0.1f;
        if (Input.GetKey(KeyCode.Minus))
            mainCamera.orthographicSize += 0.1f;



        Level_01.Update(Time.deltaTime); 

        /*
        if (Input.GetMouseButtonDown(0))
        {
            tapTime = Time.time;

        }
        if (Input.GetMouseButtonUp(0))
        {
            float tDiffMS = (Time.time - tapTime) * 1000;

            if (tDiffMS < 250)
            {
                int[] tileIndex = tileBehavior.GetTileIndex(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
                tileBehavior.StartPath(tileIndex); 
            }

        }
        */
        creatureManager.UpdateAllCreatures(); 
        //EditorControlHandler.Update(creatureManager, controls);      
    }

    public void DestroyGameObject(GameObject obj)
    {
        Destroy(obj);
    }



    public void InitiateRandomWalk(TileBehavior tileBehavior, int length, Fighter fighter)
    {
        // IEnumerator coroutine = SearchPath(tileBehavior, length, fighter);
        // StartCoroutine(coroutine);
        staticTileBehavior = tileBehavior;
        Main.length = length;
        Main.fighter = fighter;
        Thread t = new Thread(NewThread);
        t.Start();

    }
    static TileBehavior staticTileBehavior;
    static int length;
    static Fighter fighter; 


    static void NewThread()
    {
        bool[,] tiles = staticTileBehavior.mapUnveiled;
        int[] currentPosition = staticTileBehavior.GetTileIndex(fighter.GetPosition());
        List<int[]> unveiledIndexList = staticTileBehavior.unveiledIndexList;
        List<Vector2> allPath = new List<Vector2>();

        for (int i = 0; i < length; i++)
        {

            int randomIndex;
            int[] tileIndex;
            List<Vector2> path; 

            System.Random rnd = new System.Random();

            // randomIndex = Random.Range(0, unveiledIndexList.Count);
            randomIndex = rnd.Next(0, unveiledIndexList.Count); 
            tileIndex = unveiledIndexList[randomIndex];
            path = new Astar(staticTileBehavior.mapUnveiledJagged, currentPosition, tileIndex, "Euclidean").result;


            while (path.Count <= 1)
            {
                //randomIndex = Random.Range(0, unveiledIndexList.Count);
                randomIndex = rnd.Next(0, unveiledIndexList.Count);
                tileIndex = unveiledIndexList[randomIndex];
                path = new Astar(staticTileBehavior.mapUnveiledJagged, currentPosition, tileIndex, "Euclidean").result;
            }

            foreach (Vector2 pathVec in path)
                allPath.Add(pathVec);

            currentPosition = tileIndex;
        }

        fighter.SetupWalkingPath(allPath, staticTileBehavior.mapPositionMatrix);

    }


}



/*
Vector2 spriteCenter = new Vector2(mapBounds.center.x, mapBounds.center.y);
Vector2 spriteBounds = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

int sqrtTiles = (int)Mathf.Sqrt(strawTiles.Length);

float tileWidth = (spriteBounds.x*2) / sqrtTiles;
float tileHeight = (spriteBounds.y*2) / sqrtTiles;

//Debug.Log("nTiles = " + sqrtTiles + " tileWidth = " + tileWidth + " tileHeight = " + tileHeight);
//Debug.Log("spritebounds.x = " + spriteBounds.x + " spritebounds.y = " + spriteBounds.y); 

int tileCount = 0; 

for (float j = spriteBounds.y; j > -spriteBounds.y + tileHeight; j-=tileHeight)
    for (float i = -spriteBounds.x; i < spriteBounds.x - tileWidth; i += tileWidth)
    {
        GameObject gobj = new GameObject();
        gobj.AddComponent<SpriteRenderer>();
        gobj.GetComponent<SpriteRenderer>().sprite = strawTiles[tileCount];
        gobj.transform.position = new Vector3(i,j,0); 

        tileCount++;
        //Debug.Log("tilecount = " + tileCount + "i = " + i + " j = " + j);
    }
*/
