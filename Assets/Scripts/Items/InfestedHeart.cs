using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfestedHeart : Item
{
    public float itemDelay = 2.5f;
    public float damage = 1f;

    private float lastUseTime = 0f;

    public override void RunItem()
    {
        stats.radiationSpeed = -0.005f;
        stats.moveSpeed = 1.5f;
        stats.attackSpeed = -0.1f;

        if (lastUseTime + itemDelay <= Time.time)
        {
            lastUseTime = Time.time;
            GameManager.instance.player.OnDamage(damage);
        }
    }
}