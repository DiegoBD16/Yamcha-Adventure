using UnityEngine;

public class Enemy_Attack_Sensor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            return;
        }
        Player_Script jugador = collision.GetComponent<Player_Script>();
        if (jugador == null)
        {
            jugador = collision.GetComponentInParent<Player_Script>();
        }
        if (jugador != null)
        {
            jugador.RecibirDanio();
        }
    }
}