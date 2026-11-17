using UnityEngine;
using System.Collections;

public class Enemy_Controller : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaPatrulla = 3f; 
    private Vector2 posicionInicial;
    private int direccionPatrulla = 1;
    public float distanciaDeteccion = 5f; 
    public float distanciaAtaque = 1.2f;  
    private Transform jugador;
    private bool estaAtacandoCooldown = false; 
    private bool estaRecibiendoKnockback = false;
    private bool estaMuerto = false;
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

    private void PatrullarArea()
    {
        float desplazamientoX = transform.position.x - posicionInicial.x;
        if (desplazamientoX >= distanciaPatrulla)
            direccionPatrulla = -1;
        else if (desplazamientoX <= -distanciaPatrulla)
            direccionPatrulla = 1;
        cuerpo.linearVelocity = new Vector2(direccionPatrulla * velocidad, cuerpo.linearVelocity.y);
        spriteRenderer.flipX = (direccionPatrulla < 0);
        animator.SetBool("En_movimiento", true);
        animator.SetBool("Subiendo", false);
        animator.SetBool("Bajando", false);
        animator.SetBool("left", spriteRenderer.flipX);
    }

    private void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        cuerpo.linearVelocity = direccion * (velocidad * 1.2f);
        spriteRenderer.flipX = (direccion.x < 0);
        animator.SetBool("En_movimiento", true);
        animator.SetBool("Subiendo", cuerpo.linearVelocity.y > 0.1f);
        animator.SetBool("Bajando", cuerpo.linearVelocity.y < -0.1f);
        animator.SetBool("left", spriteRenderer.flipX);
        
    }

    private IEnumerator AtacarJugadorRutina()
    {
        estaAtacandoCooldown = true;
        cuerpo.linearVelocity = Vector2.zero;
        cuerpo.gravityScale = 0;

        if (jugador != null)
        {
            float dirX = jugador.position.x - transform.position.x;
            if (dirX != 0)
            {
                spriteRenderer.flipX = (dirX < 0);
            }
        }
        animator.SetBool("En_movimiento", false);
        animator.SetBool("Subiendo", false);
        animator.SetBool("Bajando", false);
        animator.SetBool("left", spriteRenderer.flipX);
        animator.SetTrigger("Atacando"); 
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
        yield return new WaitForSeconds(0.2f); 
        enabled = false;
        StopAllCoroutines();
        cuerpo.linearVelocity = Vector2.zero;
        cuerpo.simulated = false;
        animator.ResetTrigger("Atacando");
        animator.SetBool("En_movimiento", false);
        animator.SetBool("Subiendo", false);
        animator.SetBool("Bajando", false);
        animator.SetTrigger("Morir");
        yield return new WaitForSeconds(1.5f); 
        spriteRenderer.enabled = false;
        Destroy(gameObject);
    }
}