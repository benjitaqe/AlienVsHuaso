using UnityEngine;
using System.IO;
using System;

public static class SistemaGuardadoJSON
{
    private static string ObtenerRuta(int slot)
    {
        return Path.Combine(Application.persistentDataPath, "partida_slot" + slot + ".json");
    }

    public static void GuardarPartida(int slot, string jugadorId, DatosPartida datos)
    {
        if (datos == null)
        {
            Debug.LogError("No se pueden guardar datos nulos.");
            return;
        }

        datos.jugadorId = jugadorId;
        datos.numeroSlot = slot;
        datos.fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        string json = JsonUtility.ToJson(datos, true);

        string ruta = ObtenerRuta(slot);
        File.WriteAllText(ruta, json);

        Debug.Log("Partida guardada con éxito en: " + ruta + " | Jugador: " + jugadorId);
    }

    public static DatosPartida CargarPartida(int slot)
    {
        string ruta = ObtenerRuta(slot);

        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            DatosPartida datos = JsonUtility.FromJson<DatosPartida>(json);
            return datos;
        }
        else
        {
            Debug.Log("No existe archivo de guardado en el slot " + slot);
            return null;
        }
    }

    public static bool ExistePartida(int slot)
    {
        return File.Exists(ObtenerRuta(slot));
    }

    public static void BorrarPartida(int slot)
    {
        string ruta = ObtenerRuta(slot);
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
            Debug.Log("Partida del slot " + slot + " eliminada.");
        }
    }
}