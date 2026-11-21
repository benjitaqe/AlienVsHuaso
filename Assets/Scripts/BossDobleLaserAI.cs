using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossDobleLaserAI : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador;

    [Header("Cañones del Boss")]
    public Transform canonIzquierdo;
    public LineRenderer lineaLaserIzquierda;
    public Transform canonDerecho;
    public LineRenderer lineaLaserDerecha;

    [Header("Ajustes de Ataque")]
    public float distanciaDisparo = 15f;
    public float duracionPorRayo = 1.2f;       
    public float pausaEntreCanones = 0.5f;     
    public float tiempoRecargaTotal = 3.5f;    
    public float danoPorDisparo = 20f;        
    public float velocidadGiroAtaque = 2.0f;   

    [Header("Audio")]
    public AudioClip sonidoLaser;
    private AudioSource audioSource;

    private NavMeshAgent agente;
    private bool estaDisparando = false;
    private float tiempoSiguienteAtaque = 0f;
    private Transform canonActivo = null;
    private LineRenderer laserActivo = null;
    private bool golpeAcertadoEnEsteTiro = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.baseOffset = 0f; 
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (jugador == null)
        {
            GameObject objHuaso = GameObject.FindWithTag("Player");
            if (objHuaso != null) jugador = objHuaso.transform;
        }

        if (lineaLaserIzquierda != null) lineaLaserIzquierda.enabled = false;
        if (lineaLaserDerecha != null) lineaLaserDerecha.enabled = false;
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (estaDisparando)
        {
            if (canonActivo != null && laserActivo != null)
            {
                ActualizarRayoLaser(canonActivo, laserActivo);
            }
            return;
        }

        if (distancia <= distanciaDisparo && Time.time >= tiempoSiguienteAtaque)
        {
            StartCoroutine(SecuenciaAtaqueDoble());
        }
        else
        {
            if (agente != null && agente.isOnNavMesh)
            {
                agente.isStopped = false;
                agente.SetDestination(jugador.position);
            }
        }
    }

    IEnumerator SecuenciaAtaqueDoble()
    {
        estaDisparando = true;
        if (agente != null && agente.isOnNavMesh) agente.isStopped = true;

        yield return StartCoroutine(DispararUnCanon(canonIzquierdo, lineaLaserIzquierda));

        yield return new WaitForSeconds(pausaEntreCanones);

        yield return StartCoroutine(DispararUnCanon(canonDerecho, lineaLaserDerecha));

        estaDisparando = false;
        tiempoSiguienteAtaque = Time.time + tiempoRecargaTotal;

        if (agente != null && agente.isOnNavMesh) agente.isStopped = false;
    }

    IEnumerator DispararUnCanon(Transform canon, LineRenderer laser)
    {
        if (canon == null || laser == null) yield break;

        canonActivo = canon;
        laserActivo = laser;
        golpeAcertadoEnEsteTiro = false;
        laserActivo.enabled = true;

        if (sonidoLaser != null && audioSource != null)
        {
            audioSource.clip = sonidoLaser;
            audioSource.loop = true;
            audioSource.volume = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            audioSource.Play();
        }

        float tiempoInicio = Time.time;
        while (Time.time < tiempoInicio + duracionPorRayo)
        {
            yield return null;
        }

        laserActivo.enabled = false;
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();

        canonActivo = null;
        laserActivo = null;
    }

    void ActualizarRayoLaser(Transform canon, LineRenderer laser)
    {
        Vector3 inicio = canon.position;

        Vector3 puntoMeta = jugador.position + Vector3.up * 1f;

        Vector3 dirJugadorPlano = puntoMeta - transform.position;
        dirJugadorPlano.y = 0;
        if (dirJugadorPlano != Vector3.zero)
        {
            Quaternion rotDeseada = Quaternion.LookRotation(dirJugadorPlano);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDeseada, Time.deltaTime * velocidadGiroAtaque);
        }

        Vector3 direccionRayo = (puntoMeta - inicio).normalized;
        Vector3 puntoFinal = inicio + (direccionRayo * distanciaDisparo);

        RaycastHit hit;
        if (Physics.Raycast(inicio, direccionRayo, out hit, distanciaDisparo))
        {
            puntoFinal = hit.point;

            if (!golpeAcertadoEnEsteTiro && (hit.collider.CompareTag("Player") || hit.collider.name.ToLower().Contains("huaso")))
            {
                VidaHuaso vida = hit.collider.GetComponent<VidaHuaso>();
                if (vida == null) vida = hit.collider.GetComponentInParent<VidaHuaso>();

                if (vida != null)
                {
                    vida.RecibirDano(danoPorDisparo);
                    golpeAcertadoEnEsteTiro = true;
                }
            }
        }

        laser.SetPosition(0, inicio);
        laser.SetPosition(1, puntoFinal);
    }
}