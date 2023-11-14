using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject knightDead;

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

    public float defaultSpeed = 2f;
    public float defaultMaxHealth = 100f;
    public float defaultDefense = 5f;
    public float defaultAttack = 20f;
    public float defaultAttackSpeed = 1.0f;
    public float defaultAttackRange = 0.8f;
    public float defaultSight = 8f;

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

    public List<GameObject> enemiesInSight;
    public GameObject nearestEnemyInSight;

    float deathTimer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //knightDead = GetComponent<GameObject>();

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

        freeWalkingTimer = Time.time;
        freeWalkingBoundsX.Add(-5f);
        freeWalkingBoundsX.Add(5f);
        freeWalkingBoundsY.Add(-5f);
        freeWalkingBoundsY.Add(5f);
        UpdateFreeWalkingDirection();

        state = 1;
        deathTimer = 0f;
    }

    void Update()
    {
        updateNearestEnemyInSight();


        if(enemiesInSight.Count > 0)
        {
            state = 2;
        }
        else
        {
            state = 1;
        }

        if (health <= 0f)
        {
            state = 3;
        }

        if (state == 1)
        {
            animator.SetInteger("state", 1);
            float freeWalkingTime = Mathf.Abs(freeWalkingBoundsX[0] - freeWalkingBoundsX[1]) + Mathf.Abs(freeWalkingBoundsY[0] - freeWalkingBoundsY[1]);
            if(Time.time - freeWalkingTimer > freeWalkingTime) 
            {
                UpdateFreeWalkingDirection();
                freeWalkingTimer = Time.time;
            }
            if (new Vector2(freeWalkingDirection.x, freeWalkingDirection.y) == new Vector2(transform.position.x, transform.position.y))
            {
                animator.SetInteger("state", 0);
            }

            Vector2 dir = Vector2.MoveTowards(transform.position, freeWalkingDirection, speed * Time.deltaTime);
            if(dir.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(180f, 0f, 180f);
            }
            else if (dir.x < transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            }
            rb.MovePosition(dir);
        }

        if(state == 2)
        {
            Vector2 thisPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 enemyPosition = new Vector2(nearestEnemyInSight.transform.position.x, nearestEnemyInSight.transform.position.y);
            if(Vector2.Distance(thisPosition, enemyPosition) <= attackRange)
            {

            }
            else
            {
                animator.SetInteger("state", 1);
                Vector2 dir = Vector2.MoveTowards(transform.position, enemyPosition, speed * Time.deltaTime);

                if (dir.x > transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(180f, 0f, 180f);
                }
                else if (dir.x < transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                }
                rb.MovePosition(dir);
            }
        }

        if(state == 3)
        {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("sight") && !collision.CompareTag(this.tag))
        {
            enemiesInSight.Add(collision.gameObject);
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("sight") && !collision.CompareTag(this.tag))
        {
            enemiesInSight.Remove(collision.gameObject);
        }
    }

    void lvlUp()
    {
        while (exp >= nextLvlExp)
        {
            lvl++;
            exp -= nextLvlExp;
            nextLvlExp = 1f + (lvl - 1f) * 0.25f;
        }
        maxHealth = defaultMaxHealth + defaultMaxHealth * (lvl - 1) * 0.1f;
        defense = defaultDefense + defaultDefense * (lvl - 1) * 0.1f;
        attack = defaultAttack + defaultAttack * (lvl - 1) * 0.1f;

    }

    void updateNearestEnemyInSight()
    {
        if (enemiesInSight.Count > 0)
        {
            float minDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(enemiesInSight[0].transform.position.x, enemiesInSight[0].transform.position.y));
            nearestEnemyInSight = enemiesInSight[0].gameObject;
            for (int i = 0; i < enemiesInSight.Count; i++)
            {
                float tempDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(enemiesInSight[i].transform.position.x, enemiesInSight[i].transform.position.y)); ;
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    nearestEnemyInSight = enemiesInSight[i].gameObject;
                }
            }
        }
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
    }
}
