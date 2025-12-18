using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteractor interactor);
}

public class PlayerInteractor : MonoBehaviour
{
    public float InteractRange = 2f;
    public LayerMask InteractableMask;
    public KeyCode InteractKey = KeyCode.E;

    public Camera PlayerCamera;

    private IInteractable currentInteractable;
    private BodyDrag bodyDrag;

    void Awake()
    {
        bodyDrag = GetComponent<BodyDrag>();
    }

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(InteractKey))
        {
            if (bodyDrag != null && bodyDrag.isDragging)
            {
                bodyDrag.StopDragging(); // Drop body even if not looking
            }
            else if (currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }
    }

    void CheckForInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, InteractRange, InteractableMask))
        {
            currentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}