using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour
{
    [Header("Movimiento y Patrulla")]
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f; 
    private Vector2 posicionInicial;
    private int direccionPatrulla = 1;

    [Header("Detección y Combate")]
    public float distanciaDeteccion = 5f; 
    public float distanciaAtaque = 1.2f;  
    public float fuerzaImpactoJugador = 15f; 
    private Transform jugador;
    private bool estaAtacandoCooldown = false; 
    private bool estaRecibiendoKnockback = false;
    private bool estaMuerto = false;

    [Header("Componentes")]
    private Rigidbody2D cuerpo;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        posicionInicial = transform.position;
        cuerpo = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (estaMuerto || jugador == null) return;
        if (estaRecibiendoKnockback || estaAtacandoCooldown) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            StartCoroutine(AtacarJugadorRutina());
        }
        else if (distanciaAlJugador <= distanciaDeteccion)
        {
            PerseguirJugador();
        }
        else
        {
            PatrullarArea();
        }
    }

    private IEnumerator KnockbackRutina(Vector2 direccion, float fuerza)
    {
        estaRecibiendoKnockback = true;
        
        // Controlamos la velocidad exacta en lugar de aplicar una fuerza ciega de física
        cuerpo.linearVelocity = new Vector2(direccion.x * fuerza, 3f); // 3f es el pequeño salto hacia arriba

        // Tiempo que dura el empuje antes de devolverle el control a la IA
        yield return new WaitForSeconds(0.2f);

        estaRecibiendoKnockback = false;
    }

    private void PatrullarArea()
    {
        float desplazamientoX = transform.position.x - posicionInicial.x;

        if (desplazamientoX >= distanciaPatrulla)
            direccionPatrulla = -1;
        else if (desplazamientoX <= -distanciaPatrulla)
            direccionPatrulla = 1;

        cuerpo.linearVelocity = new Vector2(direccionPatrulla * velocidad, cuerpo.linearVelocity.y);
        
        // Actualizamos el flip según la dirección
        spriteRenderer.flipX = (direccionPatrulla < 0);

        if (animator != null)
        {
            animator.SetBool("En_movimiento", true);
            animator.SetBool("Subiendo", false);
            animator.SetBool("Bajando", false);
            // Activamos "left" si está mirando a la izquierda
            animator.SetBool("left", spriteRenderer.flipX);
        }
    }

    private void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        cuerpo.linearVelocity = direccion * (velocidad * 1.2f);

        // Actualizamos el flip según la posición del jugador
        spriteRenderer.flipX = (direccion.x < 0);

        if (animator != null)
        {
            animator.SetBool("En_movimiento", true);
            animator.SetBool("Subiendo", cuerpo.linearVelocity.y > 0.1f);
            animator.SetBool("Bajando", cuerpo.linearVelocity.y < -0.1f);
            // Activamos "left" si está mirando a la izquierda
            animator.SetBool("left", spriteRenderer.flipX);
        }
    }

    private IEnumerator AtacarJugadorRutina()
    {
        estaAtacandoCooldown = true;
        cuerpo.linearVelocity = Vector2.zero;
        cuerpo.gravityScale = 0;

        // Opcional: si quieres que al atacar también mire hacia donde está el jugador antes de golpear
        if (jugador != null)
        {
            float dirX = jugador.position.x - transform.position.x;
            if (dirX != 0)
            {
                spriteRenderer.flipX = (dirX < 0);
            }
        }

        if (animator != null)
        {
            animator.SetBool("En_movimiento", false);
            animator.SetBool("Subiendo", false);
            animator.SetBool("Bajando", false);
            animator.SetBool("left", spriteRenderer.flipX);
            animator.SetTrigger("Atacando"); 
        }

        if (jugador != null)
        {
            float distanciaActual = Vector2.Distance(transform.position, jugador.position);
            if (distanciaActual <= distanciaAtaque + 0.3f)
            {
                Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
                if (rbJugador != null)
                {
                    float direccionEmpuje = Mathf.Sign(jugador.position.x - transform.position.x);
                    rbJugador.AddForce(new Vector2(direccionEmpuje * fuerzaImpactoJugador, fuerzaImpactoJugador * 0.4f), ForceMode2D.Impulse);
                }
            }
        }

        yield return new WaitForSeconds(2.08f);
        estaAtacandoCooldown = false;
    }

    public void Morir()
    {
        if (estaMuerto) return;
        StartCoroutine(MorirRutina());
    }

    private IEnumerator MorirRutina()
    {
        estaMuerto = true;
        
        // Esperamos a que termine el empuje físico antes de activar la animación de muerte y congelarlo
        yield return new WaitForSeconds(0.2f); 

        enabled = false;
        StopAllCoroutines();
        cuerpo.linearVelocity = Vector2.zero;
        cuerpo.simulated = false; // Desactiva físicas para que no se siga moviendo raro

        if (animator != null)
        {
            animator.ResetTrigger("Atacando");
            animator.SetBool("En_movimiento", false);
            animator.SetBool("Subiendo", false);
            animator.SetBool("Bajando", false);
            animator.SetTrigger("Morir");
        }

        // Espera a que termine la animación de muerte
        yield return new WaitForSeconds(1.5f); 
        spriteRenderer.enabled = false;
        Destroy(gameObject);
    }
}