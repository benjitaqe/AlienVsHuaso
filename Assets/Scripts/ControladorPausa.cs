using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    [Header("Paneles de Pausa")]
    public GameObject panelPausa;
    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
            {
                ReanudarJuego();
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
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void IrAOpciones()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetString("EscenaPreviaOpciones", "SampleScene");
        PlayerPrefs.Save();
        OpcionesDentro.escenaOrigenPausa = "SampleScene";

        SceneManager.LoadScene("OpcionesDentro");
    }

    public void GuardarProgresoActual()
    {
        DatosPartida datos = new DatosPartida();

        string escenaActual = SceneManager.GetActiveScene().name;
        datos.escenaReal = escenaActual;
        datos.nombreVisible = (escenaActual == "SampleScene") ? "Nivel 1" : escenaActual;
        datos.fechaHora = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        GameObject huaso = GameObject.Find("huaso");
        if (huaso != null)
        {
            VidaHuaso vida = huaso.GetComponent<VidaHuaso>();
            if (vida != null)
            {
                datos.vidaJugador = vida.vidaActual;
            }

            datos.posX = huaso.transform.position.x;
            datos.posY = huaso.transform.position.y;
            datos.posZ = huaso.transform.position.z;
        }

        if (ContadorMisiones.Instancia != null)
        {
            datos.enemigosRestantes = ContadorMisiones.Instancia.enemigosRestantes;
            datos.pistasEncontradas = ContadorMisiones.Instancia.pistasEncontradas;

            int enemigosMatados = ContadorMisiones.Instancia.totalEnemigos - ContadorMisiones.Instancia.enemigosRestantes;
            datos.porcentajeProgreso = Mathf.Clamp((int)((float)enemigosMatados / ContadorMisiones.Instancia.totalEnemigos * 100f), 0, 100);
        }
        else
        {
            datos.enemigosRestantes = 25;
            datos.pistasEncontradas = 0;
            datos.porcentajeProgreso = 0;
        }

        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Guardar;
        MenuPartidasGuardadas.datosPendientesParaGuardar = datos;

        Time.timeScale = 1f;
        SceneManager.LoadScene("EscenaPartidasGuardadas");
    }

    public void IrAPartidasGuardadas()
    {
        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Cargar;
        MenuPartidasGuardadas.datosPendientesParaGuardar = null;

        Time.timeScale = 1f;
        SceneManager.LoadScene("EscenaPartidasGuardadas");
    }

    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInteraccion");
    }
}