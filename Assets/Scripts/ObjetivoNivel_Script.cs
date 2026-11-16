using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ObjetivoNivel_Script : MonoBehaviour
{
    private Animator animatorObjetivo;
    private bool objetivoCompletado = false; // Evita que se active varias veces si hay múltiples colisiones

    void Start()
    {
        // Obtenemos el Animator que esté en este mismo objeto (el objetivo)
        animatorObjetivo = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objetivoCompletado) return;

        // Buscamos el script del jugador en el objeto que colisiona o en sus padres
        Player_Script jugador = collision.GetComponent<Player_Script>();
        if (jugador == null)
        {
            jugador = collision.GetComponentInParent<Player_Script>();
        }

        // Si el que ha tocado es el jugador:
        if (jugador != null)
        {
            objetivoCompletado = true;

            // 1. Activar el bool "completado" en el Animator del objetivo
            if (animatorObjetivo != null)
            {
                animatorObjetivo.SetBool("completado", true);
            }

            // 2. Desactivar el script del jugador (ya no podrá moverse ni atacar)
            jugador.enabled = false;

            // 3 y 4. Esperar 2 segundos y cambiar a la escena MainMenu
            StartCoroutine(EsperarYCambiarEscena(2f));
        }
    }

    private IEnumerator EsperarYCambiarEscena(float segundos)
    {
        // Espera el tiempo especificado (2 segundos)
        yield return new WaitForSeconds(segundos);

        // Carga la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }
}