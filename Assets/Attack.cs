using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform attackBox;
    public GameObject destroyParticle;
    public float damage;
    public float attackSpeed;

    private float curTime;

    private void Start()
    {
        curTime = 0f;
    }

    private void Update()
    {
        if (curTime + attackSpeed <= Time.time && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("attack!");
            curTime = Time.time;

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBox.transform.position, attackBox.localScale, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.CompareTag("Ground"))
                {
                    Debug.Log("Ground!");
                    Instantiate(destroyParticle, collider.transform.position, Quaternion.identity);
                    Destroy(collider.gameObject);
                    break;
                }
                else if (collider.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy!");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackBox.transform.position, attackBox.localScale);
    }
}
