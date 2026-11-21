using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VidaAlien : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public float vidaActual = 100f;

    [Header("Efecto de Impacto")]
    public AudioClip sonidoImpacto;
    public float duracionFlash = 0.12f;

    [Header("Drop de Munición")]
    public GameObject prefabMunicion;          
    [Range(0f, 1f)]
    public float probabilidadDrop = 0.5f;       
    public float alturaAparicionDrop = 0.5f;    

    private List<Renderer> renderers = new List<Renderer>();
    private List<Color> coloresOriginales = new List<Color>();
    private bool estaEnFlash = false;
    private bool yaMurio = false;

    void Start()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (r != null && r.material != null && r.material.HasProperty("_Color"))
            {
                renderers.Add(r);
                coloresOriginales.Add(r.material.color);
            }
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (yaMurio) return;

        vidaActual -= cantidad;

        if (sonidoImpacto != null)
        {
            float vol = PlayerPrefs.GetFloat("VolumenSonido", 1f);
            AudioSource.PlayClipAtPoint(sonidoImpacto, transform.position, vol);
        }

        if (vidaActual <= 0f)
        {
            Morir();
        }
        else if (!estaEnFlash && gameObject.activeInHierarchy)
        {
            StartCoroutine(RutinaFlashRojo());
        }
    }

    IEnumerator RutinaFlashRojo()
    {
        estaEnFlash = true;

        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null) renderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(duracionFlash);

        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null) renderers[i].material.color = coloresOriginales[i];
        }

        estaEnFlash = false;
    }

    void Morir()
    {
        yaMurio = true;

        if (prefabMunicion != null)
        {
            if (Random.value <= probabilidadDrop)
            {
                Vector3 posicionDrop = transform.position + (Vector3.up * alturaAparicionDrop);
                Instantiate(prefabMunicion, posicionDrop, Quaternion.identity);
                Debug.Log("¡El alien soltó una caja de munición!");
            }
        }

        if (GestorOleadasNivel2.Instancia != null)
        {
            GestorOleadasNivel2.Instancia.RegistrarBajaAlien();
        }
        else if (ContadorMisiones.Instancia != null)
        {
            ContadorMisiones.Instancia.RegistrarBajaAlien();
        }

        Destroy(gameObject);
    }
}