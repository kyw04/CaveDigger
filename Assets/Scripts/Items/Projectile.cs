using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Destroy(this.gameObject);
    }
}
