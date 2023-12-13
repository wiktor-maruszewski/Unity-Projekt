using UnityEngine;

public class UnitDeath : MonoBehaviour
{
    public SpriteRenderer[] bodyParts;
    float startTime;
    void Start()
    {
        bodyParts = GetComponentsInChildren<SpriteRenderer>();
        startTime = Time.realtimeSinceStartup;
    }

    void Update()
    {

        if (Time.realtimeSinceStartup - startTime > 5f)
        {
            Destroy(transform.gameObject);
        }
    }
}
