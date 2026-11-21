using UnityEngine;

public class PistaInteractiva : MonoBehaviour
{
    [Header("Configuración de la Pista")]
    public string nombrePista = "Campana de la vaca";
    private bool yaRecogida = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaRecogida)
        {
            RecogerPista();
        }
    }

    void RecogerPista()
    {
        yaRecogida = true;
        Debug.Log("Pista encontrada: " + nombrePista);

        if (ContadorMisiones.Instancia != null)
        {
            ContadorMisiones.Instancia.RegistrarPistaEncontrada(nombrePista);
        }

        gameObject.SetActive(false);
    }
}