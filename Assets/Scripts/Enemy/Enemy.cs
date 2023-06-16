using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform[] attackBoxs;
    public Stats stats;
    public float seeRange;

    public State state;
    private Dungeon dungeon;
    private Rigidbody2D rb;
    private Animator ani;
    private Player target;
    private float lastTime;
    private bool targetFind;

    private void Start()
    {
        if (transform.parent != null)
        {
            dungeon = GetComponentInParent<Dungeon>();
            dungeon.enemys.Add(this.gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponentInParent<Animator>();
        state = State.Idle;
        targetFind = false;
    }

    private void Update()
    {
        if (target == null)
            target = FindTarget();

        if (target != null)
        {
            Move(target.transform.position - transform.position);

            float dis = Vector2.Distance(target.transform.position, transform.position);
            if (dis <= stats.attackRange && lastTime + stats.attackSpeed <= Time.time)
            {
                Attack();
            }
        }
    }

    private void Move(Vector3 dir)
    {
        if (state == State.Idle)
        {
            Vector3 scale = Vector3.one;
            scale.x = dir.x < 0 ? -1 : 1;
            if (dungeon != null) 
            { 
                scale.x /= dungeon.transform.localScale.x;
                scale.y /= dungeon.transform.localScale.y;
            }
            transform.localScale = scale;

            rb.MovePosition(transform.position + dir.normalized * stats.moveSpeed * Time.deltaTime);
        }
    }

    private Player FindTarget()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, seeRange, LayerMask.GetMask("Player"));

        if (collider != null)
            return collider.GetComponent<Player>();

        return null;
    }

    private void Attack()
    {
        lastTime = Time.time;
        ani.SetTrigger("Attack");
        state = State.Attack;

        Collider2D collider = Physics2D.OverlapBox(transform.position, transform.localScale * 2f, 0, LayerMask.GetMask("Player"));
        if (collider)
        {
            targetFind = true;
        }
    }

    private void GiveDamage()
    {
        if (targetFind)
        {
            target.OnDamage(stats.damage);
        }
        targetFind = false;
    }

    private void SetIdle()
    {
        state = State.Idle;
    }

    public void OnDamage(float damage)
    {
        if (state == State.Die)
            return;

        stats.health -= damage;
        state = State.Hit;

        if (stats.health <= 0)
        {
            state = State.Die;
            Dead();
        }
        else
            ani.SetTrigger("Hit");
    }

    private void Dead()
    {
        state = State.Die;
        ani.SetTrigger("Dead");
        if (dungeon != null) { dungeon.enemys.Remove(this.gameObject); }
        Destroy(this.gameObject, 0.3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeRange);
        //Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
