using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Publikus változók
    [Tooltip("Az alap cella prefabja.")]
    public GameObject CellPrefab;
    
    [Tooltip("Csak a terrain és víz layer kell, hogy benne maradjon, mert annak a magassága alapján kerülnek le a grid cellek.")]
    public LayerMask GroundLayerMask;
    
    [Tooltip("Csak a water plane layer maskja kell, hogy erre ne kerüljön cell (mit kell ignoreolni).")]
    public LayerMask WaterLayerMask;

    public GameObject DebugPrefab;
    
    #endregion
    
    #region Privát változók

    private Collider _cameraBoundCollider;
    private List<Collider> _gridBlockColliders;
    private Vector3 _closestPointToCameraBoundCollider;
    private List<GameObject> _cells;
    
    #endregion
    
    #region Main
    
    private void Awake()
    {
        _gridBlockColliders = new List<Collider>();
        GameObject cameraBound = GameObject.FindGameObjectWithTag("CameraBound");
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("GridBlock");
        foreach (var block in blocks)
        {
            //Debug.Log("blocker found");
            _gridBlockColliders.Add(block.GetComponent<Collider>());
        }
        _cameraBoundCollider = cameraBound.GetComponent<Collider>();
        
        Generate();
    }
    
    #endregion

    #region Generálás

    private void Generate()
    {
        _cells = new List<GameObject>();

        _closestPointToCameraBoundCollider = _cameraBoundCollider.ClosestPointOnBounds(new Vector3(-9999f, -9999f, -9999f));

        int row = 0;
        Vector3 nextCellPosition = _closestPointToCameraBoundCollider;
        
        while (row < Metrics.MaxGridSize)
        {
            nextCellPosition += Metrics.Corners[1];
            
            if (! _cameraBoundCollider.bounds.Contains(nextCellPosition))
            {
                break;
            }

            int column = 0;
            while (column < Metrics.MaxGridSize)
            {
                nextCellPosition += Metrics.Corners[3];
            
                if (! _cameraBoundCollider.bounds.Contains(nextCellPosition))
                {
                    row++;
                    nextCellPosition.x = _closestPointToCameraBoundCollider.x;
                    break;
                }

                if (! InBlock(nextCellPosition))
                {
                    CreateCell(nextCellPosition);
                }
                    
                column++;
            }
        }
    }

    private bool InBlock(Vector3 position)
    {
        foreach(var collider in _gridBlockColliders)
        {
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }

    private void CreateCell(Vector3 position)
    {
        position.y = 999f;
        RaycastHit[] hits = new RaycastHit[4];
        //Debug.Log("CELL:");
        for(int i = 0; i < 4; i++)
        {
            if(Physics.Raycast(position + Metrics.Corners[i], Vector3.down, out hits[i], Mathf.Infinity, GroundLayerMask))
            {
                //Debug.Log(hits[i].point + ", " + hits[i].transform.gameObject.layer);
                float slope = Vector3.Angle(hits[i].normal, Vector3.up);
                if (slope > 0.2f || WaterLayerMask.value == 1 << hits[i].transform.gameObject.layer)
                {
                    return;
                }

                if (LayerMask.NameToLayer("Default") == hits[i].transform.gameObject.layer) return;
            } else
            {
                return;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(position + Metrics.Corners[2]*0.5f, Vector3.down, out hit, Mathf.Infinity, GroundLayerMask))
        {
            //Debug.Log("Center: " + hit.point + ", " + hit.transform.gameObject.layer);
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > 0.2f || WaterLayerMask.value == 1<<hit.transform.gameObject.layer)
            {
                return;
            }
            
        } else
        {
            return;
        }

        float maxDiff = 0f;
        for(int i = 0; i < 4; i++)
        {
            if(Mathf.Abs(hit.point.y - hits[i].point.y) > maxDiff) {
                maxDiff = Mathf.Abs(hit.point.y - hits[i].point.y);
            }
        }
        //Debug.Log(maxDiff);
        if(maxDiff > 0.05f)
        {
            return;
        }

        position.y = hit.point.y + Metrics.GridCellTerrainGap;

        GameObject cell = Instantiate(CellPrefab, position + Metrics.Corners[2] * 0.5f, Quaternion.identity, transform);

        _cells.Add(cell);
    }

    #endregion

    #region Detektálás

    public GameObject SelectCellByRay(Ray ray)
    {
        LayerMask mask = LayerMask.GetMask("GridCell");
        
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            // így mondjuk lehet, hogy tök felesleges a _cells lista
            return hit.transform.gameObject;
        }

        return null;
    }

    public GameObject GetNearestCell(Vector3 point, bool noMineral = false)
    {
        GameObject nearestCell = null;
        float nearestDistance = 99999f;
        foreach(GameObject cell in _cells)
        {
            Cell cellScript = cell.GetComponent<Cell>();
            if(cellScript.Built || (noMineral && cellScript.HasMineral))
            {
                continue;
            }

            float distance = Vector3.Distance(cell.transform.position, point);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCell = cell;
            }
        }

        return nearestCell;
    }

    #endregion
}
