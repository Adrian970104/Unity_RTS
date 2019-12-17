using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitColliderBehaviour : MonoBehaviour
{
    private UnitBehaviour _parent;

    public void Awake()
    {
        _parent = GetComponentInParent<UnitBehaviour>();
    }

    // A következő 3 függvény csak wrapper függvény, amikre a layeres bohóckodás miatt van szükség.
    // Ha már úgyis kellenek, akkor még itt lecsekkolja, hogy érdemes-e egyáltalán targetelni.
    // Nem érdemes: ha ugyanahoz a unithoz tartoznak, vagy ha két különböző unit/building, de ugyanahoz az Ownerhez tartoznak.

    public void OnTriggerEnter(Collider other)
    {
        ObjectBehaviour otherParent = other.gameObject.GetComponentInParent<ObjectBehaviour>();
        if (otherParent == null) return;
        if(_parent != otherParent && _parent.Owner.GetPlayerId() != otherParent.Owner.GetPlayerId())
        {
            _parent.OnTriggerEnter2(otherParent);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        ObjectBehaviour otherParent = other.gameObject.GetComponentInParent<ObjectBehaviour>();
        if (otherParent == null) return;
        if (_parent != otherParent && _parent.Owner.GetPlayerId() != otherParent.Owner.GetPlayerId())
        {
            _parent.OnTriggerExit2(otherParent);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        
        ObjectBehaviour otherParent = other.gameObject.GetComponentInParent<ObjectBehaviour>();
        if (otherParent == null) return;
        if (_parent != otherParent && _parent.Owner.GetPlayerId() != otherParent.Owner.GetPlayerId())
        {
            _parent.OnTriggerStay2(otherParent);
        }
    }
}
