using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{

    private List<GameObject> _selectedUnits;
    private GameObject _selectedBuilding;
    private SelectedPanelController _spc;

    public void Start()
    {
        _selectedUnits = new List<GameObject>();
        _spc = GetComponentInChildren<SelectedPanelController>();
    }

    public void RefreshRightPanel(List<GameObject> list)
    {
        _selectedUnits = list;
        _spc.RefreshPanel(list);
        
    }

    public void RefreshRightPanel(GameObject building)
    {
         _spc.RefreshPanel(building); 
    }

    public void RefreshPlayer(ResourceManager rm)
    {
        BuilderButton[] bb = GetComponentsInChildren<BuilderButton>();
        foreach (var button in bb)
        {
            button.SetPlayerResources(rm);
        }
        MineralDisplay md = GetComponentInChildren<MineralDisplay>();
        if(md != null)
        {
            md.SetPlayer(rm);
        }



    }

}

