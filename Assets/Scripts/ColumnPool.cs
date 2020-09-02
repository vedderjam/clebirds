using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnPool : MonoBehaviour
{
    public int columnPoolSize = 5;
    public List<GameObject> logsPrefabs;
    private float spawnRate = 2f;
    private float columnMin = -1f;
    private float columnMax = 3.5f;
    private float bottomColumnOffset = -12f;


    private GameObject[] columns;
    private Vector2 objectPoolPosition = new Vector2(-15f, -25f);
    private float timeSinceLastSpawned;
    private float spawnXPosition = 5f;
    private int currentColumn = 0;

    private void Awake()
    {
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.StartIdling += StartPlaying;
        UpdateManager.UpdateEvent += UpdateEvent;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;

        columns = new GameObject[columnPoolSize];
        for (int i = 0; i < columnPoolSize; i++)
        {
            float rnd = Random.value;
            var log = rnd < 0.5f ? logsPrefabs[0] : logsPrefabs[1];
            columns[i] = Instantiate(log, objectPoolPosition, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        EventBroker.StartPlaying -= StartPlaying;
        EventBroker.StartIdling -= StartPlaying;
        UpdateManager.UpdateEvent -= UpdateEvent;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
    }

    private void StartPlaying()
    {
        timeSinceLastSpawned = 0f;

        foreach(var column in columns)
        {
            column.transform.position = objectPoolPosition;
            column.transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateEvent()
    {
        timeSinceLastSpawned += Time.deltaTime;

        if(GameControl.Instance.gameState == GameState.Playing && timeSinceLastSpawned >= spawnRate)
        {
            timeSinceLastSpawned = 0f;
            float spawnYPosition = Random.Range(columnMin, columnMax);
            columns[currentColumn].transform.position = new Vector2(spawnXPosition, spawnYPosition);
            currentColumn++;
            if(currentColumn >= columnPoolSize)
            {
                currentColumn = 0;
            }
        }
    }

    private void ChangeDifficultyLevel()
    {
        spawnRate = GameControl.Instance.currentDifficultyLevel.spawnRate;
        columnMin = GameControl.Instance.currentDifficultyLevel.columnMin;
        columnMax = GameControl.Instance.currentDifficultyLevel.columnMax;
        bottomColumnOffset = GameControl.Instance.currentDifficultyLevel.bottomColumnOffset;
        SetColumnsOffset();
    }

    private void SetColumnsOffset()
    {
        foreach(var column in columns)
        {
            var top = column.transform.GetChild(0);
            var bottom = column.transform.GetChild(1);
            bottom.transform.position = new Vector3(bottom.transform.position.x, top.transform.position.y + bottomColumnOffset, bottom.transform.position.z);
        }
    }
}
