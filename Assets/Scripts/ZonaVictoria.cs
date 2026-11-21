using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        VidaHuaso vidaJugador = other.GetComponentInParent<VidaHuaso>();

        if (vidaJugador != null)
        {
            Debug.Log("¡Huaso detectado en la Zona de Victoria!");
            vidaJugador.GanarNivel();
        }
    }
}