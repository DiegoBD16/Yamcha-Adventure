using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // Obligatorio para el Nuevo Input System
using UnityEngine.UI; // Necesario para trabajar con el Slider
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class Player_Script : MonoBehaviour
{
    private float velocidad = 4.0f;
    private Animator animator;
    private Rigidbody2D cuerpo;
    private SpriteRenderer spriteRenderer;
    private bool estaAtacando = false;
    private bool girar = false;

    // Control de inmunidad y estado de muerte
    private bool esInvulnerable = false; 
    private bool estaMuerto = false;

    [Header("UI y Salud")]
    [SerializeField] private Slider barraSalud; // Arrastra tu Slider desde el Inspector aquí

    void Start()
    {
        animator = GetComponent<Animator>();
        cuerpo = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cuerpo.gravityScale = 1f;
    }

    void Update()
    {
        // Si el personaje está muerto, bloqueamos cualquier input o movimiento
        if (estaMuerto) return;

        RevisarMovimiento();
        RevisarAtaques();
        RevisarSubida();
        RevisarEspacio();
    }

    public void RecibirDanio()
    {
        if (esInvulnerable || estaMuerto) return;

        // Restar 25 puntos de salud al Slider si está asignado
        if (barraSalud != null)
        {
            barraSalud.value -= 25f;

            // Comprobar si la salud llega a 0 (o menos)
            if (barraSalud.value <= 0f)
            {
                Morir();
                return; // Evita activar la invulnerabilidad si ya ha muerto
            }
        }

        StartCoroutine(RutinaInvulnerabilidad());
    }

    private void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        
        cuerpo.linearVelocity = Vector2.zero; // Detener el movimiento físico
        animator.SetTrigger("muerto"); // Activar el trigger de muerte en el Animator

        // Cambia 1.5f por los segundos exactos que dura tu animación de muerte
        StartCoroutine(EsperarMuerteYCambiarEscena(1f));
    }

    private IEnumerator EsperarMuerteYCambiarEscena(float duracionAnimacion)
    {
        // Espera a que termine de reproducirse la animación de muerte
        yield return new WaitForSeconds(duracionAnimacion);

        // Cambia a la escena del menú principal (asegúrate de que se llama exactamente así)
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator RutinaInvulnerabilidad()
    {
        esInvulnerable = true;
        animator.SetBool("danio", true);
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < 2f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.15f);
            tiempoTranscurrido += 0.15f;
        }
        esInvulnerable = false;
        animator.SetBool("danio", false);
    }

    private void RevisarMovimiento()
    {
        float inputX = 0f;
        bool estaMoviendose = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed)
            {
                inputX = 1f;
                estaMoviendose = true;
                girar = false;
                animator.SetBool("Izquierda", false);
            }
            else if (Keyboard.current.aKey.isPressed)
            {
                inputX = -1f;
                estaMoviendose = true;
                girar = true;
                animator.SetBool("Izquierda", true);
            }
        }

        cuerpo.linearVelocity = new Vector2(inputX * velocidad, cuerpo.linearVelocity.y);
        animator.SetBool("En_Movimiento", estaMoviendose);
        
        if (girar)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
           spriteRenderer.flipX = false; 
        }
    }

    private void RevisarSubida()
    {
        bool estaSubiendo = false;
        bool estaBajando = false;
        float inputY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                cuerpo.gravityScale = 0f;
            }

            if (Keyboard.current.wKey.isPressed)
            {
                inputY = 1f;
                estaSubiendo = true;
            }
            else if (Keyboard.current.sKey.isPressed) 
            {
                inputY = -1f;
                estaBajando = true;
            }
            else
            {
                inputY = 0f;
                cuerpo.linearVelocity = new Vector2(cuerpo.linearVelocity.x, inputY * velocidad);
            }
        }

        if (cuerpo.gravityScale == 0f)
        {
            if (estaSubiendo || estaBajando)
            {
                cuerpo.linearVelocity = new Vector2(cuerpo.linearVelocity.x, inputY * velocidad);
            }
        }

        if (animator.GetBool("En_Movimiento") == true)
        {
            animator.SetBool("Subiendo", false);
            animator.SetBool("Bajando", false);
        }
        else
        {
            animator.SetBool("Subiendo", estaSubiendo);
            animator.SetBool("Bajando", estaBajando);
        }
    }

    private void RevisarEspacio()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                cuerpo.gravityScale = 10f;
            }
        }
    }

    private void RevisarAtaques()
    {
        if (estaAtacando) return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                StartCoroutine(EjecutarAtaqueConEspera("Attack", "Ataque Normal", 0.8f));
            }
        }
    }

    private System.Collections.IEnumerator EjecutarAtaqueConEspera(string triggerAnimacion, string nombreAtaque, float duracionAnimacion)
    {
        estaAtacando = true;
        if (animator != null)
        {
            animator.SetTrigger(triggerAnimacion);
        }

        print(nombreAtaque);
        yield return new WaitForSeconds(duracionAnimacion);

        estaAtacando = false;
    }
    public void Curar(float cantidad)
    {
        if (estaMuerto) return; // Si está muerto, no se puede curar

        if (barraSalud != null)
        {
            barraSalud.value += cantidad;
            if (barraSalud.value > barraSalud.maxValue)
            {
                barraSalud.value = barraSalud.maxValue;
            }
        }
    }
}