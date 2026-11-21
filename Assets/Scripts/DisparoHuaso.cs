using UnityEngine;
using TMPro;
using System.Collections;

public class DisparoHuaso : MonoBehaviour
{
    [Header("Configuración de la Escopeta")]
    public Transform puntoDisparo;
    public float alcance = 50f;
    public float cadencia = 0.5f;
    public float danoPorTiro = 50f;

    [Header("Sistema de Munición")]
    public int capacidadCargador = 2;
    public int municionActual = 2;
    public int reservaMunicion = 10;
    public float tiempoRecarga = 1.5f;

    [Header("Interfaz (HUD)")]
    public TextMeshProUGUI textoMunicion;
    public GameObject textoMunicionBaja; 
    public Transform flechaGuiaMunicion;  
    public float velocidadParpadeo = 4f;

    [Header("Sonidos")]
    public AudioClip sonidoDisparo;
    public AudioClip sonidoCargarArma;

    private float cronometroCadencia;
    private AudioSource fuenteAudio;
    private bool estaRecargando = false;
    private AutoAimHuaso autoAim;
    private CanvasGroup canvasGroupAlerta;

    void Start()
    {
        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
        {
            fuenteAudio = gameObject.AddComponent<AudioSource>();
        }

        autoAim = GetComponentInParent<AutoAimHuaso>();

        if (textoMunicion == null)
        {
            GameObject objTexto = GameObject.Find("TextoMunicion");
            if (objTexto != null)
            {
                textoMunicion = objTexto.GetComponent<TextMeshProUGUI>();
            }
        }

        if (textoMunicionBaja != null)
        {
            canvasGroupAlerta = textoMunicionBaja.GetComponent<CanvasGroup>();
            if (canvasGroupAlerta == null)
            {
                canvasGroupAlerta = textoMunicionBaja.AddComponent<CanvasGroup>();
            }
            textoMunicionBaja.SetActive(false);
        }

        if (flechaGuiaMunicion != null)
        {
            flechaGuiaMunicion.gameObject.SetActive(false);
        }

        ActualizarHUD();
    }

    void Update()
    {
        if (Time.timeScale <= 0f) return;

        if (cronometroCadencia > 0)
        {
            cronometroCadencia -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!estaRecargando && municionActual < capacidadCargador && reservaMunicion > 0)
            {
                StartCoroutine(RutinaRecargar());
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            IntentarDisparar();
        }

   
        bool sinMunicionTotal = (municionActual + reservaMunicion) <= 0;
        if (sinMunicionTotal)
        {
            if (textoMunicionBaja != null && canvasGroupAlerta != null)
            {
                if (!textoMunicionBaja.activeSelf) textoMunicionBaja.SetActive(true);
                canvasGroupAlerta.alpha = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
            }

            ActualizarFlechaMunicion();
        }
    }

    void ActualizarFlechaMunicion()
    {
        if (flechaGuiaMunicion == null) return;

        ItemMunicion[] cajas = FindObjectsByType<ItemMunicion>();
        Transform masCercana = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (ItemMunicion caja in cajas)
        {
            if (!caja.gameObject.activeInHierarchy) continue;
            float dist = Vector3.Distance(transform.position, caja.transform.position);
            if (dist < distanciaMinima)
            {
                distanciaMinima = dist;
                masCercana = caja.transform;
            }
        }

        if (masCercana != null)
        {
            if (!flechaGuiaMunicion.gameObject.activeSelf)
                flechaGuiaMunicion.gameObject.SetActive(true);

            Vector3 direccion = masCercana.position - flechaGuiaMunicion.position;
            direccion.y = 0f;
            if (direccion != Vector3.zero)
            {
                flechaGuiaMunicion.rotation = Quaternion.LookRotation(direccion);
            }
        }
        else if (flechaGuiaMunicion.gameObject.activeSelf)
        {
            flechaGuiaMunicion.gameObject.SetActive(false);
        }
    }

    public void IntentarDisparar()
    {
        if (Time.timeScale <= 0f || estaRecargando) return;

        if (municionActual > 0)
        {
            if (cronometroCadencia <= 0)
            {
                DispararEscopeta();
            }
        }
        else
        {
            if (reservaMunicion > 0)
            {
                StartCoroutine(RutinaRecargar());
            }
        }
    }

    void DispararEscopeta()
    {
        if (municionActual <= 0 || estaRecargando) return;

        cronometroCadencia = cadencia;
        municionActual--;
        ActualizarHUD();

        if (fuenteAudio != null)
        {
            float volumen = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            if (sonidoDisparo != null)
            {
                fuenteAudio.PlayOneShot(sonidoDisparo, volumen);
            }
            else
            {
                fuenteAudio.volume = volumen;
                fuenteAudio.Play();
            }
        }

        Vector3 origenDisparo = puntoDisparo != null ? puntoDisparo.position : transform.position;
        Vector3 direccionDisparo = puntoDisparo != null ? puntoDisparo.forward : transform.forward;

        if (autoAim != null && autoAim.TieneObjetivoActivo())
        {
            Transform objetivo = autoAim.ObtenerObjetivoActual();
            if (objetivo != null)
            {
                Vector3 puntoEnemigo = objetivo.position + Vector3.up * 1.0f;
                direccionDisparo = (puntoEnemigo - origenDisparo).normalized;
            }
        }

        RaycastHit hit;
        float radioDisparoAsistido = 0.4f;

        if (Physics.SphereCast(origenDisparo, radioDisparoAsistido, direccionDisparo, out hit, alcance))
        {
            if (hit.collider.CompareTag("Alien"))
            {
                VidaAlien vida = hit.collider.GetComponent<VidaAlien>();
                if (vida == null)
                {
                    vida = hit.collider.GetComponentInParent<VidaAlien>();
                }

                if (vida != null)
                {
                    vida.RecibirDano(danoPorTiro);
                }
            }
        }

        if (municionActual <= 0 && reservaMunicion > 0)
        {
            StartCoroutine(RutinaRecargar());
        }
    }

    IEnumerator RutinaRecargar()
    {
        estaRecargando = true;

        if (fuenteAudio != null && sonidoCargarArma != null)
        {
            fuenteAudio.PlayOneShot(sonidoCargarArma, PlayerPrefs.GetFloat("VolumenSonido", 1f));
        }

        yield return new WaitForSeconds(tiempoRecarga);

        int faltantes = capacidadCargador - municionActual;
        int aRecargar = Mathf.Min(faltantes, reservaMunicion);

        municionActual += aRecargar;
        reservaMunicion -= aRecargar;

        ActualizarHUD();
        estaRecargando = false;
    }

    public void AgregarMunicion(int cantidad)
    {
        reservaMunicion += cantidad;
        ActualizarHUD();
    }

    public void ActualizarHUD()
    {
        if (textoMunicion != null)
        {
            textoMunicion.text = $"{municionActual}/{reservaMunicion}";
        }

       
        if ((municionActual + reservaMunicion) > 0)
        {
            if (textoMunicionBaja != null && textoMunicionBaja.activeSelf)
                textoMunicionBaja.SetActive(false);

            if (flechaGuiaMunicion != null && flechaGuiaMunicion.gameObject.activeSelf)
                flechaGuiaMunicion.gameObject.SetActive(false);
        }
    }
}