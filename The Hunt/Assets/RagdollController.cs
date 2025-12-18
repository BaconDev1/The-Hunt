using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody[] RagdollBodies;
    public Collider[] RagdollColliders;

    void Awake()
    {
        SetRagdoll(false);
    }

    public void SetRagdoll(bool enabled)
    {
        // Animator OFF when ragdoll ON
        Animator.enabled = !enabled;

        foreach (Rigidbody rb in RagdollBodies)
        {
            rb.isKinematic = !enabled;
            rb.interpolation = enabled
                ? RigidbodyInterpolation.Interpolate
                : RigidbodyInterpolation.None;

            rb.collisionDetectionMode = enabled
                ? CollisionDetectionMode.Continuous
                : CollisionDetectionMode.Discrete;
        }

        foreach (Collider col in RagdollColliders)
        {
            col.enabled = enabled;
        }
    }
}