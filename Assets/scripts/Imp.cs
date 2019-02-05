using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp {

    Sprite[] walkingWeaponSprites;
    Sprite[] attackingBodySprites;
    Sprite[] walkingBodySprites;
    Sprite[] attackingWeaponSprites;

    PolygonCollider2D[] weaponColliders;

    public List<PolygonCollider2D> allColliders; 

    GameObject bodyWeaponObject;
    GameObject weaponBodyObject;
    GameObject bodySpriteObject;
    GameObject weaponSpriteObject;

    PhysicsMaterial2D physicsMaterial;

    public bool attacking = false;
    public bool moving = false;

    public float attackIndex;
    public float moveIndex;

    int nAttackSprites = 23;
    int nWalkingSprites = 25;

    Vector2 destinationVector;
    Vector2 directionVector;
    bool autoWalking;
    List<Vector2> currentPath;

    int currentPathIndex = 0;

    public bool movingToToggled = false;
    string taggedTileName = ""; 
    int tileAttackCount = 0;
    TileBehavior tileBehavior;
    Tile currentTile; 

    int attackingSoundIndex = 8;
    int hitCount = 0; 

    public Imp(Vector2 startingPosition)
    {
        Physics2D.gravity = Vector2.zero;

        physicsMaterial = new PhysicsMaterial2D();

        walkingWeaponSprites = Resources.LoadAll<Sprite>("players/staff_walking_weapon");
        attackingBodySprites = Resources.LoadAll<Sprite>("players/staff_attacking_body");
        walkingBodySprites = Resources.LoadAll<Sprite>("players/staff_walking_body");
        attackingWeaponSprites = Resources.LoadAll<Sprite>("players/staff_attacking_weapon");

        bodyWeaponObject = new GameObject();
        weaponBodyObject = new GameObject();
        bodySpriteObject = new GameObject();
        weaponSpriteObject = new GameObject();

        float scalef = 0.45f;
        bodyWeaponObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        weaponBodyObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        bodySpriteObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        weaponSpriteObject.transform.localScale = new Vector3(scalef, scalef, 1f);

        bodyWeaponObject.AddComponent<SpriteRenderer>();
        bodyWeaponObject.GetComponent<SpriteRenderer>().sprite = walkingWeaponSprites[0];
      //  bodyWeaponObject.AddComponent<PolygonCollider2D>();
       // bodyWeaponObject.GetComponent<PolygonCollider2D>().sharedMaterial = physicsMaterial;
        bodyWeaponObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        bodySpriteObject.AddComponent<SpriteRenderer>();
        bodySpriteObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[0];
        bodySpriteObject.AddComponent<Rigidbody2D>();
      //  bodySpriteObject.AddComponent<PolygonCollider2D>();
      //  bodySpriteObject.GetComponent<PolygonCollider2D>().sharedMaterial = physicsMaterial;

        bodySpriteObject.AddComponent<Hit>();

        bodyWeaponObject.transform.parent = bodySpriteObject.transform;
        bodySpriteObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

       // Physics2D.IgnoreCollision(bodySpriteObject.GetComponent<PolygonCollider2D>(), bodyWeaponObject.GetComponent<PolygonCollider2D>(), true);

      //  allColliders = new List<PolygonCollider2D>(); 
      //  allColliders.Add(bodySpriteObject.GetComponent<PolygonCollider2D>());
      //  allColliders.Add(bodyWeaponObject.GetComponent<PolygonCollider2D>());

        weaponBodyObject.AddComponent<SpriteRenderer>();
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[0];
        weaponBodyObject.AddComponent<Rigidbody2D>();
      //  weaponBodyObject.AddComponent<PolygonCollider2D>();
      //  weaponBodyObject.GetComponent<PolygonCollider2D>().sharedMaterial = physicsMaterial;

        weaponSpriteObject.AddComponent<SpriteRenderer>();
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[0];
        weaponSpriteObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        weaponBodyObject.SetActive(false);
        weaponSpriteObject.SetActive(false);

        weaponSpriteObject.transform.parent = weaponBodyObject.transform;
        weaponBodyObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

      //  Physics2D.IgnoreCollision(weaponBodyObject.GetComponent<PolygonCollider2D>(), weaponBodyObject.GetComponent<PolygonCollider2D>(), true);

     //   allColliders.Add(weaponBodyObject.GetComponent<PolygonCollider2D>());
     //   allColliders.Add(weaponBodyObject.GetComponent<PolygonCollider2D>());


        weaponColliders = new PolygonCollider2D[nAttackSprites];
        for (int i = 0; i < nAttackSprites; i++)
        {
            weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[i];
          //  PolygonCollider2D collider = weaponSpriteObject.AddComponent<PolygonCollider2D>();
           // collider.sharedMaterial = physicsMaterial;
         //   weaponColliders[i] = collider;
        }




    }

    /*
    public void IgnoreCollisionsWithOtherCharacter(Character other)
    {
        foreach (PolygonCollider2D collider in allColliders)
            foreach (PolygonCollider2D otherCollider in other.allColliders)
                Physics2D.IgnoreCollision(collider, otherCollider, true); 


    }
    */
    public void InitiateTileMove()
    {
        movingToToggled = true;
        tileAttackCount = 100; 
    }

    public void SetupWalkingPath(List<Vector2> path, Vector2[,] mapPositionMatrix, string taggedTileName, TileBehavior tileBehavior, Tile currentTile)
    {

        this.tileBehavior = tileBehavior; 
        this.taggedTileName = taggedTileName;
        this.currentTile = currentTile; 

        Vector2 currentPosition = bodySpriteObject.transform.position;
        Vector2 tilePosition = mapPositionMatrix[(int)path[1].x, (int)path[1].y];
        Vector2 directionToTile = (tilePosition - currentPosition).normalized;

        autoWalking = true;

        directionVector = directionToTile;
        destinationVector = tilePosition;

        currentPathIndex = 1;

        currentPath = new List<Vector2>();
        for (int i = 0; i < path.Count; i++)
            currentPath.Add(mapPositionMatrix[(int)path[i].x, (int)path[i].y]);
    }

    public void UpdateWalkingPath()
    {
        currentPathIndex++;
        if (currentPathIndex >= currentPath.Count)
        {
            autoWalking = false;       
        }
        else
        {
            Vector2 currentPosition = bodySpriteObject.transform.position;
            destinationVector = currentPath[currentPathIndex];
            directionVector = (destinationVector - currentPosition).normalized;
        }
    }

    public void CancelMovementToTagged(string canceledTileName)
    {
        if (canceledTileName.Equals(taggedTileName))
        {
            autoWalking = false;
            moving = false;
            movingToToggled = false;

            if (attacking)
                EndAttack(); 
            
            taggedTileName = ""; 
        }
    }

    public void CheckForMoreTagged()
    {
        tileBehavior.UpdateDestroyedAndRequestAnother(taggedTileName, bodyWeaponObject.transform.position); 
    }

    public void Update()
    {
        if (attacking == true)
        {
            ContinueAttack();

            int currentAttackIndex = (int)attackIndex++ % nAttackSprites;

            UpdateAttackingSprite(currentAttackIndex);

            if (currentAttackIndex == attackingSoundIndex && !currentTile.IsDestroyed())
            {
                Main.audioSource.PlayOneShot(Main.tin, 0.2f);
                tileBehavior.DegradeTileAlpha(taggedTileName, hitCount++);
                currentTile.Hit(1); 
            }

            if (movingToToggled && tileAttackCount > 0)
            {
                tileAttackCount--;
                UpdateRotation((int)Vector2.SignedAngle(Vector2.up, directionVector));
            }
            if (movingToToggled && currentTile.IsDestroyed())
            {
                movingToToggled = false; 
                EndAttack();
                CheckForMoreTagged();
            }
        }
        else if (moving == true)
        {
            UpdateWalkingSprite((int)moveIndex++ % nWalkingSprites);
        }
        else if (autoWalking == true)
        {
            Vector2 currentPosition = bodySpriteObject.transform.position;
            float norm = (currentPosition - destinationVector).magnitude;

            float rand = Random.Range(0.1f, 0.55f);
            float rand2 = Random.Range(0.01f, 0.13f);

            if (currentPathIndex >= currentPath.Count - 1 && movingToToggled == true && norm < rand)
            {
                OnAttack(); 
                autoWalking = false; 
            }
            else if (norm > rand2)
            {
                UpdateRotation((int)Vector2.SignedAngle(Vector2.up, directionVector));
                UpdatePosition(directionVector);
                UpdateWalkingSprite((int)moveIndex++ % nWalkingSprites);
            }
            else
            {
                UpdateWalkingPath();  
            }
        }
    }


    public void UpdateRotation(int moveRotAngle)
    {
            bodySpriteObject.transform.rotation = Quaternion.Euler(0f, 0f, moveRotAngle);
            weaponBodyObject.transform.rotation = Quaternion.Euler(0f, 0f, moveRotAngle); 
    }

    public void UpdatePosition(Vector3 newPositionVector)
    {
        if ((moving == true || autoWalking == true) && attacking == false)
        {
            bodySpriteObject.GetComponent<Rigidbody2D>().velocity = new Vector3();
            bodySpriteObject.transform.position += newPositionVector * 1 * Time.deltaTime;
        }
    }

    public void UpdateWalkingSprite(int spriteIndex)
    {
        bodyWeaponObject.GetComponent<SpriteRenderer>().sprite = walkingWeaponSprites[spriteIndex];
        bodySpriteObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[spriteIndex];
    }

    public void UpdateAttackingSprite(int spriteIndex)
    {
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[spriteIndex];
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[spriteIndex];
    }

    public void OnAttack()
    {
        //Main.audioSource.PlayOneShot(Main.tin, 1f);

        attacking = true; 

        bodyWeaponObject.SetActive(false);
        bodySpriteObject.SetActive(false);

        weaponBodyObject.transform.localRotation = bodySpriteObject.transform.localRotation;
        weaponBodyObject.transform.position = bodySpriteObject.transform.position; 

        weaponSpriteObject.SetActive(true);
        weaponBodyObject.SetActive(true);

        attackIndex = 0; 
    }

    public void ContinueAttack()
    {
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[(int)attackIndex% nAttackSprites];
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[(int)attackIndex% nAttackSprites];

        /*
        for (int i = 0; i < weaponColliders.Length; i++)
        {
            if (i == (int)attackIndex % nAttackSprites)
                weaponColliders[i].enabled = true;
            else weaponColliders[i].enabled = false; 
        }
        */
    }

    public void EndAttack()
    {
        attacking = false;
        hitCount = 0; 

        bodyWeaponObject.SetActive(true);
        bodySpriteObject.SetActive(true);

        weaponSpriteObject.SetActive(false);
        weaponBodyObject.SetActive(false);

        bodySpriteObject.transform.localRotation = weaponBodyObject.transform.localRotation;
        bodySpriteObject.transform.position = weaponBodyObject.transform.position;

    }

    public void EndMovement()
    {
        moving = false; 
    }

    public Vector2 GetPosition()
    {
        return bodySpriteObject.transform.position; 

    }

}
