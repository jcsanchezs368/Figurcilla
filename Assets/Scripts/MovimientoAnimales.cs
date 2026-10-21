using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoAnimales : MonoBehaviour
{
    // Podemos usar este script en cualquier animal, no solo en el Corgi.
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public bool usarAnimaciones = false;

    private Rigidbody rb;
    private Animator animator;
    private Transform destino;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        // Es necesario asignar los dos puntos en el Inspector.
        if (puntoA == null || puntoB == null)
        {
            enabled = false;
            return;
        }

        // Empezamos en A y vamos hacia B.
        rb.position = puntoA.position;
        destino = puntoB;

        if (usarAnimaciones && animator != null)
            animator.SetTrigger("Run");
    }

    void FixedUpdate()
    {
        if (destino == null) return;

        // Calculamos hacia dónde debe desplazarse.
        Vector3 direccion = destino.position - rb.position;
        direccion.y = 0; // La gravedad sigue funcionando en el eje Y.

        // Al llegar a B vamos a A, y al llegar a A volvemos a B.
        if (direccion.magnitude < 0.15f)
        {
            destino = destino == puntoB ? puntoA : puntoB;
            return;
        }

        direccion.Normalize();

        // Movemos al animal sin modificar su velocidad vertical.
        rb.linearVelocity = new Vector3(
            direccion.x * velocidad,
            rb.linearVelocity.y,
            direccion.z * velocidad
        );

        // Giramos al animal hacia donde está caminando.
        rb.MoveRotation(Quaternion.LookRotation(direccion));
    }
}