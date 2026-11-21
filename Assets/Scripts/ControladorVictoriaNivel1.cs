using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorVictoriaNivel1 : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string escenaPartidasGuardadas = "EscenaPartidasGuardadas";
    public string escenaNivel2 = "Nivel2_Espacio";

    public void BotonGuardarYContinuar()
    {
        Time.timeScale = 1f;

        MenuPartidasGuardadas.modoActual = MenuPartidasGuardadas.ModoPantalla.Guardar;

        DatosPartida datosProgreso = new DatosPartida();
        datosProgreso.jugadorId = "HUASO-001";
        datosProgreso.nombreVisible = "Nivel 2: Espacio";
        datosProgreso.escenaReal = escenaNivel2;
        datosProgreso.porcentajeProgreso = 50;
        datosProgreso.fechaHora = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        MenuPartidasGuardadas.datosPendientesParaGuardar = datosProgreso;

        SceneManager.LoadScene(escenaPartidasGuardadas);
    }

    public void BotonContinuarSinGuardar()
    {
        Time.timeScale = 1f;

        ControladorPantallaCarga.CargarConPantalla(escenaNivel2, 4f);
    }
}