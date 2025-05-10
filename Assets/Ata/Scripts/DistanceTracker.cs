using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceTracker : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float currentDistance = 0f;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float maxSpeedMetersPerMinute = 300f; // %100 can varken dakikada katedilen mesafe
    [SerializeField] private float minSpeedMetersPerMinute = 50f;  // %10 can varken dakikada katedilen mesafe

    [Header("Speed Monitoring (Read Only)")]
    [SerializeField][ReadOnly] private float currentSpeedMetersPerMinute; // Editörde görüntülemek için
    [SerializeField][ReadOnly] private float currentSpeedMetersPerSecond; // Editörde görüntülemek için

    [Header("UI Elements")]
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("References")]
    [SerializeField] private HealthBarManager healthManager;

    [Header("Environment Movement")]
    [SerializeField] private Transform objectToMoveUp;

    // Hasar alýnca yavaþlama kontrolü için deðiþkenler
    private bool isSlowed = false;
    private float slowDuration = 2f; // Yavaþlama süresi (saniye)
    private float slowTimer = 0f;
    private float slowMultiplier = 0.5f; // Yavaþlama çarpaný (yarýya düþürüyor)

    // Son health deðeri (hasar alýnýp alýnmadýðýný kontrol etmek için)
    private float lastHealth;

    private void Start()
    {
        currentDistance = 0f;

        if (distanceSlider != null)
        {
            distanceSlider.minValue = 0f;
            distanceSlider.maxValue = maxDistance;
            distanceSlider.value = currentDistance;
        }

        UpdateDistanceUI();

        if (healthManager == null)
        {
            healthManager = FindObjectOfType<HealthBarManager>();
            if (healthManager == null)
            {
                Debug.LogError("DistanceTracker: HealthBarManager bulunamadý!");
            }
        }

        if (healthManager != null)
        {
            lastHealth = healthManager.GetCurrentHealth();
        }
    }

    private void Update()
    {
        if (healthManager == null) return;

        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                isSlowed = false;
                Debug.Log("Normal hýza dönüldü.");
            }
        }

        UpdateDistance();
    }

    private void UpdateDistance()
    {
        float healthPercent = healthManager.GetCurrentHealth() / healthManager.GetMaxHealth();
        healthPercent = Mathf.Max(0.1f, healthPercent);

        float currentSpeedMetersPerMinute = Mathf.Lerp(minSpeedMetersPerMinute, maxSpeedMetersPerMinute, healthPercent);
        float metersPerSecond = currentSpeedMetersPerMinute / 60f;

        if (isSlowed)
        {
            metersPerSecond *= slowMultiplier;
        }

        currentDistance += metersPerSecond * Time.deltaTime;
        currentDistance = Mathf.Min(currentDistance, maxDistance);

        UpdateDistanceUI();
        UpdateObjectPosition();

        if (currentDistance >= maxDistance)
        {
            Debug.Log("Maksimum mesafeye ulaþýldý: 1000m!");
        }
    }

    private void UpdateDistanceUI()
    {
        if (distanceSlider != null)
        {
            distanceSlider.value = currentDistance;
        }

        if (distanceText != null)
        {
            distanceText.text = Mathf.Floor(currentDistance).ToString() + "m / " + maxDistance.ToString() + "m";
        }
    }

    private void UpdateObjectPosition()
    {
        if (objectToMoveUp == null) return;

        Vector3 newPosition = objectToMoveUp.position;
        newPosition.y = -currentDistance;
        objectToMoveUp.position = newPosition;
    }

    public void NotifyDamageTaken()
    {
        isSlowed = true;
        slowTimer = slowDuration;
    }

    public float GetCurrentDistance()
    {
        return currentDistance;
    }

    public void ResetDistance()
    {
        currentDistance = 0f;
        UpdateDistanceUI();
        UpdateObjectPosition();
    }
}