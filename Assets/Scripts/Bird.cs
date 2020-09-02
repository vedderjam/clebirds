using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float upForce = 200f;

    private bool isDead = true;
    private Rigidbody2D rb2d;
    private Animator anim;
    private Vector3 initialPosition;
    private int currentDifficultyLevel = 0;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip scoreClip;
    public AudioClip dieClip;
    public AudioClip hopClip;

    #region Unity Callbacks

    private void OnEnable()
    {
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.StartIdling += ResetBird;
        UpdateManager.UpdateEvent += UpdateEvent;
        EventBroker.ChangeDifficultyLevel += ChangeDifficultyLevel;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        initialPosition = new Vector3(-1.5f, 0f, 0f);
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.velocity = Vector2.zero;
    }

    private void UpdateEvent()
    {
        if (!isDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Flap();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb2d.velocity = Vector2.zero;
        if(!isDead)
            EventBroker.CallGameOver();
        isDead = true;
        anim.SetInteger("Speed", -1);
    }

    private void OnDisable()
    {
        EventBroker.StartPlaying -= StartPlaying;
        EventBroker.StartIdling -= ResetBird;
        UpdateManager.UpdateEvent -= UpdateEvent;
        EventBroker.ChangeDifficultyLevel -= ChangeDifficultyLevel;
    }

    private void OnDrawGizmos()
    {
        if(rb2d != null)
            Gizmos.DrawLine(transform.position, rb2d.velocity);
    }
    
    #endregion

    #region Events
    
    private void StartPlaying()
    {
        isDead = false;
        ResetBird();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        Flap();
    }
    
    #endregion

    #region Methods
    
    private void Flap()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(0f, upForce));

        audioSource.PlayOneShot(hopClip);
    }

    private void ResetBird()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.Sleep();
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.WakeUp();
        anim.SetInteger("Speed", currentDifficultyLevel);
    }

    private void ChangeDifficultyLevel()
    {
        currentDifficultyLevel = (int) GameControl.Instance.currentDifficultyLevel.level;

        if(anim == null)
            Debug.Log("anim not assigned");
        else if(currentDifficultyLevel >= 0 && currentDifficultyLevel <= 2)
            anim.SetInteger("Speed", currentDifficultyLevel);
    }

    #endregion
}
