using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickOutline;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour
{
    [Range(0f, 6f)]
    public float DefaultOutlineWidth;
    public GameObject MineralPrefab;
    public GameObject MinePrefab;
    public bool Built { get; private set; }
    public bool HasMineral { get; private set; }
    
    private GameObject _buildingPrefab;
    private MeshRenderer _renderer;
    private MouseController _mc;
    private Outline _outline;
    private Color _clearColor = Color.clear;
    private Color _activeClearColor = new Color(1, 1, 1, 0.05f);
    private bool _mouseOver = false;

    private void Start()
    {
        // 7.5% esély
        HasMineral = Random.value < .075f;
        PlaceMineral();

        _renderer = GetComponent<MeshRenderer>();
        SetMeshColor(_clearColor);

        _mc = GameObject.Find("Camera").GetComponent<MouseController>();
    }

    private void FixedUpdate()
    {
        // TODO: race condition megszüntetése a színezés esetén
        if(_mouseOver)
        {
            return;
        }

        if(_mc.GetBuildingToBuild() == null)
        {
            _clearColor = Color.clear;
        }
        else
        {
            _clearColor = _activeClearColor;
        }
        SetMeshColor(_clearColor);
    }

    private void PlaceMineral()
    {
        if (! HasMineral || transform.childCount > 0)
        {
            return;
        }
        
        Instantiate(MineralPrefab, transform.position, Quaternion.identity, transform);
    }

    private bool CanBuild(GameObject buildingPrefabToBuild)
    {
        if (Built)
        {
            return false;
        }

        if (HasMineral)
        {
            if (buildingPrefabToBuild != MinePrefab)
            {
                return false;
            }
        }
        else
        {
            if (buildingPrefabToBuild == MinePrefab)
            {
                return false;
            }
        }

        return true;
    }

    public bool Build(GameObject buildingPrefabToBuild, Player owner)
    {
        if (CanBuild(buildingPrefabToBuild))
        {
            if (HasMineral) 
            {
                transform.Find("Crystalsv09(Clone)").gameObject.SetActive(false);
            }

            _buildingPrefab = Instantiate(buildingPrefabToBuild, transform.position, Quaternion.identity, transform);
            _buildingPrefab.GetComponent<ObjectBehaviour>().SetOwner(owner);
            Built = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset()
    {
        Built = false;
        _buildingPrefab = null;
        if (HasMineral)
        {
            transform.Find("Crystalsv09(Clone)").gameObject.SetActive(true);
        }
    }

    public Player GetOwner()
    {
        if(!Built)
        {
            return null;
        }

        return _buildingPrefab.GetComponent<ObjectBehaviour>().Owner;
    }

    private void SetMeshColor(Color color)
    {
        _renderer.material.color = color;
    }

    public void CellSelectionStart()
    {
        if (_buildingPrefab == null)
        {
            return;
        }

        _buildingPrefab.GetComponent<Outline>().OutlineWidth = DefaultOutlineWidth;
    }

    public void CellSelectionEnd()
    {
        if (_buildingPrefab == null)
        {
            return;
        }

        _buildingPrefab.GetComponent<Outline>().OutlineWidth = 0;
    }

    public void OnMouseEnter()
    {
        _mouseOver = true;
        GameObject building = _mc.GetBuildingToBuild();
        if (building == null)
        {
            SetMeshColor(_clearColor);
            return;
        }

        if (CanBuild(building))
        {
            SetMeshColor(Color.green);
        }
        else
        {
            SetMeshColor(Color.red);
        }
    }

    public void OnMouseExit()
    {
        _mouseOver = false;
        SetMeshColor(_clearColor);
    }
}
