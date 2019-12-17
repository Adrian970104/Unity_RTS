using UnityEngine;

public class TankAnimationBehaviour : MonoBehaviour
{
    private ObjectBehaviour _target;

    void Update()
    {
        _target = GetComponentInParent<UnitBehaviour>().GetTarget();
        if(_target == null)
        {
            transform.forward = transform.parent.transform.parent.transform.forward;
        } else
        {
            Vector3 f = _target.gameObject.transform.position - transform.position;
            transform.forward = f.normalized;
        }
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
