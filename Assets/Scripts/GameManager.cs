using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public Image playerHealthImage;
    public TextMeshProUGUI playerHealthText;
    public Image playerRadiationImage;
    public Image gameOverImage;
    public TextMeshProUGUI playerRadiationText;
    public TimeManager playerTime;
    public Inventory inventory;
    public float itemUseDelay = 1.5f;


    private void Awake()
    {
        if (instance == null) { instance = this; }
        Time.timeScale = 1.0f;
        gameOverImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameOverImage.gameObject.activeSelf &&
            Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SetUI()
    {
        playerHealthImage.fillAmount = player.realStats.health / player.realStats.maxHealth;
        playerHealthText.text = player.realStats.health.ToString("F0") + " / " + player.realStats.maxHealth.ToString("F0");
        playerRadiationImage.fillAmount = player.realStats.radiation / player.realStats.maxRadiation;
        playerRadiationText.text = (player.realStats.radiation / player.realStats.maxRadiation * 100f).ToString("F2") + "%";
    }
}
