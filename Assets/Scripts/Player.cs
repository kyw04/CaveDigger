using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Transform attackBox;
    public Image healthImage;
    public TextMeshProUGUI healthText;
    public Image radiationImage;
    public TextMeshProUGUI radiationText;
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

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Vector3 attackBoxDir;
    private Vector3 startScale;
    private float curTime;

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

        DirectionSetting();

        if (curTime + attackSpeed <= Time.time && Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }

        UISetting();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
    }

    private void DirectionSetting()
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
            if (collider.CompareTag("Ground"))
            {
                q.Enqueue(collider.gameObject);
                //Debug.Log("Ground!");
            }
            else if (collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy!");
            }
        }

        float minDis = -1;
        GameObject minDisGround = null;
        while (q.Count > 0)
        {
            GameObject ground = q.Dequeue();
            float distance;
            distance = Mathf.Abs(Vector2.Distance(ground.transform.position, transform.position));
            if (minDis == -1 || minDis > distance)
            {
                minDis = distance;
                minDisGround = ground;
            }
        }

        if (minDisGround != null)
        {
            Instantiate(BlockDestroyParticles[0], minDisGround.transform.position, Quaternion.identity);
            Destroy(minDisGround.gameObject);
        }
    }

    private void UISetting()
    {
        healthImage.fillAmount = health / maxHealth;
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
        radiationImage.fillAmount = radiation / maxRadiation;
        radiationText.text = (radiation / maxRadiation * 100f).ToString("F2") + "%";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackBox.transform.position, attackBox.localScale);
    }
}
