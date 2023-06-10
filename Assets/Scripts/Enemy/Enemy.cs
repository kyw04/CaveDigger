using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Stats stats;
    public float seeRange;

    private void Update()
    {
        
    }

    private GameObject FindTarget()
    {
        GameObject target = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, seeRange);

        return target;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeRange);
    }
}
