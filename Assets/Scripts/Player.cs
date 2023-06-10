using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Stats
{
    public float maxHealth;// = 100f;
    public float health;// = 100f;
    public float maxRadiation;// = 100;
    public float radiation;// = 0f;
    public float radiationSpeed;// = 0.005f;
    public float moveSpeed;// = 5f;
    public float damage;// = 3.5f;
    public float attackRange;// = 1f;
    public float attackSpeed;// = 1.5f;

    public void Add(Stats stats)
    {
        this.maxHealth += stats.maxHealth;
        this.maxRadiation += stats.maxRadiation;
        this.radiation += stats.radiation;
        this.radiationSpeed += stats.radiationSpeed;
        this.moveSpeed += stats.moveSpeed;
        this.damage += stats.damage;
        this.attackRange += stats.attackRange;
        this.attackSpeed += stats.attackSpeed;
    }

    public void Zero()
    {
        this.maxHealth = 0;
        this.maxRadiation = 0;
        this.radiation = 0;
        this.radiationSpeed = 0;
        this.moveSpeed = 0;
        this.damage = 0;
        this.attackRange = 0;
        this.attackSpeed = 0;
    }
}

public enum State
{
    Idle,
    Attack
}

public class Player : MonoBehaviour
{
    public Transform attackBox;
    public GameObject buttonHoldImage;
    public Image buttonHoldShow;
    public Animator anim;
    public ParticleSystem attackParticle;
    public GameObject[] BlockDestroyParticles;
    public Stats defaultStats;
    public Stats realStats;
    private Stats addStats;
    public float pickupRange = 3f;
    [HideInInspector] public Vector3 AttackDirection;

    private State state;
    private TimeManager timeManager;
    private Rigidbody2D rb;
    private Animator attackAnim;
    private Vector2 movement;
    private Vector3 startScale;
    private float curTime;
    private float pickupTime = 0f;
    private float pickupDelay;
    private GameObject currentPickupItem;
    private Vector3 attackBoxStartScale;

    private const float defaultMovemetSpeed = 5f;

    private void Start()
    {
        realStats.health = defaultStats.health;
        pickupDelay = GameManager.instance.itemUseDelay;

        timeManager = GetComponent<TimeManager>();
        curTime = 0f;
        rb = GetComponent<Rigidbody2D>();
        attackAnim = attackBox.GetComponent<Animator>();
        AttackDirection = Vector3.up;
        startScale = transform.lossyScale;
        attackBoxStartScale = attackBox.transform.localScale;
    }

    private void Update()
    {
        anim.speed = timeManager.scale * realStats.moveSpeed / defaultMovemetSpeed;

        if (timeManager.scale == 0f)
            return;

        defaultStats.radiation += realStats.radiationSpeed * timeManager.deltaTime;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        SetDirection();

        ItemPickup();

        if (curTime + realStats.attackSpeed <= Time.time && 
            Input.GetKeyDown(KeyCode.X) && state == State.Idle)
        {
            Attack();
        }

        SetAbility();
    }

    //move
    private void FixedUpdate()
    {
        if (state != State.Idle)
            return;

        rb.MovePosition(rb.position + movement.normalized * realStats.moveSpeed * timeManager.deltaTime);
    }

    private void SetDirection()
    {
        if (state != State.Idle)
            return;

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x != 0 || movement.y != 0)
        {
            anim.SetFloat("LastHorizontal", movement.x);
            anim.SetFloat("LastVertical", movement.y);

            AttackDirection = Vector3.zero;
            Vector3 newScale = startScale;
            if (movement.y != 0)
            {
                AttackDirection.y = movement.y;
                attackBox.localScale = new Vector3(attackBoxStartScale.x, attackBoxStartScale.y, 1);
            }
            else
            {
                AttackDirection.x = movement.x;
                attackBox.localScale = new Vector3(attackBoxStartScale.y, attackBoxStartScale.x, 1);
                newScale.x = movement.x;
            }
            transform.localScale = newScale;
        }
        attackBox.position = transform.position + AttackDirection * realStats.attackRange;
    }

    private void Attack()
    {
        Debug.Log("attack!");
        curTime = Time.time;
        state = State.Attack;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Attack");
        attackParticle.transform.position = attackBox.position;
        attackParticle.Play();
        StartCoroutine(AttackDone(0.1f));

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBox.localScale, 0);

        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy!");
            }
        }

        Vector2 rayPos = attackBox.localPosition;
        if (transform.localScale.x == -1f)
        {
            rayPos.x = -1f;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayPos, 1f, LayerMask.GetMask("Wall"));
        Debug.DrawRay(transform.position, rayPos, Color.green, 1f);
        if (hit)
        {
            Instantiate(BlockDestroyParticles[0], hit.transform.position, Quaternion.identity);
            Map.instance.BreakWall(hit.transform);
        }
    }

    private IEnumerator AttackDone(float time)
    {
        yield return new WaitForSeconds(time);
        state = State.Idle;
    }

    private void ItemPickup()
    {
        if (GameManager.instance.inventory.isFull)
        {
            buttonHoldImage.SetActive(false);
            return;
        }

        Collider2D[] surroundingItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));
        //Debug.Log($"Surrounding Items Count: {surroundingItems.Length}");

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
            buttonHoldImage.transform.localPosition = minDisItem.transform.position + Vector3.up;

            if (Input.GetKey(KeyCode.F) && minDisItem == currentPickupItem)
            {
                //Debug.Log(pickupTime.ToString("F0"));
                if (pickupTime >= pickupDelay)
                {
                    pickupTime = 0f;
                    if (minDisItem.CompareTag("Pickup"))
                    {
                        minDisItem.GetComponent<Item>().GetItem();
                    }
                    else if (minDisItem.CompareTag("Chest"))
                    {
                        minDisItem.GetComponent<RandomChest>().Opening();
                    }
                }

                pickupTime += Time.deltaTime * timeManager.scale;
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

    public void SetAbility()
    {
        //Debug.Log("set ability");

        float currentHealth = 1;
        if (realStats.maxHealth > 0)
            currentHealth = realStats.health / realStats.maxHealth;

        addStats.Zero();
        realStats.Zero();

        addStats.Add(defaultStats);

        Item[] items = GetComponentsInChildren<Item>();
        foreach (Item item in items)
        {
            addStats.Add(item.stats);
        }
        realStats.Add(addStats);
        realStats.health = realStats.maxHealth * currentHealth;
        
        GameManager.instance.SetUI();
    }

    public void OnDamage(float value)
    {
        realStats.health -= value;
    }

    public void Heal(float value)
    {
        defaultStats.health += value;
        
        if (defaultStats.health > realStats.maxHealth)
        {
            defaultStats.health = realStats.maxHealth;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackBox.transform.position, attackBox.localScale);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
