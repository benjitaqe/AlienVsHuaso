using UnityEngine;

public class ItemCuracion : MonoBehaviour
{
    [Header("Configuración de Curación")]
    public float cantidadCuracion = 20f;
    public float velocidadRotacion = 90f;
    public float velocidadFlotacion = 2f;
    public float alturaFlotacion = 0.15f;

    [Header("Audio")]
    public AudioClip sonidoCurar;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.ToLower().Contains("huaso"))
        {
            VidaHuaso vida = other.GetComponent<VidaHuaso>();
            if (vida == null) vida = other.GetComponentInParent<VidaHuaso>();

            if (vida != null)
            {
                if (vida.vidaActual < vida.vidaMaxima)
                {
                    float nuevaVida = vida.vidaActual + cantidadCuracion;
                    vida.SetVida(nuevaVida);

                    if (sonidoCurar != null)
                    {
                        AudioSource.PlayClipAtPoint(sonidoCurar, transform.position, PlayerPrefs.GetFloat("VolumenSonido", 1f));
                    }

                    Debug.Log($"¡Huaso curado! +{cantidadCuracion} HP. Vida actual: {vida.vidaActual}");
                    Destroy(gameObject);
                }
            }
        }
    }
}