using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicaFondo : MonoBehaviour
{
    private static MusicaFondo instancia;
    private AudioSource audioSource;

    [Header("Escenas donde la música DEBE PARAR")]
    [Tooltip("Nombre exacto de la escena donde se juega el Nivel 1")]
    public string nombreEscenaJuego = "SampleScene";

    [Tooltip("Nombre exacto de la escena cinemática inicial")]
    public string nombreEscenaCinematica = "EscenaCinematica";

    [Tooltip("Nombre exacto de la escena del Nivel 2")]
    public string nombreEscenaNivel2 = "Nivel2_Espacio";

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;

        if (instancia == this)
        {
            instancia = null;
        }
    }

    void Start()
    {
        ActualizarVolumen();

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        ActualizarVolumen();
    }

    public void ActualizarVolumen()
    {
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        }
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == nombreEscenaJuego || escena.name == nombreEscenaCinematica || escena.name == nombreEscenaNivel2)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            instancia = null;
            Destroy(gameObject);
        }
        else
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}