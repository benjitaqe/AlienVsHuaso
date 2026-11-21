using UnityEngine;
using UnityEngine.AI;

public class IAAlien : MonoBehaviour
{
    private Transform objetivo;
    private NavMeshAgent agente;
    private Animator animator;

    [Header("Configuración de Ataque")]
    public float danoPorGolpe = 20f;
    public float tiempoEntreAtaques = 1.8f;
    public float rangoAtaque = 1.8f;

    private float tiempoSiguienteAtaque = 0f;
    private VidaHuaso vidaJugador;
    private bool enSecuenciaFinal = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        GameObject jugador = GameObject.Find("huaso");
        if (jugador != null)
        {
            objetivo = jugador.transform;
            vidaJugador = jugador.GetComponent<VidaHuaso>();
        }
        else
        {
            Debug.LogWarning("No se encontró al personaje 'huaso' en la escena.");
        }
    }

    void Update()
    {
        if (enSecuenciaFinal) return;
        if (objetivo == null || agente == null || !agente.enabled || !agente.isOnNavMesh) return;
        if (vidaJugador != null && vidaJugador.EstaMuerto()) return;

        agente.SetDestination(objetivo.position);

        Vector3 posAlienPlana = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 posJugadorPlana = new Vector3(objetivo.position.x, 0f, objetivo.position.z);
        float distanciaAlHuaso = Vector3.Distance(posAlienPlana, posJugadorPlana);

        if (animator != null)
        {
            bool seMueve = agente.velocity.magnitude > 0.15f;
            animator.SetBool("estaCaminando", seMueve);
        }

        if (distanciaAlHuaso <= rangoAtaque)
        {
            if (Time.time >= tiempoSiguienteAtaque)
            {
                AtacarAlHuaso();
            }
        }
    }

    void AtacarAlHuaso()
    {
        tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;

        if (animator != null)
        {
            animator.SetTrigger("atacar");
        }

        if (vidaJugador != null)
        {
            vidaJugador.RecibirDano(danoPorGolpe);
            Debug.Log("¡El alien te golpeó! Te quitó " + danoPorGolpe + " de vida.");
        }
    }

    public void PausarPorMuerteJugador()
    {
        enSecuenciaFinal = true;

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
            agente.ResetPath();
        }

        if (animator != null)
        {
            animator.SetBool("estaCaminando", false);
        }
    }

    public void PosicionarYCelebrar(Vector3 posHuaso, Vector3 forwardHuaso, float offsetLateral, Vector3 posCamara)
    {
        Vector3 puntoDetras = posHuaso - (forwardHuaso * 2.2f) + (transform.right * offsetLateral);

        if (agente != null && agente.isOnNavMesh)
        {
            agente.Warp(puntoDetras);
        }
        else
        {
            transform.position = puntoDetras;
        }

        if (posCamara != Vector3.zero)
        {
            Vector3 direccionCamara = posCamara - transform.position;
            direccionCamara.y = 0f;
            if (direccionCamara != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccionCamara);
            }
        }

        if (animator != null)
        {
            animator.SetBool("estaCaminando", false);
            animator.SetTrigger("bailar");
        }
    }
}