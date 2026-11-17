using UnityEngine;

public class Item_Script : MonoBehaviour
{
    [SerializeField] private float cantidadCuracion = 25f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player_Script jugador = collision.GetComponent<Player_Script>();
        if (jugador == null)
        {
            jugador = collision.GetComponentInParent<Player_Script>();
        }
        if (jugador != null)
        {
            jugador.Curar(cantidadCuracion);
            Destroy(gameObject);
        }
    }
}