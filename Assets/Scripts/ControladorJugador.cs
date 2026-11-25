using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Identificación del Jugador")]
    [Tooltip("ID único del jugador para el guardado JSON")]
    public string jugadorId = "HUASO-001";

    [Header("Movimiento")]
    public float speed = 5.0f;
    public float gravity = 20.0f;
    public float velocidadRotacion = 10.0f;

    [Header("Cámara (Estilo Mario 64)")]
    public Transform camaraTransform;
    public float distanciaCamara = 6.0f;
    public float alturaCamara = 3.2f;
    public float suavizadoCamara = 5.0f;
    public float alturaMiraHuaso = 1.2f;

    [Header("Animaciones")]
    public Animator animator;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private AutoAimHuaso scriptAutoAim;
    private VidaHuaso vidaHuaso;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        scriptAutoAim = GetComponent<AutoAimHuaso>();
        vidaHuaso = GetComponent<VidaHuaso>();
    }

    void Start()
    {
        if (camaraTransform == null && Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        StartCoroutine(RutinaRestaurarPartida());
    }

    IEnumerator RutinaRestaurarPartida()
    {
        yield return null;

        string escenaActual = SceneManager.GetActiveScene().name;
        int slot = PlayerPrefs.GetInt("SlotACargar", -1);
        bool posicionRestaurada = false;

        // [2026-11-25][WHITE-BOX] Rama 1: Aqui se verifica si existe el slot de la partida guardada en json
        if (slot != -1 && SistemaGuardadoJSON.ExistePartida(slot))
        {
            DatosPartida datos = SistemaGuardadoJSON.CargarPartida(slot);

            Debug.Log($"[2026-11-25][WHITE-BOX] Rama RestaurarPartida ACTIVA | Slot={slot} | EscenaActual={escenaActual} | EscenaGuardada={datos?.escenaReal}");

            if (datos != null)
            {
                if (!string.IsNullOrEmpty(datos.jugadorId))
                {
                    jugadorId = datos.jugadorId;
                }

                if (datos.escenaReal == escenaActual)
                {
                    MoverJugadorSeguro(new Vector3(datos.posX, datos.posY, datos.posZ));
                    posicionRestaurada = true;
                    Debug.Log($"[2026-11-25][WHITE-BOX] Sub-rama PosicionRestaurada=TRUE en {escenaActual}");
                }

                if (vidaHuaso != null && datos.vidaJugador > 0)
                {
                    vidaHuaso.SetVida(datos.vidaJugador);
                }

                if (ContadorMisiones.Instancia != null && escenaActual == "SampleScene")
                {
                    ContadorMisiones.Instancia.RestaurarProgreso(datos.enemigosRestantes, datos.pistasEncontradas);
                }
            }

            PlayerPrefs.SetInt("SlotACargar", -1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log($"[2026-11-25][WHITE-BOX] Rama RestaurarPartida OMITIDA (sin slot pendiente) | Slot={slot} | Escena={escenaActual}");
        }

        // [2026-11-25][WHITE-BOX] Rama 2: Si la posicion no se restauro en nivel2_espacio, usa spawnpoint_nivel2
        if (!posicionRestaurada && escenaActual == "Nivel2_Espacio")
        {
            GameObject spawnNivel2 = GameObject.Find("SpawnPoint_Nivel2");
            if (spawnNivel2 != null)
            {
                MoverJugadorSeguro(spawnNivel2.transform.position);
                Debug.Log($"[2026-11-25][WHITE-BOX] Rama SpawnPoint_Nivel2 ejecutada en {spawnNivel2.transform.position}");
            }
        }
    }

    private void MoverJugadorSeguro(Vector3 destino)
    {
        if (characterController != null) characterController.enabled = false;
        transform.position = destino;
        if (characterController != null) characterController.enabled = true;
    }

    public void GuardarPartidaActual(int slot)
    {
        DatosPartida datos = new DatosPartida();

        datos.jugadorId = jugadorId;
        datos.numeroSlot = slot;
        datos.nombreVisible = SceneManager.GetActiveScene().name == "SampleScene" ? "Nivel 1 - Campo" : "Nivel 2 - Nave Espacial";
        datos.escenaReal = SceneManager.GetActiveScene().name;

        datos.posX = transform.position.x;
        datos.posY = transform.position.y;
        datos.posZ = transform.position.z;
        datos.vidaJugador = vidaHuaso != null ? vidaHuaso.vidaActual : 100f;

        if (ContadorMisiones.Instancia != null)
        {
            datos.enemigosRestantes = ContadorMisiones.Instancia.enemigosRestantes;
            datos.pistasEncontradas = ContadorMisiones.Instancia.pistasEncontradas;
            datos.misionAliensCompletada = (ContadorMisiones.Instancia.enemigosRestantes <= 0);
            datos.pistasCompletadas = (ContadorMisiones.Instancia.pistasEncontradas >= 5);
        }

        SistemaGuardadoJSON.GuardarPartida(slot, jugadorId, datos);
        Debug.Log($"[2026-11-25][WHITE-BOX] GuardarPartidaActual ejecutado | Slot={slot} | Escena={datos.escenaReal} | Vida={datos.vidaJugador}");
    }

    void Update()
    {
        if (vidaHuaso != null && vidaHuaso.EstaMuerto())
        {
            if (animator != null) animator.speed = 1.0f;
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDireccion = new Vector3(moveX, 0.0f, moveZ).normalized;

        if (inputDireccion.magnitude >= 0.1f)
        {
            moveDirection.x = inputDireccion.x * speed;
            moveDirection.z = inputDireccion.z * speed;

            if (scriptAutoAim == null || !scriptAutoAim.TieneObjetivoActivo())
            {
                float targetAngle = Mathf.Atan2(inputDireccion.x, inputDireccion.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadRotacion * Time.deltaTime);
            }

            if (animator != null)
            {
                animator.SetBool("estaCaminando", true);
                animator.speed = 1.0f;
            }
        }
        else
        {
            moveDirection.x = 0f;
            moveDirection.z = 0f;

            if (animator != null)
            {
                animator.SetBool("estaCaminando", false);
                animator.speed = 0.0f;
            }
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -2f;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (camaraTransform != null)
        {
            Vector3 posicionDeseada = transform.position - (Vector3.forward * distanciaCamara) + (Vector3.up * alturaCamara);
            camaraTransform.position = Vector3.Lerp(camaraTransform.position, posicionDeseada, suavizadoCamara * Time.deltaTime);

            Vector3 puntoMira = transform.position + (Vector3.up * alturaMiraHuaso);
            Quaternion rotacionDeseada = Quaternion.LookRotation(puntoMira - camaraTransform.position);
            camaraTransform.rotation = Quaternion.Slerp(camaraTransform.rotation, rotacionDeseada, suavizadoCamara * Time.deltaTime);
        }
    }
}