using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpcionesDentro : MonoBehaviour
{
    public static string escenaOrigenPausa = "SampleScene";

    [Header("Configuraciones de Componentes")]
    public Toggle togglePantallaCompleta;
    public Slider sliderMusica;
    public Slider sliderSonido;
    public GameObject panelAdvertencia;

    void Start()
    {
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

    public void VolverAlJuego()
    {
        Time.timeScale = 1f;

        string escenaADevolver = PlayerPrefs.GetString("EscenaPreviaOpciones", escenaOrigenPausa);

        if (string.IsNullOrEmpty(escenaADevolver))
        {
            escenaADevolver = "SampleScene";
        }

        if (MenuPartidasGuardadas.datosPendientesParaGuardar != null)
        {
            SistemaGuardadoJSON.GuardarPartida(99, "HUASO-001", MenuPartidasGuardadas.datosPendientesParaGuardar);
            PlayerPrefs.SetInt("SlotACargar", 99);
            PlayerPrefs.Save();
            MenuPartidasGuardadas.datosPendientesParaGuardar = null;
        }
        if (escenaADevolver == "SampleScene")
        {
            PlayerPrefs.SetInt("CargarDesdeGuardado", 1);
            PlayerPrefs.Save();
        }

        Debug.Log("[OpcionesDentro] Regresando con éxito a: " + escenaADevolver);
        SceneManager.LoadScene(escenaADevolver);
    }

    public void OnFullScreenToggleChanged(bool unValor)
    {
        if (!unValor)
        {
            if (panelAdvertencia != null) panelAdvertencia.SetActive(true);
            if (togglePantallaCompleta != null) togglePantallaCompleta.isOn = true;
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