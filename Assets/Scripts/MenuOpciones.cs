using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    [Header("Configuraciones de Componentes")]
    public Toggle togglePantallaCompleta;
    public Slider sliderMusica;
    public Slider sliderSonido;
    public GameObject panelAdvertencia;

    [Header("Detección de Guardado (UI)")]
    public GameObject botonCargarPartida;

    private static string escenaAnterior = "MenuInteraccion";

    void Start()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);

        if (botonCargarPartida != null)
        {
            bool hayPartidas = SistemaGuardadoJSON.ExistePartida(1) ||
                               SistemaGuardadoJSON.ExistePartida(2) ||
                               SistemaGuardadoJSON.ExistePartida(3);

            botonCargarPartida.SetActive(hayPartidas);
        }

        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 1f);
            sliderMusica.onValueChanged.AddListener(SetVolumeMusica);
        }

        if (sliderSonido != null)
        {
            sliderSonido.value = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            sliderSonido.onValueChanged.AddListener(SetVolumeSonido);
        }
        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.isOn = Screen.fullScreen;
            togglePantallaCompleta.onValueChanged.AddListener(OnFullScreenToggleChanged);
        }

        if (panelAdvertencia != null) panelAdvertencia.SetActive(false);
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("SlotACargar", -1);
        PlayerPrefs.Save();

        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Cargar;
        MenuPartidasGuardadas.datosPendientesParaGuardar = null;

        SceneManager.LoadScene("EscenaCinematica");
    }

    public void AbrirPartidasGuardadas()
    {
        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Cargar;
        MenuPartidasGuardadas.datosPendientesParaGuardar = null;

        SceneManager.LoadScene("EscenaPartidasGuardadas");
    }

    public void IrAEscenaOpciones()
    {
        escenaAnterior = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("EscenaOpciones");
    }

    public void VolverDeOpciones()
    {
        SceneManager.LoadScene(escenaAnterior);
    }

    public void IrAEscenaExtras()
    {
        SceneManager.LoadScene("EscenaExtras");
    }

    public void VolverDeExtras()
    {
        SceneManager.LoadScene("MenuInteraccion");
    }

    public void QuitGame()
    {
        Debug.Log("Juego cerrado correctamente.");
        Application.Quit();
    }

    public void OnFullScreenToggleChanged(bool unValor)
    {
        if (!unValor)
        {
            if (panelAdvertencia != null)
            {
                panelAdvertencia.SetActive(true);
            }
            if (togglePantallaCompleta != null)
            {
                togglePantallaCompleta.isOn = true;
            }
        }
        else
        {
            Screen.fullScreen = true;
        }
    }

    public void CerrarAdvertencia()
    {
        if (panelAdvertencia != null) panelAdvertencia.SetActive(false);
    }

    public void SetVolumeMusica(float valor)
    {
        PlayerPrefs.SetFloat("VolumenMusica", valor);
        PlayerPrefs.Save();
    }

    public void SetVolumeSonido(float valor)
    {
        PlayerPrefs.SetFloat("VolumenSonido", valor);
        PlayerPrefs.Save();
    }
}