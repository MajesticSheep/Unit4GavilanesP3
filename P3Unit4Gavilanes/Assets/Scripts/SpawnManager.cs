using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public GameObject enemyPrefab;
    private float spawnRange = 9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        Instantiate(enemyPrefab, new Vector3(0, 0, 6), enemyPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
