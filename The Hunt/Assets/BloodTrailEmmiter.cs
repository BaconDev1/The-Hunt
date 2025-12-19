using UnityEngine;

public class BloodTrailEmitter : MonoBehaviour
{
    public GameObject BloodDecalPrefab;
    public float SpawnInterval = 0.35f;
    public float GroundRayHeight = 0.5f;
    public float MinMoveDistance = 0.15f;

    bool emitting;
    float timer;
    Vector3 lastSpawnPos;

    Rigidbody hipsRB;

    void Awake()
    {
        hipsRB = GetComponentInChildren<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!emitting || hipsRB == null || BloodDecalPrefab == null)
            return;

        timer += Time.fixedDeltaTime;
        if (timer < SpawnInterval)
            return;

        Vector3 groundCheckPos = hipsRB.position + Vector3.up * GroundRayHeight;

        if (Vector3.Distance(groundCheckPos, lastSpawnPos) < MinMoveDistance)
            return;

        if (Physics.Raycast(groundCheckPos, Vector3.down, out RaycastHit hit, 2f))
        {
            SpawnDecal(hit);
            lastSpawnPos = groundCheckPos;
            timer = 0f;
        }
    }

    void SpawnDecal(RaycastHit hit)
    {
        Quaternion rot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
        Vector3 pos = hit.point + hit.normal * 0.01f;

        GameObject decal = Instantiate(BloodDecalPrefab, pos, rot);

        float scale = Random.Range(0.15f, 0.3f);
        decal.transform.localScale = Vector3.one * scale;
    }

    // =====================
    // EXTERNAL CONTROL
    // =====================
    public void StartEmitting()
    {
        lastSpawnPos = Vector3.zero;
        timer = 0f;
        emitting = true;
    }

    public void StopEmitting()
    {
        emitting = false;
    }
}