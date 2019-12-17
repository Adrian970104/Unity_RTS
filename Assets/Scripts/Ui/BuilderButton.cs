using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuilderButton : MonoBehaviour
{

    [Tooltip("ObjectScriptableObject-et vár. Ezt a unitot vagy buildinget fogja legyártani kattintásra, és ez határozza meg a gomb sprite-ját.")]
    public ObjectScriptableObject obj;
    [Tooltip("Ha igaz, akkor a gomb működik, és a gomb grafikája színes.\nHa hamis, akkor a gomb nem csinál semmit, és szürke")]
    public bool active = true;

    private static Color _inactiveColor = new Color(.3f,.3f,.3f); // Gombon kép elszínezése: Szürke
    private static Color _activeColor = new Color(1f, 1f, 1f); // Gombon kép elszínezése: Fehér (Azaz nem színeződik el)
    private static Color _costTextColor = new Color(1f, 1f, 1f); // Ár szöveg színe ha van elég zsé
    private static Color _noCostTextColor = new Color(1f, .2f, .2f); // Ár szöveg színe ha nincs elég zsé
    private UnityEngine.UI.Image _image; // A gombon megjelenő kép(sprite)
    private TextMeshProUGUI _nameText; // Az egység nevét mutató szöveg
    private TextMeshProUGUI _costText; // Az egység árát mutató szöveg

    private MouseController _mc;

    private ResourceManager _playerResources;

    public void SetPlayerResources(ResourceManager rm)
    {
        _playerResources = rm;
    }

    void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseController>();
        active = true;

        // Van-e behúzva referencia az Inspector ablakban
        if (obj == null)
        {
            Debug.Log(this + "Missing ObjectScriptableObject reference in BuilderButton script!");
        }
        else
        {
            
            _image = GetComponentInChildren<UnityEngine.UI.Image>(); //Megkeresi a komponenst

            _image.sprite = obj.uiGraphic43;
            _image.color = _activeColor;

            _nameText = GetComponent<Transform>().Find("NameBackground").GetComponent<Transform>().Find("NameText").GetComponent<TextMeshProUGUI>(); //Megkeresi a komponenst
            _costText = GetComponent<Transform>().Find("CostBackground").GetComponent<Transform>().Find("CostText").GetComponent<TextMeshProUGUI>(); //Megkeresi a komponenst

            _nameText.text = obj.name;
            _costText.text = obj.cost.ToString();
        }
        
    }

    private void Update()
    {
        /* TODO: Ha nincs elég lóvé, akkor legyen inactive*/

        

        if (obj is UnitScriptableObject uso)
        {
            BuildingBehaviour bb = _mc.GetSelectedBuilding();
            if(bb != null)
            {
                BuildingScriptableObject bso = (BuildingScriptableObject)bb.Attributes;
                if (bso != null)
                {
                    active = (uso.builtFrom == bso.type);
                }
                else
                {
                    active = true;
                }
            } else // Ha nincs kijelölve épület, akkor az összes unitbutton legyen inaktív
            {
                active = false;
            }

        }

        // Beállítja a gomb színét az aktivitásának megfelelően
        if (active)
        {
            _image.color = _activeColor;
        }
        else
        {
            _image.color = _inactiveColor;
        }
        /* TODO: Feltételbe: van-e elég ásvány */
        if(obj.cost <= _playerResources.GetMineral())
        {
            _costText.color = _costTextColor;
        }
        else
        {
            _costText.color = _noCostTextColor;
        }
    }

    public void OnClick()
    {

        if(obj.cost > _playerResources.GetMineral())
        {
            return;
        }

        if(obj is BuildingScriptableObject) {
            _mc.Build((BuildingScriptableObject)obj);
        } else if(obj is UnitScriptableObject)
        {
            BuildingBehaviour bb = _mc.GetSelectedBuilding();
            if(bb != null)
            {
                bb.AddToQueue((UnitScriptableObject)obj);
            } else
            {
                Debug.Log("Hiba! 0");
            }
            
        }

        _playerResources.TakeMineral(obj.cost);
    }

}

