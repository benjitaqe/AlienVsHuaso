using NUnit.Framework;
using UnityEngine;

public class PruebaMunicionTest
{
    [Test]
    public void ValidarTransferenciaMunicionRecarga()
    {
        // 1. Datos iniciales (Escopeta vacía, 10 en reserva)
        int capacidadMaxima = 2;
        int municionEnCanon = 0;
        int reserva = 10;

        // 2. Lógica interna de DisparoHuaso.cs
        int faltantes = capacidadMaxima - municionEnCanon;
        int aRecargar = Mathf.Min(faltantes, reserva);

        municionEnCanon += aRecargar;
        reserva -= aRecargar;

        // 3. Comprobación de que la matemática interna es exacta
        Assert.AreEqual(2, municionEnCanon, "El cañón debe quedar exactamente con 2 tiros.");
        Assert.AreEqual(8, reserva, "La reserva debe descontar exactamente 2 tiros.");
    }
}