using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable][CreateAssetMenu()]
public class UserData : ScriptableObject
{
    #region Members & properties

    public List<DifficultyLevel> difficultyLevels;
    private DifficultyLevel difficultyLevel;

    private int _normalRecord;
    public int NormalRecord
    {
        get {return _normalRecord;}
        set
        {
            if(value > _normalRecord)
                _normalRecord = value;
        }
    }

    private int _easyRecord;
    public int EasyRecord
    {
        get {return _easyRecord;}
        set
        {
            if(value > _easyRecord)
                _easyRecord = value;
        }
    }

    private int _hardRecord;
    public int HardRecord
    {
        get {return _hardRecord;}
        set
        {
            if(value > _hardRecord)
                _hardRecord = value;
        }
    }

    private int _coins;
    public int Coins
    {
        get{ return _coins; }
        set
        {
            if( value < 0)
                _coins = 0;
            else
                _coins = value;
        }
    }

    public int CurrentBirdIndex{get; set;}
    #endregion

    #region Methods

    public bool SubstractCoins(int coinsToSubstract)
    {
        if(coinsToSubstract > Coins)
        {
            return false;
        }
        else
        {
            Coins -= coinsToSubstract;
            return true;
        }
    }

    public void AddCoins(int coinsToAdd)
    {
        Coins += coinsToAdd;
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
    }

    public int GetCurrentLevelRecord()
    {
        int record = -1;

        switch (difficultyLevel.level)
        {
            case Difficulty.Easy:
                record = EasyRecord;
                break;
            case Difficulty.Normal:
                record = NormalRecord;
                break;
            case Difficulty.Hard:
                record = HardRecord;
                break;
            default:
                record = NormalRecord;
                break;
        }

        return record;
    }

    public DifficultyLevel GetDifficultyLevel()
    {
        return difficultyLevel;
    }

    public bool SetDifficultyLevelByIndex(int index)
    {
        bool set = false;

        if(index >= 0 && index < difficultyLevels.Count)
        {
            difficultyLevel = difficultyLevels[index];
            set = true;
        }

        return set;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(nameof(difficultyLevel), (int) difficultyLevel.level);
        PlayerPrefs.SetInt(nameof(EasyRecord), EasyRecord);
        PlayerPrefs.SetInt(nameof(NormalRecord), NormalRecord);
        PlayerPrefs.SetInt(nameof(HardRecord), HardRecord);
        PlayerPrefs.SetInt(nameof(Coins), Coins);
        PlayerPrefs.SetInt(nameof(CurrentBirdIndex), CurrentBirdIndex);
        PlayerPrefs.Save();
    }
    
    public void Load()
    {
        if (PlayerPrefs.HasKey(nameof(difficultyLevel)))
        {
            var index = PlayerPrefs.GetInt(nameof(difficultyLevel));
            SetDifficultyLevelByIndex(index);
        }
        else difficultyLevel = difficultyLevels[0];

        if(PlayerPrefs.HasKey(nameof(EasyRecord)))
            EasyRecord = PlayerPrefs.GetInt(nameof(EasyRecord));
        else EasyRecord = 0;

        if(PlayerPrefs.HasKey(nameof(NormalRecord)))
            NormalRecord = PlayerPrefs.GetInt(nameof(NormalRecord));
        else NormalRecord = 0;

        if(PlayerPrefs.HasKey(nameof(HardRecord)))
            HardRecord = PlayerPrefs.GetInt(nameof(HardRecord));
        else HardRecord = 0;

        if(PlayerPrefs.HasKey(nameof(Coins)))
            Coins = PlayerPrefs.GetInt(nameof(Coins));
        else Coins = 0;

        CurrentBirdIndex = PlayerPrefs.GetInt(nameof(CurrentBirdIndex), 0);
    }

    #endregion
}
