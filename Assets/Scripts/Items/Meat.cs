using UnityEngine;

public class Meat : Item
{
    //private float health = 10f;

    public override void RunItem()
    {
        player.SetAbility();
    }
}
