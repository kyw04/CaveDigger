using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChest : MonoBehaviour
{
    public GameObject[] items;

    public void GetItem()
    {
        Instantiate(items[Random.Range(0, items.Length)], transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
