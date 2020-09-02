using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePool : MonoBehaviour
{
    public int treePoolSize = 3;
    public List<GameObject> logsPrefabs;
    private float spawnRate = 2f;
    private float minOffset = -1.5f;
    private float maxOffset = 1.5f;
    private float bushOffset;

    private GameObject[] trees;
    private Vector2 objectPoolPosition = new Vector2(-15f, -25f);
    private float timeSinceLastSpawned;
    private float spawnXPosition = 5f;
    private float spawnYPosition = 2.5f;
    private int currentTree = 0;

    private void Awake()
    {
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.StartIdling += StartPlaying;
        UpdateManager.UpdateEvent += UpdateEvent;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;

        trees = new GameObject[treePoolSize];
        for (int i = 0; i < treePoolSize; i++)
        {
            float rnd = Random.value;
            var log = rnd < 0.5f ? logsPrefabs[0] : logsPrefabs[1];
            trees[i] = Instantiate(log, objectPoolPosition, Quaternion.identity);
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

        foreach(var tree in trees)
        {
            tree.transform.position = objectPoolPosition;
            tree.transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateEvent()
    {
        timeSinceLastSpawned += Time.deltaTime;

        if(GameControl.Instance.gameState == GameState.Playing && timeSinceLastSpawned >= spawnRate)
        {
            timeSinceLastSpawned = 0f;
            trees[currentTree].transform.position = new Vector2(spawnXPosition, spawnYPosition);
            trees[currentTree].GetComponent<Tree>().SetBushesPositions();
            currentTree++;
            if(currentTree >= treePoolSize)
            {
                currentTree = 0;
            }
        }
    }

    private void ChangeDifficultyLevel()
    {
        spawnRate = GameControl.Instance.currentDifficultyLevel.spawnRate;
        minOffset = GameControl.Instance.currentDifficultyLevel.minOffset;
        maxOffset = GameControl.Instance.currentDifficultyLevel.maxOffset;
        bushOffset = GameControl.Instance.currentDifficultyLevel.bushOffset;
        UpdateTreesBushesOffset();
        SetTreesBushesPositions();
    }

    private void UpdateTreesBushesOffset()
    {
        foreach(var tree in trees)
        {
            tree.GetComponent<Tree>().SetBushOffset(bushOffset);
        }
    }

    private void SetTreesBushesPositions()
    {
        foreach(var tree in trees)
        {
            tree.GetComponent<Tree>().SetBushesPositions();
        }
    }
}
