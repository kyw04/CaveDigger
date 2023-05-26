using UnityEngine;
using UnityEngine.UI;

public enum Rank
{
    None, Normal, Rare, Epic, Legend
}

public abstract class Item : MonoBehaviour
{
    public Stats stats;
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

    private Slot itemSlot;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D itemCollider;

    protected Player player;

    public abstract void RunItem();

    private void Start()
    {
        stats.Zero();
        player = GameManager.instance.player;
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
        itemSlot = GameManager.instance.inventory.GetItemSlot();
        if (itemSlot == null)
            return;
        //Debug.Log("get item");

        GameManager.instance.inventory.items.Add(this);
        itemSlot.item = this;
        itemSlot.GetComponent<Image>().sprite = sprite;
        itemSlot.gameObject.SetActive(true);
        spriteRenderer.enabled = false;
        itemCollider.enabled = false;
        transform.SetParent(GameManager.instance.player.transform);
    }

    public void PutItem()
    {
        itemSlot = null;
        transform.parent = null;
        spriteRenderer.enabled = true;
        itemCollider.enabled = true;
        transform.localScale = Vector3.one;
        transform.position = GameManager.instance.player.transform.position;
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
