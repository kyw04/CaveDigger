using UnityEngine;

public class Stone : Item
{
    public GameObject stonePrefab;
    public float speed;
    private float damage;
    private float lastUseTime = 0f;
    private float itemDelay = 2.5f;

    public override void RunItem()
    {
        damage = player.realStats.damage;
        if (lastUseTime + itemDelay <= Time.time)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                GameObject newStone = Instantiate(stonePrefab, player.transform.position, Quaternion.identity);
                newStone.GetComponent<Rigidbody2D>().AddForce(player.AttackDirection * speed, ForceMode2D.Impulse);
                Destroy(newStone, 2.5f);
                lastUseTime = Time.time;
            }
        }
    }
}
