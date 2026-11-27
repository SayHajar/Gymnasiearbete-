using System.Collections;
using TMPro;
using UnityEngine;

public class ItemWorldObj : MonoBehaviour
{
    // storkleken på trigger col så att man kan alltid plocka upp
    // den adapterar till sprite-n
    private const float TriggerScaleFactor = 1.3f;
    private const float PickupCooldown = 0.5f; // palla inte med mer sizing shit så cooldown ig

    [SerializeField] private int amount;
    private TextMeshPro amountTxt;
    [SerializeField] private ItemDefinition itemDef;
    private float pickupReadyTime;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        amountTxt = transform.Find("amountTxt").GetComponent<TextMeshPro>();

    }

    private void Start()
    {
        spriteRenderer.sprite = itemDef.icon;
        spriteRenderer.sortingOrder = 55;
    }

    public bool CanPickup()
    {
        return Time.time >= pickupReadyTime;
    }

    public static ItemWorldObj SpawnItemWorldObj(Vector3 position, ItemDefinition itemDef, int amount,
        ItemWorldObj prefab, bool fling = false)
    {
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.SetItem(itemDef, amount);


        if (fling)
        {
            obj.pickupReadyTime = Time.time + PickupCooldown;

            var rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // bigger sprites flung harder so you dont pick them up instantely
                var size = obj.spriteRenderer.bounds.size.magnitude;
                var flingMultiplier = 1f + size;

                var baseVelocity = new Vector2(1.5f, 1.5f) * flingMultiplier;
                rb.linearVelocity = baseVelocity + Random.Range(-1f, 1f) * new Vector2(1f, 0.5f);
                obj.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f));
                rb.angularVelocity = Random.Range(-500f, 500f);
            }
        }


        return obj;
    }

    /// <summary>
    /// Spawnar ett föremål som kastas med en bestämd hastighet (kraft) och tyngd.
    /// </summary>
    public static ItemWorldObj SpawnThrown(Vector3 position, ItemDefinition itemDef, int amount,
        ItemWorldObj prefab, Vector2 velocity, float gravityScale = 1f, float spin = 360f)
    {
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.SetItem(itemDef, amount);
        obj.pickupReadyTime = Time.time + PickupCooldown;

        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
            rb.linearVelocity = velocity;
            rb.angularVelocity = Random.Range(-spin, spin);
        }

        obj.StartCoroutine(obj.IgnorePlayerCollisionRoutine());
        return obj;
    }

    // Gör så att det kastade föremålet inte studsar på / plockas upp av spelaren direkt
    private IEnumerator IgnorePlayerCollisionRoutine()
    {
        // SetItem använder Destroy() på gamla colliders, vänta en frame så de är borta
        yield return null;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        var mine = GetComponents<Collider2D>();
        var theirs = player.GetComponentsInChildren<Collider2D>();

        foreach (var a in mine)
            foreach (var b in theirs)
                if (a != null && b != null)
                    Physics2D.IgnoreCollision(a, b, true);

        yield return new WaitForSeconds(PickupCooldown);

        foreach (var a in mine)
            foreach (var b in theirs)
                if (a != null && b != null)
                    Physics2D.IgnoreCollision(a, b, false);
    }

    public void SetItem(ItemDefinition itemDef, int amount)
    {
        this.itemDef = itemDef;
        this.amount = amount;
        spriteRenderer.sprite = itemDef.icon;
        spriteRenderer.sortingOrder = 55;
        amountTxt.enabled = amount > 1;
        if (amount > 1) amountTxt.text = amount.ToString();

        // regenerate collider geometry to match new sprite
        // might be stupid but i like funni physics!!!
        var existingColliders = GetComponents<PolygonCollider2D>();
        foreach (var col in existingColliders)
            Destroy(col);

        // physics collider for funni
        var physicsCollider = gameObject.AddComponent<PolygonCollider2D>();
        physicsCollider.isTrigger = false;

        // trigger collider (för pickup) slightly larger than the sprite shape
        var triggerCollider = gameObject.AddComponent<PolygonCollider2D>();
        triggerCollider.isTrigger = true;
        for (var i = 0; i < triggerCollider.pathCount; i++)
        {
            var path = triggerCollider.GetPath(i);
            for (var j = 0; j < path.Length; j++)
                path[j] *= TriggerScaleFactor;
            triggerCollider.SetPath(i, path);
        }
    }

    public (ItemDefinition, int) GetItem()
    {
        return (itemDef, amount);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && this.CanPickup())
        {
            collision.gameObject.GetComponent<Inventory>().AddItem(itemDef, amount);
            DestroySelf();
        }

    }
}