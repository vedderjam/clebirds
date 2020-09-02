using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState{ Idle, Playing, GameOver}

public class GameControl : Singleton<GameControl>
{
    #region Members

    public DifficultyLevel currentDifficultyLevel;
    public UserData userData;
    public GameState gameState;
    private AudioSource audioSource;
    private Vector3 initialPosition = new Vector3(-1.5f, 0f, 0f);

    [Header("Audio clips")]
    private AudioClip scoreClip;
    private AudioClip gameOverClip;
    public AudioClip selectClip;
    public AudioClip cancelClip;

    public static int Score
    {
        get;
        private set;
    }
    public static int Record
    {
        get;
        private set;
    }
    public static int Coins
    {
        get;
        private set;
    }
    public static int Reward
    {
        get;
        private set;
    }
    public static int MoreReward
    {
        get;
        private set;
    }
    public static bool NewRecord
    {
        get;
        private set;
    }
    public static int CurrentBirdIndex
    {
        private set;
        get;
    }
    public static int lastPillIndexShown;
    
    private static readonly int scoreToTransitionTime = 20;

    [Header("Birds")]
    public Transform birdsParentTransform;
    [SerializeField]private GameObject[] birds;
    public GameObject currentBird;
    
    public BirdHouse birdHouse;

    private int minReward = 10;
    private int maxReward = 100;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        EventBroker.GameOver += GameOver;
        EventBroker.BirdScored += BirdScored;
        EventBroker.GamePaused += PauseGame;
        EventBroker.GameResumed += ResumeGame;
        EventBroker.EarnedRewardedAd += EventBroker_EarnedRewardedAd;
    }

    private void Start()
    {
        birds = new GameObject[birdHouse.birdInfos.Capacity];
        Load();
        currentDifficultyLevel = userData.GetDifficultyLevel();
        CurrentBirdIndex = GetCurrentBirdIndex();
        SelectBird(CurrentBirdIndex, false);
        EventBroker.CallChangeDifficultyLevel();
        Score = 0;
        Record = userData.GetCurrentLevelRecord();
        Coins = userData.Coins;
        gameState = GameState.Idle;
        UpdateHighScore();
    }

    private void OnDestroy()
    {
        EventBroker.GameOver -= GameOver;
        EventBroker.BirdScored -= BirdScored;
        EventBroker.GamePaused -= PauseGame;
        EventBroker.GameResumed -= ResumeGame;
        EventBroker.EarnedRewardedAd -= EventBroker_EarnedRewardedAd;
    }

    #endregion

    #region Methods
    private void Load()
    {
        userData.Load();
        birdHouse.Load();
    }

    private int GetCurrentBirdIndex()
    {
        int current = userData.CurrentBirdIndex;

        if(current >= birdHouse.birdInfos.Capacity)
        {
            current = 0;
        }

        return current;
    }

    private void BirdScored()
    {
        if(gameState != GameState.Playing) return;

        Score++;
        audioSource.PlayOneShot(scoreClip);

        if(Score % scoreToTransitionTime == 0)
            EventBroker.CallTransitionTime();
    }

    private void GameOver()
    {
        UpdateHighScore();
        PostLeaderboard();
        GPGSController.Instance.CheckAchievements(Score);
        UpdateRewardAndCoinsAfterLosing();
        CheckNewInfoPillsToShow(Score);
        UpdateBirdAggregatedScore();
        ResetScore();
        gameState = GameState.GameOver;
        audioSource.PlayOneShot(gameOverClip);
        userData.Save();
        birdHouse.Save();
    }

    private void PostLeaderboard()
    {
        if (Social.localUser.authenticated)
        {
            string leaderboardID = GetLeaderboardToPost();
            EventBroker.CallPostScore(Score, leaderboardID);
        }
    }

    private string GetLeaderboardToPost()
    {
        string leaderboardID;

        switch (currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                leaderboardID = GPGSIds.leaderboard_easy_level_high_scores;
                break;
            case Difficulty.Normal:
                leaderboardID = GPGSIds.leaderboard_normal_level_high_scores;
                break;
            case Difficulty.Hard:
                leaderboardID = GPGSIds.leaderboard_hard_level_high_scores;
                break;
            default:
                leaderboardID = "";
                break;
        }

        return leaderboardID;
    }
    
    private void GetLastInfoPillIndexShown()
    {
        lastPillIndexShown = birdHouse.birdInfos[CurrentBirdIndex].LastPillIndexShown;
    }

    private void CheckNewInfoPillsToShow(int scoreToAdd)
    {
        var number = birdHouse.birdInfos[CurrentBirdIndex].NumberOfNewInfoPillsToShow(scoreToAdd);
        if(number > 0)
            EventBroker.CallNewInfoPills(number);
    }

    private void UpdateBirdAggregatedScore()
    {
        birdHouse.birdInfos[userData.CurrentBirdIndex].aggregatedScore += Score;
    }

    private void PauseGame()
    {
        print("GameControl PauseGame");
        Time.timeScale = 0.0f;
    }

    private void ResumeGame()
    {
        print("GameControl ResumeGame");
        Time.timeScale = 1.0f;
    }
    
    private void EventBroker_EarnedRewardedAd()
    {
        Reward += MoreReward;
        userData.AddCoins(MoreReward);
        Coins = userData.Coins;
    }

    private void UpdateHighScore()
    {
        switch (currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                if(Score > userData.EasyRecord)
                    NewRecord = true;
                else NewRecord = false;
                userData.EasyRecord = Score;
                Record = userData.EasyRecord;
                break;
            case Difficulty.Normal:
                if(Score > userData.NormalRecord)
                    NewRecord = true;
                else NewRecord = false;
                userData.NormalRecord = Score;
                Record = userData.NormalRecord;
                break;
            case Difficulty.Hard:
                if(Score > userData.HardRecord)
                    NewRecord = true;
                else NewRecord = false;
                userData.HardRecord = Score;
                Record = userData.HardRecord;
                break;
        }
    }

    private void UpdateRewardAndCoinsAfterLosing()
    {
        var coinsMultiplier = currentDifficultyLevel.coinsMultiplier;

        Reward = (int)(Score * coinsMultiplier);
        userData.AddCoins(Reward);
        Coins = userData.Coins;

        MoreReward = CalculateMoreReward();
    }

    private int CalculateMoreReward()
    {
        return Mathf.Clamp(Reward, minReward, maxReward);
    }

    private void ResetReward()
    {
        Reward = 0;
    }

    public void Idle()
    {
        if(gameState == GameState.GameOver)
        {
            gameState = GameState.Idle;
            EventBroker.CallStartIdling();
        }
    }

    public void StartPlaying()
    {
        gameState = GameState.Playing;
        EventBroker.CallStartPlaying();
    }

    private void ResetScore()
    {
        Score = 0;
    }

    public void SetDifficultyLevel(int index)
    {
        if(userData.SetDifficultyLevelByIndex(index))
        {
            currentDifficultyLevel = userData.GetDifficultyLevel();
            UpdateHighScore();
            EventBroker.CallChangeDifficultyLevel();
        }
    }

    public void TryToBuyAndOrSelectBird(int index)
    {
        if (!birdHouse.birdInfos[index].purchased)
        {
            var coins = Coins;
            var purchased = birdHouse.Purchase(index, ref coins);
            if(purchased)
            {
                Coins = coins;
                userData.SetCoins(Coins);
                EventBroker.CallBirdPurchased(index);
                SelectBird(index);
                userData.Save();
                birdHouse.Save();
            }
            else
            {
                audioSource.PlayOneShot(cancelClip);
                EventBroker.CallNotEnoughCoins();
            }
        }
        else SelectBird(index);
    }

    private void SelectBird(int index, bool playSound = true)
    {
        if(birds[index] == null)
        {
            if(currentBird != null)
                currentBird.SetActive(false); // Prior bird
            
            birds[index] = Instantiate(birdHouse.birdInfos[index].prefab, initialPosition, Quaternion.identity, birdsParentTransform);
            currentBird = birds[index];
            scoreClip = currentBird.GetComponent<Bird>().scoreClip;
            gameOverClip = currentBird.GetComponent<Bird>().dieClip;
        }
        else
        {
            currentBird.SetActive(false);
            currentBird = birds[index];
            currentBird.SetActive(true);
        }

        CurrentBirdIndex = index;
        userData.CurrentBirdIndex = CurrentBirdIndex;
        GetLastInfoPillIndexShown();
        EventBroker.CallBirdSelected(CurrentBirdIndex);
        if(playSound)
            audioSource.PlayOneShot(selectClip);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    #endregion
}
