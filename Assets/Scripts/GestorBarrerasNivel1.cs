using UnityEngine;

public class GestorBarrerasNivel1 : MonoBehaviour
{
    [Header("Barreras Físicas")]
    public GameObject barreraAliens; 
    public GameObject barreraPistas; 

    [Header("Condiciones de Desbloqueo")]
    public int aliensRequeridos = 25;
    public int pistasRequeridas = 3;

    private bool barreraAliensAbierta = false;
    private bool barreraPistasAbierta = false;

    void Start()
    {
        if (barreraAliens != null) barreraAliens.SetActive(true);
        if (barreraPistas != null) barreraPistas.SetActive(true);
    }

    void Update()
    {
        if (ContadorMisiones.Instancia == null) return;

        if (!barreraAliensAbierta)
        {
            int aliensRestantes = ContadorMisiones.Instancia.enemigosRestantes;

            if (aliensRestantes <= 0)
            {
                barreraAliensAbierta = true;
                if (barreraAliens != null) barreraAliens.SetActive(false);
                Debug.Log("[Misiones] ¡25 aliens eliminados! Primera barrera abierta.");
            }
        }

        if (!barreraPistasAbierta)
        {
            int pistasRecogidas = ContadorMisiones.Instancia.pistasEncontradas;

            if (pistasRecogidas >= pistasRequeridas)
            {
                barreraPistasAbierta = true;
                if (barreraPistas != null) barreraPistas.SetActive(false);
                Debug.Log("[Misiones] ¡3 pistas encontradas! Camino a la nave desbloqueado.");
            }
        }
    }
}