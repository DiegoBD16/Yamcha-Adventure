using UnityEngine;

public class Player_Attack_Script : MonoBehaviour
{
    public float fuerzaEmpuje = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        Vector2 forceDirection = (collision.transform.position - transform.position).normalized;
        rb.mass  = 1;
        rb.AddForce(forceDirection * fuerzaEmpuje, ForceMode2D.Impulse);
        Enemy_Controller enemigo = collision.GetComponent<Enemy_Controller>();
        enemigo.Morir();
    }
}