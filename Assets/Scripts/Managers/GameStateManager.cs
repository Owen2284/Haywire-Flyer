using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public Text healthText;
    public Text distanceText;

    private PlayerBehaviour player;

    private int timeToNextWave;
    private int timeToNextBoss;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        timeToNextWave = 0;
        timeToNextBoss = 50000;
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

        if (timeToNextBoss == 0 && bossCount == 0 && enemyCount == 0) {
            FinalizeUI(true);
        }
        else if (player == null) {
            FinalizeUI(false);
        }
    }

    private void UpdateEnemies() {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

        // Enemy spawning
        if (timeToNextWave > 0) {
            --timeToNextWave;
        }
        if (timeToNextWave == 0 || enemyCount == 0 && (timeToNextBoss > 0 || bossCount > 0)) {
            // TODO: Spawn wave
            timeToNextWave = 1000;
        }

        // Boss spawning
        if (timeToNextBoss > 0) {
            --timeToNextBoss;
        }
        if (timeToNextBoss == 0) {
            // TODO: Spawn boss
        }
    }

    private void UpdateUI() {
        float healthValue = player.GetCurrentHealth() * 10;
        healthText.text = $"Current health: {healthValue}";

        if (timeToNextBoss > 0) {
            int displayDistance = (int)Mathf.Ceil(timeToNextBoss / 100);
            distanceText.text = $"Distance to target: {displayDistance}";
        }
        else {
            distanceText.text = "TARGET APPROACHING";
        }
    }

    private void FinalizeUI(bool victory) {
        healthText.text = "";

        if (victory) {
            distanceText.text = "VICTORY!";
        }
        else {
            distanceText.text = "Defeat...";
        }
    }
}
