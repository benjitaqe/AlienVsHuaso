using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VidaHuaso : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public float vidaMaxima = 100f;
    [HideInInspector] public float vidaActual;

    [Header("Interfaz de Usuario")]
    public Image barraVidaRelleno;

    [Header("Paneles de Estado (Misma Escena)")]
    public GameObject panelMuerte;
    public GameObject panelVictoria;

    [Header("Componentes y Animación")]
    public Animator animator;
    public GameObject escopetaOArma;

    [Header("Tiempos de Secuencia Final")]
    public float tiempoCaidaHuaso = 2.0f;
    public float tiempoDeBaile = 4.5f;

    [Header("Configuración de Audio")]
    public AudioClip sonidoDano;
    public AudioClip sonidoMuerte;
    private AudioSource fuenteAudio;

    private bool estaMuerto = false;
    private bool haGanado = false;

    void Awake()
    {
        vidaActual = vidaMaxima;
    }

    void Start()
    {
        Time.timeScale = 1f;

        ActualizarInterfaz();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
        {
            fuenteAudio = gameObject.AddComponent<AudioSource>();
        }

        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    public void SetVida(float valor)
    {
        vidaActual = Mathf.Clamp(valor, 0f, vidaMaxima);
        ActualizarInterfaz();
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto || haGanado) return;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);

        ActualizarInterfaz();

        if (sonidoDano != null && fuenteAudio != null)
        {
            float volSonido = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            fuenteAudio.PlayOneShot(sonidoDano, volSonido);
        }

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    public void ActualizarInterfaz()
    {
        if (barraVidaRelleno != null)
        {
            barraVidaRelleno.fillAmount = vidaActual / vidaMaxima;
        }
    }

    void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        Debug.Log("¡El huaso cayó en combate!");

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        ControladorJugador scriptMov = GetComponent<ControladorJugador>();
        if (scriptMov != null) scriptMov.enabled = false;

        AutoAimHuaso scriptAim = GetComponent<AutoAimHuaso>();
        if (scriptAim != null) scriptAim.enabled = false;

        MonoBehaviour[] todosLosScripts = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour s in todosLosScripts)
        {
            if (s != this) s.enabled = false;
        }

        if (escopetaOArma != null) escopetaOArma.SetActive(false);

        if (animator != null)
        {
            animator.speed = 1.0f;
            animator.SetTrigger("morir");
        }

        if (sonidoMuerte != null && fuenteAudio != null)
        {
            float volSonido = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            fuenteAudio.PlayOneShot(sonidoMuerte, volSonido);
        }

        StartCoroutine(SecuenciaMuerteYCelebracion());
    }

    IEnumerator SecuenciaMuerteYCelebracion()
    {
        IAAlien[] aliensEnEscena = Object.FindObjectsByType<IAAlien>();
        foreach (IAAlien alien in aliensEnEscena)
        {
            if (alien != null) alien.PausarPorMuerteJugador();
        }

        yield return new WaitForSeconds(tiempoCaidaHuaso);

        Camera camaraPrincipal = Camera.main;
        Vector3 posCam = camaraPrincipal != null ? camaraPrincipal.transform.position : Vector3.zero;

        for (int i = 0; i < aliensEnEscena.Length; i++)
        {
            if (aliensEnEscena[i] != null)
            {
                float offsetLateral = (i - (aliensEnEscena.Length - 1) / 2f) * 1.5f;
                aliensEnEscena[i].PosicionarYCelebrar(transform.position, transform.forward, offsetLateral, posCam);
            }
        }

        yield return new WaitForSeconds(tiempoDeBaile);

        if (ControladorUINivel2.Instancia != null)
        {
            ControladorUINivel2.Instancia.MostrarPantallaMuerte();
        }
        else
        {
            if (panelMuerte != null) panelMuerte.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void GanarNivel()
    {
        if (estaMuerto || haGanado) return;
        haGanado = true;

        if (ControladorUINivel2.Instancia != null)
        {
            ControladorUINivel2.Instancia.MostrarPantallaVictoria();
        }
        else
        {
            if (panelVictoria != null) panelVictoria.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public bool EstaMuerto() => estaMuerto;
    public bool HaGanado() => haGanado;

    public void ReintentarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInteraccion");
    }
}