using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Meat : Item
{
    //private float health = 10f;

    public override void RunItem()
    {
        stats.maxHealth = 10f;
    }
}
