using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class IAVaca : MonoBehaviour
{
    [Header("Configuración de IA")]
    public float radioCaminata = 20.0f;     
    public float tiempoMinPastar = 3.0f;    
    public float tiempoMaxPastar = 7.0f;    

    private NavMeshAgent agent;
    private float cronometroPastar;
    private bool estaEsperando = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        
        BuscarNuevoRumbo();
    }

    void Update()
    {
     
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!estaEsperando)
            {
                
                estaEsperando = true;
                cronometroPastar = Random.Range(tiempoMinPastar, tiempoMaxPastar);
            }
        }

       
        if (estaEsperando)
        {
            cronometroPastar -= Time.deltaTime;

            if (cronometroPastar <= 0)
            {
                
                estaEsperando = false;
                BuscarNuevoRumbo();
            }
        }
    }

    void BuscarNuevoRumbo()
    {
        
        Vector3 direccionAleatoria = Random.insideUnitSphere * radioCaminata;
        direccionAleatoria += transform.position; 

        NavMeshHit hit;
       
        if (NavMesh.SamplePosition(direccionAleatoria, out hit, radioCaminata, 1))
        {
            
            agent.SetDestination(hit.position);
        }
    }
}