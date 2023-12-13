using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject altar;
    public GameObject canvas;
    public GameObject UpgradePanel;
    public List<Button> buttons;
    public List<TextMeshProUGUI> texts;
    public List<Button> upgradeButtons;
    public List<TextMeshProUGUI> upgradeLvls;
    public List<TextMeshProUGUI> upgradeBonuses;
    public List<TextMeshProUGUI> upgradeCosts;
    public Button upgradesExitButton;
    public TextMeshProUGUI moneyIndicatorText;
    public TextMeshProUGUI waveIndicatorText;
    public GameObject gameOverPanel;
    public Button GameOverButton;
    public AudioSource audioSource;


    public static List<GameObject> playerUnits = new List<GameObject>();
    public static List<GameObject> enemyUnits = new List<GameObject>();

    public List<GameObject> playerUnit = new List<GameObject>();
    public List<GameObject> enemyUnit = new List<GameObject>();

    public bool gameOver = false;
    public static float money;

    private float moneyIncomePerSecond;
    public int upgradeMoneyIncomeLevel = 0;
    public float upgradeMoneyIncomeCost = 50f;
    public float upgradeMoneyIncomeMultiplier = 0.5f;

    public float unitStartingExp = 0f;
    public int upgradeExpLevel = 0;
    public float upgradeExpCost = 50f;
    public float upgradeExpMultiplier = 1f;

    public int upgradeAltarHealthLevel = 0;
    public float upgradeAltarHealthCost = 50f;
    public float upgradeAltarHealthMultiplier = 500f;

    public int upgradeAltarHealthRegenerationLevel = 0;
    public float upgradeAltarHealthRegenerationCost = 50f;
    public float upgradeAltarHealthRegenerationMultiplier = 1f;



    public int wave = 1;
    // 0 spawning
    // 1 fighting
    // 2 waiting
    public int waveState = 0;
    public float waveForce = 2;
    public float currentWaveForce = 0;

    public float timeBetweenWaves = 1f;
    public float betweenWavesTimer = 0f;
    public float currentWaveSpawningTime = 0f;
    public float waveSpawnerTimer = 0f;
    public float spawningDuration = 0f;

    public static bool isLvlAndHpVisible = false;
    private float timeNextIncome = 0f;

    private void Awake()
    {
        playerUnits = new List<GameObject>();
        enemyUnits = new List<GameObject>();
    }

    void Start()
    {
        money = 100f;
        moneyIncomePerSecond = 2f;

        upgradeMoneyIncomeLevel = 0;
        upgradeMoneyIncomeCost = 50f;
        upgradeMoneyIncomeMultiplier = 0.5f;
        unitStartingExp = 0f;
        upgradeExpLevel = 0;
        upgradeExpCost = 50f;
        upgradeExpMultiplier = 1f;
        upgradeAltarHealthLevel = 0;
        upgradeAltarHealthCost = 50f;
        upgradeAltarHealthMultiplier = 500f;
        upgradeAltarHealthRegenerationLevel = 0;
        upgradeAltarHealthRegenerationCost = 50f;
        upgradeAltarHealthRegenerationMultiplier = 1f;


        foreach (Button button in canvas.transform.GetComponentsInChildren<Button>())
        {
            if (button.CompareTag("Upgrade"))
            {
                upgradeButtons.Add(button);
            }

            if (button.CompareTag("AddUnit"))
            {
                buttons.Add(button);
            }

            if (button.CompareTag("Exit"))
            {
                upgradesExitButton = button;
            }
        }


        foreach (TextMeshProUGUI text in canvas.transform.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (text.CompareTag("AddUnit"))
            {
                texts.Add(text);
            }

            if (text.CompareTag("Lvl"))
            {
                upgradeLvls.Add(text);
            }

            if (text.CompareTag("Bonus"))
            {
                upgradeBonuses.Add(text);
            }

            if (text.CompareTag("Cost"))
            {
                upgradeCosts.Add(text);
            }
        }

        texts[0].text = playerUnit[0].GetComponent<Unit>().cost.ToString();
        texts[1].text = playerUnit[1].GetComponent<Unit>().cost.ToString();

        float altarPosX = altar.transform.position.x;
        float altarPosY = altar.transform.position.y;

        UpgradePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        audioSource.Play();
    }

 

    void Update()
    {
        if (altar.GetComponent<Altar>().health <= 0)
        {
            gameOver = true;
        }

        if(gameOver)
        {
            Button[] allButtons = canvas.GetComponentsInChildren<Button>();
            foreach (Button button in allButtons)
            {
                button.interactable = false;
            }


            UpgradePanel.SetActive(false);

            Time.timeScale = 0.05f;

            gameOverPanel.SetActive(true);
            GameOverButton.interactable = true;
        }

        moneyIndicatorText.text = string.Format("{0:N0}", money);
        waveIndicatorText.text = string.Format("{0:N0}", wave);

        if (waveState == 0)
        {
            currentWaveSpawningTime += Time.deltaTime;
            waveForce = (wave * 3f) - 1f;
            spawningDuration = (float)wave / 15f;
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



        if (Input.GetKeyDown(KeyCode.T))
        {
            isLvlAndHpVisible = !isLvlAndHpVisible;
        }

        for(int i = 0; i < buttons.Count; i++)
        {
            if (!gameOver)
            {
                if (money < float.Parse(texts[i].text))
                {
                    buttons[i].interactable = false;
                }
                else
                {
                    buttons[i].interactable = true;
                }
            }
        }

        for(int i = 0; i < upgradeButtons.Count; i++)
        {
            if(money < float.Parse(upgradeCosts[i].text))
            {
                upgradeButtons[i].interactable = false;
            }
            else
            {
                upgradeButtons[i].interactable = true;
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
            money += unit.GetComponent<Unit>().cost * 0.38f;
        }
        else if (unit.CompareTag("Player"))
        {
            playerUnits.Remove(unit);
        }
        
    }

    public static bool GetIsLvlAndHpVisible()
    {
        return isLvlAndHpVisible;
    }

    public void BuyUnit(int code)
    {
        GameObject unit = Instantiate(playerUnit[code], Random.insideUnitCircle.normalized * 2, Quaternion.identity);
        Unit unitComponent = unit.GetComponent<Unit>();
        unitComponent.exp = unitStartingExp;
        money -= unitComponent.cost;
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
            Vector2 enemyPosition = Random.insideUnitCircle.normalized * 20;
            int whichEnemy = Random.Range(0, canSpawn.Count);
            if(whichEnemy == 3)
            {
                whichEnemy = Random.Range(0, canSpawn.Count);
            }
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
    public void upgradeMoneyIncome()
    {
        upgradeMoneyIncomeLevel++;
        money -= float.Parse(upgradeCosts[2].text);
        moneyIncomePerSecond = 2f + (upgradeMoneyIncomeLevel * 1f * upgradeMoneyIncomeMultiplier);
        upgradeLvls[2].text = string.Format("{0:N0}", upgradeMoneyIncomeLevel);
        upgradeBonuses[2].text = string.Format("{0:N2}", moneyIncomePerSecond);
        upgradeCosts[2].text = string.Format("{0:N0}", upgradeMoneyIncomeCost + (upgradeMoneyIncomeLevel * upgradeMoneyIncomeCost));
    }

    public void upgradeExp()
    {
        upgradeExpLevel++;
        money -= float.Parse(upgradeCosts[3].text);
        unitStartingExp = upgradeExpLevel * upgradeExpMultiplier;
        upgradeLvls[3].text = string.Format("{0:N0}", upgradeExpLevel);
        upgradeBonuses[3].text = string.Format("{0:N2}", unitStartingExp);
        upgradeCosts[3].text = string.Format("{0:N0}", upgradeExpCost + (upgradeExpLevel * upgradeExpCost));
    }

    public void upgradeAltarHealth()
    {
        upgradeAltarHealthLevel++;
        money -= float.Parse(upgradeCosts[0].text);
        altar.GetComponent<Altar>().maxHealth = 1000f + (upgradeAltarHealthLevel * upgradeAltarHealthMultiplier);
        upgradeLvls[0].text = string.Format("{0:N0}", upgradeAltarHealthLevel);
        upgradeBonuses[0].text = string.Format("{0:N0}", altar.GetComponent<Altar>().maxHealth);
        upgradeCosts[0].text = string.Format("{0:N0}", upgradeAltarHealthCost + (upgradeAltarHealthLevel * upgradeAltarHealthCost));
    }

    public void upgradeAltarHealthRegeneration()
    {
        upgradeAltarHealthRegenerationLevel++;
        money -= float.Parse(upgradeCosts[1].text);
        altar.GetComponent<Altar>().healthRegen = 1f + (1f * upgradeAltarHealthRegenerationLevel * upgradeAltarHealthRegenerationMultiplier);
        upgradeLvls[1].text = string.Format("{0:N0}", upgradeAltarHealthRegenerationLevel);
        upgradeBonuses[1].text = string.Format("{0:N2}", altar.GetComponent<Altar>().healthRegen);
        upgradeCosts[1].text = string.Format("{0:N0}", upgradeAltarHealthRegenerationCost + (upgradeAltarHealthRegenerationLevel * upgradeAltarHealthRegenerationCost));
    }

    public void ToggleUpgradesExitButtonActive()
    {
        UpgradePanel.SetActive(!UpgradePanel.activeSelf);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
