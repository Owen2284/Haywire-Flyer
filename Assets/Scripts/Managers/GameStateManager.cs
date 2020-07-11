using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public List<GameObject> spawnableEnemies;
    public Vector2 spawnRoot;

    public float secondsBetweenWaves;
    public float secondsBetweenBosses;

    public Text healthText;
    public Text distanceText;
    public Text haywireText;
    public RectTransform haywireBar;
    public Image progressTracker;

    private PlayerBehaviour player;

    private float secondsToNextWave;
    private float secondsToNextBoss;

    private bool? finishState;

    private List<WaveDefinition> waveDefinitions;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        secondsToNextWave = secondsBetweenWaves / 3;
        secondsToNextBoss = secondsBetweenBosses;

        InitWaves();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCompletion();

        UpdateEnemies();
        UpdateUI();
    }

    private void CheckCompletion() {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

        if (secondsToNextBoss == 0 && bossCount == 0 && enemyCount == 0) {
            finishState = true;
        }
        else if (player == null) {
            finishState = false;
        }

        if (finishState != null) {
            FinalizeUI(finishState.Value);
        }
    }

    private void UpdateEnemies() {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

        // Enemy spawning
        if (secondsToNextWave > 0) {
            secondsToNextWave -= Time.deltaTime;
        }
        if (secondsToNextWave <= 0 || enemyCount == 0) {
            Debug.Log(secondsToNextWave <= 0);
            Debug.Log(enemyCount == 0);
            SpawnWave();
            secondsToNextWave = secondsBetweenWaves;
        }

        // Boss spawning
        if (secondsToNextBoss > 0) {
            secondsToNextBoss -= Time.deltaTime;
        }
        if (secondsToNextBoss <= 0) {
            // TODO: Spawn boss
        }
    }

    private void UpdateUI() {
        float healthValue = (player.GetCurrentHealth() / player.maxHealth) * 100;
        healthText.text = $"Current health: {healthValue}";

        if (secondsToNextBoss > 0) {
            float distanceFactor = secondsToNextBoss / secondsBetweenBosses;
            distanceText.text = $"Distance to target: {((int)(distanceFactor * 10000))}";
        }
        else {
            distanceText.text = "TARGET APPROACHING";
        }

        List<string> activeHaywires = player.GetActiveHaywires();
        float haywireFactor = player.GetProgressToHaywire();

        haywireText.text = string.Join("\n", activeHaywires);
        haywireBar.anchorMax = new Vector2(haywireFactor, 1);
    }

    private void FinalizeUI(bool victory) {
        healthText.text = "";
        haywireText.text = "";

        if (victory) {
            distanceText.text = "VICTORY!";
        }
        else {
            distanceText.text = "Defeat...";
        }
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
            //BaseEnemyBehaviour enemyBehaviour = enemy.GetComponent<BaseEnemyBehaviour>();
        }
    }
}
