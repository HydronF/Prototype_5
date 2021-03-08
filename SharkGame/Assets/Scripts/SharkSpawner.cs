using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnWave {
        public int sharkCount;
        public List<GameObject> sharkComponents;
        public float spawnGap;
    }

    public List<SpawnWave> waves;
    public GameObject sharkPrefab;
    int currentWave = 0;
    int spawnedThisWave = 0;
    List<EnemyShark> enemySharks;
    float spawnTimer;
    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        enemySharks = new List<EnemyShark>();
    }

    // Update is called once per frame
    void Update()
    {
        if (started) {
            CheckNextWave();
            if (spawnTimer < 0 && (waves[currentWave].sharkCount == -1 || spawnedThisWave < waves[currentWave].sharkCount)) {
                SpawnShark();
                spawnTimer = waves[currentWave].spawnGap;
            }
            spawnTimer -= Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(0)) { started = true; }
    }

    private void CheckNextWave()
    {
        if (spawnedThisWave == waves[currentWave].sharkCount && enemySharks.Count == 0)
        {
            ++currentWave;
            spawnedThisWave = 0;
            Debug.Log("New Wave: Wave " + currentWave);
        }
    }

    void SpawnShark() {
        // Instantiate new shark
        EnemyShark newShark = Instantiate(sharkPrefab, transform.position, transform.rotation, transform).GetComponent<EnemyShark>();
        newShark.spawner = this;
        enemySharks.Add(newShark);
        ++spawnedThisWave;
        // Give it components
        foreach (GameObject compPrefab in waves[currentWave].sharkComponents) {
            if (Random.Range(0, 2) == 1) {
                SharkComponent newComp = Instantiate(compPrefab, transform.position, transform.rotation, newShark.transform).GetComponent<SharkComponent>();
                newComp.shark = newShark;
            }
        }
    }

    public void RemoveShark(EnemyShark shark) {
        enemySharks.Remove(shark);
    }
}
