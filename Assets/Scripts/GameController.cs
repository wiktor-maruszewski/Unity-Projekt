using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static List<GameObject> playerUnits = new List<GameObject>();
    public static List<GameObject> enemyUnits = new List<GameObject>();

    public List<GameObject> playerUnit = new List<GameObject>();
    public List<GameObject> enemyUnit = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 250; i++)
        {
            GameObject unit = Instantiate(playerUnit[0], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            playerUnits.Add(unit);
        }

        for (int i = 0; i < 250; i++)
        {
            GameObject unit = Instantiate(enemyUnit[0], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            enemyUnits.Add(unit);
        }

        for (int i = 0; i < 120; i++)
        {
            GameObject unit = Instantiate(playerUnit[1], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            playerUnits.Add(unit);
        }

        for (int i = 0; i < 120; i++)
        {
            GameObject unit = Instantiate(enemyUnit[1], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            enemyUnits.Add(unit);
        }
    }

    // Update is called once per frame
    void Update()
    {
/*        for(int i = 0; i < playerUnits.Count ; i++)
        {
            if (playerUnits[i] == null || playerUnits[i].tag == "Untagged")
            {
                playerUnits.Remove(playerUnits[i]);
            }
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (enemyUnits[i] == null || enemyUnits[i].tag == "Untagged")
            {
                enemyUnits.Remove(enemyUnits[i]);
            }
        }*/
    }

    public static List<GameObject> GetPlayerUnits()
    {
        return playerUnits;
    }

    public static List<GameObject> GetEnemyUnits()
    {
        return enemyUnits;
    }

    public static void RemoveFromUnits(GameObject unit)
    {
        if (unit.CompareTag("Enemy"))
        {
            enemyUnits.Remove(unit);
        }
        else if (unit.CompareTag("Player"))
        {
            playerUnits.Remove(unit);
        }
        
    }
/*
    public static void RemoveFromEnemyUnits(GameObject unit)
    {
        enemyUnits.Remove(unit);
    }*/
}
