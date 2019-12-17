using UnityEngine;

public class AvAnimationBehaviour : MonoBehaviour
{
    [SerializeField]
    private ObjectBehaviour _target;
    private UnitBehaviour _parent;

    void Update()
    {
        _parent = GetComponentInParent<UnitBehaviour>();
        _target = _parent.GetTarget();
        if (_target == null)
        {
            transform.forward = _parent.transform.forward;
        }
        else
        {
            Vector3 f = _target.gameObject.transform.position - transform.position;
            transform.forward = f.normalized;
        }
        transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y + 90, 0);
    }
}
