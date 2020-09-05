using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("GameObject")]
    public GameObject difficultyPanel;
    public GameObject gameOverPanel;
    public GameObject mainUIPanel;
    public GameObject birdsSelectionPanel;
    public GameObject startButton;
    public GameObject difficultyButton;
    public GameObject birdsSelectionButton;
    public GameObject creditsButton;
    public GameObject gamePausedPanel;
    public GameObject pauseButton;
    public GameObject newRecord;
    public GameObject playGamesButtonsParent;

    [Header("Birds selection")]
    public BirdHouse birdHouse;
    public Transform birdsButtonsParent;
    public Button[] birdsButtons;

    [Header("Text")]
    public Text scoreText;
    public Text hiScoreText;
    public Text coinsText;
    public Text selectionScreenCoinsText;
    public Text finalScoreText;
    public Text rewardText;
    public Text moreRewardText;
    public Text birdNameText;
    public Text birdNameInWikiText;
    public Text aggregatedScoreText;
    public List<Text> pillTexts;
    public Text newInfoPillsText;
    public Text GPGSButtonText;

    [Header("Animators")]
    public Animator coinsTextAnimator;
    public Animator coinsImageAnimator;
    public Animator rewardTextAnimator;
    public Animator hiScoreTextAnimator;    
    public Animator notEnoughCoinsAnimator;
    public Animator moneyEffectAnimator;

    [Header("Images")]
    public List<Image> difficultyLevelImages;
    public List<Image> selectedBirdImages;

    [Header("Buttons")]
    public Button playGamesSignInButton;
    public Button playGamesSignOutButton;
    public Button playGamesLeaderboards;

    private void Awake()
    {
        EventBroker.BirdScored += BirdScored;
        EventBroker.GameOver += GameOver;
        EventBroker.StartIdling += Idle;
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;
        EventBroker.NotEnoughCoins += NotEnoughCoins;
        EventBroker.BirdPurchased += BirdPurchased;
        EventBroker.BirdSelected += BirdSelected;
        EventBroker.GamePaused += PauseGame;
        EventBroker.GameResumed += ResumeGame;
        EventBroker.EarnedRewardedAd += EventBroker_EarnedRewardedAd;
        EventBroker.NewInfoPills += EventBroker_NewInfoPills;
        EventBroker.SocialSignedIn += EventBroker_SocialSignedIn;
        EventBroker.SocialSignedOut += EventBroker_SocialSignedOut;
    }

    private void Start()
    {
        SetUITexts();
        SetBirdsSelectionButtons();

        #if DEVELOPMENT_BUILD || UNITY_EDITOR
            pauseButton.SetActive(true);
        #endif
    }

    private void SetUITexts()
    {
        scoreText.text = $"{GameControl.Score}";
        hiScoreText.text = $"RÉCORD: {GameControl.Record}";
        coinsText.text = GameControl.Coins.ToString();
        selectionScreenCoinsText.text = GameControl.Coins.ToString();
    }

    private void SetBirdsSelectionButtons()
    {
        birdsButtons = birdsButtonsParent.GetComponentsInChildren<Button>();

        int numberOfBirds = birdHouse.birdInfos.Capacity;
        int count = 0;
        foreach(var button in birdsButtons)
        {
            if(count < numberOfBirds)
            {
                button.interactable = true;
                Debug.Assert(button.interactable);
                var birdInfo = birdHouse.birdInfos[count];
                button.image.sprite = birdInfo.sprite;
                var texts = button.GetComponentsInChildren<Text>(true);
                texts[0].text = birdInfo.name;

                if (!birdInfo.purchased)
                {
                    texts[1].text = birdInfo.prize.ToString();
                    texts[1].gameObject.SetActive(true);
                    button.transform.GetChild(3).gameObject.SetActive(true); // activate the coin image
                }

                int i = count; // Dear closure
                button.onClick.AddListener(delegate{ 
                    GameControl.Instance.TryToBuyAndOrSelectBird(i);
                });
                count++;
            }
        }
    }

    private void StartPlaying()
    {
        startButton.SetActive(false);
        scoreText.text = "0";
    }

    private void OnDestroy()
    {
        EventBroker.BirdScored -= BirdScored;
        EventBroker.GameOver -= GameOver;
        EventBroker.StartIdling -= Idle;
        EventBroker.StartPlaying -= StartPlaying;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
        EventBroker.NotEnoughCoins -= NotEnoughCoins;
        EventBroker.BirdPurchased -= BirdPurchased;
        EventBroker.BirdSelected -= BirdSelected;
        EventBroker.GamePaused -= PauseGame;
        EventBroker.GameResumed -= ResumeGame;
        EventBroker.EarnedRewardedAd -= EventBroker_EarnedRewardedAd;
        EventBroker.NewInfoPills -= EventBroker_NewInfoPills;
        EventBroker.SocialSignedIn -= EventBroker_SocialSignedIn;
        EventBroker.SocialSignedOut -= EventBroker_SocialSignedOut;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        UpdateRewardText();
        UpdateSelectedBirdInfo();
        SetWikiForSelectedBird();
    }

    private void Idle()
    {
        UpdateHighScore();
        UpdateHighScoreColor();
        UpdateCoins();
        gameOverPanel.SetActive(false);
        startButton.SetActive(true);
        difficultyButton.SetActive(true);
        birdsSelectionButton.SetActive(true);
        creditsButton.SetActive(true);
        newInfoPillsText.gameObject.SetActive(false);
        playGamesButtonsParent.SetActive(true); //TODO: Reactivar cuando se habilite Games Service
    }

    private void BirdScored()
    {
        scoreText.text = $"{GameControl.Score}";
    }

    private void ChangeDifficultyLevel()
    {
        UpdateHighScore();
        UpdateHighScoreColor();
        UpdateDifficultyButtonsColor();
    }

    private void NotEnoughCoins()
    {
        notEnoughCoinsAnimator.gameObject.SetActive(true);
        notEnoughCoinsAnimator.SetTrigger("Fade");
    }
    
    private void BirdPurchased(int index)
    {
        UpdateCoins();
        HideBirdPrize(index);
        ShowMoneySpentEffect(index);
    }

    private void HideBirdPrize(int index)
    {
        var children = birdsButtons[index].GetComponentsInChildren<RectTransform>(true);
        children[3].gameObject.SetActive(false); // Prize text
        children[4].gameObject.SetActive(false); // Coin Image
    }
            
    private void ShowMoneySpentEffect(int birdIndex)
    {
        moneyEffectAnimator.gameObject.GetComponentInChildren<Text>().text = $"-{birdHouse.birdInfos[birdIndex].prize}";
        moneyEffectAnimator.gameObject.SetActive(true);
        moneyEffectAnimator.SetTrigger("Fade");
    }

    private void BirdSelected(int index)
    {
        int counter = selectedBirdImages.Capacity;
        for(int i = 0; i < counter; i++)
        {
            if(i == index)
                selectedBirdImages[i].gameObject.SetActive(true);
            else selectedBirdImages[i].gameObject.SetActive(false);
        }
        
        UpdateSelectedBirdInfo();
        SetWikiForSelectedBird();
    }
    
    private void EventBroker_NewInfoPills(int obj)
    {
        if(obj == 1)
            newInfoPillsText.text = "¡NUEVA PÍLDORA DESBLOQUEADA!";
        else 
            newInfoPillsText.text = $"{obj} NUEVAS PÍLDORAS DESBLOQUEADAS!";

        newInfoPillsText.gameObject.SetActive(true);
    }

    private void EventBroker_SocialSignedOut()
    {
        Debug.Log("Signed Out");
        playGamesSignOutButton.gameObject.SetActive(false);
        playGamesSignInButton.gameObject.SetActive(true);
        playGamesLeaderboards.gameObject.SetActive(false);
    }

    private void EventBroker_SocialSignedIn()
    {
        Debug.Log("Signed In");
        playGamesSignOutButton.gameObject.SetActive(true);
        playGamesSignInButton.gameObject.SetActive(false);
        playGamesLeaderboards.gameObject.SetActive(true);
    }

    private void UpdateSelectedBirdInfo()
    {
        aggregatedScoreText.text = birdHouse.birdInfos[GameControl.CurrentBirdIndex].aggregatedScore.ToString();
        birdNameText.text = birdHouse.birdInfos[GameControl.CurrentBirdIndex].name;
    }

    public void RaiseGamePausedEvent()
    {
        EventBroker.CallGamePaused();
    }

    private void PauseGame()
    {
        print("UIManager PauseGame");
        mainUIPanel.SetActive(false);
        gamePausedPanel.SetActive(true);
    }

    public void RaiseResumeGameEvent()
    {
        EventBroker.CallGameResumed();
    }

    private void ResumeGame()
    {
        print("UIManager ResumeGame");
        mainUIPanel.SetActive(true);
        gamePausedPanel.SetActive(false);
    }
    
    private void EventBroker_EarnedRewardedAd()
    {
        rewardText.text = GameControl.Reward.ToString();
        rewardTextAnimator.SetTrigger("Flash");
    }

    private void UpdateHighScore()
    {
        hiScoreText.text = $"RÉCORD: {GameControl.Record}";
        if(GameControl.NewRecord)
            hiScoreTextAnimator.SetTrigger("Flash");
    }

    private void UpdateHighScoreColor()
    {
        switch (GameControl.Instance.currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                hiScoreText.color = Color.green;
                break;
            case Difficulty.Normal:
                hiScoreText.color = Color.yellow;
                break;
            case Difficulty.Hard:
                hiScoreText.color = Color.red;
                break;
            default:
                hiScoreText.color = Color.white;
                break;
        }
    }

    private void UpdateDifficultyButtonsColor()
    {
        switch (GameControl.Instance.currentDifficultyLevel.level)
        {
            case Difficulty.Easy:
                difficultyLevelImages[0].color = Color.green;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.white;
                break;
            case Difficulty.Normal:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.grey;
                difficultyLevelImages[2].color = Color.white;
                break;
            case Difficulty.Hard:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.red;
                break;
            default:
                difficultyLevelImages[0].color = Color.white;
                difficultyLevelImages[1].color = Color.white;
                difficultyLevelImages[2].color = Color.white;
                break;
        }
    }

    private void UpdateCoins()
    {
        selectionScreenCoinsText.text = GameControl.Coins.ToString();
        coinsText.text = GameControl.Coins.ToString();
        coinsTextAnimator.SetTrigger("Flash");
        coinsImageAnimator.SetTrigger("Flash");
    }

    private void UpdateRewardText()
    {
        if(GameControl.NewRecord)
            newRecord.SetActive(true);
        else newRecord.SetActive(false);

        finalScoreText.text = scoreText.text;
        rewardText.text =  GameControl.Reward.ToString();
        
        if(GameControl.MoreReward == GameControl.Reward)
            moreRewardText.text = $"x2";
        else 
            moreRewardText.text = $"+{GameControl.MoreReward}";
    }

    private void ToMainUIPanel()
    {
        difficultyPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        birdsSelectionPanel.SetActive(false);

        mainUIPanel.SetActive(true);
    }

    private void SetWikiForSelectedBird()
    {
        int currentBird = GameControl.CurrentBirdIndex;
        int count = pillTexts.Capacity;
        float rectSize = 0f;

        for(int i = 0; i < count; i++)
        {
            if(birdHouse.birdInfos[currentBird].aggregatedScore >= birdHouse.birdInfos[currentBird].scoreNeededToShowInfoPill[i])
                pillTexts[i].text = birdHouse.birdInfos[currentBird].infoPills[i];
            else
            {
                int remainingPoints = birdHouse.birdInfos[currentBird].scoreNeededToShowInfoPill[i] - birdHouse.birdInfos[currentBird].aggregatedScore;
                pillTexts[i].text = $"Necesitas {remainingPoints} puntos más para desbloquear esta píldora.";
            }
            rectSize += pillTexts[i].preferredHeight;
        }

        birdNameInWikiText.text = birdNameText.text;
    }
}
