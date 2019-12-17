using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildingBehaviour : ObjectBehaviour
{
    private static readonly int mineIncome = 1;

    /* Ebbe az osztályba kerülnek azok a viselkedések, amelyek csak az épületre vonatkoznak (pl build queue, gyártás) */
    public bool DrawBuildQueueInScene = true;

    private List<UnitScriptableObject> _buildQueue;
  
    [SerializeField]
    private BuildingScriptableObject _buildingAttributes;

    private ObjectScriptableObject _commonAttributes;
    [SerializeField]
    private float _buildTimer;
    [SerializeField]
    private readonly float _buildTime = 5;
    [SerializeField]
    private bool _currentlyBuilding;

    private float _nextActionTime = 0.0f;

    public override void Start()
    {
        base.Start();
        _buildingAttributes = (BuildingScriptableObject)Attributes;
        _buildQueue = new List<UnitScriptableObject>();
        //_healthBarCanvas.enabled = false;

    }

    public override void FixedUpdate()
    {

        base.FixedUpdate();

        if (this._buildQueue.Count > 0 && !this._currentlyBuilding)
        {
            StartTimer();
        }

        if (this._currentlyBuilding)
        {
            _buildTimer -= Time.fixedDeltaTime;

            if(this._buildTimer <= 0)
            {
                CreateUnit();
            }
        }
    }

    public override void Die()
    {
        transform.parent.GetComponent<Cell>().Reset();
        base.Die();
    }

    public override void Update()
    {
        base.Update();

        _nextActionTime += Time.deltaTime;
        if (_nextActionTime >= 1f)
        {
            _nextActionTime = _nextActionTime % 1f;
            if (_buildingAttributes.type == BuildingType.MINE)
            {
                Owner.GetComponent<ResourceManager>().AddMineral(mineIncome);
            }
        }
        
        

    }

    // Igaz, ha a unit építhető ebből az épületből és van hely a buildQueue-ban
    public bool CanBuildUnit(UnitScriptableObject t)
    {
        return this._buildQueue.Count < 8 && t.builtFrom == _buildingAttributes.type;
    }

    public bool AddToQueue(UnitScriptableObject t)
    {

        if(CanBuildUnit(t))
        {
            this._buildQueue.Add(t);
            return true;
        } else
        {
            return false;
        }
    }

    private void CreateUnit()
    {
        string path = "Prefabs/Units/Variants/";
        path += _buildQueue[0].name.ToUpper();
        GameObject instantiated = Instantiate(Resources.Load<GameObject>(path),gameObject.transform.position + Vector3.forward* 3,Quaternion.identity);
        instantiated.GetComponent<ObjectBehaviour>().SetOwner(Owner);
        _buildQueue.RemoveAt(0);
        _currentlyBuilding = false;
        _buildTimer = 0;
    }

    private void StartTimer()
    {
        this._buildTimer = _buildTime;
        this._currentlyBuilding = true;
    }

    /*private void OnDrawGizmos()
    {
        if (Attributes == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(Attributes.ToString() + "\ntimer: " + System.Math.Round(_buildTimer,2));
        if(this.DrawBuildQueueInScene && _buildQueue != null)
        {
            sb.Append("\nBuild queue: (" + _buildQueue.Count + ")");
            foreach (UnitScriptableObject t in this._buildQueue)
            {
                sb.Append("\n");
                sb.Append(t.ToString());
            }
            
        }
        sb.Append("\nOwnerID: " + Owner.GetPlayerId());

        if (DrawNameInScene)
            Handles.Label(transform.position, sb.ToString());

    }*/

    public List<UnitScriptableObject> GetQueue()
    {
        return this._buildQueue;
    }

    public float Get_buildTimer()
    {
        return this._buildTimer;
    }

    public float Get_buildTime()
    {
        return this._buildTime;
    }

}
