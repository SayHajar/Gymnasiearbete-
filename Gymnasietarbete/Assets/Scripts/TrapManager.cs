using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private GameObject spike;
    
    bool playerInTrap = false;
    public float minY = -1.2f;
    bool hasLanded = false;
    public Rigidbody2D trap1Rb;  
    public Rigidbody2D trap2Rb;
    void Start()
    {
        if (spike == null && transform.childCount > 0)
        {
            spike = transform.GetChild(0).gameObject;
        }

        if (spike != null)
        {
            spike.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if(playerInTrap)
        {
            spike.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerInTrap = true;
        }

        if (collision.gameObject.CompareTag("Help"))
        {
            trap1Rb.gravityScale = 0f;
            trap2Rb.gravityScale = 0f;
        }
    }
}
