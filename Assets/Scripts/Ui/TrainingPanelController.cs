using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainingPanelController : MonoBehaviour
{
    private List<UnitScriptableObject> _bbQueue;
    private Image[] _unitIconList = new Image[8];
    private BuildingBehaviour _buildingbehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _unitIconList = gameObject.GetComponentsInChildren<Image>();
    }

    public void SetbuildingBehaviour(BuildingBehaviour bb)
    {
        this._buildingbehaviour = bb;
    }

    public void Update()
    {
        ClearUnitsIcon();
        SetQueue(_buildingbehaviour);
        SetUnitsIcon(this._bbQueue);
        SetUnitIconFill(_buildingbehaviour);
    }

    public void SetQueue(BuildingBehaviour bb)
    {
        try
        {
            _bbQueue = bb.GetQueue();
        }catch(Exception e)
        {
            _bbQueue = new List<UnitScriptableObject>();
        }
    }

    public void SetUnitsIcon(List<UnitScriptableObject> list)
    {
        Sprite iconSprite;
        int i = 1;
        foreach (UnitScriptableObject unit in list)
        {
            iconSprite = unit.uiGraphic;
            this._unitIconList[i].sprite = iconSprite;
            i += 1;
        }
    }

    public void ClearUnitsIcon()
    {
        foreach (Image image in _unitIconList)
        {
            image.sprite = Resources.Load<Sprite>("Textures/UI/TrainingPanelBackground1");
            image.fillAmount = 1f;
        }
    }

    public void SetUnitIconFill(BuildingBehaviour bb)
    {
        if (_bbQueue.Count > 0)
        {
            float f = bb.Get_buildTimer() / bb.Get_buildTime();
            this._unitIconList[1].fillAmount = f;
        }
    }

    public void OffTrainingPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void OnTrainingPanel()
    {
        this.gameObject.SetActive(true);
    }
}