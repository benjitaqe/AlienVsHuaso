using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControladorCinematica : MonoBehaviour
{
    [Header("Componentes de la Interfaz")]
    public Image contenedorImagen;
    public Button botonSiguiente;
    public TextMeshProUGUI textoBoton;

    [Header("Lista de Viñetas (Historia)")]
    public Sprite[] vinetas;

    [Header("Efectos de Audio de la Cinemática")]
    public AudioClip audioVoz;
    public AudioClip audioSonidoNoticia;
    public AudioClip audioBoom;
    public AudioClip audioCargarArma;

    [Header("Configuración de Escena Siguiente")]
    public string nombreEscenaJuego = "SampleScene";

    private int indiceActual = 0;

    private AudioSource audioSourcePrincipal;
    private AudioSource audioSourceSecundario;

    void Awake()
    {
        AudioSource[] fuentes = GetComponents<AudioSource>();
        if (fuentes.Length >= 2)
        {
            audioSourcePrincipal = fuentes[0];
            audioSourceSecundario = fuentes[1];
        }
        else if (fuentes.Length == 1)
        {
            audioSourcePrincipal = fuentes[0];
            audioSourceSecundario = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSourcePrincipal = gameObject.AddComponent<AudioSource>();
            audioSourceSecundario = gameObject.AddComponent<AudioSource>();
        }

        audioSourcePrincipal.loop = false;
        audioSourceSecundario.loop = false;
    }

    void Start()
    {
        if (contenedorImagen != null)
        {
            contenedorImagen.color = Color.white;
            contenedorImagen.gameObject.SetActive(true);
        }

        if (vinetas != null && vinetas.Length > 0)
        {
            indiceActual = 0;
            MostrarVineta(0);
        }

        if (botonSiguiente != null)
        {
            botonSiguiente.onClick.RemoveAllListeners();
            botonSiguiente.onClick.AddListener(AvanzarVineta);
        }
    }

    public void AvanzarVineta()
    {
        indiceActual++;

        if (indiceActual < vinetas.Length)
        {
            MostrarVineta(indiceActual);
        }
        else
        {
            CargarJuego();
        }
    }

    void MostrarVineta(int nuevoIndice)
    {
        if (contenedorImagen != null && vinetas != null && nuevoIndice < vinetas.Length)
        {
            contenedorImagen.sprite = vinetas[nuevoIndice];
            contenedorImagen.color = Color.white;
        }

        ActualizarTextoBoton();
        ReproducirAudiosPorIndice(nuevoIndice);
    }

    void ReproducirAudiosPorIndice(int indice)
    {
        float volSonido = PlayerPrefs.GetFloat("VolumenSonido", 1f);

        if (audioSourcePrincipal != null) audioSourcePrincipal.Stop();
        if (audioSourceSecundario != null) audioSourceSecundario.Stop();

        switch (indice)
        {
            case 0:
                if (audioSonidoNoticia != null && audioSourcePrincipal != null)
                {
                    audioSourcePrincipal.clip = audioSonidoNoticia;
                    audioSourcePrincipal.volume = volSonido * 0.7f;
                    audioSourcePrincipal.Play();
                }

                if (audioVoz != null && audioSourceSecundario != null)
                {
                    audioSourceSecundario.clip = audioVoz;
                    audioSourceSecundario.volume = volSonido;
                    audioSourceSecundario.Play();
                }
                break;

            case 1:
                if (audioBoom != null && audioSourcePrincipal != null)
                {
                    audioSourcePrincipal.PlayOneShot(audioBoom, volSonido);
                }
                break;

            case 2:
                if (audioBoom != null && audioSourcePrincipal != null)
                {
                    audioSourcePrincipal.PlayOneShot(audioBoom, volSonido);
                }
                break;

            case 3:
                if (audioCargarArma != null && audioSourcePrincipal != null)
                {
                    audioSourcePrincipal.PlayOneShot(audioCargarArma, volSonido);
                }
                break;
        }
    }

    void ActualizarTextoBoton()
    {
        if (textoBoton != null)
        {
            if (indiceActual == vinetas.Length - 1)
            {
                textoBoton.text = "JUGAR";
            }
            else
            {
                textoBoton.text = "SIGUIENTE";
            }
        }
    }

    void CargarJuego()
    {
        if (audioSourcePrincipal != null) audioSourcePrincipal.Stop();
        if (audioSourceSecundario != null) audioSourceSecundario.Stop();

        ControladorPantallaCarga.CargarConPantalla(nombreEscenaJuego, 4f);
    }
}