using UnityEngine;

public class MovimientoHuaso : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocidadJugador;
    private bool estaEnElSuelo;

    
    [Header("Componentes")]
    public Animator anim;

    [Header("Configuración de Movimiento")]
    public float velocidadCaminar = 5.0f;
    public float gravedad = -9.81f;
    public float velocidadRotacion = 10.0f;

    void Start()
    {
        
        controller = GetComponent<CharacterController>();

        
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
       
        estaEnElSuelo = controller.isGrounded;
        if (estaEnElSuelo && velocidadJugador.y < 0)
        {
            velocidadJugador.y = -2f; 
        }

     
        float moverH = Input.GetAxis("Horizontal"); 
        float moverV = Input.GetAxis("Vertical");   

       
        Vector3 direccion = new Vector3(moverH, 0f, moverV).normalized;

       
        if (direccion.magnitude >= 0.1f)
        {
            
            controller.Move(direccion * velocidadCaminar * Time.deltaTime);

            
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);

            
            if (anim != null)
            {
                anim.SetBool("Walk", true);
            }
        }
        else
        {
          
            if (anim != null)
            {
                anim.SetBool("Walk", false);
            }
        }

        velocidadJugador.y += gravedad * Time.deltaTime;
        controller.Move(velocidadJugador * Time.deltaTime);
    }
}