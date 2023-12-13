using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject knightDead;
    public SpriteRenderer healthIndicator;
    private GameObject altar;
    public List<AudioClip> clips;

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
    public float force = 0;

    public float cost = 0f;

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

    float deathTimer;
    float attackTimer;

    public bool showHealthAndLvl = false;

    private void Awake()
    {
        exp = 0f;
    }
    void Start()
    {
        altar = GameObject.FindGameObjectWithTag("Altar");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        lvl = 1;
        nextLvlExp = 1f + (lvl - 1f) * 0.25f;

        speed = defaultSpeed;
        maxHealth = defaultMaxHealth;
        health = maxHealth;
        defense = defaultDefense;
        attack = defaultAttack;
        attackSpeed = defaultAttackSpeed;
        attackRange = defaultAttackRange;
        sight = defaultSight;

        freeWalkingTimer = Time.time;
        freeWalkingTime = Random.Range(3f, 15f);
        freeWalkingBoundsX.Add(-4f);
        freeWalkingBoundsX.Add(4f);
        freeWalkingBoundsY.Add(-4f);
        freeWalkingBoundsY.Add(4f);
        if (transform.CompareTag("Player"))
        {
        UpdateFreeWalkingDirection();
        } else if (transform.CompareTag("Enemy"))
        {
            freeWalkingDirection = new Vector2(0f, 0f);
        }

        deathTimer = 0f;
        attackTimer = attackSpeed / 2f;
        updateNearestEnemyInSight2();
        UpdateHealthBar();
        HandleShowHpAndLvl();

        state = 1;
    }

    void Update()
    {
        if (Time.time >= timeNextCheck)
        {
            updateNearestEnemyInSight2();

            showHealthAndLvl = GameController.GetIsLvlAndHpVisible();

            HandleShowHpAndLvl();
            if(state == 1)
            {
                Heal(0.2f);
            }

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

            if(transform.CompareTag("Player") && Time.time - freeWalkingTimer > freeWalkingTime) 
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


            if(nearestEnemyInSight == altar)
            {
                attackRange = altar.GetComponent<CircleCollider2D>().radius + transform.GetComponent<CircleCollider2D>().radius + 0.1f;
            } else
            {
                attackRange = defaultAttackRange;
            }

            if ((thisPosition - enemyPosition).sqrMagnitude <= attackRange * attackRange)
            {
                if(animator.GetInteger("state") != 2)
                {
                    animator.SetInteger("state", 2);
                }
                    
                if (attackTimer >= attackSpeed)
                {
                    AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Count)], transform.position, 0.6f);
                    Unit enemyUnit;
                    if (nearestEnemyInSight != altar)
                    {
                        enemyUnit = nearestEnemyInSight.GetComponent<Unit>();
                        enemyUnit.Hurt(attack * Random.Range(0.5f, 2.0f));
                        if(enemyUnit.health <= 0f)
                        {
                            exp += 1f + Mathf.Sqrt(enemyUnit.lvl);
                        }
                    } else
                    {
                        altar.GetComponent<Altar>().Hurt(attack * Random.Range(0.5f, 1.5f));
                    }
                    attackTimer = 0f;

                }
                attackTimer += Time.deltaTime;
            }
            else
            {
                attackTimer = attackSpeed / 2f;
                animator.SetInteger("state", 1);
                Vector2 dir = Vector2.MoveTowards(thisPosition, enemyPosition, speed * Time.deltaTime);
                transform.position = dir;
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
        if (exp >= nextLvlExp && state != 3)
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
        if (transform.CompareTag("Enemy"))
        {
            float altarDistanceSqrMagnitude = (new Vector2(transform.position.x, transform.position.y) - new Vector2(altar.transform.position.x, altar.transform.position.y)).sqrMagnitude;
            if (altarDistanceSqrMagnitude < sight * sight)
            {
                if (altarDistanceSqrMagnitude < minDistance)
                {
                    nearestEnemyInSight = altar;
                }
            }
        }
        if (transform.CompareTag("Player"))
        {
            if(nearestEnemyInSight != null)
            {
                float enemyDistance = Vector2.Distance(altar.transform.position, nearestEnemyInSight.transform.position);
                float thisUnitDistance = Vector2.Distance(altar.transform.position, transform.position);
                if (enemyDistance > (2f + Mathf.Sqrt(GameController.GetPlayerUnits().Count) / 4f))
                {
                    if(enemyDistance > thisUnitDistance && Random.Range(0, 5) == 0)
                    {
                        nearestEnemyInSight = null;
                        UpdateFreeWalkingDirection();
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
        health += maxHealth * 0.08f * lvlsUp;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        updateLvlStar();
        UpdateHealthBar();

    }

    private void UpdateFreeWalkingDirection()
    {
        float x = Random.Range(freeWalkingBoundsX[0], freeWalkingBoundsX[1]);
        float y = Random.Range(freeWalkingBoundsY[0], freeWalkingBoundsY[1]);
        freeWalkingDirection = Random.insideUnitCircle * (2f + Mathf.Sqrt(GameController.GetPlayerUnits().Count)/4f);
    }

    public void Hurt(float attackDamage)
    {
        float dmg = attackDamage * (1f - (defense / (defense + 40)));
        health -= dmg;
        UpdateHealthBar();
    }

    public void Heal(float maxHealthPercent)
    {
        health += maxHealth * maxHealthPercent/100f;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthBar();
    }

    public void GiveExp(float experience)
    {
        this.exp = exp + experience;
        updateLvlStar();
    }

    void UpdateHealthBar()
    {
        int size = (int)((int)health / maxHealth * 22);

        healthIndicator.transform.localScale = new Vector3(size, 1, 1);
        healthIndicator.transform.localPosition = new Vector3(-0.2365f + 0.01075f * size, 0, 0);
    }

    void updateLvlStar()
    {
        SpriteRenderer starSprite = healthIndicator.transform.parent.Find("star").GetComponent<SpriteRenderer>();
        starSprite.color = new Color(1f, 1f, 1f - (lvl/(lvl + 10f)));
    }

    void HandleShowHpAndLvl()
    {
        if(showHealthAndLvl)
        {
            healthIndicator.transform.parent.transform.gameObject.SetActive(true);
        }
        else
        {
            healthIndicator.transform.parent.transform.gameObject.SetActive(false);
        }
    }
}
