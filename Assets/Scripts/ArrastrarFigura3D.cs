using UnityEngine;
using UnityEngine.InputSystem;
public class ArrastrarFigura3D : MonoBehaviour
{
    private Camera camara;
    private Rigidbody rb;
    private Animator animator;

    private Plane plano;
    private Vector3 desplazamiento;
    private bool arrastrando;
    private bool tactil;

    void Start()
    {
        // Buscamos la cámara y los componentes del objeto.
        camara = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si no tenemos cogido el objeto, esperamos un clic o un toque.
        if (!arrastrando)
        {
            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                Coger(Touchscreen.current.primaryTouch.position.ReadValue(), true);
            else if (Mouse.current != null &&
                     Mouse.current.leftButton.wasPressedThisFrame)
                Coger(Mouse.current.position.ReadValue(), false);

            return;
        }

        // Si lo tenemos cogido, seguimos el dedo o el ratón.
        if (tactil)
        {
            if (Touchscreen.current == null ||
                !Touchscreen.current.primaryTouch.press.isPressed)
                Soltar();
            else
                Mover(Touchscreen.current.primaryTouch.position.ReadValue());
        }
        else
        {
            if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
                Soltar();
            else
                Mover(Mouse.current.position.ReadValue());
        }
    }

    void Coger(Vector2 posicion, bool esTactil)
    {
        // Un rayo invisible indica qué Collider hemos pulsado.
        Ray rayo = camara.ScreenPointToRay(posicion);

        // Solo cogemos ESTE objeto o uno de sus hijos.
        if (!Physics.Raycast(rayo, out RaycastHit hit) ||
            !hit.collider.transform.IsChildOf(transform))
            return;

        arrastrando = true;
        tactil = esTactil;
        
        // Plano horizontal para arrastrar sin cambiar la altura.
        plano = new Plane(Vector3.up, transform.position);
        plano.Raycast(rayo, out float distancia);

        // Guardamos la separación entre el clic y el centro del objeto.
        desplazamiento = transform.position - rayo.GetPoint(distancia);

        // Paramos las físicas mientras sujetamos el objeto.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        
    }

    void Mover(Vector2 posicion)
    {
        // Calculamos dónde toca el rayo el plano horizontal.
        Ray rayo = camara.ScreenPointToRay(posicion);

        if (plano.Raycast(rayo, out float distancia))
            transform.position = rayo.GetPoint(distancia) + desplazamiento;
    }

    void Soltar()
    {
        arrastrando = false;

        // El objeto vuelve a responder a la gravedad y las colisiones.
        rb.isKinematic = false;
        
    }
}