using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody[] RagdollBodies;

    void Awake()
    {
        SetRagdoll(false);
    }

    public void SetRagdoll(bool enabled)
    {
        Animator.enabled = !enabled;

        foreach (Rigidbody rb in RagdollBodies)
        {
            rb.isKinematic = !enabled;
        }
    }
}