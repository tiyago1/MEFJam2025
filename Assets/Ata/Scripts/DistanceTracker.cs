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

    // Hasar alýnca yavaþlama kontrolü için deðiþkenler
    private bool isSlowed = false;
    private float slowDuration = 2f; // Yavaþlama süresi (saniye)
    private float slowTimer = 0f;
    private float slowMultiplier = 0.5f; // Yavaþlama çarpaný (yarýya düþürüyor)

    // Son health deðeri (hasar alýnýp alýnmadýðýný kontrol etmek için)
    private float lastHealth;

    private void Start()
    {
        // Baþlangýçta mesafe sýfýr
        currentDistance = 0f;

        // UI öðelerini ayarla
        if (distanceSlider != null)
        {
            distanceSlider.minValue = 0f;
            distanceSlider.maxValue = maxDistance;
            distanceSlider.value = currentDistance;
        }

        UpdateDistanceUI();

        // Health manager'ý kontrol et
        if (healthManager == null)
        {
            // Sahnede HealthBarManager ararýz
            healthManager = FindObjectOfType<HealthBarManager>();

            if (healthManager == null)
            {
                Debug.LogError("DistanceTracker: HealthBarManager bulunamadý!");
            }
        }

        // Ýlk health deðerini kaydet
        if (healthManager != null)
        {
            lastHealth = healthManager.GetCurrentHealth();
        }
    }

    private void Update()
    {
        if (healthManager == null) return;

        // Yavaþlama süresi kontrolü
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0)
            {
                isSlowed = false;
                Debug.Log("Normal hýza dönüldü.");
            }
        }

        // Mesafeyi güncelle
        UpdateDistance();
    }

    private void UpdateDistance()
    {
        // Can yüzdesini hesapla (0-1 arasý)
        float healthPercent = healthManager.GetCurrentHealth() / healthManager.GetMaxHealth();

        // En düþük can %10 kabul ediyoruz, daha düþükse de %10 gibi davranýr
        healthPercent = Mathf.Max(0.1f, healthPercent);

        // Can yüzdesine göre hýz hesapla (dakikada metre)
        // %10 can -> 50m/dk, %100 can -> 300m/dk, doðrusal interpolasyon
        float currentSpeedMetersPerMinute = Mathf.Lerp(minSpeedMetersPerMinute, maxSpeedMetersPerMinute, healthPercent);

        // Saniyede ne kadar mesafe katettiðini hesapla
        float metersPerSecond = currentSpeedMetersPerMinute / 60f;

        // Eðer yavaþlama aktifse, hýzý yarýya düþür
        if (isSlowed)
        {
            metersPerSecond *= slowMultiplier;
        }

        // Mesafeyi güncelle
        currentDistance += metersPerSecond * Time.deltaTime;

        // Maksimum mesafeyi aþmamasýný saðla
        currentDistance = Mathf.Min(currentDistance, maxDistance);

        // UI'ý güncelle
        UpdateDistanceUI();

        // Eðer maksimum mesafeye ulaþýldýysa
        if (currentDistance >= maxDistance)
        {
            // Buraya ulaþýldýðýnda yapmak istediðiniz iþlemleri ekleyebilirsiniz
            // Örneðin: LevelComplete();
            Debug.Log("Maksimum mesafeye ulaþýldý: 1000m!");
        }
    }

    private void UpdateDistanceUI()
    {
        // Slider'ý güncelle
        if (distanceSlider != null)
        {
            distanceSlider.value = currentDistance;
        }

        // Metni güncelle
        if (distanceText != null)
        {
            // Ondalýk kýsmý olmadan mesafeyi göster ve metre ekle
            distanceText.text = Mathf.Floor(currentDistance).ToString() + "m / " + maxDistance.ToString() + "m";
        }
    }

    // Hasar alýndýðýný dýþarýdan bildiren fonksiyon (HealthBarManager içine eklenebilir)
    public void NotifyDamageTaken()
    {
        isSlowed = true;
        slowTimer = slowDuration;
    }

    // Mevcut mesafe deðerini döndüren fonksiyon
    public float GetCurrentDistance()
    {
        return currentDistance;
    }

    // Mesafeyi sýfýrlayan fonksiyon
    public void ResetDistance()
    {
        currentDistance = 0f;
        UpdateDistanceUI();
    }
}