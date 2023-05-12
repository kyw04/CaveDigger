using UnityEngine;

public enum Rank
{
    None, Normal, Rare, Epic, Legend
}

public class Item : MonoBehaviour
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

    private void Start()
    {
        RankSetting();
    }

    private void RankSetting()
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

    public virtual void Ability()
    {

    }

    public virtual void GiveAbility()
    {

    }

    public void Copy(Item _itme)
    {
        this.sprite = _itme.sprite;
        this.itemName = _itme.itemName;
        this.rank = _itme.rank;
        this.explanation = _itme.explanation;

        RankSetting();
    }
}
