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

    public Text haywireText;
    public RectTransform haywireBar;

    public RectTransform healthBar;

    public RectTransform progressBarPanel;
    public Transform progressIcon;
    public Transform targetIcon;
    public Sprite progressFullImage;

    public RectTransform resultsPanel;
    public Text resultsHeading;
    public Text resultsBody;

    private PlayerBehaviour player;

    private float secondsToNextWave;
    private float secondsToNextBoss;
    private float secondsToNextHaywire;
    private float secondsToNextHaywireIncrease;

    private bool bossSpawned;
    private bool? finishState;

    private int score;

    private List<WaveDefinition> waveDefinitions;

    private HaywireCollection activeHaywires;

    // Start is called before the first frame update
    void Start()
    {
        resultsPanel.gameObject.SetActive(false);

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

        if (bossSpawned && bossCount == 0) {
            finishState = true;
            DestroyRemainingEnemies();
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
        float healthFactor = player.GetCurrentHealth() / player.maxHealth;
        healthBar.anchorMax = new Vector2(healthFactor, 1);

        if (finishState != null) {
            FinalizeUI(finishState.Value);
        }
        else {
            if (secondsToNextBoss > 0) {
                float distanceFactor = 1 - (secondsToNextBoss / secondsBetweenBosses);
                float start = 12.1f;
                float end = targetIcon.transform.position.x - 10;
                float newProgressX = start + ((end - start) * distanceFactor);
                progressIcon.transform.position = new Vector3(
                    newProgressX,
                    progressIcon.transform.position.y,
                    progressIcon.transform.position.z
                );
            }
            else {
                progressBarPanel.GetComponent<Image>().sprite = progressFullImage;
                progressIcon.GetComponent<Image>().enabled = false;
                targetIcon.GetComponent<Image>().enabled = false;
            }

            List<string> activeHaywiresText = GetActiveHaywiresText();
            float haywireFactor = GetProgressToHaywire();

            haywireText.text = string.Join("\n", activeHaywiresText);
            haywireBar.anchorMax = new Vector2(haywireFactor, 1);
        }
    }

    private void FinalizeUI(bool victory) {
        resultsPanel.gameObject.SetActive(true);
        resultsBody.text = $"Final Score: {score}";

        if (victory) {
            haywireText.text = "All systems online";
            resultsHeading.text = "Victory!";
            resultsBody.text += "\n\nThanks for playing!";
        }
        else {
            haywireText.text = "All systems offline";
            resultsHeading.text = "Defeat...";
        }
    }

    void HandleHaywires() {
        secondsToNextHaywire -= Time.deltaTime;
        if (secondsToNextHaywire <= 0) {
            activeHaywires = new HaywireCollection(haywireCount);
            // activeHaywires = new HaywireCollection(new List<HaywireType> {
            //     HaywireType.ShipProjectilesWeighted
            // });
            
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
        if (activeHaywires.IsActive(HaywireType.ShipMovementVerticalUncontrollable)) {
            haywireTextList.Add("Ship steering uncontrollably!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipCannonsBackwards)) {
            haywireTextList.Add("Cannons firing backwards!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipCannonsSpin)) {
            haywireTextList.Add("Cannons rotating freely!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipCannonsNonStop)) {
            haywireTextList.Add("Cannons won't stop firing!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipSpinUncontrollable)) {
            haywireTextList.Add("Ship spinning uncontrollably!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipArmorWeightIncreased)) {
            haywireTextList.Add("Armor drawing increased power!");
        }
        if (activeHaywires.IsActive(HaywireType.ShipProjectilesWeighted)) {
            haywireTextList.Add("Projectile targetting offline!");
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

    public void IncrementScore(int value) {
        score += value;
    }

    private void DestroyRemainingEnemies() {
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        foreach (GameObject enemy in enemies) {
            Destroy(enemy);
        }

        List<GameObject> projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile").ToList();
        foreach (GameObject projectile in projectiles) {
            Destroy(projectile);
        }
    }
}
