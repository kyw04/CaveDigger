using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Transform attackBox;
    public Image healthImg;
    public Image radiationImg;
    public GameObject[] BlockDestroyParticles;

    public float maxHealth = 100f;
    public float health = 100f;
    public float maxRadiation = 0f;
    public float radiation = 0f;
    public float speed = 5f;
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
        curTime = 0f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        attackBoxDir = Vector3.up;
        startScale = transform.lossyScale;
    }

    private void Update()
    {
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
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
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
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Ground"))
            {
                Debug.Log("Ground!");
                Instantiate(BlockDestroyParticles[0], collider.transform.position, Quaternion.identity);
                Destroy(collider.gameObject);
                break;
            }
            else if (collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy!");
            }
        }
    }

    private void UISetting()
    {
        healthImg.fillAmount = health / maxHealth;
        radiationImg.fillAmount = radiation / maxRadiation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackBox.transform.position, attackBox.localScale);
    }
}
