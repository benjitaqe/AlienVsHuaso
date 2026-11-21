using UnityEngine;
using System.IO;

public class ControladorDatos : MonoBehaviour
{
    
    private static string nombreArchivo = "/DataSave.json";

    [System.Serializable]
    public class DatosJuego
    {
        public string jugadorId;    
        public float posX;
        public float posY;
        public float posZ;
    }

    public static void GuardarProgreso(string idJugador, Vector3 posicionJugador)
    {
        DatosJuego datos = new DatosJuego();
        datos.jugadorId = idJugador;
        datos.posX = posicionJugador.x;
        datos.posY = posicionJugador.y;
        datos.posZ = posicionJugador.z;

        string json = JsonUtility.ToJson(datos, true);

        string rutaCompleta = Application.persistentDataPath + nombreArchivo;
        File.WriteAllText(rutaCompleta, json);

        PlayerPrefs.SetInt("TienePartidaGuardada", 1);
        PlayerPrefs.SetString("UltimoJugadorId", idJugador);
        PlayerPrefs.Save();

        Debug.Log("Partida guardada exitosamente en: " + rutaCompleta + " para el jugador: " + idJugador);
    }

    public static bool ExistePartida()
    {
        string rutaCompleta = Application.persistentDataPath + nombreArchivo;
        return File.Exists(rutaCompleta) && PlayerPrefs.GetInt("TienePartidaGuardada", 0) == 1;
    }

    public static DatosJuego CargarProgresoCompleto()
    {
        string rutaCompleta = Application.persistentDataPath + nombreArchivo;

        if (File.Exists(rutaCompleta))
        {
            string json = File.ReadAllText(rutaCompleta);
            DatosJuego datos = JsonUtility.FromJson<DatosJuego>(json);
            return datos;
        }

        return null;
    }

    public static Vector3 CargarProgresoPosicion()
    {
        DatosJuego datos = CargarProgresoCompleto();
        if (datos != null)
        {
            return new Vector3(datos.posX, datos.posY, datos.posZ);
        }
        return Vector3.zero;
    }
}