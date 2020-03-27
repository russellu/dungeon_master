using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation
{

    private Sprite[] animation;
    private Vector2 position;
    private int animCount = 0;
    GameObject animationTile; 

    public SpriteAnimation(Sprite[] animation, Vector2 position) {
        this.animation = animation;
        this.position = position;

        animationTile = new GameObject("animation");
        animationTile.AddComponent<SpriteRenderer>();
        animationTile.GetComponent<SpriteRenderer>().sprite = animation[0];
        animationTile.transform.position = new Vector3(position.x, position.y, -3.25f);
    }

    public GameObject GetAnimationTile() {
        return animationTile; 
    }
    

    public bool AdvanceAnimation() {
        //Debug.Log("animating!"); 
        animCount++;
        if (animCount < animation.Length)
        {
            animationTile.GetComponent<SpriteRenderer>().sprite = animation[animCount]; 
            return false;
        }
        else {
            return true;    
        }
    
    }

}
