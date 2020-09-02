using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [Header("Positions")]
    public List<Transform> bushSpriteTransforms;
    public List<Vector3> originalPositionOfBushes;

    [Header("Sprites")]
    public List<SpriteRenderer> spriteRenderersA;
    public List<SpriteRenderer> spriteRenderersB;
    public List<SpriteRenderer> spriteRenderersC;

    [Header("Offsets")]
    public float minOffset;
    public float maxOffset;
    public float bushOffset;

    [Header("Sorting Layers")]
    public string visibleLayer = "Midground2";
    public string nonVisibleLayer = "Invisible";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Bird>() != null)
        {
            EventBroker.CallBirdScored();
        }
    }

    public void SetBushOffset(float bushOffset)
    {
        this.bushOffset = bushOffset;
        originalPositionOfBushes[0] = new Vector3(0f, bushOffset);
        originalPositionOfBushes[2] = new Vector3(0f, -bushOffset);
    }

    public void SetBushesPositions()
    {
        var numberOfSprites = bushSpriteTransforms.Capacity;

        for(int i = 0; i < numberOfSprites; i++)
        {
            bushSpriteTransforms[i].localPosition = 
                originalPositionOfBushes[i] + new Vector3(0f, Random.Range(minOffset, maxOffset));
        }

        ToggleBushesSprites(ref spriteRenderersA);
        ToggleBushesSprites(ref spriteRenderersB);
        ToggleBushesSprites(ref spriteRenderersC);
    }

    private void ToggleBushesSprites(ref List<SpriteRenderer> sprites)
    {
        int max = sprites.Capacity;
        int groupCount = bushSpriteTransforms.Capacity;
        int rnd = Random.Range(0, groupCount);
        int low = rnd * groupCount;
        int high = rnd * groupCount + groupCount;
        
        for(int i = 0; i < max; i++)
        {
            if(i >= low && i < high)
            {
                sprites[i].sortingLayerName = visibleLayer;
                var collider = sprites[i].gameObject.GetComponent<PolygonCollider2D>();
                if(collider != null)
                    collider.isTrigger = false;
            }
            else
            {
                sprites[i].sortingLayerName = nonVisibleLayer;
                var collider = sprites[i].gameObject.GetComponent<PolygonCollider2D>();
                if(collider != null)
                    collider.isTrigger = true;
            }
        }
    }
}
