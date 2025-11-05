using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;


    private void Start()
    {
        rb =  GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }
}
