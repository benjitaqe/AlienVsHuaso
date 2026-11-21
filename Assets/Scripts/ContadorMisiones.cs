using UnityEngine;
using TMPro;

public class ContadorMisiones : MonoBehaviour
{
    public static ContadorMisiones Instancia;

    [Header("Fase 1: Combate")]
    public int totalEnemigos = 25;
    [HideInInspector] public int enemigosRestantes;

    [Header("Fase 2: Búsqueda de Pistas")]
    public int totalPistas = 3;
    [HideInInspector] public int pistasEncontradas = 0;
    public GameObject contenedorPistas;

    [Header("Fase 3: Llegada a la Nave")]
    public GameObject brujulaContenedor;

    [Header("Referencias")]
    public SpawnManager spawnManager;
    public TextMeshProUGUI textoMisionUI;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }

        enemigosRestantes = totalEnemigos;
    }

    void Start()
    {
        if (brujulaContenedor != null) brujulaContenedor.SetActive(false);
        if (contenedorPistas != null) contenedorPistas.SetActive(false);

        ActualizarTextoCombate();
    }

    public void RestaurarProgreso(int restantesGuardados, int pistasGuardadas)
    {
        enemigosRestantes = restantesGuardados;
        pistasEncontradas = pistasGuardadas;

        if (enemigosRestantes <= 0)
        {
            if (spawnManager != null) spawnManager.DetenerSpawn();

            if (pistasEncontradas >= totalPistas)
            {
                if (contenedorPistas != null) contenedorPistas.SetActive(false);
                CompletarInvestigacion();
            }
            else
            {
                if (contenedorPistas != null) contenedorPistas.SetActive(true);
                if (textoMisionUI != null)
                {
                    textoMisionUI.text = "Pistas encontradas: " + pistasEncontradas + " / " + totalPistas;
                    textoMisionUI.color = Color.yellow;
                }
            }
        }
        else
        {
            ActualizarTextoCombate();
        }
    }

    public void RegistrarBajaAlien()
    {
        if (enemigosRestantes > 0)
        {
            enemigosRestantes--;
            ActualizarTextoCombate();

            if (enemigosRestantes <= 0)
            {
                IniciarFasePistas();
            }
        }
    }

    void ActualizarTextoCombate()
    {
        if (textoMisionUI != null)
        {
            textoMisionUI.text = "Enemigos restantes: " + enemigosRestantes;
            textoMisionUI.color = Color.white;
        }
    }

    void IniciarFasePistas()
    {
        if (spawnManager != null) spawnManager.DetenerSpawn();
        if (contenedorPistas != null) contenedorPistas.SetActive(true);

        if (textoMisionUI != null)
        {
            textoMisionUI.text = "Pistas encontradas: 0 / " + totalPistas;
            textoMisionUI.color = Color.yellow;
        }
    }

    public void RegistrarPistaEncontrada(string nombre)
    {
        pistasEncontradas++;

        if (textoMisionUI != null)
        {
            textoMisionUI.text = "Pistas encontradas: " + pistasEncontradas + " / " + totalPistas;
        }

        if (pistasEncontradas >= totalPistas)
        {
            CompletarInvestigacion();
        }
    }

    void CompletarInvestigacion()
    {
        if (brujulaContenedor != null)
        {
            brujulaContenedor.SetActive(true);
        }

        if (textoMisionUI != null)
        {
            textoMisionUI.text = "¡Rastro completado! Sigue la flecha hacia la nave";
            textoMisionUI.color = Color.green;
        }
    }
}