using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls {

    Sprite attackPadSprite;
    Sprite movePadSprite;

    GameObject attackPadObject;
    GameObject movePadObject;

    bool attackPadDown = false;
    bool movePadDown = false; 

    Main main; 

    public Controls(Main main)
    {
        this.main = main; 

        attackPadSprite = Resources.Load<Sprite>("players/attackpad") as Sprite;
        movePadSprite = Resources.Load<Sprite>("players/movepad") as Sprite;

        attackPadObject = new GameObject("attackpad");
        movePadObject = new GameObject("movepad");

        attackPadObject.AddComponent<SpriteRenderer>();
        attackPadObject.GetComponent<SpriteRenderer>().sprite = attackPadSprite;
        attackPadObject.AddComponent<BoxCollider2D>();
        attackPadObject.layer = 1;

        movePadObject.AddComponent<SpriteRenderer>();
        movePadObject.GetComponent<SpriteRenderer>().sprite = movePadSprite;
        movePadObject.AddComponent<BoxCollider2D>();
        movePadObject.layer = 1; 

        movePadObject.transform.localScale = movePadObject.transform.localScale / 1.5f;
        attackPadObject.transform.localScale = attackPadObject.transform.localScale / 1.5f; 

        attackPadObject.transform.position = new Vector3(2f,-1.0f,-3f);
        movePadObject.transform.position = new Vector3(-2f, -1.0f, -3f);

        attackPadObject.SetActive(false);
        movePadObject.SetActive(false); 
    }

    private Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }


}
