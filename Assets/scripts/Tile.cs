using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{

    public GameObject baseObject; 
    float life;
    bool occupied; 

    public Tile(GameObject baseObject, float initialLife)
    {
        this.baseObject = baseObject; 
        life = initialLife; 

    }

    public void SetLife(float newLife) 
    {
        life = newLife; 
    }

    public void Hit(float damage)
    {
        life -= damage; 

    }

    public bool IsDestroyed()
    {
        if (life <= 0)
            return true;
        else
            return false; 
    }





    
}
