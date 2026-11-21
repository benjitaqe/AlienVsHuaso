using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

public class GestorOleadasNivel2 : MonoBehaviour
{
    public static GestorOleadasNivel2 Instancia { get; private set; }

    [Header("UI del HUD")]
    public TextMeshProUGUI textoContadorAliens;

    [Header("Prefabs de Enemigos")]
    public GameObject prefabAlienNormal;
    public GameObject prefabAlienFuerte;

    [Header("Configuración Oleada 1")]
    public Transform[] spawnsOleada1;
    public int totalOleada1 = 10;
    private int restantesOleada1;

    [Header("Configuración Oleada 2")]
    public Transform[] spawnsOleada2;
    public int totalOleada2 = 15;
    private int restantesOleada2;

    [Header("Configuración Boss")]
    public GameObject bossObjeto;
    public Transform puntoSpawnBoss;

    [Header("Final del Juego")]
    public GameObject vacaObjeto;

    [Header("Barreras y Bloqueos")]
    public GameObject barreraHaciaZona2;
    public GameObject barreraHaciaBoss;

    [Header("Triggers de Zona")]
    public GameObject triggerOleada2;

    private int oleadaActual = 1;
    private bool oleada1Terminada = false;
    private bool oleada2Terminada = false;
    private bool oleada2Iniciada = false;

    private Coroutine rutinaActualSpawn;

    void Awake()
    {
        Time.timeScale = 1f;
        Instancia = this;
    }

    void Start()
    {
        if (barreraHaciaZona2 != null) barreraHaciaZona2.SetActive(true);
        if (barreraHaciaBoss != null) barreraHaciaBoss.SetActive(true);
        if (bossObjeto != null) bossObjeto.SetActive(false);
        if (vacaObjeto != null) vacaObjeto.SetActive(false);

        int slotCargar = PlayerPrefs.GetInt("SlotACargar", -1);
        if (slotCargar != -1 && SistemaGuardadoJSON.ExistePartida(slotCargar))
        {
            RestaurarEstadoGuardado(slotCargar);
        }
        else
        {
            oleadaActual = 1;
            restantesOleada1 = totalOleada1;
            ActualizarHUD();
            rutinaActualSpawn = StartCoroutine(RutinaSpawnOleada1(restantesOleada1));
        }
    }

    void RestaurarEstadoGuardado(int slot)
    {
        DatosPartida datos = SistemaGuardadoJSON.CargarPartida(slot);
        if (datos == null) return;

        GameObject objHuaso = GameObject.FindWithTag("Player");
        if (objHuaso != null)
        {
            CharacterController cc = objHuaso.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            objHuaso.transform.position = new Vector3(datos.posX, datos.posY, datos.posZ);
            objHuaso.transform.rotation = Quaternion.Euler(0f, datos.rotY, 0f);

            if (cc != null) cc.enabled = true;

            VidaHuaso vida = objHuaso.GetComponent<VidaHuaso>();
            if (vida != null && datos.vidaJugador > 0)
            {
                vida.vidaActual = datos.vidaJugador;
            }

            DisparoHuaso disparo = objHuaso.GetComponent<DisparoHuaso>();
            if (disparo == null) disparo = objHuaso.GetComponentInChildren<DisparoHuaso>();
            if (disparo != null)
            {
                if (datos.enemigosRestantes > 0 || datos.pistasEncontradas > 0)
                {
                    disparo.municionActual = datos.enemigosRestantes;
                    disparo.reservaMunicion = datos.pistasEncontradas;
                    disparo.ActualizarHUD();
                }
            }
        }

        oleadaActual = (datos.oleadaActualNivel2 > 0) ? datos.oleadaActualNivel2 : 1;

        if (oleadaActual == 1)
        {
            if (datos.enemigosRestantesNivel2 <= 0)
            {
                restantesOleada1 = 0;
                CompletarOleada1();
            }
            else
            {
                restantesOleada1 = datos.enemigosRestantesNivel2;
                ActualizarHUD();
                rutinaActualSpawn = StartCoroutine(RutinaSpawnOleada1(restantesOleada1));
            }
        }
        else if (oleadaActual == 2)
        {
            oleada1Terminada = true;
            oleada2Iniciada = true;

            if (barreraHaciaZona2 != null) barreraHaciaZona2.SetActive(false);
            if (triggerOleada2 != null) triggerOleada2.SetActive(false);

            if (datos.enemigosRestantesNivel2 <= 0)
            {
                restantesOleada2 = 0;
                CompletarOleada2();
            }
            else
            {
                restantesOleada2 = datos.enemigosRestantesNivel2;
                ActualizarHUD();
                rutinaActualSpawn = StartCoroutine(RutinaSpawnOleada2(restantesOleada2));
            }
        }
        else if (oleadaActual == 3)
        {
            oleada1Terminada = true;
            oleada2Terminada = true;
            oleada2Iniciada = true;

            if (barreraHaciaZona2 != null) barreraHaciaZona2.SetActive(false);
            if (barreraHaciaBoss != null) barreraHaciaBoss.SetActive(false);
            if (triggerOleada2 != null) triggerOleada2.SetActive(false);

            CompletarOleada2();
        }
        else if (oleadaActual >= 4)
        {
            oleada1Terminada = true;
            oleada2Terminada = true;
            oleada2Iniciada = true;

            if (barreraHaciaZona2 != null) barreraHaciaZona2.SetActive(false);
            if (barreraHaciaBoss != null) barreraHaciaBoss.SetActive(false);
            if (triggerOleada2 != null) triggerOleada2.SetActive(false);

            if (bossObjeto != null) Destroy(bossObjeto);
            if (vacaObjeto != null) vacaObjeto.SetActive(true);

            if (textoContadorAliens != null)
            {
                textoContadorAliens.text = "¡Ve a rescatar a la vaca!";
            }
        }
    }

