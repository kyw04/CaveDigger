using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum Rank
{
    None, Normal, Rare, Epic, Legend
}

public abstract class Item : MonoBehaviour
{
    public Sprite sprite;
    public string itemName;
    public Rank rank;
    [HideInInspector]
    public string rankText;
    [HideInInspector]
    public Color32 rankColor;
    [Multiline(3)]
    public string explanation;
    public GameObject itemPrefab;

    public int itemSlotIndex;
    private Transform UIItemSlot;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D itemCollider;

    public abstract void RunItem();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<BoxCollider2D>();
        SetRank();
    }

    private void Update()
    {
        if (transform.parent == null || !transform.parent.CompareTag("Player"))
            return;

        RunItem();
    }

    public void GetItem()
    {
        UIItemSlot = GameManager.instance.inventory.GetItemSlot();
        if (UIItemSlot == null)
            return;

        UIItemSlot.GetComponent<Image>().sprite = sprite;
        UIItemSlot.gameObject.SetActive(true);
        spriteRenderer.enabled = false;
        itemCollider.enabled = false;
        transform.SetParent(GameManager.instance.player.transform);
        itemSlotIndex = GameManager.instance.inventory.fullItemSlot.Count - 1;
    }

    public void PutItem()
    {
        UIItemSlot = null;
        transform.parent = null;
        transform.position = GameManager.instance.player.transform.position;
        transform.localScale = Vector3.one;
        spriteRenderer.enabled = true;
        itemCollider.enabled = true;
    }

    private void SetRank()
    {
        Color color = Color.gray;

        switch (rank)
        {
            case Rank.None:
                rankText = "None";
                break;
            case Rank.Normal:
                rankText = "Normal";
                break;
            case Rank.Rare:
                rankText = "Rare";
                ColorUtility.TryParseHtmlString("#6182d6", out color);
                break;
            case Rank.Epic:
                rankText = "Epic";
                ColorUtility.TryParseHtmlString("#9d5cbb", out color);
                break;
            case Rank.Legend:
                rankText = "Legend";
                ColorUtility.TryParseHtmlString("#ff843a", out color);
                break;
        }

        rankColor = color;
    }
}
