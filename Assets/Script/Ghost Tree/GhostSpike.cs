﻿using System.Collections;
using UnityEngine;

public class GhostSpike : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject smallSpikePrefab;
    public Transform[] smallSpikeSpawnPoints;
    public float explosionDelay = 2f;
    public float explosionRadius = 5f; 
    public float explosionDamage = 10f;
    public LayerMask playerMask;
    private bool isStuck = false;

    private Rigidbody2D rb2d;
    private void Start()
    {
        Collider2D[] playerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }

        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb2d.isKinematic = true;
            if (!isStuck)
            {
                isStuck = true;
                rb2d.isKinematic = false;
                rb2d.velocity = Vector2.zero;
                StartCoroutine(HandleExplosion());
            }
        }
    }
    private IEnumerator HandleExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);

        foreach (Transform spawnPoint in smallSpikeSpawnPoints)
        {
            GameObject smallSpike = Instantiate(smallSpikePrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = smallSpike.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (spawnPoint.position - (Vector3)transform.position).normalized;
                rb.velocity = direction * 20f;
            }
        }

        Collider2D player = Physics2D.OverlapCircle(transform.position, explosionRadius, playerMask);
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(explosionDamage, 0.5f, 0.65f, 0.1f);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}