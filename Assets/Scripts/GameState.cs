using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameState : MonoBehaviour
{
    private List<PlayerBehaviour> players;

    private int timeToNextWave;
    private int timeToNextBoss;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player").Select(x => x.GetComponent<PlayerBehaviour>()).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        // List<BaseEnemyBehaviour> enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(x => x.GetComponent<BaseEnemyBehaviour>());
        // List<ProjectileBehaviour> projectiles = GameObject.FindGameObjectsWithTag("Projectile").Select(x => x.GetComponent<ProjectileBehaviour>());

        // CleanUp(ref enemies, ref projectiles);
    }

    void CleanUp(ref List<BaseEnemyBehaviour> enemies, ref List<ProjectileBehaviour> projectiles) {
        // var escapedEnemies = enemies.Where();
        // var deadEnemies = enemies.Where(x => x.GetCurrentHealth() <= 0);
        
    }
}
