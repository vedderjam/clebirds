using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DayTime {Dawn = 0, Noon, Afternoon, Night};

public class TransitionTime : MonoBehaviour
{
    [SerializeField] private DayTime dayTime;
    [SerializeField] private SpriteRenderer[] spriteRenderers; // 0 = Noon, 1 = Dawn/Afternoon, 2 = Night
    [SerializeField] readonly float transitionTime = 4f;

    #region Unity Callbacks

    private void OnEnable()
    {
        EventBroker.TimeTransition += Transition;
        EventBroker.GameOver += EventBroker_GameOver;
    }

    private void OnDisable()
    {
        EventBroker.TimeTransition -= Transition;
        EventBroker.GameOver -= EventBroker_GameOver;
    }

    void Start()
    {
        dayTime = DayTime.Noon;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        var count = spriteRenderers.Length - 1;
        
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = count;
            count--;
        }
    }

    #endregion

    #region Events

    private void EventBroker_GameOver()
    {
        StopAllCoroutines();
        StartCoroutine(ToNoon());
    }

    public void Transition()
    {
        StopAllCoroutines();
        switch (dayTime)
        {
            case DayTime.Dawn:
                StartCoroutine(nameof(FromDawnToNoon));
                break;
            case DayTime.Noon:
                StartCoroutine(nameof(FromNoonToAfternoon));
                break;
            case DayTime.Afternoon:
                StartCoroutine(nameof(FromAfternoonToNight));
                break;
            case DayTime.Night:
                StartCoroutine(nameof(FromNightToDawn));
                break;
        }
    }

    #endregion

    #region Methods

    private void SetCurrentTimeSprites()
    {
        var noon = spriteRenderers[0];
        var dawn = spriteRenderers[1];

        switch (dayTime)
        {
            case DayTime.Dawn:
                break;
            case DayTime.Noon:
                break;
            case DayTime.Afternoon:
                break;
            case DayTime.Night:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set noon's sprite alpha color to 0
    /// </summary>
    /// <returns></returns>
    private IEnumerator FromNoonToAfternoon()
    {
        dayTime = DayTime.Afternoon;
        SpriteRenderer noon = spriteRenderers[0];
        var targetAlpha = 0f;
        var velocity = 0f;

        while(!Mathf.Approximately(noon.color.a, targetAlpha))
        {
            var alpha = Mathf.SmoothDamp(noon.color.a, targetAlpha, ref velocity, transitionTime);
            noon.color = new Color(noon.color.r, noon.color.g, noon.color.b, alpha);

            yield return null;
        }
    }

    /// <summary>
    /// Set afternoon's sprite alpha color to 0
    /// </summary>
    private IEnumerator FromAfternoonToNight()
    {
        dayTime = DayTime.Night;
        SpriteRenderer afternoon = spriteRenderers[1];
        var targetAlpha = 0f;
        var velocity = 0f;

        while(!Mathf.Approximately(afternoon.color.a, targetAlpha))
        {
            Color color = afternoon.color;

            var alpha = Mathf.SmoothDamp(color.a, targetAlpha, ref velocity, transitionTime);
            afternoon.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }
    }

    /// <summary>
    /// Set dawn's sprite alpha color to 1
    /// </summary>
    private IEnumerator FromNightToDawn()
    {
        dayTime = DayTime.Dawn;
        SpriteRenderer dawn = spriteRenderers[1];
        var targetAlpha = 1f;            
        var velocity = 0f;

        while(!Mathf.Approximately(dawn.color.a, targetAlpha))
        {
            Color color = dawn.color;

            var alpha = Mathf.SmoothDamp(color.a, targetAlpha, ref velocity, transitionTime);
            dawn.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }
    }

    /// <summary>
    /// Set noon's sprite alpha color to 1
    /// </summary>
    private IEnumerator FromDawnToNoon()
    {
        dayTime = DayTime.Noon;
        SpriteRenderer noon = spriteRenderers[0];
        var targetAlpha = 1f;
        var velocity = 0f;

        while(!Mathf.Approximately(noon.color.a, targetAlpha))
        {
            Color color = noon.color;

            var alpha = Mathf.SmoothDamp(color.a, targetAlpha, ref velocity, transitionTime);
            noon.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }
    }

    private IEnumerator ToNoon()
    {
        dayTime = DayTime.Noon;

        var noon = spriteRenderers[0];
        var dawn = spriteRenderers[1];
        var targetAlpha = 1f;
        var noonVelocity = 0f;
        var dawnVelocity = 0f;

        while(!Mathf.Approximately(noon.color.a, targetAlpha))
        {
            var noonAlpha = Mathf.SmoothDamp(noon.color.a, targetAlpha, ref noonVelocity, transitionTime);
            noon.color = new Color(noon.color.r, noon.color.g, noon.color.b, noonAlpha);

            var dawnAlpha = Mathf.SmoothDamp(dawn.color.a, targetAlpha, ref dawnVelocity, transitionTime);
            dawn.color = new Color(dawn.color.r, dawn.color.g, dawn.color.b, dawnAlpha);

            yield return null;
        }
    }

    #endregion
}
