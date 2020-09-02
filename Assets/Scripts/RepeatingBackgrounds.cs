using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RepeatingBackgrounds : MonoBehaviour
{
    private BoxCollider2D groundCollider;
    private float groundHorizontalLength;

    void Start()
    {
        groundCollider = GetComponent<BoxCollider2D>();
        groundHorizontalLength = groundCollider.size.x;
    }

    private void UpdateEvent()
    {
        if(transform.position.x < -groundHorizontalLength)
        {
            RepositionBackground();
        }
    }

    private void Awake()
    {
        UpdateManager.UpdateEvent += UpdateEvent;
    }

    private void OnDestroy()
    {
        UpdateManager.UpdateEvent -= UpdateEvent;
    }

    private void RepositionBackground()
    {
        Vector2 groundOffset = new Vector2(groundHorizontalLength * 2f, 0);
        transform.position = (Vector2) transform.position + groundOffset;
    }
}
