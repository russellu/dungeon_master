using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlHandler {

    static float count;

    static Vector2 movePadCenter = new Vector2(-2, -1);
    static Vector2 attackPadCenter = new Vector2(2, -1);

    static Touch touch1;
    static Touch touch2;

    static int motionPadIndex = -1;
    static int attackPadIndex = -1;

	public static void Update(Imp character, Controls controls) {

        character.Update();

        int touchCount = Input.touchCount;
        if (touchCount > 1)
        {
            touch1 = Input.GetTouch(0);
            touch2 = Input.GetTouch(1);
            Vector2 position1 = new Vector2(touch1.position.x, touch1.position.y);
            Vector2 position2 = new Vector2(touch2.position.x, touch2.position.y);

            if (touch1.phase.Equals(TouchPhase.Began))
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position1), Vector2.zero);
                if (hitInfo)
                {
                    if (hitInfo.transform.gameObject.name.Equals("movepad"))
                    {
                        motionPadIndex = 0;
                        character.moving = true;
                    }
                    else if (hitInfo.transform.gameObject.name.Equals("attackpad"))
                    {
                        attackPadIndex = 0;
                        character.OnAttack();
                    }
                }
            }
            else if (touch1.phase.Equals(TouchPhase.Moved) || touch1.phase.Equals(TouchPhase.Stationary))
            {
                Vector2 newPosition = new Vector2();
                if (motionPadIndex == 0)
                    newPosition = Camera.main.ScreenToWorldPoint(new Vector2(touch1.position.x, touch1.position.y));

                Vector2 moveDragDirection = newPosition - movePadCenter;

                if (motionPadIndex == 0)
                { 
                    character.UpdateRotation((int)Vector2.SignedAngle(Vector2.up, moveDragDirection.normalized));
                    character.UpdatePosition(moveDragDirection.normalized);
                }
            }
            else if (touch1.phase.Equals(TouchPhase.Ended))
            {
                if (attackPadIndex == 0)
                {
                    character.EndAttack();
                    attackPadIndex = -1;
                }
                else if (motionPadIndex == 0)
                {
                    character.EndMovement();
                    motionPadIndex = -1;
                }
            }

            if (touch2.phase.Equals(TouchPhase.Began))
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position2), Vector2.zero);
                if (hitInfo)
                {
                    if (hitInfo.transform.gameObject.name.Equals("movepad"))
                    {
                        motionPadIndex = 1;
                        character.moving = true;
                    }
                    else if (hitInfo.transform.gameObject.name.Equals("attackpad"))
                    {
                        attackPadIndex = 1;
                        character.OnAttack();
                    }
                }
            }
            else if (touch2.phase.Equals(TouchPhase.Moved) || touch2.phase.Equals(TouchPhase.Stationary))
            {
                Vector2 newPosition = new Vector2();
                if (motionPadIndex == 1)
                    newPosition = Camera.main.ScreenToWorldPoint(new Vector2(touch2.position.x, touch2.position.y));

                Vector2 moveDragDirection = newPosition - movePadCenter;
                if(motionPadIndex == 1)
                { 
                    character.UpdateRotation((int)Vector2.SignedAngle(Vector2.up, moveDragDirection.normalized));
                    character.UpdatePosition(moveDragDirection.normalized);
                }
            }
            else if (touch2.phase.Equals(TouchPhase.Ended))
            {
                if (attackPadIndex == 1)
                {
                    character.EndAttack();
                    attackPadIndex = -1;
                }
                else if (motionPadIndex == 1)
                {
                    character.EndMovement();
                    motionPadIndex = -1;
                }
            }

        }
        else if (touchCount == 1)
        {
            touch1 = Input.GetTouch(0);
            Vector2 position1 = new Vector2(touch1.position.x, touch1.position.y);
            if (touch1.phase.Equals(TouchPhase.Began))
            {
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position1), Vector2.zero);
                if (hitInfo)
                {
                    if (hitInfo.transform.gameObject.name.Equals("movepad"))
                    {
                        motionPadIndex = 0;
                        attackPadIndex = -1;
                        character.moving = true;
                    }
                    else if (hitInfo.transform.gameObject.name.Equals("attackpad"))
                    {
                        attackPadIndex = 0;
                        motionPadIndex = -1;
                        character.OnAttack();
                    }
                }
            }
            else if (touch1.phase.Equals(TouchPhase.Moved) || touch1.phase.Equals(TouchPhase.Stationary))
            {
                Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector2(touch1.position.x, touch1.position.y));
                if (motionPadIndex == 0 || motionPadIndex == 1)
                {
                    Vector2 moveDragDirection = newPosition - movePadCenter;
                    character.UpdateRotation((int)Vector2.SignedAngle(Vector2.up, moveDragDirection.normalized));
                    character.UpdatePosition(moveDragDirection.normalized);
                }
            }
            else if (touch1.phase.Equals(TouchPhase.Ended))
            {
                if (motionPadIndex == 0 || motionPadIndex == 1)
                {
                    character.EndMovement();
                    motionPadIndex = -1;
                }
                else
                {
                    character.EndAttack();
                    attackPadIndex = -1;
                }
            }
        }
    }
}
