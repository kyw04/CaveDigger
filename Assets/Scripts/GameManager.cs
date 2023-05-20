using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public Image playerHealthImage;
    public TextMeshProUGUI playerHealthText;
    public Image playerRadiationImage;
    public TextMeshProUGUI playerRadiationText;
    public TimeManager playerTime;
    public Inventory inventory;
    public float itemUseDelay = 1.5f;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    public void SetUI()
    {
        playerHealthImage.fillAmount = player.health / player.maxHealth;
        playerHealthText.text = player.health.ToString() + " / " + player.maxHealth.ToString();
        playerRadiationImage.fillAmount = player.radiation / player.maxRadiation;
        playerRadiationText.text = (player.radiation / player.maxRadiation * 100f).ToString("F2") + "%";
    }
}
