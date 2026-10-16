using System;
using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrapManager trapManager;
    public Rigidbody2D parentrb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Help"))
        {
          rb.gravityScale = 0f;
            StartCoroutine(GravityTimer());
        }
    }

    private IEnumerator GravityTimer()
    {
        yield return new WaitForSeconds(1.5f);

        rb.gravityScale = parentrb.gravityScale; 
    }
}
