using Cainos.PixelArtTopDown_Basic;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject altar;
    public GameObject canvas;
    public Button[] buttons;
    TextMeshProUGUI[] texts;

    public static List<GameObject> playerUnits = new List<GameObject>();
    public static List<GameObject> enemyUnits = new List<GameObject>();

    public List<GameObject> playerUnit = new List<GameObject>();
    public List<GameObject> enemyUnit = new List<GameObject>();

    public bool gameOver = false;
    public float money;
    private float moneyIncomePerSecond;

    public int wave = 1;
    // 0 spawning
    // 1 fighting
    // 2 waiting
    public int waveState = 0;
    public float waveForce = 2;
    public float currentWaveForce = 0;

    public float timeBetweenWaves = 10f;
    public float betweenWavesTimer = 0f;
    public float currentWaveSpawningTime = 0f;
    public float waveSpawnerTimer = 0f;
    public float spawningDuration = 0f;

    public static bool isLvlAndHpVisible = false;
    private float timeNextIncome = 0f;
    // Start is called before the first frame update
    void Start()
    {
        money = 100f;
        moneyIncomePerSecond = 2f;

        //canvas = GameObject.Find("Canvas");
        texts = canvas.transform.GetComponentsInChildren<TextMeshProUGUI>();
        buttons = canvas.transform.GetComponentsInChildren<Button>();

        texts[0].text = playerUnit[0].GetComponent<Unit>().cost.ToString();
        texts[1].text = playerUnit[1].GetComponent<Unit>().cost.ToString();

        float altarPosX = altar.transform.position.x;
        float altarPosY = altar.transform.position.y;

/*        for (int i = 0; i < 200; i++)
        {
            GameObject unit = Instantiate(playerUnit[0], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            playerUnits.Add(unit);
        }

        for (int i = 0; i < 200; i++)
        {
            GameObject unit = Instantiate(enemyUnit[0], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            enemyUnits.Add(unit);
        }

        for (int i = 0; i < 40; i++)
        {
            GameObject unit = Instantiate(playerUnit[1], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            playerUnits.Add(unit);
        }

        for (int i = 0; i < 40; i++)
        {
            GameObject unit = Instantiate(enemyUnit[1], new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), 0f), Quaternion.identity);
            enemyUnits.Add(unit);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (waveState == 0)
        {
            currentWaveSpawningTime += Time.deltaTime;
            waveForce = wave * 2;
            spawningDuration = (float)wave / 8f;
            if (currentWaveSpawningTime > spawningDuration)
            {
                waveState = 1;
                currentWaveSpawningTime = 0f;
            }
            if (Time.time >= waveSpawnerTimer)
            {
                waveSpawn(waveForce, spawningDuration);
                waveSpawnerTimer = Time.time + 0.3f;
            }
        }

        if(waveState == 1)
        {
            if(enemyUnits.Count <= 0)
            {
                waveState = 2;
                betweenWavesTimer = Time.time + timeBetweenWaves;
            }
        }

        if(waveState == 2)
        {
            if(betweenWavesTimer <= Time.time)
            {
                currentWaveForce = 0f;
                wave++;
                waveState = 0;
            }
        }

        if (altar.GetComponent<Altar>().health <= 0)
        {
            gameOver = true;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            isLvlAndHpVisible = !isLvlAndHpVisible;
        }

        for(int i = 0; i < buttons.Length; i++)
        {
            if(money < float.Parse(texts[i].text))
            {
                buttons[i].interactable = false;
            }
            else
            {
                buttons[i].interactable = true;
            }
        }

        if (Time.time >= timeNextIncome)
        {
            money += moneyIncomePerSecond;
            timeNextIncome = Time.time + 1f;
        }
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

    public static bool GetIsLvlAndHpVisible()
    {
        return isLvlAndHpVisible;
    }

    public void BuyUnit(int code)
    {
        GameObject unit = Instantiate(playerUnit[code], Random.insideUnitCircle.normalized * 2, Quaternion.identity);
        this.money -= unit.GetComponent<Unit>().cost;
        playerUnits.Add(unit);
    }

    public void waveSpawn(float force, float time)
    {
        List<int> canSpawn = new List<int>();

        for (int i = 0; i < enemyUnit.Count; i++)
        {
            if (currentWaveForce + enemyUnit[i].GetComponent<Unit>().force < force)
            {
                canSpawn.Add(i);
            }
        }

        if (canSpawn.Count == 0)
        {
            return;
        }

        float timeCompletion = currentWaveSpawningTime / time;
        float forceCompletion = currentWaveForce / force;

        while (canSpawn.Count != 0 && forceCompletion < timeCompletion)
        {
            Vector2 enemyPosition = Random.insideUnitCircle.normalized * 15;
            int whichEnemy = Random.Range(0, canSpawn.Count);
            float unitExp = Random.Range(0f ,(float)wave/4f);
            float additionalForceFromExp = unitExp / 10f;
            GameObject unit = Instantiate(enemyUnit[whichEnemy], enemyPosition, Quaternion.identity);
            Unit unitComponent = unit.GetComponent<Unit>();
            unitComponent.exp = unitExp;
            enemyUnits.Add(unit);
            currentWaveForce += unit.GetComponent<Unit>().force + additionalForceFromExp;
            forceCompletion = currentWaveForce / force;

            canSpawn = new List<int>();

            for (int i = 0; i < enemyUnit.Count; i++)
            {
                if (currentWaveForce + enemyUnit[i].GetComponent<Unit>().force < force)
                {
                    canSpawn.Add(i);
                }
            }
        }

    }

    public void HealPlayerUnits(float maxHealthPercent)
    {
        foreach (GameObject unit in playerUnits)
        {
            unit.GetComponent<Unit>().Heal(maxHealthPercent);
        }
    }
}
