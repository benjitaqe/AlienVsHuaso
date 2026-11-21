using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Configuración del Spawn")]
    public GameObject alienPrefab;
    public float tiempoEntreSpawns = 6f;
    public float radioSpawn = 15f;

    private Transform jugador;
    private VidaHuaso vidaHuaso;

    void Start()
    {
        GameObject huasoObj = GameObject.Find("huaso");
        if (huasoObj != null)
        {
            jugador = huasoObj.transform;
            vidaHuaso = huasoObj.GetComponent<VidaHuaso>();
        }

        InvokeRepeating("SpawnAlien", 2f, tiempoEntreSpawns);
    }

    void SpawnAlien()
    {
        if (vidaHuaso != null && vidaHuaso.EstaMuerto())
        {
            CancelInvoke("SpawnAlien");
            Debug.Log("Generador de Aliens apagado. Jugador muerto.");
            return;
        }

        if (jugador == null || alienPrefab == null) return;

        Vector2 puntoAleatorio = Random.insideUnitCircle.normalized * radioSpawn;
        Vector3 posicionSpawn = new Vector3(
            jugador.position.x + puntoAleatorio.x,
            jugador.position.y,
            jugador.position.z + puntoAleatorio.y
        );

        Instantiate(alienPrefab, posicionSpawn, Quaternion.identity);
    }

    public void DetenerSpawn()
    {
        CancelInvoke("SpawnAlien");
        enabled = false;
        Debug.Log("Oleada completada: SpawnManager apagado.");
    }
}