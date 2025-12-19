using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float Damage = 25f;
    public float BulletRange = 100f;

    [Header("Blood Effects (Human Only)")]
    public GameObject BloodHitParticle;
    public GameObject BloodDecalPrefab;
    public float ParticleLifetime = 3f;
    public float DecalLifetime = 30f;
    public float DecalSurfaceOffset = 0.01f;

    Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    public void Shoot()
    {
        // 🔥 Panic on gunshot (hit OR miss)
        foreach (Human h in Human.AllHumans)
        {
            h.TriggerGlobalPanic();
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, BulletRange))
            return;

        // Only proceed if we hit a HUMAN (ragdoll-safe)
        Human human = hit.collider.GetComponentInParent<Human>();
        if (human == null)
            return;

        Transform hitTransform = hit.collider.transform;

        // =====================
        // 🩸 BLOOD PARTICLES
        // =====================
        if (BloodHitParticle != null)
        {
            GameObject particle = Instantiate(
                BloodHitParticle,
                hit.point,
                Quaternion.LookRotation(hit.normal),
                hitTransform // stick to limb
            );

            Destroy(particle, ParticleLifetime);
        }

        // =====================
        // 🩸 BLOOD DECAL
        // =====================
        if (BloodDecalPrefab != null)
        {
            Vector3 localPos = hitTransform.InverseTransformPoint(
                hit.point + hit.normal * DecalSurfaceOffset
            );

            Quaternion localRot = Quaternion.LookRotation(-hit.normal);

            GameObject decal = Instantiate(
                BloodDecalPrefab,
                hitTransform
            );

            decal.transform.localPosition = localPos;
            decal.transform.localRotation = localRot;

            Destroy(decal, DecalLifetime);
        }

        // =====================
        // 💀 DAMAGE
        // =====================
        human.TakeDamage(Damage);
    }
}
