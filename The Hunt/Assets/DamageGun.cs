using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float Damage;
    public float BulletRange;
    private Transform PlayerCamera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
      



    }

    public void Shoot()
    {
        Human[] humans = FindObjectsByType<Human>(FindObjectsSortMode.None);
        foreach (Human h in humans)
        {
            h.TriggerGlobalPanic();
        }

        Ray gunRay = new Ray(PlayerCamera.position, PlayerCamera.forward);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, BulletRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Human enemy))
            {
                enemy.TakeDamage(Damage);
            }
        }
    }
    
}
