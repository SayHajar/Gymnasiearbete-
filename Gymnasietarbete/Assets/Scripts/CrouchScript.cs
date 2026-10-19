using UnityEngine;

public class CrouchScript : MonoBehaviour
{
    [SerializeField] private float crouchHeightMultiplier = 0.5f;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private GameObject spriteObj;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalSpriteScale;

    private void Start()
    {
        if (col == null)
            col = GetComponent<BoxCollider2D>();

        if (col != null)
        {
            originalColliderSize = col.size;
            originalColliderOffset = col.offset;
        }

        if (spriteObj != null)
        {
            originalSpriteScale = spriteObj.transform.localScale;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
        }
        else
        {
            StandUp();
        }
    }

    private void Crouch()
    {
        if (col != null)
        {
            float newHeight = originalColliderSize.y * crouchHeightMultiplier;
            col.size = new Vector2(originalColliderSize.x, newHeight);

            float heightDelta = originalColliderSize.y - newHeight;
            col.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDelta / 2f));
        }

        if (spriteObj != null)
        {
            spriteObj.transform.localScale = new Vector3(
                originalSpriteScale.x,
                originalSpriteScale.y * crouchHeightMultiplier,
                originalSpriteScale.z
            );
        }
    }

    private void StandUp()
    {
        if (col != null)
        {
            col.size = originalColliderSize;
            col.offset = originalColliderOffset;
        }

        if (spriteObj != null)
        {
            spriteObj.transform.localScale = originalSpriteScale;
        }
    }
}