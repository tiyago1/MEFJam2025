using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameplayHUD;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button backFromCreditsButton;
    [SerializeField] private Button backFromSettingsButton;

    private bool isPaused = false;
    private bool isGameStarted = false;

    private void Awake()
    {
        // Baþlangýçta doðru panellerin aktif olmasýný saðla
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameplayHUD.SetActive(false);

        // Zaman ölçeðini normal olarak ayarla
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // Buton dinleyicilerini ekle
        playButton.onClick.AddListener(PlayGame);
        creditsButton.onClick.AddListener(OpenCredits);
        backFromCreditsButton.onClick.AddListener(CloseCredits);
        backFromSettingsButton.onClick.AddListener(CloseSettings);
    }

    private void Update()
    {
        // Sadece oyun baþladýysa ESC tuþunu dinle
        if (isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void PlayGame()
    {
        // Ana menüyü kapat ve oyun HUD'ýný aç
        mainMenuPanel.SetActive(false);
        gameplayHUD.SetActive(true);
        isGameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;

        // Burada oyunu baþlatan kodu çaðýrabilirsiniz
        // Örneðin: GameManager.Instance.StartGame();
        Debug.Log("Oyun baþlatýldý!");
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        Debug.Log("Krediler açýldý!");
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Ana menüye dönüldü!");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Oyunu duraklat ve ayarlar menüsünü aç
            Time.timeScale = 0f;
            settingsPanel.SetActive(true);
            gameplayHUD.SetActive(false);
            Debug.Log("Oyun duraklatýldý, ayarlar açýldý!");
        }
        else
        {
            // Oyunu devam ettir ve ayarlar menüsünü kapat
            Time.timeScale = 1f;
            settingsPanel.SetActive(false);
            Debug.Log("Oyun devam ediyor!");
            gameplayHUD.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        // Ayarlar menüsünden çýkýldýðýnda ana menüye dön
        settingsPanel.SetActive(false);

        if (isGameStarted)
        {
            // Oyun devam ediyor ve duraklatýlmýþsa, duraklatma durumunu kaldýr
            isPaused = false;
            Time.timeScale = 1f;
            Debug.Log("Ayarlar kapatýldý, oyun devam ediyor!");
            gameplayHUD.SetActive(true);
        }
        else
        {
            // Henüz oyun baþlamadýysa ana menüye dön
            mainMenuPanel.SetActive(true);
            Debug.Log("Ayarlar kapatýldý, ana menüye dönüldü!");
        }
    }

    public void ReturnToMainMenu()
    {
        // Oyunu durdur ve ana menüye dön
        Time.timeScale = 1f;
        isPaused = false;
        isGameStarted = false;

        settingsPanel.SetActive(false);
        gameplayHUD.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // Burada oyunu sýfýrlayan kodu çaðýrabilirsiniz
        // Örneðin: GameManager.Instance.ResetGame();
        Debug.Log("Ana menüye dönüldü!");
    }
}