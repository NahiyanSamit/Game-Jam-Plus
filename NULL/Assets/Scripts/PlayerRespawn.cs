using UnityEngine;
using System.Collections.Generic;

public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -10f;
    public float historyTime = 2f;

    private Rigidbody _rb;
    private readonly Queue<Vector3> _positionHistory = new();
    private float _fixedDelta;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _fixedDelta = Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        _positionHistory.Enqueue(transform.position);
        if (_positionHistory.Count > historyTime / _fixedDelta)
            _positionHistory.Dequeue();

        if (transform.position.y < fallThreshold)
            Respawn();
    }

    void Respawn()
    {
        transform.position = _positionHistory.Count > 0 ? _positionHistory.Peek() : Vector3.up * 2;
        _rb.linearVelocity = Vector3.zero;
    }
}