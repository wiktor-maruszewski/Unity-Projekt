using UnityEngine;

public class Altar : MonoBehaviour
{
    public SpriteRenderer healthIndicator;

    public float maxHealth;
    public float health;
    public float healthRegen;
    private float nextHealthRegenTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 1000f;
        health = maxHealth;
        healthRegen = 1f;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        if (Time.time >= nextHealthRegenTime && health < maxHealth)
        {
            nextHealthRegenTime = Time.time + 1f;
            health += healthRegen;
            if(health > maxHealth)
            {
                health = maxHealth;
            }
        }

    }
    public void Hurt(float damage)
    {
        health -= damage;
    }

    void UpdateHealthBar()
    {
        int size = (int)(health / maxHealth * 22f);
        healthIndicator.transform.localScale = new Vector3(size, 1, 1);
        //healthIndicator.transform.SetPositionAndRotation(new Vector3(size, 0, 0), Quaternion.identity);
        healthIndicator.transform.localPosition = new Vector3(-0.2365f + 0.01075f * size, 0, 0);
    }

}