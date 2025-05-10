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
    [SerializeField][ReadOnly] private float currentSpeedMetersPerMinute; // Edit�rde g�r�nt�lemek i�in
    [SerializeField][ReadOnly] private float currentSpeedMetersPerSecond; // Edit�rde g�r�nt�lemek i�in

    [Header("UI Elements")]
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private TextMeshProUGUI distanceText;

    [Header("References")]
    [SerializeField] private HealthBarManager healthManager;

    // Hasar al�nca yava�lama kontrol� i�in de�i�kenler
    private bool isSlowed = false;
    private float slowDuration = 2f; // Yava�lama s�resi (saniye)
    private float slowTimer = 0f;
    private float slowMultiplier = 0.5f; // Yava�lama �arpan� (yar�ya d���r�yor)

    // Son health de�eri (hasar al�n�p al�nmad���n� kontrol etmek i�in)
    private float lastHealth;

    private void Start()
    {
        // Ba�lang��ta mesafe s�f�r
        currentDistance = 0f;

        // UI ��elerini ayarla
        if (distanceSlider != null)
        {
            distanceSlider.minValue = 0f;
            distanceSlider.maxValue = maxDistance;
            distanceSlider.value = currentDistance;
        }

        UpdateDistanceUI();

        // Health manager'� kontrol et
        if (healthManager == null)
        {
            // Sahnede HealthBarManager arar�z
            healthManager = FindObjectOfType<HealthBarManager>();

            if (healthManager == null)
            {
                Debug.LogError("DistanceTracker: HealthBarManager bulunamad�!");
            }
        }

        // �lk health de�erini kaydet
        if (healthManager != null)
        {
            lastHealth = healthManager.GetCurrentHealth();
        }
    }

    private void Update()
    {
        if (healthManager == null) return;

        // Yava�lama s�resi kontrol�
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0)
            {
                isSlowed = false;
                Debug.Log("Normal h�za d�n�ld�.");
            }
        }

        // Mesafeyi g�ncelle
        UpdateDistance();
    }

    private void UpdateDistance()
    {
        // Can y�zdesini hesapla (0-1 aras�)
        float healthPercent = healthManager.GetCurrentHealth() / healthManager.GetMaxHealth();

        // En d���k can %10 kabul ediyoruz, daha d���kse de %10 gibi davran�r
        healthPercent = Mathf.Max(0.1f, healthPercent);

        // Can y�zdesine g�re h�z hesapla (dakikada metre)
        // %10 can -> 50m/dk, %100 can -> 300m/dk, do�rusal interpolasyon
        float currentSpeedMetersPerMinute = Mathf.Lerp(minSpeedMetersPerMinute, maxSpeedMetersPerMinute, healthPercent);

        // Saniyede ne kadar mesafe katetti�ini hesapla
        float metersPerSecond = currentSpeedMetersPerMinute / 60f;

        // E�er yava�lama aktifse, h�z� yar�ya d���r
        if (isSlowed)
        {
            metersPerSecond *= slowMultiplier;
        }

        // Mesafeyi g�ncelle
        currentDistance += metersPerSecond * Time.deltaTime;

        // Maksimum mesafeyi a�mamas�n� sa�la
        currentDistance = Mathf.Min(currentDistance, maxDistance);

        // UI'� g�ncelle
        UpdateDistanceUI();

        // E�er maksimum mesafeye ula��ld�ysa
        if (currentDistance >= maxDistance)
        {
            // Buraya ula��ld���nda yapmak istedi�iniz i�lemleri ekleyebilirsiniz
            // �rne�in: LevelComplete();
            Debug.Log("Maksimum mesafeye ula��ld�: 1000m!");
        }
    }

    private void UpdateDistanceUI()
    {
        // Slider'� g�ncelle
        if (distanceSlider != null)
        {
            distanceSlider.value = currentDistance;
        }

        // Metni g�ncelle
        if (distanceText != null)
        {
            // Ondal�k k�sm� olmadan mesafeyi g�ster ve metre ekle
            distanceText.text = Mathf.Floor(currentDistance).ToString() + "m / " + maxDistance.ToString() + "m";
        }
    }

    // Hasar al�nd���n� d��ar�dan bildiren fonksiyon (HealthBarManager i�ine eklenebilir)
    public void NotifyDamageTaken()
    {
        isSlowed = true;
        slowTimer = slowDuration;
    }

    // Mevcut mesafe de�erini d�nd�ren fonksiyon
    public float GetCurrentDistance()
    {
        return currentDistance;
    }

    // Mesafeyi s�f�rlayan fonksiyon
    public void ResetDistance()
    {
        currentDistance = 0f;
        UpdateDistanceUI();
    }
}