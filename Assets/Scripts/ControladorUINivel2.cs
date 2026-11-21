using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorUINivel2 : MonoBehaviour
{
    public static ControladorUINivel2 Instancia { get; private set; }

    [Header("Paneles Principales")]
    public GameObject panelPausa;
    public GameObject panelMuerte;
    public GameObject panelVictoria;

    [Header("Elementos del HUD a Ocultar")]
    public GameObject hudContador;
    public GameObject hudMunicion;
    public GameObject hudVida;

    [Header("Escenas")]
    public string escenaMenuPrincipal = "MenuInteraccion";
    public string escenaPartidasGuardadas = "EscenaPartidasGuardadas";
    public string escenaOpciones = "OpcionesDentro";

    private bool estaPausado = false;
    private bool juegoTerminado = false;

    void Awake()
    {
        Instancia = this;

        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    void Start()
    {
        Time.timeScale = 1f;
        BloquearCursor(true);
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                BotonReanudar();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        estaPausado = true;
        Time.timeScale = 0f;

        if (panelPausa != null) panelPausa.SetActive(true);
        BloquearCursor(false);
    }

    public void BotonReanudar()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);

        Time.timeScale = 1f;
        BloquearCursor(true);
    }

    public void BotonGuardarPartida()
    {
        Time.timeScale = 1f;

        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Guardar;

        DatosPartida datosProgreso = new DatosPartida();
        datosProgreso.jugadorId = "HUASO-001";
        datosProgreso.nombreVisible = "Nivel 2: Espacio";
        datosProgreso.escenaReal = "Nivel2_Espacio";
        datosProgreso.fechaHora = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        GameObject objHuaso = GameObject.FindWithTag("Player");
        if (objHuaso != null)
        {
            datosProgreso.posX = objHuaso.transform.position.x;
            datosProgreso.posY = objHuaso.transform.position.y;
            datosProgreso.posZ = objHuaso.transform.position.z;
            datosProgreso.rotY = objHuaso.transform.eulerAngles.y;

            VidaHuaso vida = objHuaso.GetComponent<VidaHuaso>();
            if (vida != null)
            {
                datosProgreso.vidaJugador = vida.vidaActual;
            }

            DisparoHuaso disparo = objHuaso.GetComponent<DisparoHuaso>();
            if (disparo == null) disparo = objHuaso.GetComponentInChildren<DisparoHuaso>();
            if (disparo != null)
            {
                datosProgreso.enemigosRestantes = disparo.municionActual;
                datosProgreso.pistasEncontradas = disparo.reservaMunicion;
            }
        }

        if (GestorOleadasNivel2.Instancia != null)
        {
            int oleada = GestorOleadasNivel2.Instancia.ObtenerOleadaActual();
            datosProgreso.oleadaActualNivel2 = oleada;
            datosProgreso.enemigosRestantesNivel2 = GestorOleadasNivel2.Instancia.ObtenerEnemigosRestantes();

            if (oleada == 1) datosProgreso.porcentajeProgreso = 50;
            else if (oleada == 2) datosProgreso.porcentajeProgreso = 75;
            else if (oleada == 3) datosProgreso.porcentajeProgreso = 90;
            else if (oleada >= 4) datosProgreso.porcentajeProgreso = 100; 
        }
        else
        {
            datosProgreso.porcentajeProgreso = 50;
        }

        MenuPartidasGuardadas.datosPendientesParaGuardar = datosProgreso;
        SceneManager.LoadScene(escenaPartidasGuardadas);
    }

    public void BotonIrAOpciones()
    {
        Time.timeScale = 1f;

        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("EscenaPreviaOpciones", escenaActual);
        PlayerPrefs.Save();
        OpcionesDentro.escenaOrigenPausa = escenaActual;

        SceneManager.LoadScene(escenaOpciones);
    }

    public void MostrarPantallaMuerte()
    {
        juegoTerminado = true;
        Time.timeScale = 0f;

        DesactivarDisparoHuaso();
        OcultarHUDCompleto();

        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(true);

        BloquearCursor(false);
    }

    public void MostrarPantallaVictoria()
    {
        juegoTerminado = true;
        Time.timeScale = 0f;

        DesactivarDisparoHuaso();
        OcultarHUDCompleto();

        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(true);

        BloquearCursor(false);
    }

    void OcultarHUDCompleto()
    {
        if (hudContador != null) hudContador.SetActive(false);
        if (hudMunicion != null) hudMunicion.SetActive(false);
        if (hudVida != null) hudVida.SetActive(false);
    }

    void DesactivarDisparoHuaso()
    {
        GameObject objHuaso = GameObject.FindWithTag("Player");
        if (objHuaso != null)
        {
            DisparoHuaso disparo = objHuaso.GetComponent<DisparoHuaso>();
            if (disparo == null) disparo = objHuaso.GetComponentInChildren<DisparoHuaso>();
            if (disparo != null) disparo.enabled = false;
        }
    }

    public void BotonReiniciarNivel()
    {
        Time.timeScale = 1f;
        ControladorPantallaCarga.CargarConPantalla("Nivel2_Espacio", 4f);
    }

    public void BotonIrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    void BloquearCursor(bool bloquear)
    {
        Cursor.lockState = bloquear ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloquear;
    }
}