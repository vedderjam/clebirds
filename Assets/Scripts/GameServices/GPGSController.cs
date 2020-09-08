using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public class GPGSController : Singleton<GPGSController>
{
    #region Members
    private Texture2D savedImage;
    public ISavedGameMetadata currentGame;

    [Header("Scriptable object")]
    public UserData userData;
    public BirdHouse birdHouse;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        EventBroker.PostScore += EventBroker_PostScore;
    }

    private void OnDestroy()
    {
        EventBroker.PostScore -= EventBroker_PostScore;
    }

    private void EventBroker_PostScore(int score, string leaderboardID)
    {
        Social.ReportScore(score, leaderboardID, (bool success) => {
            // handle success or failure
            Debug.Assert(success, $"ERROR: {leaderboardID} couldn't be posted to.");
        });
    }

    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        // requests the email address of the player be available.
        // Will bring up a prompt for consent.
        //.RequestEmail()
        // requests a server auth code be generated so it can be passed to an
        //  associated back end server application and exchanged for an OAuth token.
        //.RequestServerAuthCode(false)
        // requests an ID token be generated.  This OAuth token can be used to
        //  identify the player to other services such as Firebase.
        //.RequestIdToken()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
            PlayGamesPlatform.DebugLogEnabled = true;
        #endif
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        //authenticate user:
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            if (result == SignInStatus.Success)
            {
                EventBroker.CallSocialSignedIn();
            }
        });
    }

    #endregion

    #region Methods
    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
        {
            if (result == SignInStatus.Success)
                EventBroker.CallSocialSignedIn();
        });
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
        EventBroker.CallSocialSignedOut();
    }

    public void ShowLeaderBoards()
    {
        Social.ShowLeaderboardUI();
    }

    private void LoadPlayerRecordFromLeaderboard(Difficulty level)
    {
        string leaderboardID;

        switch (level)
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
                leaderboardID = GPGSIds.leaderboard_easy_level_high_scores;
                break;
        }

        PlayGamesPlatform.Instance.LoadScores(
            leaderboardID,
            LeaderboardStart.PlayerCentered,
            1,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            OnLeaderboardLoadScores
        );
    }

    private void OnLeaderboardLoadScores(LeaderboardScoreData scoreData)
    {
        if(scoreData.Status == ResponseStatus.Success || scoreData.Status == ResponseStatus.SuccessWithStale)
        {
            var record = (int)scoreData.PlayerScore.value;

            if(scoreData.Id.CompareTo(GPGSIds.leaderboard_easy_level_high_scores) == 0)
            {
                if(userData.EasyRecord < record)
                    userData.EasyRecord = record;
            }
            else if(scoreData.Id.CompareTo(GPGSIds.leaderboard_normal_level_high_scores) == 0)
            {
                if(userData.NormalRecord < record)
                    userData.NormalRecord = record;
            }
            else if(scoreData.Id.CompareTo(GPGSIds.leaderboard_hard_level_high_scores) == 0)
            {
                if(userData.HardRecord < record)
                    userData.HardRecord = record;
            }
        }
    }

    #endregion

    #region Achievements Methods
    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }
    
    public void CheckAchievements(int score)
    {
        if (EndedBeforeStartedAchievement(score))
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(GPGSIds.achievement_ended_before_started, 100.0f, (bool success) => {
                Debug.Assert(success, "ERROR: achievement EndedBeforeStarted couldn't be reported");
            });
        }
        else if (Score25PointsAchievement(score))
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(GPGSIds.achievement_chick, 100.0f, (bool success) => {
                Debug.Assert(success, "ERROR: achievement Chick couldn't be reported");
            });
        }
        else if (Score50PointsAchievement(score))
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(GPGSIds.achievement_fly_the_nest, 100.0f, (bool success) => {
                Debug.Assert(success, "ERROR: achievement Fly the nest couldn't be reported");
            });
        }
        else if (Score75PointsAchievement(score))
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(GPGSIds.achievement_experimented_flier, 100.0f, (bool success) => {
                Debug.Assert(success, "ERROR: achievement Experimented flier couldn't be reported");
            });
        }
        else if (Score100PointsAchievement(score))
        {
            // unlock achievement (achievement ID "Cfjewijawiu_QA")
            Social.ReportProgress(GPGSIds.achievement_king_of_the_sky, 100.0f, (bool success) => {
                Debug.Assert(success, "ERROR: achievement King of the sky couldn't be reported");
            });
        }
    }

    private bool EndedBeforeStartedAchievement(int score)
    {
        return score == 0;
    }

    private bool Score25PointsAchievement(int score)
    {
        return score == 25;
    }
        
    private bool Score50PointsAchievement(int score)
    {
        return score == 50;
    }
        
    private bool Score75PointsAchievement(int score)
    {
        return score == 75;
    }
        
    private bool Score100PointsAchievement(int score)
    {
        return score == 100;
    }
    #endregion

    #region Cloud Save/Load Methods
    
    public void ShowSelectUI() 
    {
        uint maxNumToDisplay = 5;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }

    public void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata game) 
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            OpenSavedGameForReading(game.Filename);
        }
        else
        {
            // handle cancel or error
        }
    }

    public void OpenSavedGameForWriting(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForWriting);
    }

    public void OpenSavedGameForReading(string filename) 
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForReading);
    }

    public void OnSavedGameOpenedForWriting(SavedGameRequestStatus status, ISavedGameMetadata game) 
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle writing of saved game.
            currentGame = game;
            // Prepare the data for writing
            int numberOfValues = 6 + birdHouse.birdInfos.Capacity * 2;
            char separator = ';';
            string userDataString = $"{(int)userData.GetDifficultyLevel().level}{separator}" +
                $"{userData.EasyRecord}{separator}" +
                $"{userData.NormalRecord}{separator}" +
                $"{userData.HardRecord}{separator}" +
                $"{userData.Coins}{separator}" +
                $"{userData.CurrentBirdIndex}{separator}";

            string birdHouseString = "";
            foreach(var birdInfo in birdHouse.birdInfos)
            {
                birdHouseString += (birdInfo.purchased == true ? "1" : "0") + separator;
            }
            foreach(var birdInfo in birdHouse.birdInfos)
            {
                birdHouseString += birdInfo.aggregatedScore.ToString() + separator;
            }
            
            byte[] newData = System.Text.Encoding.UTF8.GetBytes(userDataString + birdHouseString);
            
            TimeSpan totalPlayTime = new TimeSpan();
            if(userData.FirstTimeOnDevice == 1)
                totalPlayTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            else
                totalPlayTime = game.TotalTimePlayed + TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            
            SaveGame(game, newData, totalPlayTime);
        } 
        else
        {
            // handle error
            Debug.Log($"Error when opening save game. Status = {status.ToString()}");
        }
    }

    public void OnSavedGameOpenedForReading(SavedGameRequestStatus status, ISavedGameMetadata game) 
    {
        if (status == SavedGameRequestStatus.Success) {
            // handle reading or writing of saved game.
            currentGame = game;
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, ReadBinaryCallback);
        } else {
            // handle error
            Debug.Log($"Error when reading from the cloud. Status = {status.ToString()}");
        }
    }

    /// <summary>
    /// Stores the cloud save locally
    /// </summary>
    /// <param name="status"></param>
    /// <param name="data"></param>
    private void ReadBinaryCallback (SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success) 
        {    
            if(userData.FirstTimeOnDevice == 1 || data == null) return; // No prior data

            // Read score from the Saved Game
            try
            {
                string decodedString = System.Text.Encoding.UTF8.GetString(data);
                // Handle the data
                char separator = ';';
                string[] values = decodedString.Split(separator);
                userData.SetDifficultyLevelByIndex(int.Parse(values[0]));
                userData.EasyRecord = int.Parse(values[1]);
                userData.NormalRecord = int.Parse(values[2]);
                userData.HardRecord = int.Parse(values[3]);
                userData.Coins = int.Parse(values[4]);
                userData.CurrentBirdIndex = int.Parse(values[5]);

                int numberOfBirds = birdHouse.birdInfos.Capacity;
                int maxNumber = numberOfBirds + 6;
                int counter = 0;
                for(int i = 6; i < maxNumber; i++)
                {
                    birdHouse.birdInfos[counter].purchased = int.Parse(values[i]) == 1 ? true : false;
                    counter++;
                }
                counter = 0;
                int newMaxNumber = maxNumber + numberOfBirds;
                for(int i = maxNumber; i < newMaxNumber; i++)
                {
                    birdHouse.birdInfos[counter].aggregatedScore = int.Parse(values[i]);
                    counter++;
                }
                
                // Make sure that Leaderboard records are the same that the cloud saves' ones
                LoadPlayerRecordFromLeaderboard(Difficulty.Easy);
                LoadPlayerRecordFromLeaderboard(Difficulty.Normal);
                LoadPlayerRecordFromLeaderboard(Difficulty.Hard);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        
        EventBroker.CallCloudSaveLoaded();
    }

    void SaveGame (ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime) 
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        Debug.Log($"totalPlaytime = {totalPlaytime.Seconds}");
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription($"Saved game at {DateTime.Now}");
        if (savedImage != null)
        {
            // This assumes that savedImage is an instance of Texture2D
            // and that you have already called a function equivalent to
            // getScreenshot() to set savedImage
            // NOTE: see sample definition of getScreenshot() method below
            byte[] pngData = savedImage.EncodeToPNG();
            builder = builder.WithUpdatedPngCoverImage(pngData);
        }
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) 
    {
        if (status == SavedGameRequestStatus.Success) {
            // handle reading or writing of saved game.
            Debug.Log("Game saved to the cloud successfully");
        } else {
            // handle error
            Debug.Log($"Error when trying to save to the cloud. Status = {status.ToString()}");
        }
    }

    public Texture2D getScreenshot() 
    {
        // Create a 2D texture that is 1024x700 pixels from which the PNG will be
        // extracted
        Texture2D screenShot = new Texture2D(1024, 700);

        // Takes the screenshot from top left hand corner of screen and maps to top
        // left hand corner of screenShot texture
        screenShot.ReadPixels(
            new Rect(0, 0, Screen.width, (Screen.width/1024)*700), 0, 0);
        return screenShot;
    }

    void LoadGameData (ISavedGameMetadata game) 
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) 
    {
        if (status == SavedGameRequestStatus.Success) {
            // handle processing the byte array data
        } else {
            // handle error
        }
    }

    void DeleteGameData (string filename) 
    {
        // Open the file to get the metadata.
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
    }

    public void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game) 
    {
        if (status == SavedGameRequestStatus.Success) {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(game);
        } else {
            // handle error
        }
    }

    #endregion
}
