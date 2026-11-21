using UnityEngine;

public class AutoAimHuaso : MonoBehaviour
{
    [Header("Configuración de Puntería")]
    public float rangoDeteccion = 30f;
    public float velocidadGiro = 10f;
    public LayerMask capaAliens;

    private Transform objetivoMasCercano;
    private VidaHuaso vidaHuaso;
    private DisparoHuaso scriptDisparo;

    void Start()
    {
        vidaHuaso = GetComponent<VidaHuaso>();
        scriptDisparo = GetComponentInChildren<DisparoHuaso>();
    }

    void Update()
    {
        if (vidaHuaso != null && vidaHuaso.EstaMuerto())
        {
            objetivoMasCercano = null;
            return;
        }

        BuscarObjetivoMasCercano();

        if (objetivoMasCercano != null)
        {
            GirarHuasoCompletoHaciaObjetivo();
        }

        if (Input.GetMouseButtonDown(1) && Time.timeScale > 0)
        {
            if (scriptDisparo != null)
            {
                scriptDisparo.IntentarDisparar();
            }
        }
    }

    void BuscarObjetivoMasCercano()
    {
        GameObject[] todosLosAliens = GameObject.FindGameObjectsWithTag("Alien");
        float distanciaMasCorta = Mathf.Infinity;
        objetivoMasCercano = null;

        foreach (GameObject alien in todosLosAliens)
        {
            float distancia = Vector3.Distance(transform.position, alien.transform.position);
            if (distancia < distanciaMasCorta && distancia <= rangoDeteccion)
            {
                distanciaMasCorta = distancia;
                objetivoMasCercano = alien.transform;
            }
        }
    }

    void GirarHuasoCompletoHaciaObjetivo()
    {
        Vector3 direccionAlAlien = objetivoMasCercano.position - transform.position;
        direccionAlAlien.y = 0;

        if (direccionAlAlien.sqrMagnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionAlAlien);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
        }
    }

    public bool TieneObjetivoActivo()
    {
        if (vidaHuaso != null && vidaHuaso.EstaMuerto()) return false;
        return objetivoMasCercano != null;
    }

    public Transform ObtenerObjetivoActual()
    {
        return objetivoMasCercano;
    }
}