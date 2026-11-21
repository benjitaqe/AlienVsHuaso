using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorPausa : MonoBehaviour
{
    [Header("Interfaz de Pausa (UI)")]
    public GameObject panelPausa;
    public GameObject objetoJugador;

    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GuardarPartida()
    {
        DatosPartida datos = new DatosPartida();

        string escenaActual = SceneManager.GetActiveScene().name;
        datos.escenaReal = escenaActual;
        datos.nombreVisible = (escenaActual == "SampleScene") ? "Nivel 1" : (escenaActual == "Nivel2_Espacio" ? "Nivel 2 - Nave Espacial" : escenaActual);
        datos.fechaHora = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        if (objetoJugador != null)
        {
            ControladorJugador controlador = objetoJugador.GetComponent<ControladorJugador>();
            if (controlador != null && !string.IsNullOrEmpty(controlador.jugadorId))
            {
                datos.jugadorId = controlador.jugadorId;
            }
            else
            {
                datos.jugadorId = "HUASO-001";
            }

            VidaHuaso vida = objetoJugador.GetComponent<VidaHuaso>();
            if (vida != null)
            {
                datos.vidaJugador = vida.vidaActual;
            }

            datos.posX = objetoJugador.transform.position.x;
            datos.posY = objetoJugador.transform.position.y;
            datos.posZ = objetoJugador.transform.position.z;
        }
        else
        {
            datos.jugadorId = "HUASO-001";
        }

        if (ContadorMisiones.Instancia != null)
        {
            datos.enemigosRestantes = ContadorMisiones.Instancia.enemigosRestantes;
            datos.pistasEncontradas = ContadorMisiones.Instancia.pistasEncontradas;

            datos.misionAliensCompletada = (ContadorMisiones.Instancia.enemigosRestantes <= 0);
            datos.pistasCompletadas = (ContadorMisiones.Instancia.pistasEncontradas >= 5);

            int enemigosMatados = ContadorMisiones.Instancia.totalEnemigos - ContadorMisiones.Instancia.enemigosRestantes;
            datos.porcentajeProgreso = Mathf.Clamp((int)((float)enemigosMatados / ContadorMisiones.Instancia.totalEnemigos * 100f), 0, 100);
        }
        else
        {
            datos.enemigosRestantes = 25;
            datos.pistasEncontradas = 0;
            datos.porcentajeProgreso = 0;
            datos.misionAliensCompletada = false;
            datos.pistasCompletadas = false;
        }

        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Guardar;
        MenuPartidasGuardadas.datosPendientesParaGuardar = datos;

        Time.timeScale = 1f;
        SceneManager.LoadScene("EscenaPartidasGuardadas");
    }

    public void BotonReanudar() => Reanudar();

    public void BotonOpciones()
    {
        Time.timeScale = 1f;

        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("EscenaPreviaOpciones", escenaActual);
        PlayerPrefs.Save();
        OpcionesDentro.escenaOrigenPausa = escenaActual;

        SceneManager.LoadScene("OpcionesDentro");
    }

    public void BotonSalir()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInteraccion");
    }
}