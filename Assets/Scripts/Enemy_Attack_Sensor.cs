using UnityEngine;

public class Enemy_Attack_Sensor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto con el que colisiona tiene el tag "Ground", lo ignoramos y salimos
        if (collision.CompareTag("Ground"))
        {
            return;
        }

        // Opcional pero recomendado: calcular la dirección de la fuerza si hay Rigidbody2D
        if (collision.GetComponent<Rigidbody2D>() != null)
        {
            Vector2 forceDirection = collision.transform.position - transform.position; 
        }

        // Buscamos el script del jugador (en el mismo objeto o en sus padres)
        Player_Script jugador = collision.GetComponent<Player_Script>();
        if (jugador == null)
        {
            jugador = collision.GetComponentInParent<Player_Script>();
        }

        // Si realmente encontramos al jugador, le infligimos daño
        if (jugador != null)
        {
            jugador.RecibirDanio();
        }
    }
}