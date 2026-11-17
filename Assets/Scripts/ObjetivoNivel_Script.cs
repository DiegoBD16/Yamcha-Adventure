using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ObjetivoNivel_Script : MonoBehaviour
{
    private Animator animatorObjetivo;
    private bool objetivoCompletado = false;

    void Start()
    {
        animatorObjetivo = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objetivoCompletado) return;
        Player_Script jugador = collision.GetComponent<Player_Script>();
        jugador = collision.GetComponentInParent<Player_Script>();
        objetivoCompletado = true;
        animatorObjetivo.SetBool("completado", true);
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        Animator animJugador = jugador.GetComponent<Animator>();
        rbJugador.linearVelocity = Vector2.zero;
        animJugador.SetBool("En_Movimiento", false);
        jugador.enabled = false;
        StartCoroutine(EsperarYCambiarEscena(2f));
        
    }

    private IEnumerator EsperarYCambiarEscena(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        SceneManager.LoadScene("MainMenu");
    }
}