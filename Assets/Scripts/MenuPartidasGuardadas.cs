using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuPartidasGuardadas : MonoBehaviour
{
    public enum ModoPantalla { Cargar, Guardar }

    public static ModoPantalla modoActual = ModoPantalla.Cargar;
    public static DatosPartida datosPendientesParaGuardar = null;

    [System.Serializable]
    public class SlotUI
    {
        public int numeroSlot;
        public Button botonSlot;
        public TextMeshProUGUI textoDetalles;
    }

    [Header("Configuración de los 3 Slots")]
    public SlotUI slot1;
    public SlotUI slot2;
    public SlotUI slot3;

    [Header("Configuración de Escenas")]
    public string escenaNivel1 = "SampleScene";
    public string escenaMenuAtras = "MenuInteraccion";

    [Header("Título Dinámico (Opcional)")]
    public TextMeshProUGUI textoTitulo;

    void Start()
    {
        if (textoTitulo != null)
        {
            textoTitulo.text = (modoActual == ModoPantalla.Guardar) ? "ELIGE DÓNDE GUARDAR" : "PARTIDAS GUARDADAS";
        }

        ActualizarVistaSlots();
    }

    public void ActualizarVistaSlots()
    {
        ConfigurarSlot(slot1, 1);
        ConfigurarSlot(slot2, 2);
        ConfigurarSlot(slot3, 3);
    }

    void ConfigurarSlot(SlotUI slot, int numero)
    {
        slot.numeroSlot = numero;

        if (SistemaGuardadoJSON.ExistePartida(slot.numeroSlot))
        {
            DatosPartida datos = SistemaGuardadoJSON.CargarPartida(slot.numeroSlot);

            if (slot.textoDetalles != null)
            {
                string idMostrar = !string.IsNullOrEmpty(datos.jugadorId) ? $"[{datos.jugadorId}] " : "";
                string titulo = !string.IsNullOrEmpty(datos.nombreVisible) ? datos.nombreVisible : datos.escenaReal;

                slot.textoDetalles.text = $"{idMostrar}{titulo}\n{datos.porcentajeProgreso}%\n{datos.fechaHora}";
            }
        }
        else
        {
            if (slot.textoDetalles != null)
            {
                slot.textoDetalles.text = (modoActual == ModoPantalla.Guardar) ? "VACÍO\n(Guardar Aquí)" : "VACÍO\nNueva Partida";
            }
        }
    }

    public void SeleccionarSlot(int numeroSlot)
    {
        if (modoActual == ModoPantalla.Guardar)
        {
            if (datosPendientesParaGuardar != null)
            {
                string idAGuardar = !string.IsNullOrEmpty(datosPendientesParaGuardar.jugadorId)
                    ? datosPendientesParaGuardar.jugadorId
                    : "HUASO-001";

                datosPendientesParaGuardar.numeroSlot = numeroSlot;
                SistemaGuardadoJSON.GuardarPartida(numeroSlot, idAGuardar, datosPendientesParaGuardar);
                Debug.Log($"[MenuSlots] Guardado con éxito en Slot {numeroSlot} con posición ({datosPendientesParaGuardar.posX}, {datosPendientesParaGuardar.posY}, {datosPendientesParaGuardar.posZ})");

                string escenaRegreso = datosPendientesParaGuardar.escenaReal;
                datosPendientesParaGuardar = null;

                PlayerPrefs.SetInt("SlotACargar", numeroSlot);
                PlayerPrefs.Save();

                Time.timeScale = 1f;
                CargarEscenaSegura(escenaRegreso);
            }
            else
            {
                PlayerPrefs.SetInt("SlotACargar", -1);
                CargarEscenaSegura(escenaNivel1);
            }
        }
        else
        {
            if (SistemaGuardadoJSON.ExistePartida(numeroSlot))
            {
                DatosPartida datos = SistemaGuardadoJSON.CargarPartida(numeroSlot);
                PlayerPrefs.SetInt("SlotACargar", numeroSlot);
                PlayerPrefs.Save();

                string escenaDestino = !string.IsNullOrEmpty(datos.escenaReal) ? datos.escenaReal : escenaNivel1;
                CargarEscenaSegura(escenaDestino);
            }
            else
            {
                PlayerPrefs.SetInt("SlotACargar", -1);
                PlayerPrefs.Save();
                CargarEscenaSegura(escenaNivel1);
            }
        }
    }

    public void BotonAtras()
    {
        Time.timeScale = 1f;

        if (modoActual == ModoPantalla.Guardar && datosPendientesParaGuardar != null)
        {
            SistemaGuardadoJSON.GuardarPartida(99, "HUASO-001", datosPendientesParaGuardar);
            PlayerPrefs.SetInt("SlotACargar", 99);
            PlayerPrefs.Save();

            string escenaRetorno = datosPendientesParaGuardar.escenaReal;
            datosPendientesParaGuardar = null;

            SceneManager.LoadScene(escenaRetorno);
            return;
        }

        SceneManager.LoadScene(escenaMenuAtras);
    }

    void CargarEscenaSegura(string nombreEscena)
    {
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            ControladorPantallaCarga.CargarConPantalla(nombreEscena, 4f);
        }
        else
        {
            Debug.LogError($"[MenuPartidasGuardadas] La escena '{nombreEscena}' no se puede cargar.");
        }
    }
}