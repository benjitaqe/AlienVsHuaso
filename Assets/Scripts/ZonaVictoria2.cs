using UnityEngine;

public class ZonaVictoria2 : MonoBehaviour
{
    private bool completado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completado) return;

        VidaHuaso vidaJugador = other.GetComponentInParent<VidaHuaso>();

        if (vidaJugador != null)
        {
            completado = true;
            Debug.Log("[ZonaVictoria2] ¡Huaso llegó a la vaca! Activando victoria...");

            vidaJugador.GanarNivel();

            if (ControladorUINivel2.Instancia != null)
            {
                ControladorUINivel2.Instancia.MostrarPantallaVictoria();
            }

            ControladorJugador mov = vidaJugador.GetComponent<ControladorJugador>();
            if (mov != null) mov.enabled = false;

            CharacterController cc = vidaJugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            DisparoHuaso disp = vidaJugador.GetComponent<DisparoHuaso>();
            if (disp == null) disp = vidaJugador.GetComponentInChildren<DisparoHuaso>();
            if (disp != null) disp.enabled = false;

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}