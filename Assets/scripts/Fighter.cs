using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter
{



    Sprite[] walkingWeaponSprites;
    Sprite[] attackingBodySprites;
    Sprite[] walkingBodySprites;
    Sprite[] attackingWeaponSprites;

    CircleCollider2D[] weaponColliders;

    public List<CircleCollider2D> allColliders;

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

    int attackingSoundIndex = 8;
    int hitCount = 0;

    public bool wagingWar = false;
    float speed = 1f;

    private Fighter currentTarget;
    private int[] currentTargetGridPos;

    private Vector3 position;

    float hitPoints = 5;
    float attackDamage = 1;
    float weaponRange = 0.3f;

    bool isDead = false; 

    CreatureManager creatureManager; 

    public Fighter(CreatureManager creatureManager, Vector3 startingPosition, int teamNumber, float startingSpeed)
    {
        this.creatureManager = creatureManager; 

        speed = startingSpeed; 
        Color teamColor = new Color(0f,0f,0f);
        position = startingPosition; 

        if (teamNumber == 0)
        {
            teamColor = new Color(1f, 1f, 1f); 
        }
        else if (teamNumber == 1)
        {
            teamColor = new Color(0.9f, 0.35f, 0.0f); 
        }

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

        float scalef = 0.55f;
        bodyWeaponObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        weaponBodyObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        bodySpriteObject.transform.localScale = new Vector3(scalef, scalef, 1f);
        weaponSpriteObject.transform.localScale = new Vector3(scalef, scalef, 1f);

        bodyWeaponObject.AddComponent<SpriteRenderer>();
        bodyWeaponObject.GetComponent<SpriteRenderer>().sprite = walkingWeaponSprites[0];

        //  bodyWeaponObject.AddComponent<CircleCollider2D>();
       //  bodyWeaponObject.GetComponent<CircleCollider2D>().sharedMaterial = physicsMaterial;


        bodyWeaponObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        bodySpriteObject.AddComponent<SpriteRenderer>();
        bodySpriteObject.GetComponent<SpriteRenderer>().sprite = walkingBodySprites[0];
        bodySpriteObject.AddComponent<Rigidbody2D>();
        bodySpriteObject.GetComponent<SpriteRenderer>().color = new Color(0.5f,0.5f,0.5f,1f);

       //  bodySpriteObject.AddComponent<CircleCollider2D>();
       //  bodySpriteObject.GetComponent<CircleCollider2D>().sharedMaterial = physicsMaterial;
       //  bodySpriteObject.GetComponent<CircleCollider2D>().radius = 0.085f; 


        bodySpriteObject.AddComponent<Hit>();

        bodyWeaponObject.transform.parent = bodySpriteObject.transform;
        bodySpriteObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

       // Physics2D.IgnoreCollision(bodySpriteObject.GetComponent<CircleCollider2D>(), bodyWeaponObject.GetComponent<CircleCollider2D>(), true);

        //  allColliders = new List<CircleCollider2D>(); 
        //  allColliders.Add(bodySpriteObject.GetComponent<CircleCollider2D>());
        //  allColliders.Add(bodyWeaponObject.GetComponent<CircleCollider2D>());

        weaponBodyObject.AddComponent<SpriteRenderer>();
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[0];
        weaponBodyObject.AddComponent<Rigidbody2D>();
        weaponBodyObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);

        //     weaponBodyObject.AddComponent<CircleCollider2D>();
        //      weaponBodyObject.GetComponent<CircleCollider2D>().sharedMaterial = physicsMaterial;


        weaponSpriteObject.AddComponent<SpriteRenderer>();
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[0];
        weaponSpriteObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);


        weaponBodyObject.SetActive(false);
        weaponSpriteObject.SetActive(false);

        weaponSpriteObject.transform.parent = weaponBodyObject.transform;
        weaponBodyObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, -3);

        //  Physics2D.IgnoreCollision(weaponBodyObject.GetComponent<CircleCollider2D>(), weaponBodyObject.GetComponent<CircleCollider2D>(), true);
        //   allColliders.Add(weaponBodyObject.GetComponent<CircleCollider2D>());
        //   allColliders.Add(weaponBodyObject.GetComponent<CircleCollider2D>());

        
        weaponColliders = new CircleCollider2D[nAttackSprites];
        for (int i = 0; i < nAttackSprites; i++)
        {
            weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[i];
         //     CircleCollider2D collider = weaponSpriteObject.AddComponent<CircleCollider2D>();
         //    collider.sharedMaterial = physicsMaterial;
         //      weaponColliders[i] = collider;
        }
        

    }

    public void CheckCurrentTargetLocation(TileBehavior tileBehavior)
    {
        Vector2 currentPosition = GetPosition();
        Vector2 enemyPosition = currentTarget.GetPosition();
        int[] enemyGridPos = tileBehavior.GetTileIndex(enemyPosition);
        int[] thisGridPos = tileBehavior.GetTileIndex(currentPosition);

        if (enemyGridPos[0] != currentTargetGridPos[0] || enemyGridPos[1] != currentTargetGridPos[1]) // if the target has moved 
        {
            List<Vector2> path = new Astar(tileBehavior.mapUnveiledJagged, thisGridPos, enemyGridPos, "Euclidean").result;
            EndAttack();
            SetupWalkingPath(path, tileBehavior.mapPositionMatrix);
            currentTargetGridPos = enemyGridPos;
        }
        else
        {
            Vector2 directionVector = (enemyPosition - currentPosition).normalized;
            UpdateRotation((int)Vector2.SignedAngle(Vector2.up, directionVector));
        }

    }

    public void SeekAndDestroy(List<Fighter> enemyFighters, TileBehavior tileBehavior)// search for all enemies in a certain radius
    {

        Vector2 currentPosition = GetPosition();
        float attackRadius = 10;
        float closestEnemyDistance = 999999f;
        Fighter closestEnemy = null; 

        foreach (Fighter enemy in enemyFighters)
        {
            Vector2 enemyPosition = enemy.GetPosition();
            float dist = Vector2.Distance(currentPosition, enemyPosition); 

            if (dist < attackRadius && dist<closestEnemyDistance)
            {
                closestEnemyDistance = dist; 
                closestEnemy = enemy; 
            }
        }

        if (closestEnemy != null)
        {
            wagingWar = true;

            int[] enemyGridPos = tileBehavior.GetTileIndex(closestEnemy.GetPosition());
            int[] thisGridPos = tileBehavior.GetTileIndex(currentPosition);
            List<Vector2> path = new Astar(tileBehavior.mapUnveiledJagged, thisGridPos, enemyGridPos, "Euclidean").result;

            SetupWalkingPath(path, tileBehavior.mapPositionMatrix);

            currentTarget = closestEnemy;
            currentTargetGridPos = enemyGridPos;
        }
        else
        {
            if(!autoWalking)
                InitiateRandomWalk(tileBehavior, 10); 
        }
    }

    public void SetupWalkingPath(List<Vector2> path, Vector2[,] mapPositionMatrix)
    {

        int startInd = 0;
        if (path.Count > 1)
            startInd = 1;

        Vector2 currentPosition = GetPosition();

        Vector2 tilePosition = mapPositionMatrix[(int)path[startInd].x, (int)path[startInd].y];
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
            Vector2 currentPosition = GetPosition();
            destinationVector = currentPath[currentPathIndex];
            directionVector = (destinationVector - currentPosition).normalized;
        }
    }

    public void InitiateRandomWalk(TileBehavior tileBehavior, int length)
    {
        creatureManager.main.InitiateRandomWalk(tileBehavior, length, this); 
    }




    public void OnHit(float damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Debug.Log("KILLED, erasing self...");
            isDead = true; 
            creatureManager.EraseCreature(this); 
            
        }
    }


    public void Update(TileBehavior tileBehavior)
    {

        if (wagingWar)
            CheckCurrentTargetLocation(tileBehavior); 

        if (attacking == true)
        {
            if (!currentTarget.isDead)
            {
                ContinueAttack();

                int currentAttackIndex = (int)attackIndex++ % nAttackSprites;

                UpdateAttackingSprite(currentAttackIndex);

                if (currentAttackIndex == attackingSoundIndex)
                {
                    Main.audioSource.PlayOneShot(Main.stab, 0.2f);
                    currentTarget.OnHit(attackDamage);
                }
            }
            else
            {
                wagingWar = false;
                EndAttack(); 
            }

        }
        else if (moving == true)
        {
            UpdateWalkingSprite((int)moveIndex++ % nWalkingSprites);
        }
        else if (autoWalking == true)
        {
            Vector2 currentPosition = GetPosition(); 
            float norm = (currentPosition - destinationVector).magnitude;

            float rangeToTarget = 9999999;

            if (wagingWar == true)
            {
                rangeToTarget = (currentTarget.GetPosition() - GetPosition()).magnitude; 
            }


            float rand = Random.Range(0.35f, 0.4f);
            float rand2 = Random.Range(0.01f, 0.06f);

            if (((currentPathIndex >= currentPath.Count - 1 && norm < rand) && rangeToTarget < weaponRange))//
            {
                OnAttack();
                autoWalking = false;
            }
            else if (norm > rand2)
            {
                UpdateRotation((int)Vector2.SignedAngle(Vector2.up, directionVector));
                UpdatePosition(directionVector*speed);
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

            position = bodySpriteObject.transform.position; 
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

        attacking = true;

        bodyWeaponObject.SetActive(false);
        bodySpriteObject.SetActive(false);

        weaponBodyObject.transform.localRotation = bodySpriteObject.transform.localRotation;
        weaponBodyObject.transform.position = GetPosition(); //getPobodySpriteObject.transform.position;

        weaponSpriteObject.SetActive(true);
        weaponBodyObject.SetActive(true);

        attackIndex = 0;
    }

    public void ContinueAttack()
    {
        weaponSpriteObject.GetComponent<SpriteRenderer>().sprite = attackingWeaponSprites[(int)attackIndex % nAttackSprites];
        weaponBodyObject.GetComponent<SpriteRenderer>().sprite = attackingBodySprites[(int)attackIndex % nAttackSprites];

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
        bodySpriteObject.transform.position = GetPosition();

    }

    public void EndMovement()
    {
        moving = false;
    }

    public Vector3 GetPosition()
    {
        return position; 

    }

    public void RemoveSelf()
    {
        EndAttack();
        EndMovement();
        wagingWar = false; 
        Object.Destroy(bodyWeaponObject);
        Object.Destroy(weaponBodyObject);
        Object.Destroy(bodySpriteObject);
        Object.Destroy(weaponSpriteObject);
    }

}
