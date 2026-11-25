// [Fecha: 2026-11-25] Prueba de Caja de Humo - Verificacion inicial de escena y persistencia
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PruebaDeHumo : MonoBehaviour
{
    void Start()
    {
        string fecha = "2026-11-25";
        string rutaGuardado = Path.Combine(Application.persistentDataPath, "DataSave.json");
        string escenaActiva = SceneManager.GetActiveScene().name;
        int objetosRaiz = SceneManager.GetActiveScene().rootCount;
        bool existeSave = File.Exists(rutaGuardado);
        bool rutaValida = !string.IsNullOrEmpty(rutaGuardado);

        Debug.Log($"[{fecha}][SMOKE] Escena={escenaActiva} | ObjetosRaiz={objetosRaiz} | ExisteDataSave={existeSave} | RutaValida={rutaValida} | Ruta={rutaGuardado}");
    }
}