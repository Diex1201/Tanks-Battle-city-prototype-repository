using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool playerDead;  //статус игрока;
    public Transform[] enemySpawn;   //массив точек спауна вражеских юнитов;
    public float enemySpawnTime;  //время в секундах, когда будет следующая волна танков;
    public int maxEnemy = 5;          //количество волн танков
    public Transform playerSpawn;    //точка возрождения нашего танка;
    public GameObject player;        //префаб нашего танчика;
    public GameObject enemy;         //префаб вражеского танчика;

   
    void Start()
    {
        maxEnemy = maxEnemy * 3;
        playerDead = false;
        Instantiate(player, playerSpawn.position, Quaternion.identity);
        StartCoroutine(WaitEnemySpawn(enemySpawnTime));
    }

   
    void Update()
    {
        if (playerDead)
        {
            playerDead = false;
            Instantiate(player, playerSpawn.position, Quaternion.identity);
        }
        if(GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            enemySpawnTime = 5;
        }
    }

    IEnumerator WaitEnemySpawn(float time)
    {
        foreach (Transform obj in enemySpawn)
        {
            maxEnemy--;
            Instantiate(enemy, obj.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(time);
        if (maxEnemy > 0) StartCoroutine(WaitEnemySpawn(enemySpawnTime));
    }
}
