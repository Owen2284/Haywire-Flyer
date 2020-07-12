using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public List<GameObject> spawnableEnemies;
    public List<GameObject> spawnableBosses;
    public Vector2 spawnRoot;

    public float haywireTime;
    public int haywireCount;

    public float secondsBetweenWaves;
    public float secondsBetweenBosses;
    public float secondsBetweenHaywireIncreases;

    public Text healthText;
    public Text distanceText;
    public Text haywireText;
    public RectTransform haywireBar;
    public Image progressTracker;

    private PlayerBehaviour player;

    private float secondsToNextWave;
    private float secondsToNextBoss;
    private float secondsToNextHaywire;
    private float secondsToNextHaywireIncrease;

    private bool bossSpawned;
    private bool? finishState;

    private List<WaveDefinition> waveDefinitions;

    private HaywireCollection activeHaywires;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        secondsToNextWave = 0;
        secondsToNextBoss = secondsBetweenBosses;
        secondsToNextHaywire = haywireTime;
        secondsToNextHaywireIncrease = secondsBetweenHaywireIncreases;

        InitWaves();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCompletion();

        HandleHaywires();
        HandleEnemies();

        UpdateUI();
    }

    private void CheckCompletion() {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

        if (bossSpawned && bossCount == 0 && enemyCount == 0) {
            finishState = true;
        }
        else if (player == null) {
            finishState = false;
        }
    }

    private void HandleEnemies() {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

        if (finishState != true) {
            // Enemy spawning
            if (secondsToNextWave > 0) {
                secondsToNextWave -= Time.deltaTime;
            }
            // Either the timer has expired, or the boss hasn't spawned and the enemies have run out
            if (secondsToNextWave <= 0 || (bossCount == 0 && enemyCount == 0)) {
                SpawnWave();
                secondsToNextWave = secondsBetweenWaves;
            }

            // Boss spawning
            if (secondsToNextBoss > 0) {
                secondsToNextBoss -= Time.deltaTime;
            }
            if (secondsToNextBoss <= 0 && !bossSpawned) {
                SpawnBoss();
            }
        }
    }

    private void UpdateUI() {
        float healthValue = (player.GetCurrentHealth() / player.maxHealth) * 100;
            healthText.text = $"Current health: {healthValue}";

        if (finishState != null) {
            FinalizeUI(finishState.Value);
        }
        else {
            if (secondsToNextBoss > 0) {
                float distanceFactor = secondsToNextBoss / secondsBetweenBosses;
                distanceText.text = $"Distance to target: {((int)(distanceFactor * 10000))}";
            }
            else {
                distanceText.text = "TARGET APPROACHING";
            }

            List<string> activeHaywiresText = GetActiveHaywiresText();
            float haywireFactor = GetProgressToHaywire();

            haywireText.text = string.Join("\n", activeHaywiresText);
            haywireBar.anchorMax = new Vector2(haywireFactor, 1);
        }
    }

    private void FinalizeUI(bool victory) {
        haywireText.text = "";

        if (victory) {
            distanceText.text = "VICTORY!";
        }
        else {
            distanceText.text = "Defeat...";
        }
    }

    void HandleHaywires() {
        secondsToNextHaywire -= Time.deltaTime;
        if (secondsToNextHaywire <= 0) {
            //activeHaywires = new HaywireCollection(haywireCount);
            activeHaywires = new HaywireCollection(new List<HaywireType> {
                HaywireType.ShipCannonsNonStop
            });
            
            player.SetHaywires(activeHaywires);

            secondsToNextHaywire = haywireTime;
        }

        secondsToNextHaywireIncrease -= Time.deltaTime;
        if (secondsToNextHaywireIncrease <= 0) {
            if (haywireCount < 3) {
                ++haywireCount;
            }

            secondsToNextHaywireIncrease = secondsBetweenHaywireIncreases;
        }
    }

    public float GetProgressToHaywire() {
        return 1 - (secondsToNextHaywire / haywireTime);
    }

    public List<string> GetActiveHaywiresText() {
        List<string> haywireTextList = new List<string>();

        if (activeHaywires == null || activeHaywires.TotalHaywires == 0) {
            haywireTextList.Add("All systems online");
            return haywireTextList;
        }

        if (activeHaywires.IsActive(HaywireType.ShipMovementVerticalOnly)) {
            haywireTextList.Add("Ship forward thrust offline!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipSpeedDoubled)) {
            haywireTextList.Add("Ship speed doubled!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipCannonsSpin)) {
            haywireTextList.Add("Cannons rotating freely!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipSpinUncontrollable)) {
            haywireTextList.Add("Ship spinning uncontrollably!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipArmorWeightIncreased)) {
            haywireTextList.Add("Armor weight increased!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipMovementVerticalUncontrollable)) {
            haywireTextList.Add("Ship steering uncontrollably!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipCannonsNonStop)) {
            haywireTextList.Add("Cannons won't stop firing!");
        }

        return haywireTextList;
    }



    private void InitWaves() {
        waveDefinitions = new List<WaveDefinition>();

        // One row, middle
        waveDefinitions.Add(new WaveDefinition() {
            positions = new List<Vector2> {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(2, 0),
                new Vector2(3, 0),
                new Vector2(4, 0),
                new Vector2(5, 0)
            }
        });

        // One row, higher
        waveDefinitions.Add(new WaveDefinition() {
            positions = new List<Vector2> {
                new Vector2(0, 3),
                new Vector2(1, 3),
                new Vector2(2, 3),
                new Vector2(3, 3),
                new Vector2(4, 3),
                new Vector2(5, 3)
            }
        });

        // One row, lower
        waveDefinitions.Add(new WaveDefinition() {
            positions = new List<Vector2> {
                new Vector2(0, -3),
                new Vector2(1, -3),
                new Vector2(2, -3),
                new Vector2(3, -3),
                new Vector2(4, -3),
                new Vector2(5, -3)
            }
        });
    }

    private void SpawnWave() {
        // Constants
        Quaternion rotation = Quaternion.Euler(0, 0, 90);

        // Pick a wave and enemy type
        WaveDefinition wave = waveDefinitions[Random.Range(0, waveDefinitions.Count)];

        List<GameObject> allowedEnemies = spawnableEnemies
            .Where(x => !wave.allowedEnemyTypes.Any() 
                || wave.allowedEnemyTypes.Contains(x.GetComponent<BaseEnemyBehaviour>().GetEnemyType()))
            .ToList();
        GameObject enemyType = allowedEnemies[Random.Range(0, allowedEnemies.Count)].gameObject;

        // Spawn all the enemies
        foreach (Vector2 position in wave.positions) {
            GameObject enemy = (GameObject)Instantiate(enemyType, position + spawnRoot, rotation);
        }
    }

    private void SpawnBoss() {
        // Constants
        Quaternion rotation = Quaternion.Euler(0, 0, 90);

        // Pick boss type
        GameObject bossType = spawnableBosses[0];

        // Spawn boss
        GameObject boss = (GameObject)Instantiate(bossType, new Vector2(2, 0) + spawnRoot, rotation);

        // Set state variable
        bossSpawned = true;
    }
}
