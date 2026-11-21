using UnityEngine;
using TMPro;

public class BarraVidaBoss : MonoBehaviour
{
    [Header("Texto de Vida")]
    public TextMeshProUGUI textoBoss;

    private VidaAlien vidaBoss;
    private float vidaMaxima = -1f;
    private const int totalBloques = 20;

    void Start()
    {
        if (textoBoss != null) textoBoss.gameObject.SetActive(false);
    }

    void Update()
    {
        if (vidaBoss == null)
        {
            GameObject bossObj = GameObject.Find("Boss_Alien");
            if (bossObj != null && bossObj.activeInHierarchy)
            {
                vidaBoss = bossObj.GetComponent<VidaAlien>();
                if (vidaBoss != null)
                {
                    vidaMaxima = vidaBoss.vidaActual;
                    if (textoBoss != null) textoBoss.gameObject.SetActive(true);
                }
            }
            return;
        }

        if (textoBoss != null && vidaBoss.gameObject.activeInHierarchy)
        {
            if (!textoBoss.gameObject.activeSelf) textoBoss.gameObject.SetActive(true);

            if (vidaBoss.vidaActual <= 0)
            {
                textoBoss.gameObject.SetActive(false);
                return;
            }

            ActualizarTexto();
        }
        else if (textoBoss != null && !vidaBoss.gameObject.activeInHierarchy)
        {
            textoBoss.gameObject.SetActive(false);
        }
    }

    void ActualizarTexto()
    {
        if (textoBoss == null || vidaBoss == null) return;
        if (vidaMaxima <= 0) vidaMaxima = Mathf.Max(vidaBoss.vidaActual, 100f);

        float ratio = Mathf.Clamp01(vidaBoss.vidaActual / vidaMaxima);
        int bloquesLlenos = Mathf.RoundToInt(ratio * totalBloques);
        int bloquesVacios = totalBloques - bloquesLlenos;

        string barra = new string('█', bloquesLlenos) + new string('░', bloquesVacios);
        textoBoss.text = $"JEFE ALIEN\n[{barra}] {(int)vidaBoss.vidaActual} HP";
    }
}