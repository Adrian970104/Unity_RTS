using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviourScript : MonoBehaviour
{
    private int _ownerId = -1;
    private float _lifeTime = 0f;

    void FixedUpdate()
    {
        _lifeTime += Time.deltaTime;

        if(_lifeTime > 5f)
        {
            Destroy(gameObject);
        }
    }

    public void SetOwnerId(int ownerId)
    {
        _ownerId = ownerId;
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectBehaviour otherObject = other.gameObject.GetComponent<ObjectBehaviour>();

        if(otherObject != null && _ownerId != -1 && otherObject.Owner.GetPlayerId() != _ownerId)
        {
            Destroy(gameObject);
        }
    }
}
