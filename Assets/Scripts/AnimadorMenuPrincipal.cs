using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class AnimadorMenuPrincipal : MonoBehaviour
{
    [Header("Pantalla de Presionar para Iniciar")]
    public GameObject contenedorMensajeInicio;
    public TextMeshProUGUI textoParpadeo;
    public float velocidadParpadeo = 2.5f;

    [Header("Soporte de Madera (desing)")]
    public RectTransform soporteMadera;
    public float duracionEntradaSoporte = 0.5f;
    public Vector2 desplazamientoInicialSoporte = new Vector2(-400f, 200f);

    [Header("Botones Colgados")]
    public RectTransform[] listaBotones;
    public float retrasoEntreBotones = 0.12f;
    public float duracionAparicionBoton = 0.35f;

    private Vector3[] escalasOriginalesBotones;
    private Vector2 posicionFinalSoporte;
    private bool menuIniciado = false;

    void Awake()
    {
        if (listaBotones != null && listaBotones.Length > 0)
        {
            escalasOriginalesBotones = new Vector3[listaBotones.Length];
            for (int i = 0; i < listaBotones.Length; i++)
            {
                if (listaBotones[i] != null)
                {
                    escalasOriginalesBotones[i] = listaBotones[i].localScale;
                    listaBotones[i].localScale = Vector3.zero;
                }
            }
        }

        if (soporteMadera != null)
        {
            posicionFinalSoporte = soporteMadera.anchoredPosition;
            soporteMadera.anchoredPosition = posicionFinalSoporte + desplazamientoInicialSoporte;
            soporteMadera.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (contenedorMensajeInicio != null)
        {
            contenedorMensajeInicio.SetActive(true);
        }
    }

    void Update()
    {
        if (!menuIniciado)
        {
            if (textoParpadeo != null)
            {
                float alfa = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
                textoParpadeo.color = new Color(textoParpadeo.color.r, textoParpadeo.color.g, textoParpadeo.color.b, Mathf.Clamp(alfa, 0.2f, 1f));
            }

            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                ComenzarSecuenciaMenu();
            }
        }
    }

    void ComenzarSecuenciaMenu()
    {
        menuIniciado = true;

        if (contenedorMensajeInicio != null)
        {
            contenedorMensajeInicio.SetActive(false);
        }

        StartCoroutine(RutinaSecuenciaCompleta());
    }

    IEnumerator RutinaSecuenciaCompleta()
    {
        if (soporteMadera != null)
        {
            soporteMadera.gameObject.SetActive(true);
            yield return StartCoroutine(AnimarEntradaSoporte());
        }

        yield return new WaitForSeconds(0.05f);

        for (int i = 0; i < listaBotones.Length; i++)
        {
            if (listaBotones[i] != null)
            {
                StartCoroutine(AnimarBotonKickBuddy(listaBotones[i], escalasOriginalesBotones[i]));
                yield return new WaitForSeconds(retrasoEntreBotones);
            }
        }
    }

    IEnumerator AnimarEntradaSoporte()
    {
        float t = 0f;
        Vector2 posInicio = posicionFinalSoporte + desplazamientoInicialSoporte;

        while (t < duracionEntradaSoporte)
        {
            t += Time.deltaTime;
            float factor = t / duracionEntradaSoporte;

            float curvaRebote = 1f - Mathf.Pow(2.718f, -6f * factor) * Mathf.Cos(factor * Mathf.PI * 3f);
            soporteMadera.anchoredPosition = Vector2.LerpUnclamped(posInicio, posicionFinalSoporte, curvaRebote);
            yield return null;
        }

        soporteMadera.anchoredPosition = posicionFinalSoporte;
    }

    IEnumerator AnimarBotonKickBuddy(RectTransform rect, Vector3 escalaMeta)
    {
        float tiempo = 0f;

        while (tiempo < duracionAparicionBoton)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionAparicionBoton;

            float rebote = 1f + Mathf.Sin(t * Mathf.PI * 2f) * (0.35f * (1f - t));
            float factorProgreso = Mathf.Sin(t * Mathf.PI * 0.5f);

            rect.localScale = Vector3.Lerp(Vector3.zero, escalaMeta * rebote, factorProgreso);
            yield return null;
        }

        rect.localScale = escalaMeta;
    }

    public void BotonNuevaPartida()
    {
        PlayerPrefs.SetInt("SlotACargar", -1);
        PlayerPrefs.Save();
        ControladorPantallaCarga.CargarConPantalla("EscenaCinematica", 4f);
    }

    public void BotonCargarPartidas()
    {
        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Cargar;
        SceneManager.LoadScene("EscenaPartidasGuardadas");
    }

    public void BotonOpciones()
    {
        SceneManager.LoadScene("EscenaOpciones");
    }

    public void BotonExtras()
    {
        SceneManager.LoadScene("EscenaExtras");
    }

    public void BotonSalir()
    {
        Application.Quit();
    }
}