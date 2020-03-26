using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UI; 


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

    public Text goldTxt;
    public int goldCount = 0; 
    public static Text nCreaturesTxt; 

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

    private static float prevPinchDist = 0;

    GameObject controlSphere;
    private bool scrolling = false;
    private float prevDown = 0;
    private Vector2 currentScrollPos; 

    void Start()
    {
        Debug.Log("loading audio grid_lvl"); 
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
        Debug.Log("DONE loading audio grid_lvl");


        // controlSphere = GameObject.Find("Sphere");
        Vector3 botRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width / 10, 
                            Screen.height / 10,0));
      //  controlSphere.transform.position = new Vector3(botRight.x,botRight.y,-4);

        creatureManager = new CreatureManager();
        creatureManager.main = this; 

        tileBehavior = new TileBehavior(this, creatureManager);
        creatureManager.SetTileBehavior(tileBehavior);

        mainCamera.transform.Translate(new Vector3(2.4f, -2.6f, 0));

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

        if (Input.touchCount >= 2)
        {
            
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);
            if (prevPinchDist != 0 && distance < prevPinchDist)
            {
                Camera.current.orthographicSize += 0.05f;
            }
            else if (prevPinchDist != 0 && distance > prevPinchDist)
            {
                Camera.current.orthographicSize -= 0.05f;
            }
            Vector3 botRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width / 10,
                         Screen.height / 10, 0));
          //  controlSphere.transform.position = new Vector3(botRight.x, botRight.y, -4);
            prevPinchDist = distance;
        }
        else if (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && scrolling))
        {
            Debug.Log("pressed, y=" + Input.mousePosition.y);
            if (Input.mousePosition.y <275 && Input.mousePosition.x > Screen.width / 2 || scrolling)
            {
                Vector2 mousePos = Input.mousePosition;
                
                if (scrolling == true)
                {
                    double xDist = mousePos.x - currentScrollPos.x;
                    double yDist = mousePos.y - currentScrollPos.y;
                    mainCamera.transform.Translate((float)xDist / 1000, (float)yDist / 1500, 0);
                }
                else
                {
                    currentScrollPos = mousePos;
                    scrolling = true;
                }
            }
            /*
            Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

            double dist = Math.Sqrt(Math.Pow(touchPosWorld2D.x - controlSphere.transform.position.x, 2) +
                Math.Pow(touchPosWorld2D.y - controlSphere.transform.position.y, 2));
            if (dist < 0.5 || scrolling)
            {
                scrolling = true;
                double yDist = touchPosWorld2D.y - controlSphere.transform.position.y;
                double xDist = touchPosWorld2D.x - controlSphere.transform.position.x;

                mainCamera.transform.Translate((float)xDist / 2, (float)yDist / 2, 0);
                controlSphere.transform.Translate(new Vector3((float)xDist / 2, (float)yDist / 2, 0));
            }
            */
        }
        else if (Input.GetMouseButtonUp(0))
        {
            scrolling = false;
        }


        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            creatureManager.SpawnImp(new Vector2(-0.5f,-0.5f)); 
        }

        if (Input.GetMouseButtonDown(0) && scrolling == false)
        {
            Vector3 pos = Input.mousePosition;
            Vector3 pointMain = mainCamera.ScreenToWorldPoint(pos);
            alreadyTagged = tileBehavior.TagTile(pointMain, 0);
        }
        else if (Input.GetMouseButton(0) && scrolling == false)
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


