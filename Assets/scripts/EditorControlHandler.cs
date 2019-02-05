using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorControlHandler {

    static Vector2 movePadCenter = new Vector2(-2, -1);
    static bool motionPadClicked = false;
    static float tapTime = 0; 


    public static void Update(CreatureManager creatureManager, Controls controls)
    {
        //character.Update();






        /*

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 position1 = new Vector2(Input.mousePosition.x, Input.mousePosition.y); 
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position1), Vector2.zero);
            if (hitInfo)
            {
                if (hitInfo.transform.gameObject.name.Equals("movepad"))
                {
                    character.moving = true;
                    motionPadClicked = true; 
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 moveDragDirection = newPosition - movePadCenter;
            character.UpdateRotation((int)Vector2.SignedAngle(Vector2.up, moveDragDirection.normalized));
            character.UpdatePosition(moveDragDirection.normalized);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            motionPadClicked = false;
            character.EndMovement(); 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.OnAttack(); 
        }
        else if (Input.GetKey(KeyCode.Space))
        {


        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            character.EndAttack(); 

        }

    */
    }
}
