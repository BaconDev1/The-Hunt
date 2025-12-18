using UnityEngine;


public class BodyDragInteractable : MonoBehaviour, IInteractable
{
    public Rigidbody DragAnchor;

    public void Interact(PlayerInteractor interactor)
    {
        BodyDrag dragger = interactor.GetComponentInParent<BodyDrag>();
        if (dragger == null || DragAnchor == null) return;

        dragger.StartDragging(DragAnchor);
    }
}
