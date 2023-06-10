using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : Item
{
    public float useDlay;
    public float value;

    private float lastUseTime;
    private Vector3 startPos;
    private Vector3 currentPos;


    public override void RunItem()
    {
        currentPos = GameManager.instance.player.transform.position;
        if (startPos != currentPos)
        {
            lastUseTime = Time.time;
            startPos = currentPos;
        }

        if (lastUseTime + useDlay <= Time.time)
        {
            lastUseTime = Time.time;
            GameManager.instance.player.Heal(value);
        }
    }
}
