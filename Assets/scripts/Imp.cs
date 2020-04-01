using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp {

    GameObject shadowSphere; 

    Sprite[] walkingWeaponSprites;
    Sprite[] attackingBodySprites;
    Sprite[] walkingBodySprites;
    Sprite[] attackingWeaponSprites;

    PolygonCollider2D[] weaponColliders;

    public List<PolygonCollider2D> allColliders; 

    GameObject walkingWeaponObject;
    GameObject attackingBodyObject;
    GameObject walkingBodyObject;
    GameObject attackingWeaponObject;

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

    bool movingToItem = false;
    int[] itemLocation; 
    public GameObject currentItemSeeking;
    GameObject currentItemHeld;
    string currentObjective; 

    int attackingSoundIndex = 8;
    int hitCount = 0;

    int claimCount = 0;

    GameObject currRubble;
    Vector2 currentClaimPosition; 

    public Imp(Vector2 startingPosition)
    {
        shadowSphere = Main.Instantiate(Resources.Load("prefabs/Cylinder")) as GameObject;
        shadowSphere.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3.1f);
        shadowSphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0f);
        Material material = new Material(Shader.Find("Custom/TransparentShadowCollector"));
        shadowSphere.GetComponent<Renderer>().material = material;
        Physics2D.gravity = Vector2.zero;
        float startZ = -3.3f; 

        physicsMaterial = new PhysicsMaterial2D();

        walkingWeaponSprites = Resources.LoadAll<Sprite>("players/staff_walking_weapon");
        attackingBodySprites = Resources.LoadAll<Sprite>("players/staff_attacking_body");
        walkingBodySprites = Resources.LoadAll<Sprite>("players/staff_walking_body");
        attackingWeaponSprites = Resources.LoadAll<Sprite>("players/staff_attacking_weapon");

        walkingWeaponObject = new GameObject();
        attackingBodyObject = new GameObject();
        walkingBodyObject = new GameObject();
        attackingWeaponObject = new GameObject();

        float scalef = 0.45f;
        walkingWeaponObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        attackingBodyObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        walkingBodyObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        attackingWeaponObject.transform.localScale = new Vector3(scalef, scalef, 1f);

        walkingWeaponObject.AddComponent<SpriteRenderer>();
        walkingWeaponObject.GetComponent<SpriteRenderer>().sprite = walkingWeaponSprites[0];
        walkingWeaponObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        walkingBodyObject.AddComponent<SpriteRenderer>();
        walkingBodyObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[0];
        walkingBodyObject.AddComponent<Rigidbody2D>();

        walkingBodyObject.AddComponent<Hit>();
        walkingBodyObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);

        walkingWeaponObject.transform.parent = walkingBodyObject.transform;
        walkingBodyObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, startZ);

        attackingBodyObject.AddComponent<SpriteRenderer>();
        attackingBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[0];
        attackingBodyObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f); 
        attackingBodyObject.AddComponent<Rigidbody2D>();
 
        attackingWeaponObject.AddComponent<SpriteRenderer>();
        attackingWeaponObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[0];
        attackingWeaponObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        attackingBodyObject.SetActive(false);
        attackingWeaponObject.SetActive(false);

        attackingWeaponObject.transform.parent = attackingBodyObject.transform;
        attackingBodyObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, startZ);

        weaponColliders = new PolygonCollider2D[nAttackSprites];
        for (int i = 0; i < nAttackSprites; i++)
        {
            attackingWeaponObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[i];
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
        Debug.Log("pathx = " + path[0].x + " pathy = " + path[0].y); 
        Vector2 currentPosition = walkingBodyObject.transform.position;
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
            Vector2 currentPosition = walkingBodyObject.transform.position;
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
        bool another = tileBehavior.UpdateDestroyedAndRequestAnother(taggedTileName, walkingWeaponObject.transform.position);

        if (!another)
        {
            if (Level_01.goldLocsNotPickedUp.Count > 0 && currentObjective != "drop_off_gold")
            {
                Debug.Log("collecting gold now, count=" + Level_01.goldsNotPickedUp.Count);
                currentObjective = "pick_up_gold";
                int[] goldLocation = Level_01.goldLocsNotPickedUp.Peek(); 
                tileBehavior.StartPath(this, goldLocation);
                currentItemSeeking = Level_01.golds[Level_01.goldNotPickedUpInds.Peek()]; 
                movingToItem = true;
                Level_01.goldLocsNotPickedUp.Dequeue();
                Level_01.goldNotPickedUpInds.Dequeue();
            }
            else if (currentObjective == "drop_off_gold")//other conditions
            {
 

                int[] smelterLocation = Smelter.smelterLocations[0];
                tileBehavior.StartPath(this, smelterLocation);
                movingToItem = true; 


            }
            else // look for some unclaimed tiles
            {
                if (tileBehavior.rubble.Count > 0) 
                {
                    GameObject rubble = tileBehavior.ClosestRubble(GetPosition());
                    if (rubble != null)
                    {
                        currentObjective = "claim_tile";
                        tileBehavior.rubbleClaiming.Add(rubble);
                        tileBehavior.StartPath(this, tileBehavior.GetTileIndex(rubble.transform.position));
                        currentItemSeeking = rubble;
                        movingToItem = true;
                        currRubble = rubble;
                    }
                }

            }
        }
    }

    public void Update()
    {

        if (claimCount > 0) {
            ReduceClaimCount();
            return; 
        }

        if (attacking == true)
        {
            ContinueAttack();
            int currentAttackIndex = (int)attackIndex++ % nAttackSprites;
            UpdateAttackingSprite(currentAttackIndex);

            if (currentAttackIndex == attackingSoundIndex && !currentTile.IsDestroyed())
            {
                UpdateRotation((int)Vector2.SignedAngle(Vector2.up, currentTile.baseObject.transform.position- walkingBodyObject.transform.position));
                Main.audioSource.PlayOneShot(Main.tin, 0.2f);
                tileBehavior.DegradeTileAlpha(taggedTileName, hitCount++);
                currentTile.Hit(1); 
            }

            if (movingToToggled && tileAttackCount > 0)
            {
                tileAttackCount--;
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
            Vector2 currentPosition = walkingBodyObject.transform.position;
            float norm = (currentPosition - destinationVector).magnitude;
            float rand2 = Random.Range(0.05f, 0.12f);

            if (currentPathIndex >= currentPath.Count - 1 && movingToToggled == true && norm < 0.4f)
            {
                OnAttack();
                autoWalking = false;
            }
            else if (movingToItem == true && currentItemSeeking == null) //another imp got to it first
            {
                movingToItem = false;
                CheckForMoreTagged();
            }
            else if (currentPathIndex >= currentPath.Count - 1 && movingToItem == true && norm < 0.2)
            {

                OnObjective();
                CheckForMoreTagged();
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
            walkingBodyObject.transform.rotation = Quaternion.Euler(0f, 0f, moveRotAngle);
            attackingBodyObject.transform.rotation = Quaternion.Euler(0f, 0f, moveRotAngle);

    }

    public void UpdatePosition(Vector3 newPositionVector)
    {
        if ((moving == true || autoWalking == true) && attacking == false)
        {
            walkingBodyObject.GetComponent<Rigidbody2D>().velocity = new Vector3();
            walkingBodyObject.transform.position += newPositionVector * 1 * Time.deltaTime*1.5f;
            shadowSphere.transform.position = walkingBodyObject.transform.position; 
           shadowSphere.transform.position = new Vector3(walkingBodyObject.transform.position.x,
               walkingBodyObject.transform.position.y, -3.1f) ; 
        }
    }

    public void UpdateWalkingSprite(int spriteIndex)
    {
        walkingWeaponObject.GetComponent<SpriteRenderer>().sprite = walkingWeaponSprites[spriteIndex];
        walkingBodyObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[spriteIndex];
    }

    public void UpdateAttackingSprite(int spriteIndex)
    {
        attackingBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[spriteIndex];
        attackingWeaponObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[spriteIndex];
    }

    public void OnObjective() 
    {
        if (currentObjective == "pick_up_gold")
        {
            Level_01.RemoveGold(currentItemSeeking);
            currentObjective = "drop_off_gold";
            currentItemSeeking = Smelter.smelters[0];

            Main.audioSource.PlayOneShot(Main.pickupGold, 2);
        }
        else if (currentObjective == "drop_off_gold")
        {
            Debug.Log("dropping off gold");
            currentObjective = "";
            Main.audioSource.PlayOneShot(Main.goldDeposit, 2);
            tileBehavior.levelState.updateGold(Random.Range(0, 2));

            Smelter.heartLight.GetComponent<Light>().intensity += 0.1f; 

        }
        else if (currentObjective == "claim_tile") {
            tileBehavior.ClaimTile(this, currentItemSeeking);
            currentObjective = "";
            tileBehavior.levelState.updateStone(Random.Range(0, 2));
        }
    }


    public void DoClaim(Vector2 currentRubblePosition) {
        // run claiming animation
        // pause for a second
        // replace the dirt with a tile 
        currentClaimPosition = currentRubblePosition; 
        claimCount = 20; 

    }

    public void ReduceClaimCount() {
        claimCount--;
        if (claimCount == 0)
            tileBehavior.FinishClaimTile(currentClaimPosition); 
    }


    public void OnAttack()
    {
        attacking = true; 

        walkingWeaponObject.SetActive(false);
        walkingBodyObject.SetActive(false);

        attackingBodyObject.transform.localRotation = walkingBodyObject.transform.localRotation;
        attackingBodyObject.transform.position = walkingBodyObject.transform.position; 

        attackingWeaponObject.SetActive(true);
        attackingBodyObject.SetActive(true);

        attackIndex = 0; 
    }

    public void ContinueAttack()
    {
        attackingWeaponObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[(int)attackIndex% nAttackSprites];
        attackingBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[(int)attackIndex% nAttackSprites];
    }

    public void EndAttack()
    {
        attacking = false;
        hitCount = 0; 

        walkingWeaponObject.SetActive(true);
        walkingBodyObject.SetActive(true);

        attackingWeaponObject.SetActive(false);
        attackingBodyObject.SetActive(false);

        walkingBodyObject.transform.localRotation = attackingBodyObject.transform.localRotation;
        walkingBodyObject.transform.position = attackingBodyObject.transform.position;

    }

    public void EndMovement()
    {
        moving = false; 
    }

    public Vector2 GetPosition()
    {
        return walkingBodyObject.transform.position; 

    }

}
