using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Unit : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject knightDead;
    public SpriteRenderer healthIndicator;
/*    public CircleCollider2D sightCollider;*/

    public int lvl;
    public float exp;
    public float speed;
    public float health;
    public float maxHealth;
    public float defense;
    public float attack;
    public float attackSpeed;
    public float attackRange;
    public float sight;

    public float defaultSpeed = 0.5f;
    public float defaultMaxHealth = 150f;
    public float defaultDefense = 5f;
    public float defaultAttack = 20f;
    public float defaultAttackSpeed = 1.0f;
    public float defaultAttackRange = 0.5f;
    public float defaultSight = 12f;

    public float nextLvlExp;

    // 0 - idle
    // 1 - walking around
    // 2 - attack
    // 3 - dead
    public int state;

    public List<float> freeWalkingBoundsX;
    public List<float> freeWalkingBoundsY;
    public Vector2 freeWalkingDirection;
    public float freeWalkingTimer;
    public float freeWalkingTime;

    public List<GameObject> enemiesInSight;
    public GameObject nearestEnemyInSight;
    private float timeNextCheck = 0f;
    private float timeBetweenChecks = 0.5f;

    float deathTimer;
    float attackTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        /*sightCollider = transform.Find("sight").GetComponent<CircleCollider2D>();*/

        lvl = 1;
        nextLvlExp = 1f + (lvl - 1f) * 0.25f;
        exp = 0f;
        speed = defaultSpeed;
        maxHealth = defaultMaxHealth;
        health = maxHealth;
        defense = defaultDefense;
        attack = defaultAttack;
        attackSpeed = defaultAttackSpeed;
        attackRange = defaultAttackRange;
        sight = defaultSight;
 /*       sightCollider.radius = sight;*/

        freeWalkingTimer = Time.time;
        freeWalkingTime = Random.Range(3f, 15f);
        freeWalkingBoundsX.Add(-5f);
        freeWalkingBoundsX.Add(5f);
        freeWalkingBoundsY.Add(-5f);
        freeWalkingBoundsY.Add(5f);
        UpdateFreeWalkingDirection();

  /*      enemiesInSight = new List<GameObject>();*/
        deathTimer = 0f;
        attackTimer = attackSpeed / 2f;
        updateNearestEnemyInSight2();
        UpdateHealthBar();

        state = 1;
    }

    void Update()
    {
        if (Time.time >= timeNextCheck)
        {
            //do distance check here
            updateNearestEnemyInSight2();

            timeNextCheck = Time.time + Random.Range(0, 1f);
        }

        if (nearestEnemyInSight != null)
        {
            state = 2;
        }
        else if(nearestEnemyInSight == null)
        {
            state = 1;
        }

        if (health <= 0f)
        {
            state = 3;
        }

        if (state == 1)
        {
            if (animator.GetInteger("state") != 1)
            {
                animator.SetInteger("state", 1);
            }

            if(Time.time - freeWalkingTimer > freeWalkingTime) 
            {
                UpdateFreeWalkingDirection();
                freeWalkingTimer = Time.time;
                freeWalkingTime = Random.Range(3f, 15f);
            }

            if (new Vector2(freeWalkingDirection.x, freeWalkingDirection.y) == new Vector2(transform.position.x, transform.position.y))
            {
                animator.SetInteger("state", 0);
            }

            Vector2 dir = Vector2.MoveTowards(transform.position, freeWalkingDirection, speed * Time.deltaTime);
            if(dir.x > transform.position.x)
            {
                healthIndicator.transform.parent.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(180f, 0f, 180f);

            }
            else if (dir.x < transform.position.x)
            {
                healthIndicator.transform.parent.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            }
            transform.position = dir;
            //rb.AddForce(new Vector2(dir.x * 0.1f, dir.y * 0.001f));
        }

        if(state == 2)
        {
            Vector2 thisPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 enemyPosition = new Vector2(nearestEnemyInSight.transform.position.x, nearestEnemyInSight.transform.position.y);

            if (enemyPosition.x > transform.position.x)
            {
                healthIndicator.transform.parent.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(180f, 0f, 180f);
            }
            else if (enemyPosition.x < transform.position.x)
            {
                healthIndicator.transform.parent.rotation = Quaternion.Euler(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            }


            /*            if (Vector2.Distance(thisPosition, enemyPosition) <= attackRange)*/
            if ((thisPosition - enemyPosition).sqrMagnitude <= attackRange * attackRange)
            {
                if(animator.GetInteger("state") != 2)
                {
                    animator.SetInteger("state", 2);
                }

/*                if (attackTimer >= attackSpeed && Random.Range(0f, 10f) < 3)
                {
                    attackTimer = 0f;
                }*/
                    
                if (attackTimer >= attackSpeed)
                {
                    Unit enemyUnit = nearestEnemyInSight.GetComponent<Unit>();
                    enemyUnit.Hurt(attack);
                    attackTimer = 0f;

                    if(enemyUnit.health <= 0f)
                    {
                        exp += 1f;
                    }
                }
                attackTimer += Time.deltaTime;
            }
            else
            {
                attackTimer = attackSpeed / 2f;
                animator.SetInteger("state", 1);
                Vector2 dir = Vector2.MoveTowards(thisPosition, enemyPosition, speed * Time.deltaTime);
                transform.position = dir;
                //rb.AddForce(dir);
                
            }
        }

        if(state == 3)
        {
            GameController.RemoveFromUnits(transform.gameObject);
            animator.SetInteger("state", 3);
            tag = "Untagged";
            deathTimer += Time.deltaTime;
            if(deathTimer >= 0.9f)
            {
                Quaternion bodyRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 90f);
                Instantiate(knightDead, transform.position + new Vector3(0f, 0f, 0f), bodyRotation);
                Destroy(transform.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (exp >= nextLvlExp)
        {
            lvlUp();
        }

    }

    void updateNearestEnemyInSight2()
    {
        nearestEnemyInSight = null;
        float minDistance = sight + 0.1f;
        List<GameObject> units = new List<GameObject>();
        if(transform.CompareTag("Enemy"))
        {
            units = GameController.GetPlayerUnits();
        } else if(transform.CompareTag("Player"))
        {
            units = GameController.GetEnemyUnits();
        }
        for(int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
            {
                /*float unitDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(units[i].transform.position.x, units[i].transform.position.y));*/
                float unitDistanceSqrMagnitude = (new Vector2(transform.position.x, transform.position.y) - new Vector2(units[i].transform.position.x, units[i].transform.position.y)).sqrMagnitude;
                if (unitDistanceSqrMagnitude < sight * sight)
                {
                    if (unitDistanceSqrMagnitude < minDistance * minDistance)
                    {
                        minDistance = unitDistanceSqrMagnitude * unitDistanceSqrMagnitude;
                        nearestEnemyInSight = units[i];
                    }
                }
            }
        }
    }

    void lvlUp()
    {
        int lvlsUp = 0;
        while (exp >= nextLvlExp)
        {
            lvl++;
            lvlsUp++;
            exp -= nextLvlExp;
            nextLvlExp = 1f + (lvl - 1f) * 0.25f;
        }
        maxHealth = defaultMaxHealth + defaultMaxHealth * (lvl - 1) * 0.1f;
        defense = defaultDefense + defaultDefense * (lvl - 1) * 0.1f;
        attack = defaultAttack + defaultAttack * (lvl - 1) * 0.1f;
        health += maxHealth * 0.2f * lvlsUp;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthBar();

    }

    private void UpdateFreeWalkingDirection()
    {
        float x = Random.Range(freeWalkingBoundsX[0], freeWalkingBoundsX[1]);
        float y = Random.Range(freeWalkingBoundsY[0], freeWalkingBoundsY[1]);
        freeWalkingDirection = new Vector2(x, y);
    }

    public void Hurt(float attackDamage)
    {
        float dmg = attackDamage * (1f - (defense / (defense + 40)));
        health -= dmg;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        int size = (int)((int)health / maxHealth * 22);
/*        if(size < 1)
        {
            size = 1;
        }*/
        healthIndicator.transform.localScale = new Vector3(size, 1, 1);
        //healthIndicator.transform.SetPositionAndRotation(new Vector3(size, 0, 0), Quaternion.identity);
        healthIndicator.transform.localPosition = new Vector3(-0.2365f + 0.01075f * size, 0, 0);
    }
}
