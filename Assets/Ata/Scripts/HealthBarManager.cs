using System;
using _Game.Scripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Zenject;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Settings")] [SerializeField]
    private float maxHealth = 100f;

    [SerializeField] private float currentHealth;
    [SerializeField] private float healthDecreaseRate = 2f; // Her saniye d��ecek can miktar�
    [SerializeField] private float damageAmount = 10f; // T tu�una bas�nca al�nan hasar miktar�
    [SerializeField] private float healAmount = 10f; // Y tu�una bas�nca kazan�lan can miktar�

    [Header("UI Elements")] [SerializeField]
    private Slider healthBar;

    [SerializeField] private Image fillImage; // Can bar�n�n doluluk g�stergesi
    [SerializeField] private TextMeshProUGUI healthText; // Can de�erini g�steren metin

    [Header("Color Settings")] [SerializeField]
    private Color fullHealthColor = Color.green;

    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f; // D���k can e�i�i

    private float timeSinceLastDecrease = 0f;
    [Inject] private SignalBus signalBus;

    private void Awake()
    {
        // Ba�lang��ta can de�erlerini ayarla
        currentHealth = 50f;

        // UI elementlerini ayarla
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        UpdateHealthUI();
    }

    private void Start()
    {
        signalBus.Subscribe<GameSignals.OnPlayerDamageTaken>(() => DecreaseHealth(damageAmount));
    }

    private void Update()
    {
        // Can bar�n�n her saniye azalmas�
        timeSinceLastDecrease += Time.deltaTime;
        if (timeSinceLastDecrease >= 1f)
        {
            // Otomatik can azalmas� i�in NotifyDamageTaken() �A�RILMAZ
            // Bu y�zden ayr� bir fonksiyon kullan�yoruz
            DecreaseHealthSilently(healthDecreaseRate);
            timeSinceLastDecrease = 0f;
        }

        // T tu�una bas�l�nca hasar al
        if (Input.GetKeyDown(KeyCode.T))
        {
            Decrease();
        }

        // Y tu�una bas�l�nca can kazanma
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Increase();
        }
    }

    public void Increase()
    {
        IncreaseHealth(healAmount);
        Debug.Log("Y tu�una bas�ld�: " + healAmount + " can kazan�ld�!");
    }

    public void Decrease()
    {
        DecreaseHealth(damageAmount);
        Debug.Log("T tu�una bas�ld�: " + damageAmount + " hasar al�nd�!");

        // T tu�una bas�ld���nda do�rudan DistanceTracker'� bilgilendir
        DistanceTracker distanceTracker = FindObjectOfType<DistanceTracker>();
        if (distanceTracker != null)
        {
            distanceTracker.NotifyDamageTaken();
            Debug.Log("T tu�una bas�ld�: �lerleme h�z� 2 saniye boyunca yar�ya d���r�ld�!");
        }
    }

    // Normal hasar fonksiyonu (T tu�u i�in)
    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;

        // Can�n 0'�n alt�na d��mesini �nle
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Can s�f�r oldu�unda sahneyi yeniden y�kle
        if (currentHealth <= 0)
        {
            Debug.Log("Can s�f�rland�! Sahne yeniden y�kleniyor...");
            Invoke("RestartScene", 1f); // 1 saniye bekleyip sahneyi yeniden y�kle
        }
    }

    // Sessiz hasar fonksiyonu (otomatik azalma i�in) - yava�lama tetiklemez
    public void DecreaseHealthSilently(float amount)
    {
        currentHealth -= amount;

        // Can�n 0'�n alt�na d��mesini �nle
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        // Can s�f�r oldu�unda sahneyi yeniden y�kle
        if (currentHealth <= 0)
        {
            Debug.Log("Can s�f�rland�! Sahne yeniden y�kleniyor...");
            Invoke("RestartScene", 1f); // 1 saniye bekleyip sahneyi yeniden y�kle
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth += amount;

        // Can�n maksimum de�eri a�mas�n� �nle
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Slider'� g�ncelle
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Metin alan�n� g�ncelle
        if (healthText != null)
        {
            healthText.text = Mathf.Ceil(currentHealth).ToString() + " / " + maxHealth.ToString();
        }

        // Can bar�n�n rengini g�ncelle
        if (fillImage != null)
        {
            // Can miktar�na g�re rengi de�i�tir (ye�ilden k�rm�z�ya)
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

    // Di�er scriptlerin can miktar�n� alabilmesi i�in
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // Di�er scriptlerin maksimum can� alabilmesi i�in
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Di�er scriptlerin can miktar�n� ayarlayabilmesi i�in
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