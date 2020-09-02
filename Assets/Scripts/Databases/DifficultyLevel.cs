using System;
using UnityEngine;

public enum Difficulty {Easy, Normal, Hard}

[Serializable][CreateAssetMenu]
public class DifficultyLevel : ScriptableObject
{
    public Difficulty level = Difficulty.Normal;
    public float coinsMultiplier = 1;
    public float scrollSpeed = -1.5f;
    public float foreGroundScrollSpeed = -1.5f;
    public float backGroundScrollSpeed = -1.5f;
    public float spawnRate = 2f;
    public float columnMin = -1f;
    public float columnMax = 3.5f;
    public float bottomColumnOffset = -12f;
    public float minOffset = -1.5f;
    public float maxOffset = 1.5f;
    public float bushOffset = 3f;
    
    public DifficultyLevel()
    {
        level = Difficulty.Normal;
        coinsMultiplier = 1;
        scrollSpeed = -1.5f;
        spawnRate = 2f;
        columnMin = -1f;
        columnMax = 3.5f;
        bottomColumnOffset = -12f;
        minOffset = -1.5f;
        maxOffset = 1.5f;
        bushOffset = 3f;
    }

    public DifficultyLevel(Difficulty level, float coinsMultiplier, float scrollSpeed, float spawnRate, 
        float columnMin, float columnMax, float bottomColumnOffset, float minOffset, float maxOffset, float bushOffset)
    {
        this.level = level;
        this.coinsMultiplier = coinsMultiplier;
        this.scrollSpeed = scrollSpeed;
        this.spawnRate = spawnRate;
        this.columnMin = columnMin;
        this.columnMax = columnMax;    
        this.bottomColumnOffset = bottomColumnOffset;
        this.minOffset = minOffset;
        this.maxOffset = maxOffset;
        this.bushOffset = bushOffset;
    }
}
