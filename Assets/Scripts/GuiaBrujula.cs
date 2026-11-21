using UnityEngine;

public class GuiaBrujula : MonoBehaviour
{
    private Transform naveObjetivo;

    [Header("Configuración del Movimiento")]
    public float velocidadGiro = 3f;      
    public float velocidadFlotacion = 4f; 
    public float amplitudFlotacion = 0.2f;

    private Vector3 posicionLocalInicial;

    void Start()
    {
        posicionLocalInicial = transform.localPosition;

        GameObject nave = GameObject.Find("naveEspacial");

        if (nave != null)
        {
            naveObjetivo = nave.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'naveEspacial' para la brújula.");
        }
    }

    void Update()
    {
        float nuevoY = posicionLocalInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
        transform.localPosition = new Vector3(posicionLocalInicial.x, nuevoY, posicionLocalInicial.z);

        if (naveObjetivo == null) return;

        Vector3 direccionANave = naveObjetivo.position - transform.position;
        direccionANave.y = 0;

        if (direccionANave.sqrMagnitude > 0.1f)
        {
            
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionANave);

          
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
        }
    }
}