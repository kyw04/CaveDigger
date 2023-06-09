using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RandomChest : MonoBehaviour
{
    public GameObject[] items;

    private bool isOpen;
    private Animator ani;

    private void Start()
    {
        isOpen = false;
        ani = GetComponent<Animator>();
    }

    public void Opening()
    {
        if (isOpen)
            return;

        gameObject.layer = LayerMask.NameToLayer("OpenChest");
        ani.SetTrigger("Open");
    }

    public void GetItem()
    {
        GameObject newItem = Instantiate(items[Random.Range(0, items.Length)], transform.position, Quaternion.identity);
        Rigidbody2D newItemRb = newItem.GetComponent<Rigidbody2D>();
        float y = Random.Range(1f, 3.5f);
        newItemRb.AddForce(new Vector2(Random.Range(-2.5f, 2.5f), y), ForceMode2D.Impulse);
        StartCoroutine(ItemRigidbodySleep(y * 0.25f, newItemRb));
    }

    IEnumerator ItemRigidbodySleep(float time, Rigidbody2D rb)
    {
        yield return new WaitForSeconds(time);
        Destroy(rb);
    }
}
