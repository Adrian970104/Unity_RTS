using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MarineAnimationBehaviour : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private float _movementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _movementSpeed = _agent.velocity.magnitude;
        _animator.SetFloat("MovementSpeed", _movementSpeed);
    }

}
