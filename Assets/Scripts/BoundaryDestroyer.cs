using UnityEngine;

public class BoundaryDestroyer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {        
        Destroy(other.gameObject);       
    }
}


