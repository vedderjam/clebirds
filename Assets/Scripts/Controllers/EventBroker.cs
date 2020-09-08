using System;
using UnityEngine;

public class EventBroker
{
    public static event Action GameOver;
    public static event Action BirdScored;
    public static event Action StartPlaying;
    public static event Action StartIdling;
    public static event Action ChangeDifficultyLevel;
    public static event Action NotEnoughCoins;
    public static event Action<int> BirdPurchased;
    public static event Action<int> BirdSelected;
    public static event Action GamePaused;
    public static event Action GameResumed;
    public static event Action EarnedRewardedAd;
    public static event Action TimeTransition;
    public static event Action<int> NewInfoPills;
    public static event Action SocialSignedIn;
    public static event Action SocialSignedOut;
    public static event Action<int, string> PostScore;
    public static event Action CloudSaveLoaded;

    public static void CallGameOver()
    {
        GameOver?.Invoke();
    }

    public static void CallBirdScored()
    {
        BirdScored?.Invoke();
    }

    public static void CallStartPlaying()
    {
        StartPlaying?.Invoke();
    }

    public static void CallStartIdling()
    {
        StartIdling?.Invoke();
    }

    public static void CallChangeDifficultyLevel()
    {
        ChangeDifficultyLevel?.Invoke();
    }

    public static void CallNotEnoughCoins()
    {
        NotEnoughCoins?.Invoke();
    }

    public static void CallBirdPurchased(int index)
    {
        BirdPurchased?.Invoke(index);
    }
    
    public static void CallBirdSelected(int index)
    {
        BirdSelected?.Invoke(index);
    }

    public static void CallGamePaused()
    {
        GamePaused?.Invoke();
    }

    public static void CallGameResumed()
    {
        GameResumed?.Invoke();
    }

    public static void CallEarnedRewardedAd()
    {
        EarnedRewardedAd?.Invoke();
    }

    public static void CallTransitionTime()
    {
        TimeTransition?.Invoke();
    }

    public static void CallNewInfoPills(int newPills)
    {
        NewInfoPills?.Invoke(newPills);
    }

    public static void CallSocialSignedIn()
    {
        SocialSignedIn?.Invoke();
    }

    public static void CallSocialSignedOut()
    {
        SocialSignedOut?.Invoke();
    }

    public static void CallPostScore(int score, string leaderboardID)
    {
        PostScore?.Invoke(score, leaderboardID);
    }

    public static void CallCloudSaveLoaded()
    {
        CloudSaveLoaded?.Invoke();
    }
}
