using UnityEngine;

public class TriggerOleada2 : MonoBehaviour
{
    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;
        if (other.CompareTag("Player") || other.name.ToLower().Contains("huaso"))
        {
            activado = true;
            Debug.Log("¡El Huaso entró a la Zona 2! Iniciando segunda oleada...");

            if (GestorOleadasNivel2.Instancia != null)
            {
                GestorOleadasNivel2.Instancia.IniciarOleada2();
            }

            gameObject.SetActive(false);
        }
    }
}