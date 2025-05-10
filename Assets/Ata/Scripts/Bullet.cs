using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private bool useGravity = false;
    
    // Components
    private Rigidbody rb;
    private TrailRenderer trail;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        
        if (rb != null)
        {
            rb.useGravity = useGravity;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        // Destroy the bullet after lifetime seconds
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector3 direction, float newSpeed = 0f)
    {
        if (newSpeed > 0)
        {
            speed = newSpeed;
        }
        
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            StartCoroutine(MoveWithoutRigidbody(direction));
        }
    }
    
    private IEnumerator MoveWithoutRigidbody(Vector3 direction)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < lifetime)
        {
            transform.position += direction * speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.contacts[0].point, collision.contacts[0].normal, collision.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Calculate hit point and normal
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.forward * 0.1f, transform.forward, out hit, 0.5f))
        {
            HandleHit(hit.point, hit.normal, other.gameObject);
        }
        else
        {
            HandleHit(transform.position, -transform.forward, other.gameObject);
        }
    }
    
    private void HandleHit(Vector3 hitPoint, Vector3 hitNormal, GameObject hitObject)
    {
        // Apply damage if the hit object has a health component
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(hitEffect, 2f); // Destroy hit effect after 2 seconds
        }
        
        // Disable the trail renderer so it doesn't disappear abruptly
        if (trail != null)
        {
            trail.transform.SetParent(null);
            trail.autodestruct = true;
        }
        
        // Destroy the bullet
        Destroy(gameObject);
    }
}

// Interface for objects that can take damage
public interface IDamageable
{
    void TakeDamage(float damage);
}
