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
    public static AudioClip goldFalling;
    public static AudioClip goldDeposit;
    public static AudioClip pickupGold; 

    TileBehavior tileBehavior;
    RoomPathFinder roomPathFinder;
    CreatureManager creatureManager; 

    Sprite circSprite;
    PhysicsMaterial2D physicsMaterial;
    GameObject circObject;
    Controls controls;

    float tapTime = 0f;

    bool alreadyTagged = false;

    public GameObject smallSphere;
    List<GameObject> spheres;

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
        goldFalling = Resources.Load<AudioClip>("audios/drop_gold");
        goldDeposit = Resources.Load<AudioClip>("audios/deposit_gold");
        pickupGold = Resources.Load<AudioClip>("audios/pickup_gold"); 


        creatureManager = new CreatureManager();
        creatureManager.main = this; 

        tileBehavior = new TileBehavior(this, creatureManager);
        creatureManager.SetTileBehavior(tileBehavior);

        /*
        Physics2D.gravity = Vector2.zero;
        Physics2D.velocityThreshold = 0.001f;

        Physics2D.IgnoreLayerCollision(0, 1, true);

        physicsMaterial = new PhysicsMaterial2D();
        physicsMaterial.bounciness = 1;
        */
 
    }



    void Update()
    {

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
        //UpdatePathSpheres(); 

        
        
        creatureManager.UpdateAllCreatures(); 
    }

    public void DestroyGameObject(GameObject obj)
    {
        Destroy(obj);
    }



    public void InitiateRandomWalk(TileBehavior tileBehavior, int length, Fighter fighter)
    {
        Thread t = new Thread(()=>ComputePathAsync(tileBehavior,fighter,length));
        t.Start();

    }

    void ComputePathAsync(TileBehavior tileBehavior, Fighter fighter, int length)
    {
        bool[,] tiles = tileBehavior.mapUnveiled;
        int[] currentPosition = tileBehavior.GetTileIndex(fighter.GetPosition());
        List<int[]> unveiledIndexList = tileBehavior.unveiledIndexList;
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
            path = new Astar(tileBehavior.mapUnveiledJagged, currentPosition, tileIndex, "Euclidean").result;


            while (path.Count <= 1)
            {
                //randomIndex = Random.Range(0, unveiledIndexList.Count);
                randomIndex = rnd.Next(0, unveiledIndexList.Count);
                tileIndex = unveiledIndexList[randomIndex];
                path = new Astar(tileBehavior.mapUnveiledJagged, currentPosition, tileIndex, "Euclidean").result;
            }

            foreach (Vector2 pathVec in path)
                allPath.Add(pathVec);

            currentPosition = tileIndex;
        }

        fighter.SetupWalkingPath(allPath, tileBehavior.mapPositionMatrix);

    }

    public void UpdatePathSpheres()
    {
        try
        {
            Fighter imp = creatureManager.fightersFriendly[0];
            if (spheres == null)
            {
                spheres = new List<GameObject>();
            }
            if (imp.currentPath != null)
                for (int i = 0; i < imp.currentPath.Count; i++)
                {
                    if (spheres.Count <= i)
                    {
                        GameObject newSphere = Instantiate(Resources.Load("prefabs/pathsphere")) as GameObject;
                        spheres.Add(newSphere);
                    }
                    spheres[i].transform.position = new Vector3(imp.currentPath[i].x, imp.currentPath[i].y, -2.5f);
                }
        }
        catch (Exception e) { }

    }
}


