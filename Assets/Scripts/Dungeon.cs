using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public List<GameObject> enemys = new List<GameObject>();
    public GameObject item;

    private bool isClear;

    private void Start()
    {
        isClear = false;
    }

    private void Update()
    {
        if (!isClear && enemys.Count == 0)
        {
            isClear = true;
            Instantiate(item, transform.position, Quaternion.identity);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