    IEnumerator RutinaSpawnOleada1(int cantidadASpawnear)
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < cantidadASpawnear; i++)
        {
            if (spawnsOleada1 != null && spawnsOleada1.Length > 0)
            {
                Transform sp = spawnsOleada1[Random.Range(0, spawnsOleada1.Length)];
                GameObject prefab = (prefabAlienNormal != null) ? prefabAlienNormal : prefabAlienFuerte;

                if (prefab != null && sp != null)
                {
                    Instantiate(prefab, sp.position, sp.rotation);
                }
            }
            yield return new WaitForSeconds(1.2f);
        }
    }

    public void RegistrarBajaAlien()
    {
        if (oleadaActual == 1 && !oleada1Terminada)
        {
            restantesOleada1--;
            if (restantesOleada1 <= 0)
            {
                restantesOleada1 = 0;
                CompletarOleada1();
            }
        }
        else if (oleadaActual == 2 && !oleada2Terminada)
        {
            restantesOleada2--;
            if (restantesOleada2 <= 0)
            {
                restantesOleada2 = 0;
                CompletarOleada2();
            }
        }

        ActualizarHUD();
    }

    void CompletarOleada1()
    {
        oleada1Terminada = true;
        if (rutinaActualSpawn != null) StopCoroutine(rutinaActualSpawn);
        if (barreraHaciaZona2 != null) barreraHaciaZona2.SetActive(false);

        if (textoContadorAliens != null)
        {
            textoContadorAliens.text = "¡Avanza a la siguiente zona!";
        }
    }

    public void IniciarOleada2()
    {
        if (oleada2Iniciada || oleada2Terminada) return;

        oleada2Iniciada = true;
        oleadaActual = 2;
        oleada1Terminada = true;

        if (triggerOleada2 != null) triggerOleada2.SetActive(false);

        restantesOleada2 = totalOleada2;
        ActualizarHUD();

        if (rutinaActualSpawn != null) StopCoroutine(rutinaActualSpawn);
        rutinaActualSpawn = StartCoroutine(RutinaSpawnOleada2(restantesOleada2));
    }

    IEnumerator RutinaSpawnOleada2(int cantidadASpawnear)
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < cantidadASpawnear; i++)
        {
            if (spawnsOleada2 != null && spawnsOleada2.Length > 0)
            {
                Transform sp = spawnsOleada2[Random.Range(0, spawnsOleada2.Length)];
                GameObject prefab = (i % 3 == 0 && prefabAlienFuerte != null) ? prefabAlienFuerte : prefabAlienNormal;

                if (prefab != null && sp != null)
                {
                    Instantiate(prefab, sp.position, sp.rotation);
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    void CompletarOleada2()
    {
        oleada2Terminada = true;
        if (rutinaActualSpawn != null) StopCoroutine(rutinaActualSpawn);
        if (barreraHaciaBoss != null) barreraHaciaBoss.SetActive(false);
        if (triggerOleada2 != null) triggerOleada2.SetActive(false);

        oleadaActual = 3;
        ActualizarHUD();

        if (bossObjeto != null)
        {
            bossObjeto.SetActive(true);

            if (puntoSpawnBoss != null)
            {
                NavMeshAgent agent = bossObjeto.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.Warp(puntoSpawnBoss.position);
                }
                else
                {
                    bossObjeto.transform.position = puntoSpawnBoss.position;
                    bossObjeto.transform.rotation = puntoSpawnBoss.rotation;
                }
            }

            StartCoroutine(RutinaEsperarMuerteBoss());
        }
    }

    IEnumerator RutinaEsperarMuerteBoss()
    {
        yield return new WaitForSeconds(1f);

        VidaAlien vidaBoss = (bossObjeto != null) ? bossObjeto.GetComponent<VidaAlien>() : null;

        while (bossObjeto != null && bossObjeto.activeInHierarchy && vidaBoss != null && vidaBoss.vidaActual > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        oleadaActual = 4;

        if (vacaObjeto != null)
        {
            vacaObjeto.SetActive(true);
        }

        if (textoContadorAliens != null)
        {
            textoContadorAliens.text = "¡Ve a rescatar a la vaca!";
        }
    }

    void ActualizarHUD()
    {
        if (textoContadorAliens == null) return;

        if (oleadaActual == 1)
        {
            if (oleada1Terminada)
                textoContadorAliens.text = "¡Avanza a la siguiente zona!";
            else
                textoContadorAliens.text = $"Enemigos restantes: {restantesOleada1}/{totalOleada1}";
        }
        else if (oleadaActual == 2)
        {
            if (oleada2Terminada)
                textoContadorAliens.text = "¡Camino al Jefe abierto!";
            else
                textoContadorAliens.text = $"Enemigos restantes: {restantesOleada2}/{totalOleada2}";
        }
        else if (oleadaActual == 3)
        {
            textoContadorAliens.text = "¡Derrota al Jefe!";
        }
        else if (oleadaActual >= 4)
        {
            textoContadorAliens.text = "¡Ve a rescatar a la vaca!";
        }
    }

    public int ObtenerOleadaActual()
    {
        return oleadaActual;
    }

    public int ObtenerEnemigosRestantes()
    {
        if (oleadaActual == 1) return restantesOleada1;
        if (oleadaActual == 2) return restantesOleada2;
        return 0;
    }
}