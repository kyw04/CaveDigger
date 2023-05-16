using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform attackBox;
    public Image healthImage;
    public TextMeshProUGUI healthText;
    public Image radiationImage;
    public TextMeshProUGUI radiationText;
    public GameObject buttonHoldImage;
    public Image buttonHoldShow;
    public GameObject[] BlockDestroyParticles;

    public float maxHealth = 100f;
    public float health = 100f;
    public float maxRadiation = 100;
    public float radiation = 0f;
    public float radiationSpeed = 0.5f;
    public float moveSpeed = 5f;
    public float damage = 3.5f;
    public float attackRange;
    public float attackSpeed;
    public float pickupRange = 3f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Vector3 attackBoxDir;
    private Vector3 startScale;
    private float curTime;
    private float pickupTime = 0;
    private float pickupDelay = 1.5f;
    private GameObject currentPickupItem;

    private void Start()
    {
        if (instance == null) instance = this;

        curTime = 0f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        attackBoxDir = Vector3.up;
        startScale = transform.lossyScale;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        radiation += radiationSpeed * Time.deltaTime;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        SetDirection();

        ItemPickup();

        if (curTime + attackSpeed <= Time.time && Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }

        SetUI();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
    }

    private void SetDirection()
    {
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x != 0 || movement.y != 0)
        {
            anim.SetFloat("LastHorizontal", movement.x);
            anim.SetFloat("LastVertical", movement.y);

            attackBoxDir = Vector3.zero;
            Vector3 newScale = startScale;
            if (movement.y != 0)
            {
                attackBoxDir.y = movement.y;
            }
            else
            {
                attackBoxDir.x = movement.x;
                newScale.x = movement.x;
            }
            transform.localScale = newScale;
        }
        attackBox.position = transform.position + attackBoxDir * attackRange;
    }

    private void Attack()
    {
        Debug.Log("attack!");
        curTime = Time.time;

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBox.localScale, 0);
        Queue<GameObject> q = new Queue<GameObject>();

        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Wall"))
            {
                q.Enqueue(collider.gameObject);
                //Debug.Log("Wall!");
            }
            else if (collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy!");
            }
        }

        float minDis = -1;
        GameObject minDisWall = null;
        while (q.Count > 0)
        {
            GameObject wall = q.Dequeue();
            float distance;
            distance = Mathf.Abs(Vector2.Distance(wall.transform.position, transform.position));
            if (minDis == -1 || minDis > distance)
            {
                minDis = distance;
                minDisWall = wall;
            }
        }

        if (minDisWall != null)
        {
            Instantiate(BlockDestroyParticles[0], minDisWall.transform.position, Quaternion.identity);
            Destroy(minDisWall.gameObject);
        }
    }

    private void ItemPickup()
    {
        if (Inventory.instance.isFull)
        {
            buttonHoldImage.SetActive(false);
            return;
        }

        Collider2D[] surroundingItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));

        float minDis = -1;
        GameObject minDisItem = null;
        foreach (Collider2D surroundingItem in surroundingItems)
        {
            float distance = Mathf.Abs(Vector2.Distance(transform.position, surroundingItem.transform.position));

            if (minDis == -1 || minDis > distance)
            {
                minDis = distance;
                minDisItem = surroundingItem.gameObject;
            }
        }

        if (minDisItem != null && currentPickupItem != null)
        {
            buttonHoldImage.SetActive(true);
            buttonHoldImage.transform.localPosition = minDisItem.transform.position;

            if (Input.GetKey(KeyCode.F) && minDisItem == currentPickupItem)
            {
                //Debug.Log(pickupTime.ToString("F0"));
                if (pickupTime >= pickupDelay)
                {
                    pickupTime = 0f;
                    minDisItem.GetComponent<Item>().GetItem();
                }

                pickupTime += Time.deltaTime;
            }
            else
            {
                currentPickupItem = minDisItem;
                pickupTime = 0f;
            }
        }
        else
        {
            buttonHoldImage.SetActive(false);
            currentPickupItem = minDisItem;
            pickupTime = 0f;
        }
        buttonHoldShow.fillAmount = pickupTime / pickupDelay;
    }

    private void SetUI()
    {
        healthImage.fillAmount = health / maxHealth;
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
        radiationImage.fillAmount = radiation / maxRadiation;
        radiationText.text = (radiation / maxRadiation * 100f).ToString("F2") + "%";
    }

    public void SetAbility()
    {
        Debug.Log("asd");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.timeScale == 0)
            return;


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackBox.transform.position, attackBox.localScale);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
