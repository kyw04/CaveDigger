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

    private Vector3 direction;
    private float curTime;

    private void Start()
    {
        curTime = 0f;
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector3.down;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector3.left;
        }
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
