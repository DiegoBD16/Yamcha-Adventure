using UnityEngine;

public class Player_Attack_Script : MonoBehaviour
{
    public float fuerzaEmpuje = 10f; // Sube un poco este valor ya que ahora es un impulso instantáneo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si el objeto tiene Rigidbody2D, lo empujamos
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // .normalized asegura que la dirección sea limpia y constante
            Vector2 forceDirection = (collision.transform.position - transform.position).normalized;

            // Usamos ForceMode2D.Impulse para que el golpe sea un impacto físico real e instantáneo
            rb.AddForce(forceDirection * fuerzaEmpuje, ForceMode2D.Impulse);
        }

        // 2. Si es un enemigo, ordenamos que muera de forma segura
        Enemy_Controller enemigo = collision.GetComponent<Enemy_Controller>();
        if (enemigo != null)
        {
            enemigo.Morir();
        }
    }
}