using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControladorMusica : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ActualizarVolumen();
    }

    void Start()
    {
        ActualizarVolumen();
    }

    void OnEnable()
    {
        ActualizarVolumen();
    }

    void Update()
    {
        ActualizarVolumen();
    }

    public void ActualizarVolumen()
    {
        if (audioSource != null)
        {
            float vol = PlayerPrefs.GetFloat("VolumenMusica", 1f);
            audioSource.volume = vol;
        }
    }
}