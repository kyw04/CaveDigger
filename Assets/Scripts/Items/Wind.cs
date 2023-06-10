using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : Projectile
{
    [HideInInspector]
    public Vector3 dir;
    public float power;

    private bool isDestroy;
    private BoxCollider2D coll;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        isDestroy = false;
        coll = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
        {
            if (isDestroy)
            {
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            collision.GetComponent<Rigidbody2D>().AddForce(dir * power, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public IEnumerator DestroyWind(float time)
    {
        yield return new WaitForSeconds(time);
        isDestroy = true;
        coll.enabled = false;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
    }
}
