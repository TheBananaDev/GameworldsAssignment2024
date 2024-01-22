using System;
using UnityEngine;

public class GroundColliderCheck : MonoBehaviour
{
    public bool isGrounded { get; private set; }
    public Collider groundCollider { get; private set; }

    public event Action onGroundHit;

    private void Start()
    {
        groundCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        onGroundHit?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other);
        isGrounded = true;
        groundCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
        groundCollider = null;
    }
}

