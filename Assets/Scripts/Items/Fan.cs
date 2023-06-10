using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : Item
{
    public GameObject windPrefab;
    public float speed;
    public float itemDelay = 2.5f;
    public float destroyTime = 1f;

    private Wind wind;
    private float lastUseTime = 0f;

    public override void RunItem()
    {
        if (lastUseTime + itemDelay <= Time.time)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                GameObject newStone = Instantiate(windPrefab, player.transform.position, Quaternion.identity);
                newStone.GetComponent<Rigidbody2D>().AddForce(player.AttackDirection * speed, ForceMode2D.Impulse);
                wind = newStone.GetComponent<Wind>();
                wind.damage = player.realStats.damage;
                wind.dir = player.AttackDirection;
                wind.StartCoroutine(wind.DestroyWind(destroyTime));
                lastUseTime = Time.time;
            }
        }
    }
}
