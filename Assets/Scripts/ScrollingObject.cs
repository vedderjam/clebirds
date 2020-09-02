using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScrollingObject : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField]private float scrollSpeed = -1.5f;

    public SpriteRenderer spriteRenderer;
    private string sortingLayer;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            sortingLayer = spriteRenderer.sortingLayerName;
        else
            sortingLayer = GetComponent<SpriteRenderer>().sortingLayerName;

        EventBroker.StartPlaying += Setup;
        EventBroker.StartIdling += Setup;
        EventBroker.GameOver += GameOver;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;
    }

    private void OnDestroy()
    {
        EventBroker.StartPlaying -= Setup;
        EventBroker.StartIdling -= Setup;
        EventBroker.GameOver -= GameOver;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
    }

    private void Setup()
    {
        rb2d.velocity = new Vector2(scrollSpeed, 0f);    
    }

    void Start()
    {
        Setup();
    }
    
    private void GameOver()
    {
        rb2d.velocity = Vector2.zero;
    }

    private void ChangeDifficultyLevel()
    {
        if(sortingLayer == "Background")
            scrollSpeed = GameControl.Instance.currentDifficultyLevel.backGroundScrollSpeed;
        else if(sortingLayer == "Foreground")
            scrollSpeed = GameControl.Instance.currentDifficultyLevel.foreGroundScrollSpeed;
        else
            scrollSpeed = GameControl.Instance.currentDifficultyLevel.scrollSpeed;
        
        Setup();
    }
}
