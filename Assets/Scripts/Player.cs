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
    public float damage = 3.5f;
    public float attackRange;
    public float attackSpeed;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 direction;
    private float curTime;

    private void Start()
    {
        curTime = 0f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        DirectionSetting();

        if (curTime + attackSpeed <= Time.time && Input.GetKeyDown(KeyCode.X))
        {
            Attack();
        }

        UISetting();
    }

    private void DirectionSetting()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0f && vertical == 0f)
        {
            anim.SetBool("isMove", false);
        }
        else
        {
            anim.SetBool("isMove", true);

            if (horizontal != 0f)
            {
                transform.localScale = new Vector3(horizontal, 1, 1);
                direction = new Vector3(horizontal, 0);
            }
            else if (vertical != 0f)
            {
                direction = new Vector3(0, vertical);
            }
        }
            
        anim.SetFloat("X", horizontal);
        anim.SetFloat("Y", vertical);
       
        attackBox.position = transform.position + direction * attackRange;

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
