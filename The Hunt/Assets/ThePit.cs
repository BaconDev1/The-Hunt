using UnityEngine;

public class ThePit : MonoBehaviour
{
    private GameManager gameManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Human>())
        {
            gameManager.CurrentBodies++;
            Destroy(other.gameObject.GetComponentInParent<Human>().gameObject);
        }
    }
}
