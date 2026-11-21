using UnityEngine;

public class ItemMunicion : MonoBehaviour
{
    [Header("Configuración")]
    public int cantidadMunicion = 4;        
    public float velocidadRotacion = 90f;   
    public float velocidadFlotacion = 2f;  
    public float alturaFlotacion = 0.25f;   

    [Header("Audio")]
    public AudioClip sonidoRecoger;       

    private Vector3 posicionInicial;
    private bool yaRecogido = false;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.World);

        float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return;

        bool esJugador = other.CompareTag("Player") ||
                         other.name.ToLower().Contains("huaso") ||
                         (other.transform.root != null && other.transform.root.name.ToLower().Contains("huaso"));

        if (esJugador)
        {
            DisparoHuaso escopeta = other.GetComponentInChildren<DisparoHuaso>();
            if (escopeta == null && other.transform.root != null)
            {
                escopeta = other.transform.root.GetComponentInChildren<DisparoHuaso>();
            }
            if (escopeta == null)
            {
                escopeta = FindAnyObjectByType<DisparoHuaso>();
            }

            if (escopeta != null)
            {
                yaRecogido = true;

                escopeta.reservaMunicion += cantidadMunicion;
                escopeta.ActualizarHUD();

                Debug.Log($"¡Munición recogida! +{cantidadMunicion} balas a la reserva.");

                if (sonidoRecoger != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position, PlayerPrefs.GetFloat("VolumenSonido", 1f));
                }

                Destroy(gameObject);
            }
        }
    }
}