using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
 
    Camera mainCamera;
    public static AudioSource audioSource;
    public static AudioClip marble;
    public static AudioClip tin;
    public static AudioClip wood;
    public static AudioClip stab; 

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

        creatureManager = new CreatureManager();

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
