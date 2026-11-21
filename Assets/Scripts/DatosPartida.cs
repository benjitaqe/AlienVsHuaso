using System;

[System.Serializable]
public class DatosPartida
{
    public string jugadorId;

    public int numeroSlot;
    public string nombreVisible;
    public string escenaReal;
    public int porcentajeProgreso;
    public string fechaHora;

 
    public float vidaJugador;
    public float posX;
    public float posY;
    public float posZ;
    public float rotY;

    public int enemigosRestantes;
    public int pistasEncontradas;
    public bool misionAliensCompletada;
    public bool pistasCompletadas;

    public int oleadaActualNivel2;
    public int enemigosRestantesNivel2;
}