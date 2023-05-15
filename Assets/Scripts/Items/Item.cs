using UnityEngine;

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

    private SpriteRenderer spriteRenderer;

    public abstract void RunItem();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetRank();
    }

    private void Update()
    {
        RunItem();
    }

    private void GetItem()
    {
        Transform itemSlot = Inventory.instance.GetItemSlot();
        if (itemSlot == null)
            return;

        transform.SetParent(itemSlot);
        itemSlot.gameObject.SetActive(true);
        spriteRenderer.enabled = false;
    }

    public void PutItem()
    {
        transform.position = Player.instance.transform.position;
        spriteRenderer.enabled = true;
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

    public void Copy(Item _itme)
    {
        this.sprite = _itme.sprite;
        this.itemName = _itme.itemName;
        this.rank = _itme.rank;
        this.explanation = _itme.explanation;

        SetRank();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.timeScale == 0)
            return;

        Debug.Log("trigger");
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {
            GetItem();
        }
    }
}
