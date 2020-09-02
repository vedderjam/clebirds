using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine;

public class GPGSController : Singleton<GPGSController>
{
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
        PlayGamesPlatform.DebugLogEnabled = true;
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
    #endregion

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
}



