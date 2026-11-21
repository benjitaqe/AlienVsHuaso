using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ControladorPantallaCarga : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer reproductorVideo;

    [Header("Configuración")]
    [Tooltip("Tiempo por defecto si no se especifica otro")]
    public float duracionPorDefecto = 4.0f;

    void Start()
    {
        string escenaDestino = PlayerPrefs.GetString("EscenaACargar", "MenuInteraccion");

        float tiempoEspera = PlayerPrefs.GetFloat("TiempoCarga", duracionPorDefecto);

        StartCoroutine(RutinaCargaFija(escenaDestino, tiempoEspera));
    }

    IEnumerator RutinaCargaFija(string nombreEscena, float tiempoRequerido)
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        operacion.allowSceneActivation = false;

        if (reproductorVideo != null)
        {
            reproductorVideo.Play();
        }

        float temporizador = 0f;

        while (temporizador < tiempoRequerido)
        {
            temporizador += Time.unscaledDeltaTime;
            yield return null;
        }

        while (operacion.progress < 0.9f)
        {
            yield return null;
        }

        PlayerPrefs.DeleteKey("TiempoCarga");

        operacion.allowSceneActivation = true;
    }

    public static void CargarConPantalla(string escenaDestino, float duracion = 4f)
    {
        PlayerPrefs.SetString("EscenaACargar", escenaDestino);
        PlayerPrefs.SetFloat("TiempoCarga", duracion);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EscenaCarga");
    }
}