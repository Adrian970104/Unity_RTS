using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.Events;

public class UnitBehaviour : ObjectBehaviour, IMovable
{
    /* Ebbe az osztályba kerülnek azok a viselkedések, amelyek csak az egységekre vonatkoznak (pl deal damage, mozgás, pathfinding) */

    [Tooltip("MoveTo híváskor meghívódó custom callbackek.")]
    public UnityEvent OnMoveTo;

    public GameObject ProjectilePrefab;

    //private Vector3 _dest;

    private NavMeshAgent _agent;
    private UnitScriptableObject _unitAttributes;
    private ObjectScriptableObject _commonAttributes;

    // Ezt a unitot lövi. Null, ha nincs target. Automatikusan frissül, trigger colliderekkel működik.
    [SerializeField]
    private ObjectBehaviour _target;
    private float _nextAttackTime = 0f;

    private AudioSource _audioSource;
    public override void Start()
    {
         base.Start();
        _audioSource = GetComponent<AudioSource>();
        _unitAttributes = (UnitScriptableObject)Attributes;
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.Log("Unit prefabon nem található NavMeshAgent komponens!");
        }
        else
        {
            if(_unitAttributes != null)
            {
                //GetComponentInChildren<UnitSelectedParticleBehaviour>().SetRingRadius(_agent.radius);
                _agent.speed = _unitAttributes.speed;
                _agent.angularSpeed = _unitAttributes.angularSpeed;
                var sc = transform.Find("DealDamageRange").GetComponent<SphereCollider>();
                if(sc != null)
                {
                    sc.radius = _unitAttributes.range;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {

        if (DrawNameInScene && Attributes != null)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(Attributes.ToString());
            if(Owner != null)
            {
                sb.Append("\nOwnerID: ");
                sb.Append(Owner.GetPlayerId());
            }
            Handles.Label(transform.position,  sb.ToString() );
        }
        
        //Handles.DrawSphere(0, _agent.destination, Quaternion.identity, 1);
        //Handles.DrawCube(0, _dest, Quaternion.identity, 1);
    }

    public override void Die()
    {
        GameObject.Find("Camera").GetComponent<MouseController>().NeedsToRemoveDeadUnits();
        base.Die();
    }

    public override void Update()
    {
        base.Update();
        if(_target != null)
        {
            Debug.DrawLine(transform.position, _target.transform.position, Color.red);
            if (Time.time > _nextAttackTime)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {

        _nextAttackTime = Time.time + _unitAttributes.attackSpeed;

        GameObject projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().AddForce((_target.transform.position - transform.position).normalized*100f, ForceMode.Impulse);
        projectile.GetComponent<ProjectileBehaviourScript>().SetOwnerId(Owner.GetPlayerId());

        int damage = _unitAttributes.damage;

        if(_unitAttributes.type == UnitType.RPG && _target is UnitBehaviour _targetUnit)
        {
            if(_targetUnit._unitAttributes.type == UnitType.TANK || _targetUnit._unitAttributes.type == UnitType.AV)
            {
                damage *= 2;
            }
            
        }
        _target.Damage(damage);
        _audioSource.Play();
    }

    public void MoveToPositionByRay(object[] param ) //Ray actionPosition)
    {
        Ray actionPosition = (Ray)param[0];
        Vector3 avgPos = (Vector3)param[1];
        Vector3 differenceFromAvg = transform.position - avgPos;

        if(differenceFromAvg.magnitude > _agent.radius * 5)
        {
            differenceFromAvg.Normalize();
            differenceFromAvg *= (_agent.radius * 5);
        }

        int layerMask = 1 << 8;

        if (Physics.Raycast(actionPosition, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //Debug.DrawRay(actionPosition.origin, actionPosition.direction * hit.distance, Color.yellow);
            MoveTo(hit.point + differenceFromAvg);
        }
        else
        {
            //Debug.DrawRay(actionPosition.origin, actionPosition.direction * 1000, Color.white);
        }
    }

    public void MoveTo(Vector3 targetPos)
    {
        //_dest = targetPos;
        _agent.SetDestination(targetPos);

        OnMoveTo.Invoke();
    }

    public void OnTriggerEnter2(ObjectBehaviour ub)
    {
        if(_target == null)
        {
            _target = ub;
            if(_agent != null)
                _agent.ResetPath();
        }
    }

    public void OnTriggerExit2(ObjectBehaviour ub)
    {
        if (ub == _target)
        {
            _target = null;
        }
    }

    public void OnTriggerStay2(ObjectBehaviour ub)
    {
        if (_target == null)
        {
            _target = ub;
        }
    }

    public ObjectBehaviour GetTarget()
    {
        return _target;
    }

}
