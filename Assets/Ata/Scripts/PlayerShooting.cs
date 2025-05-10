using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private bool autoFire = false;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip emptySound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("References")]
    [SerializeField] private CrosshairSystem crosshairSystem;

    // Components
    private AudioSource audioSource;
    private Animator animator;

    // State
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        animator = GetComponentInChildren<Animator>();

        // If the crosshair system is not assigned, try to find it
        if (crosshairSystem == null)
        {
            crosshairSystem = FindObjectOfType<CrosshairSystem>();
            if (crosshairSystem == null)
            {
                Debug.LogError("CrosshairSystem not found! Please assign it in the inspector.");
            }
        }

        // Initialize ammo
        currentAmmo = magazineSize;
    }

    private void Update()
    {
        // Don't shoot while reloading
        if (isReloading)
            return;

        // Handle shooting input
        if ((autoFire && Input.GetButton("Fire1")) || (!autoFire && Input.GetButtonDown("Fire1")))
        {
            TryShoot();
        }

        // Handle reload input
        if (Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private void TryShoot()
    {
        // Check if we can shoot
        if (Time.time < nextFireTime || currentAmmo <= 0 || !crosshairSystem.HasValidTarget)
        {
            // Play empty sound if no ammo
            if (currentAmmo <= 0 && emptySound != null && Time.time >= nextFireTime)
            {
                audioSource.PlayOneShot(emptySound);
                nextFireTime = Time.time + 0.2f; // Short cooldown for empty sound
            }
            return;
        }

        // Update fire time
        nextFireTime = Time.time + fireRate;

        // Update ammo
        currentAmmo--;

        // Play shooting animation if available
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        // Play shooting sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Play muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Shoot
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("Bullet prefab or fire point not assigned!");
            return;
        }

        // Get target direction from the crosshair system
        Vector3 shootDirection = (crosshairSystem.TargetPosition - firePoint.position).normalized;

        // Create the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        
        // Get the bullet component (assumes bullet has a Bullet script or Rigidbody)
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            // Add force to the bullet
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            // If no rigidbody, check for a Bullet script
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(shootDirection, bulletSpeed);
            }
            else
            {
                Debug.LogWarning("No Rigidbody or Bullet script found on bullet prefab!");
            }
        }

        // Destroy the bullet after some time if it doesn't have its own destruction logic
        if (!bullet.GetComponent<Bullet>())
        {
            Destroy(bullet, 5f);
        }
    }

    private IEnumerator Reload()
    {
        if (currentAmmo >= magazineSize || isReloading)
            yield break;

        isReloading = true;

        // Play reload animation if available
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }

        // Play reload sound
        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        // Wait for reload time
        yield return new WaitForSeconds(reloadTime);

        // Refill ammo
        currentAmmo = magazineSize;

        isReloading = false;
    }

    // Getter for ammo information (useful for UI)
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return magazineSize;
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}