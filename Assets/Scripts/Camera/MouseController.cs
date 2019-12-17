using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

    #region Publikus változók

    public Color Color; // A kijelölés színe alpha chanellel

    #endregion

    #region Privát változók

    private bool _draw = false; // Legyen rajzolva a gui vagy ne
    private Vector3 _start; // Kijelőlés kezdő poziciója screenspaceben
    private Vector3 _end; // Kijelölés végső poziciója screenspaceben
    private Rect _selection;
    private Camera _camera;
    private Grid _grid;
    private List<GameObject> _selectedUnits;
    private GameObject _selectedCell;
    private GameObject _selectedBuilding;
    private BuildingScriptableObject _buildingToBuild;
    private UiController _uiController;
    private Game _game;
    private bool _removeDeadUnits = false;

    #endregion

    #region Utils

    private Vector3 AveragePositionOfSelectedUnits()
    {
        Vector3 avg = Vector3.zero;
        foreach (GameObject go in _selectedUnits)
        {
            avg += go.transform.position;
        }
        avg.x = avg.x / _selectedUnits.Count;
        avg.y = avg.y / _selectedUnits.Count;
        avg.z = avg.z / _selectedUnits.Count;

        return avg;
    }

    /**
     * A bal alsó sarokból átkerül a bal felső sarokba a (0,0)
     */
    private Vector3 GetTransformedMousePosition ()
    {
        return FlipScreenPosY(Input.mousePosition);
    }

    private Vector3 FlipScreenPosY(Vector3 s)
    {
        s.y = Screen.height - s.y;
        return s;
    }

    private void RefreshSelectedUnits()
    {

        GameObject[] allUnitObjects = GameObject.FindGameObjectsWithTag("Unit");

        PurgeSelection();

        foreach (var obj in allUnitObjects)
        {
            ObjectBehaviour objectBehaviour = obj.GetComponent<ObjectBehaviour>();
            if (_selection.Contains(FlipScreenPosY(_camera.WorldToScreenPoint(obj.transform.position))) && objectBehaviour.Owner.GetPlayerId() == _game.GetLocalPlayer().GetPlayerId())
            {
                _selectedUnits.Add(obj);
            }
        }
    }

    private void SendMessageToSelectedUnits (string message)
    {
        foreach (var obj in _selectedUnits)
        {
            if(obj != null)
            {
                obj.SendMessage(message);
            }
        }
    }

    private void SendMessageToSelectedUnits (string message, object param)
    {
        foreach (var obj in _selectedUnits)
        {
            if(obj != null)
            {
                obj.SendMessage(message, param);
            }
        }
    }


    private void SendMessageToSelectedBuilding (string message)
    {
        if (_selectedBuilding == null)
        {
            return;
        }
        _selectedBuilding.SendMessage(message);
    }

    private void SendMessageToSelectedBuilding (string message, object param)
    {
        if (_selectedBuilding == null)
        {
            return;
        }
        _selectedBuilding.SendMessage(message, param);
    }

    public void RemoveDeadUnits()
    {
        _selectedUnits.RemoveAll(obj => obj == null);
        _removeDeadUnits = false;
    }

    public void NeedsToRemoveDeadUnits()
    {
        _removeDeadUnits = true;
    }
    #endregion

    #region Main

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _selectedUnits = new List<GameObject>();

        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        GameObject ui = GameObject.FindGameObjectWithTag("Ui_Controller");
        _uiController = ui.GetComponent<UiController>();
        _game = GameObject.Find("Game").GetComponent<Game>();
    }

    void Update()
    {
        if (_removeDeadUnits)
        {
            RemoveDeadUnits();
        }

        if (_buildingToBuild != null && Input.GetMouseButtonDown(1))
        {
            _buildingToBuild = null;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) && EventSystem.current.IsPointerOverGameObject())
        {
            _draw = false;
            _start = _end;
            return;
        }

        _end = GetTransformedMousePosition();

        if (Input.GetMouseButtonDown(0))
        {
            _draw = true;
            _start = _end;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _draw = false;

            if (Vector3.Distance(_start, _end) > 15f)
            {
                RefreshSelectedUnits();
                UnitSelectionPerformed();
            }
            else
            {
                ActionPerformed(_end);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            SecondaryActionPerformed(_end);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PurgeSelection();
        }
    }

    public BuildingBehaviour GetSelectedBuilding()
    {
        if(_selectedBuilding == null) return null;
        else return _selectedBuilding.GetComponentInChildren<BuildingBehaviour>();
    }

    public void Build(BuildingScriptableObject buildingScriptableObject)
    {
        _buildingToBuild = buildingScriptableObject;
    }

    public GameObject GetBuildingToBuild()
    {
        if (_buildingToBuild == null)
        {
            return null;
        }
        return _buildingToBuild.prefab;
    }

    #endregion

    #region Callback

    private void UnitSelectionPerformed ()
    {
        // Elküldi a UiControllernek a selected tömböt
        _uiController.RefreshRightPanel(_selectedUnits);

        SendMessageToSelectedUnits("OnSelectStart");
    }
    
    private void BuildingSelectionPerformed ()
    {
        // TODO: ha bulding van kijelölve akkor
        // _uiController.RefreshRightPanel(_selectedBulding);
        // hívás kell, ha nem akkor marad a _selectedUnits os hívás

        _selectedBuilding = _selectedCell;
        SendMessageToSelectedBuilding("OnSelectStart");
        _uiController.RefreshRightPanel(_selectedBuilding);
    }

    private void ActionPerformed (Vector3 screenPoint)
    {
        Ray actionPosition = GetActionPosition(screenPoint);
        UpdateSelectedBuilding(actionPosition);

        if (_selectedCell != null)
        {
            Cell cell = _selectedCell.GetComponent<Cell>();

            if (_buildingToBuild != null)
            {
                if (! cell.Built && _buildingToBuild.prefab != null)
                {
                    if (cell.Build(_buildingToBuild.prefab, _game.GetLocalPlayer()))
                    {
                        _buildingToBuild = null;
                    }
                    else
                    {
                        // nem épült meg
                    }
                }
            }
            else
            {
                if (cell.Built)
                {
                    PurgeSelection();
                    BuildingSelectionPerformed();
                }
            }
            
           PurgeSelectedCell();
        }
        else
        {
            UnitHit(actionPosition);
        }
    }

    private void UnitHit(Ray actionPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(actionPosition, out hit, Mathf.Infinity))
        {
            if(hit.transform.tag != "Unit")
            {
                return;
            }

            bool contains = false;
            foreach (var obj in _selectedUnits)
            {
                if (GameObject.ReferenceEquals(obj, hit.transform.gameObject))
                {
                    contains = true;
                    break;
                }
            }

            if(!contains)
            {
                _selectedUnits.Add(hit.transform.gameObject);
                UnitSelectionPerformed();
            }
        }
    }

    

    private void SecondaryActionPerformed(Vector3 screenPoint)
    {
        Ray actionPosition = GetActionPosition(screenPoint);
        object[] param = new object[2];
        param[0] = actionPosition;
        param[1] = AveragePositionOfSelectedUnits();
        SendMessageToSelectedUnits("MoveToPositionByRay", param);
    }

    private void UpdateSelectedBuilding(Ray actionPosition)
    {
        GameObject selectedCell = _grid.SelectCellByRay(actionPosition);

        if (selectedCell == null)
        {
            return;
        }

        Cell cell = selectedCell.GetComponent<Cell>();
        if (!cell.Built || cell.GetOwner().GetPlayerId() == _game.GetLocalPlayer().GetPlayerId())
        {
            _selectedCell = selectedCell;
        }
    }

    private Ray GetActionPosition(Vector3 screenPoint)
    {
        return _camera.ScreenPointToRay(FlipScreenPosY(screenPoint));
    }

    [Command("deselect-units", "Unit Kijelölés törlése")]
    public void PurgeSelection()
    {  
        SendMessageToSelectedUnits("OnSelectEnd");
        SendMessageToSelectedBuilding("OnSelectEnd");
        _selectedUnits.Clear();
        _selectedBuilding = null;

        _uiController.RefreshRightPanel(_selectedUnits);
    }

    private void PurgeSelectedCell()
    {
        _selectedCell = null;
    }

    #endregion

    #region GUI

    private void OnGUI()
    {
        if (! _draw) return;

        _selection = new Rect(Mathf.Min(_start.x, _end.x), Mathf.Min(_start.y, _end.y), Mathf.Abs(_end.x - _start.x), Mathf.Abs(_end.y - _start.y));

        DrawQuad(_selection, Color);
        
        #if (UNITY_EDITOR)
        
        DrawAllUnitInScreenSpace();
        
        #endif
    }

    private void DrawQuad(Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.skin.box.border = new RectOffset(0, 0, 0, 0);
        GUI.Box(position, GUIContent.none);
    }

    #endregion
    
    #region DEBUG

    private void DrawAllUnitInScreenSpace()
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Unit");


        foreach (var obj in allObjects)
        {
            Vector2 unitPosition = FlipScreenPosY(_camera.WorldToScreenPoint(obj.transform.position));
            DrawQuad(new Rect(unitPosition.x, unitPosition.y, 1f, 1f), Color.magenta);
        }
    }
    
    #endregion
}