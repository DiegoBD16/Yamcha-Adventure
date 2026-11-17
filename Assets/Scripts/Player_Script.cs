using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Player_Script : MonoBehaviour
{
    private float velocidad = 8.0f;
    private Animator animator;
    private Rigidbody2D cuerpo;
    private SpriteRenderer spriteRenderer;
    private bool estaAtacando = false;
    private bool girar = false;
    private bool esInvulnerable = false; 
    private bool estaMuerto = false;
    [SerializeField] private Slider barraSalud; 

    void Start()
    {
        animator = GetComponent<Animator>();
        cuerpo = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cuerpo.gravityScale = 1f;
    }

    void Update()
    {
        if (estaMuerto) return;
        RevisarMovimiento();
        RevisarAtaques();
        RevisarSubida();
        RevisarEspacio();
    }

    public void RecibirDanio()
    {
        if (esInvulnerable || estaMuerto) return;
        barraSalud.value -= 25f;
        if (barraSalud.value <= 0f)
        {
            Morir();
            return; 
        }
        

        StartCoroutine(RutinaInvulnerabilidad());
    }

    private void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        cuerpo.linearVelocity = Vector2.zero; 
        animator.SetTrigger("muerto"); 
        StartCoroutine(EsperarMuerteYCambiarEscena(1f));
    }

    private IEnumerator EsperarMuerteYCambiarEscena(float duracionAnimacion)
    {
        yield return new WaitForSeconds(duracionAnimacion);
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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            cuerpo.gravityScale = 10f;
        }
        
    }

    private void RevisarAtaques()
    {
        if (estaAtacando) return;
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            StartCoroutine(EjecutarAtaqueConEspera("Attack", "Ataque Normal", 0.8f));
        }
    }

    private System.Collections.IEnumerator EjecutarAtaqueConEspera(string triggerAnimacion, string nombreAtaque, float duracionAnimacion)
    {
        estaAtacando = true;
        animator.SetTrigger(triggerAnimacion);
        print(nombreAtaque);
        yield return new WaitForSeconds(duracionAnimacion);
        estaAtacando = false;
    }
    public void Curar(float cantidad)
    {
        if (estaMuerto) return;
        barraSalud.value += cantidad;
        if (barraSalud.value > barraSalud.maxValue)
        {
            barraSalud.value = barraSalud.maxValue;
        }
        
    }
}