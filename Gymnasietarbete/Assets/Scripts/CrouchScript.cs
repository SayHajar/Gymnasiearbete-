using UnityEngine;

public class CrouchScript : MonoBehaviour
{
    [SerializeField] private float crouchHeightMultiplier = 0.5f;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalPlayerScale;
    private Vector2 originalSpriteSize;

public bool isCrouching = false;

    private void Start()
    {
        // Automatically grab components on this same GameObject if not assigned in Inspector
        if (col == null)
            col = GetComponent<BoxCollider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (col != null)
        {
            originalColliderSize = col.size;
            originalColliderOffset = col.offset;
        }

        originalPlayerScale = transform.localScale;

        if (spriteRenderer != null)
        {
            originalSpriteSize = spriteRenderer.size;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StandUp();
        }
    }

    private void Crouch()
    {
        if (isCrouching) return;
        isCrouching = true;

        // 1. Resize BoxCollider2D
        if (col != null)
        {
            float newHeight = originalColliderSize.y * crouchHeightMultiplier;
            col.size = new Vector2(originalColliderSize.x, newHeight);

            float heightDelta = originalColliderSize.y - newHeight;
            col.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDelta / 2f));
        }

        // 2. Scale Sprite Visuals
        if (spriteRenderer != null && spriteRenderer.drawMode != SpriteDrawMode.Simple)
        {
            // If Draw Mode is Sliced/Tiled, scale size directly without affecting child GameObjects
            spriteRenderer.size = new Vector2(originalSpriteSize.x, originalSpriteSize.y * crouchHeightMultiplier);
        }
        else
        {
            // Fallback: Scale Player Transform Y-axis
            transform.localScale = new Vector3(
                originalPlayerScale.x,
                originalPlayerScale.y * crouchHeightMultiplier,
                originalPlayerScale.z
            );
        }
    }

    private void StandUp()
    {
        if (!isCrouching) return;
        isCrouching = false;

        // 1. Restore BoxCollider2D
        if (col != null)
        {
            col.size = originalColliderSize;
            col.offset = originalColliderOffset;
        }

        // 2. Restore Sprite Visuals
        if (spriteRenderer != null && spriteRenderer.drawMode != SpriteDrawMode.Simple)
        {
            spriteRenderer.size = originalSpriteSize;
        }
        else
        {
            transform.localScale = originalPlayerScale;
        }
    }
}