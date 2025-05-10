using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthDecreaseRate = 2f; // Her saniye düþecek can miktarý
    [SerializeField] private float damageAmount = 10f; // T tuþuna basýnca alýnan hasar miktarý
    [SerializeField] private float healAmount = 10f; // Y tuþuna basýnca kazanýlan can miktarý

    [Header("UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillImage; // Can barýnýn doluluk göstergesi
    [SerializeField] private TextMeshProUGUI healthText; // Can deðerini gösteren metin

    [Header("Color Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f; // Düþük can eþiði

    private float timeSinceLastDecrease = 0f;

    private void Awake()
    {
        // Baþlangýçta can deðerlerini ayarla
        currentHealth = maxHealth;

        // UI elementlerini ayarla
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
    }

    private void Update()
    {
        // Can barýnýn her saniye azalmasý
        timeSinceLastDecrease += Time.deltaTime;
        if (timeSinceLastDecrease >= 1f)
        {
            DecreaseHealth(healthDecreaseRate);
            timeSinceLastDecrease = 0f;
        }

        // T tuþuna basýlýnca hasar al
        if (Input.GetKeyDown(KeyCode.T))
        {
            DecreaseHealth(damageAmount);
            Debug.Log("T tuþuna basýldý: " + damageAmount + " hasar alýndý!");
        }

        // Y tuþuna basýlýnca can kazanma
        if (Input.GetKeyDown(KeyCode.Y))
        {
            IncreaseHealth(healAmount);
            Debug.Log("Y tuþuna basýldý: " + healAmount + " can kazanýldý!");
        }
    }

    public void DecreaseHealth(float amount)
    {
        float oldHealth = currentHealth;
        currentHealth -= amount;

        // Canýn 0'ýn altýna düþmesini önle
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Eðer hasar alýndýysa, mesafe takipçisini bilgilendir
        if (currentHealth < oldHealth)
        {
            // Mesafe takipçisini bul ve bildir
            DistanceTracker distanceTracker = FindObjectOfType<DistanceTracker>();
            if (distanceTracker != null)
            {
                distanceTracker.NotifyDamageTaken();
            }
        }

        // Can sýfýr olduðunda sahneyi yeniden yükle
        if (currentHealth <= 0)
        {
            Debug.Log("Can sýfýrlandý! Sahne yeniden yükleniyor...");
            Invoke("RestartScene", 1f); // 1 saniye bekleyip sahneyi yeniden yükle
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth += amount;

        // Canýn maksimum deðeri aþmasýný önle
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Slider'ý güncelle
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Metin alanýný güncelle
        if (healthText != null)
        {
            healthText.text = Mathf.Ceil(currentHealth).ToString() + " / " + maxHealth.ToString();
        }

        // Can barýnýn rengini güncelle
        if (fillImage != null)
        {
            // Can miktarýna göre rengi deðiþtir (yeþilden kýrmýzýya)
            if (currentHealth <= lowHealthThreshold)
            {
                fillImage.color = lowHealthColor;
            }
            else
            {
                float healthRatio = currentHealth / maxHealth;
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthRatio);
            }
        }
    }

    private void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Diðer scriptlerin can miktarýný alabilmesi için
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // Diðer scriptlerin maksimum caný alabilmesi için
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Diðer scriptlerin can miktarýný ayarlayabilmesi için
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Invoke("RestartScene", 1f);
        }
    }
}