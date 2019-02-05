using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour {


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("ENTER collision with " + collision.collider.gameObject.name); 
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("STAY collision with " + collision.collider.gameObject.name);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("EXIT collision with " + collision.collider.gameObject.name);
    }



}
