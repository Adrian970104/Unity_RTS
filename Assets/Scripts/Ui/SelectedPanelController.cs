using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedPanelController : MonoBehaviour
{
    //string text;
    private TextMeshProUGUI _listDatas;
    private TextMeshProUGUI _objectName;
    private TextMeshProUGUI _MarinePiece;
    private TextMeshProUGUI _AVPiece;
    private TextMeshProUGUI _TankPiece;
    private TextMeshProUGUI _RPGPiece;
    private TextMeshProUGUI _objectHBText;
    private Image _objectIcon;
    private Image _objectHBIcon;
    private TrainingPanelController _trainingPanelController;
    public GameObject SelectedUnitList;
    public GameObject OneObjectSelected;
    [SerializeField]
    private ObjectBehaviour _selected;

    private void Start()
    {
        // Kijelölt egységek darabszámának nyilvántartására használt vváltozók. Ezek kerülnek kiírásra a megfelelő kép alá, ha több mint 1 egység van kijölve
        _AVPiece = gameObject.transform.Find("SelectedUnitsList").transform.Find("AVPanel").transform.Find("Piece").GetComponent<TextMeshProUGUI>();
        _MarinePiece = gameObject.transform.Find("SelectedUnitsList").transform.Find("MarinePanel").transform.Find("Piece").GetComponent<TextMeshProUGUI>();
        _RPGPiece = gameObject.transform.Find("SelectedUnitsList").transform.Find("RPGPanel").transform.Find("Piece").GetComponent<TextMeshProUGUI>();
        _TankPiece = gameObject.transform.Find("SelectedUnitsList").transform.Find("TankPanel").transform.Find("Piece").GetComponent<TextMeshProUGUI>();
        // Egység nevét tároló változó, egy kijelölt egység esetén használjuk.
        _objectName = gameObject.transform.Find("OneObjectSelected").transform.Find("Name").GetComponent<TextMeshProUGUI>();
        // Egység képét tároló változó, egy kijelölt egység esetén használjuk.
        _objectIcon = gameObject.transform.Find("OneObjectSelected").transform.Find("Image").GetComponent<Image>();
        // Egység százalékos aktuális életerejét tartalmazó változó, egy kijelölt egység esetén használjuk.
        _objectHBIcon = gameObject.transform.Find("OneObjectSelected").transform.Find("HealthBG").transform.Find("HealthBar").GetComponent<Image>();
        // Egy kijelölt objektum aktuális és max életerjét tartalmazó változó, kiíráshoz hasznájuk. Egy kijelölt egység esetén használjuk.
        _objectHBText = gameObject.transform.Find("OneObjectSelected").transform.Find("HealthBG").GetComponentInChildren<TextMeshProUGUI>();
        //
        _trainingPanelController = GetComponentInChildren<TrainingPanelController>();
        // Kezdésként csak a SelectedUnitList-panelt szerenénk látni, ezért mindent másat eltűntetünk
        HidePanel(OneObjectSelected);


    }

    public void Update()
    {
        RefreshObjectHPBar();
        if(_selected == null)
        {
            HidePanel(OneObjectSelected);
            ShowPanel(SelectedUnitList);
        }
    }

    // A panel firssítéséért felel, a beérkező lista alapján. Váltogatja a paneleket, az alapján, hogy egy vagy több egység van kijelölve. Feltölti a panel megfelelő elemeit aktuális értékekkel.
    public void RefreshPanel(List<GameObject> list)
    {
        // _trainingPanelController.OffTrainingPanel();
        if (list.Count == 1)
        {
            _selected = list[0].GetComponent<UnitBehaviour>();
            HidePanel(SelectedUnitList);
            ShowPanel(OneObjectSelected);
            _objectName.SetText(ObjectNameToString(list[0]));
            _objectIcon.sprite = SpriteFromObject(list[0]);
            _trainingPanelController.OffTrainingPanel();
            //RefreshObjectHPBar(list[0]);
        }
        else
        {
            HidePanel(OneObjectSelected);
            ShowPanel(SelectedUnitList);
            _AVPiece.SetText(NumberOfSelectedUnitsToString("AV", list));
            _MarinePiece.SetText(NumberOfSelectedUnitsToString("Marine", list));
            _RPGPiece.SetText(NumberOfSelectedUnitsToString("RPG", list));
            _TankPiece.SetText(NumberOfSelectedUnitsToString("Tank", list));
        }
    }

    public void RefreshPanel(GameObject building)
    {
        _selected = building.GetComponentInChildren<BuildingBehaviour>();
        HidePanel(SelectedUnitList);
        ShowPanel(OneObjectSelected);
        _objectName.SetText(ObjectNameToString(building));
        _objectIcon.sprite = SpriteFromObject(building);

        if (buildingType(building) == BuildingType.BARRACK || buildingType(building) == BuildingType.FACTORY)
        {
            _trainingPanelController.OnTrainingPanel();
            _trainingPanelController.SetbuildingBehaviour((BuildingBehaviour)_selected);
        }
        else
            _trainingPanelController.OffTrainingPanel();
                // Ezt adod a TrainingPanelController-nek building.GetComponent<BuildingBehaviour>();
    }

    public void RefreshObjectHPBar()
    {
        if (_selected == null) return;
        _objectHBIcon.fillAmount = (float)_selected.Health / (float)_selected.Attributes.maxHealth;
        _objectHBIcon.color = HBPanelColor(gameObject);
        _objectHBText.SetText(ObjectHPText());
    }

    // A megadott panelt elrejti
    public void HidePanel(GameObject panel)
    {
        panel.gameObject.SetActive(false);
    }

    // A megadott panelt felfedi
    public void ShowPanel(GameObject panel)
    {
        panel.gameObject.SetActive(true);
    }

    // A megadott obejktum nevét stringként nadja vissza
    public string ObjectNameToString(GameObject gameObject)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        ObjectScriptableObject attributes = gameObject.GetComponentInChildren<ObjectBehaviour>().Attributes;
        sb.Append(attributes.name);
        return sb.ToString();
    }

    public BuildingType buildingType(GameObject gameobject)
    {
        BuildingScriptableObject attributes = (BuildingScriptableObject)gameobject.GetComponentInChildren<BuildingBehaviour>().Attributes;
        return attributes.type;
    }

    public Color HBPanelColor(GameObject gameObject)
    {
        /** Debug
         így működnie kellene, de a healthbarCritical és a healthBarFit 0-s átlátszóságot kap!
         ObjectScriptableObject mainattributes = gameObject.GetComponent<UnitBehaviour>().Attributes;
         Color hbc = Color.Lerp(mainattributes.healthBarCritical, mainattributes.healthBarFit, _objectHBIcon.fillAmount);
        **/
        Color hbc = Color.Lerp(Color.red, Color.green, _objectHBIcon.fillAmount);
        return hbc;
    }

    public string ObjectHPText()
    {
        if (_selected == null)
            return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(_selected.Attributes.maxHealth.ToString());
        sb.Append(" / ");
        sb.Append(_selected.Health.ToString());
        return sb.ToString();
    }

    // Visszaadja a "list" listából azoknak az egységeknek a számát, amelyeknek a neve "unitName"
    public int NumberOfSelectedUnits(string unitName, List<GameObject> list)
    {
        int c = 0;

        foreach (GameObject unit in list)
        {
            ObjectScriptableObject attributes = unit.GetComponent<UnitBehaviour>().Attributes;
            if (attributes.name == unitName)
                c++;

        }
        return c;
    }

    // Stringet készít a NumberOfSelectedUnits alapján
    public string NumberOfSelectedUnitsToString(string name, List<GameObject> list)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(NumberOfSelectedUnits(name, list));
        return sb.ToString();
    }

    // A megadott gameObjecthez tartozó képet adja vissza
    public Sprite SpriteFromObject(GameObject gameObject)
    {
        Sprite iconSprite;
        ObjectScriptableObject attributes = gameObject.GetComponentInChildren<ObjectBehaviour>().Attributes;
        iconSprite = attributes.uiGraphic;
        return iconSprite;
    }
}
