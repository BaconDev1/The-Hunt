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
    public float BloodSpawnInterval = 0.3f;
    public float BloodHeightOffset = 0.02f;
    public float MinMovementForBlood = 0.1f;   // 🔹 NEW

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

    float bloodTimer;
    Vector3 lastBloodPos;                       // 🔹 NEW
    public BloodTrailEmitter BloodTrail;
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

        // Blood logic
        bloodTimer += Time.fixedDeltaTime;
        if (bloodTimer >= BloodSpawnInterval)
        {
            TrySpawnBlood();
            bloodTimer = 0f;
        }
    }

    // =========================
    // BLOOD LOGIC
    // =========================
    void TrySpawnBlood()
    {
        if (BloodDecalPrefab == null || dragAnchor == null)
            return;

        Vector3 footPos = dragAnchor.position + Vector3.down * 0.4f;

        if (Vector3.Distance(footPos, lastBloodPos) < MinMovementForBlood)
            return;

        lastBloodPos = footPos;

        if (Physics.Raycast(footPos + Vector3.up, Vector3.down, out RaycastHit hit, 1.5f))
        {
            Vector3 spawnPos = hit.point + Vector3.up * BloodHeightOffset;

            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            rot *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            GameObject decal = Instantiate(BloodDecalPrefab, spawnPos, CarryPoint.rotation);

            float scale = Random.Range(0.15f, 0.35f);
            decal.transform.localScale = Vector3.one * scale;
        }
    }

    // =========================
    // DRAG CONTROL
    // =========================
    public void StartDragging(Rigidbody anchor)
    {
        if (isDragging || anchor == null)
            return;

        dragAnchor = anchor;
        dragAnchor.WakeUp();

        lastBloodPos = dragAnchor.position;
        bloodTimer = 0f;

        ApplyCarryPenalties();
        isDragging = true;
    }

    public void StopDragging()
    {
        if (!isDragging)
            return;

        RemoveCarryPenalties();
        dragAnchor = null;
        isDragging = false;
    }

    // =========================
    // PENALTIES
    // =========================
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