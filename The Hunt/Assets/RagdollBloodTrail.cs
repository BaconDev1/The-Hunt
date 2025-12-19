using UnityEngine;

public class RagdollBloodTrail : MonoBehaviour
{
    [Header("References")]
    public Rigidbody RootRB;

    [Header("Blood Decal")]
    public GameObject BloodDecalPrefab;

    [Header("Spawn Settings")]
    public float DistanceThreshold = 0.25f;
    public float GroundCheckDistance = 1.5f;
    public float HeightOffset = 0.02f;

    Vector3 lastSpawnPosition;
    bool hasStarted;

    void FixedUpdate()
    {
        if (RootRB == null || BloodDecalPrefab == null)
            return;

        // Only spawn if moving
        if (RootRB.linearVelocity.sqrMagnitude < 0.05f)
            return;

        Vector3 flatPos = new Vector3(
            RootRB.position.x,
            0f,
            RootRB.position.z
        );

        if (!hasStarted)
        {
            lastSpawnPosition = flatPos;
            hasStarted = true;
            return;
        }

        float dist = Vector3.Distance(flatPos, lastSpawnPosition);
        if (dist < DistanceThreshold)
            return;

        TrySpawnDecal();
        lastSpawnPosition = flatPos;
    }

    void TrySpawnDecal()
    {
        if (!Physics.Raycast(
                RootRB.position + Vector3.up,
                Vector3.down,
                out RaycastHit hit,
                GroundCheckDistance))
            return;

        // Project movement direction onto ground
        Vector3 moveDir = RootRB.linearVelocity;
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude < 0.01f)
            return;

        Quaternion rotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(moveDir.normalized, hit.normal),
            hit.normal
        );

        Vector3 spawnPos = hit.point + hit.normal * HeightOffset;

        Instantiate(BloodDecalPrefab, spawnPos, rotation);
    }
}