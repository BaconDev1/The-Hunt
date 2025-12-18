using UnityEngine;

public class BodyDrag : MonoBehaviour
{
    [Header("Carry Settings")]
    public Transform CarryPoint;
    public float FollowForce = 2500f;
    public float MaxDistance = 1.5f;

    [Header("Movement & Look Penalties")]
    [Range(0.05f, 0.4f)]
    public float MoveSpeedMultiplier = 0.2f;
    [Range(0.05f, 0.4f)]
    public float LookSensitivityMultiplier = 0.15f;

    [Header("Blood Trail Settings")]
    public GameObject BloodDecalPrefab;
    public float BloodSpawnInterval = 0.3f; // seconds
    public float BloodHeightOffset = 0.05f; // height above ground

    [Header("References")]
    public PlayerMovement Movement;
    public PlayerLook Look;

    Rigidbody dragAnchor;
    public bool isDragging;
    bool penaltiesApplied;

    float originalWalkSpeed;
    float originalRunSpeed;
    Vector2 originalSensitivity;

    Rigidbody carryRB;

    // Blood trail logic
    float bloodTimer;

    void Awake()
    {
        if (CarryPoint != null)
        {
            carryRB = CarryPoint.GetComponent<Rigidbody>();
            if (carryRB == null)
            {
                carryRB = CarryPoint.gameObject.AddComponent<Rigidbody>();
                carryRB.isKinematic = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDragging || dragAnchor == null || CarryPoint == null)
            return;

        // Drag physics
        Vector3 dir = CarryPoint.position - dragAnchor.position;
        if (dir.magnitude > MaxDistance)
            dragAnchor.position = CarryPoint.position;

        dragAnchor.AddForce(dir * FollowForce * Time.fixedDeltaTime, ForceMode.Acceleration);

        // Blood decal spawning
        bloodTimer += Time.fixedDeltaTime;
        if (bloodTimer >= BloodSpawnInterval)
        {
            SpawnBloodDecal();
            bloodTimer = 0f;
        }
    }

    void SpawnBloodDecal()
    {
        if (BloodDecalPrefab == null || dragAnchor == null)
            return;

        // Raycast to find ground under the body
        if (Physics.Raycast(dragAnchor.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f))
        {
            Vector3 spawnPos = hit.point + Vector3.up * BloodHeightOffset;
            Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Instantiate(BloodDecalPrefab, spawnPos, spawnRot);
        }
    }

    public void StartDragging(Rigidbody anchor)
    {
        if (isDragging || anchor == null)
            return;

        dragAnchor = anchor;
        dragAnchor.WakeUp();

        ApplyCarryPenalties();

        isDragging = true;
        bloodTimer = 0f; // reset timer
    }

    public void StopDragging()
    {
        if (!isDragging)
            return;

        RemoveCarryPenalties();
        dragAnchor = null;
        isDragging = false;
    }

    void ApplyCarryPenalties()
    {
        if (penaltiesApplied || Movement == null || Look == null) return;

        originalWalkSpeed = Movement.walkSpeed;
        originalRunSpeed = Movement.RunSpeed;
        originalSensitivity = Look.Sensitivities;

        Movement.walkSpeed *= MoveSpeedMultiplier;
        Movement.RunSpeed *= MoveSpeedMultiplier;
        Look.Sensitivities *= LookSensitivityMultiplier;

        penaltiesApplied = true;
    }

    void RemoveCarryPenalties()
    {
        if (!penaltiesApplied || Movement == null || Look == null) return;

        Movement.walkSpeed = originalWalkSpeed;
        Movement.RunSpeed = originalRunSpeed;
        Look.Sensitivities = originalSensitivity;

        penaltiesApplied = false;
    }

    void OnDisable()
    {
        if (isDragging)
            RemoveCarryPenalties();
    }
}