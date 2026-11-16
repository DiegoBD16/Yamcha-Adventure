using UnityEngine;

public class Item_Script : MonoBehaviour
{
    [SerializeField] private float cantidadCuracion = 25f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Buscamos el script del jugador en el objeto que colisiona o en sus padres
        Player_Script jugador = collision.GetComponent<Player_Script>();
        if (jugador == null)
        {
            jugador = collision.GetComponentInParent<Player_Script>();
        }

        // Si encontramos al jugador, lo curamos y destruimos el item
        if (jugador != null)
        {
            jugador.Curar(cantidadCuracion);
            
            // Destruye el objeto de la escena (la poción desaparece)
            Destroy(gameObject);
        }
    }
}