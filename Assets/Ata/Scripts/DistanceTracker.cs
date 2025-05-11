using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DistanceTracker : MonoBehaviour
{
    [Header("Distance Settings")] [SerializeField]
    private float currentDistance = 0f;

    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float maxSpeedMetersPerMinute = 300f; // %100 can varken dakikada katedilen mesafe
    [SerializeField] private float minSpeedMetersPerMinute = 50f; // %10 can varken dakikada katedilen mesafe
    public string endscene;
    [Header("Speed Monitoring (Read Only)")] [SerializeField] [ReadOnly]
    private float currentSpeedMetersPerMinute; // Edit�rde g�r�nt�lemek i�in

    [SerializeField] [ReadOnly] private float currentSpeedMetersPerSecond; // Edit�rde g�r�nt�lemek i�in

    [Header("UI Elements")] [SerializeField]
    private Slider distanceSlider;

    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("References")] [SerializeField]
    private HealthBarManager healthManager;

    [Header("Environment Movement")] [SerializeField]
    private Transform objectToMoveUp;

    // Hasar al�nca yava�lama kontrol� i�in de�i�kenler
    private bool isSlowed = false;
    private float slowDuration = 2f; // Yava�lama s�resi (saniye)
    private float slowTimer = 0f;
    private float slowMultiplier = 0.5f; // Yava�lama �arpan� (yar�ya d���r�yor)

    // Son health de�eri (hasar al�n�p al�nmad���n� kontrol etmek i�in)
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
                Debug.LogError("DistanceTracker: HealthBarManager bulunamad�!");
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
                Debug.Log("Normal h�za d�n�ld�.");
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
            Debug.Log("Maksimum mesafeye ula��ld�: 1000m!");

            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
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