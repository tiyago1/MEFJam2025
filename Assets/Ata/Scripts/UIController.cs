using _Game.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

    
    [Inject] private SignalBus _signalBus;
    
    private bool isPaused = false;
    private bool isGameStarted = false;

    private void Awake()
    {
        // Ba�lang��ta do�ru panellerin aktif olmas�n� sa�la
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameplayHUD.SetActive(false);

        // Zaman �l�e�ini normal olarak ayarla
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
        // Sadece oyun ba�lad�ysa ESC tu�unu dinle
        if (isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void PlayGame()
    {
        // Ana men�y� kapat ve oyun HUD'�n� a�
        mainMenuPanel.SetActive(false);
        gameplayHUD.SetActive(true);
        isGameStarted = true;
        isPaused = false;

        // Burada oyunu ba�latan kodu �a��rabilirsiniz
        // �rne�in: GameManager.Instance.StartGame();
        Debug.Log("Oyun ba�lat�ld�!");
        
        _signalBus.Fire<GameSignals.OnPlayButtonPressed>();
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        Debug.Log("Krediler a��ld�!");
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Ana men�ye d�n�ld�!");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Oyunu duraklat ve ayarlar men�s�n� a�
            settingsPanel.SetActive(true);
            gameplayHUD.SetActive(false);
            Debug.Log("Oyun duraklat�ld�, ayarlar a��ld�!");
        }
        else
        {
            // Oyunu devam ettir ve ayarlar men�s�n� kapat
            settingsPanel.SetActive(false);
            Debug.Log("Oyun devam ediyor!");
            gameplayHUD.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        // Ayarlar men�s�nden ��k�ld���nda ana men�ye d�n
        settingsPanel.SetActive(false);

        if (isGameStarted)
        {
            // Oyun devam ediyor ve duraklat�lm��sa, duraklatma durumunu kald�r
            isPaused = false;
            Debug.Log("Ayarlar kapat�ld�, oyun devam ediyor!");
            gameplayHUD.SetActive(true);
        }
        else
        {
            // Hen�z oyun ba�lamad�ysa ana men�ye d�n
            mainMenuPanel.SetActive(true);
            Debug.Log("Ayarlar kapat�ld�, ana men�ye d�n�ld�!");
        }
    }

    public void ReturnToMainMenu()
    {
        // Oyunu durdur ve ana men�ye d�n
        isPaused = false;
        isGameStarted = false;

        settingsPanel.SetActive(false);
        gameplayHUD.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // Burada oyunu s�f�rlayan kodu �a��rabilirsiniz
        // �rne�in: GameManager.Instance.ResetGame();
        Debug.Log("Ana men�ye d�n�ld�!");
    }
}