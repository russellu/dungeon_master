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
    public List<Vector2> currentPath;

    int currentPathIndex = 0;

    public bool movingToToggled = false;
    string taggedTileName = ""; 
    int tileAttackCount = 0;
    TileBehavior tileBehavior;
    Tile currentTile;

    public bool movingToItem = false;
    public GameObject item; 


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
        bodyWeaponObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        bodySpriteObject.AddComponent<SpriteRenderer>();
        bodySpriteObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[0];
        bodySpriteObject.AddComponent<Rigidbody2D>();

        bodySpriteObject.AddComponent<Hit>();
        bodySpriteObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);

        bodyWeaponObject.transform.parent = bodySpriteObject.transform;
        bodySpriteObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

        weaponBodyObject.AddComponent<SpriteRenderer>();
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[0];
        weaponBodyObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f); 
        weaponBodyObject.AddComponent<Rigidbody2D>();
 
        weaponSpriteObject.AddComponent<SpriteRenderer>();
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[0];
        weaponSpriteObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        weaponBodyObject.SetActive(false);
        weaponSpriteObject.SetActive(false);

        weaponSpriteObject.transform.parent = weaponBodyObject.transform;
        weaponBodyObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

        weaponColliders = new PolygonCollider2D[nAttackSprites];
        for (int i = 0; i < nAttackSprites; i++)
        {
            weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[i];
        }
    }

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
        Vector2 tilePosition = mapPositionMatrix[(int)path[0].x, (int)path[0].y];
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
        bool another = tileBehavior.UpdateDestroyedAndRequestAnother(taggedTileName, bodyWeaponObject.transform.position);

        if (!another)
        {
            if(Level_01.goldNotPickedUp.Count>0)
            {
                Debug.Log("collecting gold now, count=" + Level_01.goldNotPickedUp.Count);
                int[] goldLocation = Level_01.goldNotPickedUp[Level_01.goldNotPickedUp.Count - 1];
                
                tileBehavior.StartPath(goldLocation);
                item = Level_01.golds[Level_01.goldNotPickedUpInds[Level_01.goldNotPickedUpInds.Count-1]]; 
                movingToItem = true; 

            }
        }
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
            else if (movingToItem == true && item.activeInHierarchy == false) //another imp got to it first
            {
              //  Debug.Log("aw shucks");
                CheckForMoreTagged(); 

            }
            else if (currentPathIndex >= currentPath.Count - 1 && movingToItem == true && norm < rand)
            {
                OnPickup();
                movingToItem = false;
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

    public void OnPickup() 
    {
        Debug.Log("picking up gold");
        item.SetActive(false);
        Level_01.goldNotPickedUp.RemoveAt(Level_01.goldNotPickedUp.Count - 1);
        Level_01.goldNotPickedUpInds.RemoveAt(Level_01.goldNotPickedUpInds.Count - 1); 

    }


    public void OnAttack()
    {
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
