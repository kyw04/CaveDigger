using UnityEngine;

public class Meat : Item
{
    //private float health = 10f;

    public override void RunItem()
    {
        if (transform.parent == null || transform.parent.CompareTag("GameController"))
            return;

        Player.instance.SetAbility();
    }
}
