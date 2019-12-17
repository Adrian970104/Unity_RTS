using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public abstract class ObjectBehaviour : MonoBehaviour, IDamageable
{
    /* Ebben az osztályvan az egységek és épületek közös viselkedései vannak (pl takeDamage) 
     * Ebből az osztályból származnak a UnitBehaviour és BuildingBehaviour osztályok, értelemszerűen
     * azokban az osztályokban a külön-külön viselkedések lesznek. (pl Unit tud csak sebezni)
     * Az attributes mező az ObjectScriptableObject osztályból való. Ez a Unit és BuldingBehaviour
     * osztályokban UnitSO és BuildingSO osztályokra lesz castolva (vagy valami más hasonló megoldás)*/

    [Tooltip("Az egység/épület tulajdonságai")]
    public ObjectScriptableObject Attributes;


    [Tooltip("Ha igaz, akkor az egység/épület nevét ráírja az objektumra a Scene View-ban.\n" +
             "Ez debugoláshoz és teszteléshez hasznos az egységek és épületek megkülönböztetésére")]
    public bool DrawNameInScene = true;

    // Az egység/épület jelenlegi élete (ha 0 vagy kevesebb, akkor az egység meghal)
    public int Health { get; private set; }

    // Az objektum gazdája
    public Player Owner { get; private set; }

    // healthbar canvas
    private Canvas _healthCanvas;
    
    // Az egység/épüet HealthBarja, a canvason belül, ez a kép "filled", azaz állítható az aktuális éreterő szerint
    private Image _healthBar;
    
    public virtual void Start()
    {
        if(Attributes != null)
        {
            Health = Attributes.maxHealth;
            GetComponent<Renderer>().material.color = Attributes.testColor;

            _healthCanvas = GetComponentInChildren<Canvas>();
            _healthBar = _healthCanvas.transform.Find("HealthBG").Find("HealthBar").GetComponent<Image>();
            SetCanvasName();
            SetHealthBarFill();
            SetHealthBarColor();
        }
    }

    public virtual void FixedUpdate()
    {
        _healthCanvas.transform.rotation = Quaternion.Inverse(transform.rotation);
        _healthCanvas.transform.rotation = Quaternion.LookRotation(_healthCanvas.transform.localPosition, Vector3.up);
       
        CheckHealth();
    }

    public virtual void Update()
    {
        if(!Owner.IsActivePlayer())
        {
            _healthBar.color = Color.red;
        } else
        {
            SetHealthBarColor();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public void CheckHealth()
    {
        if (Health <= 0)
        {
            Die();
        }
    }

    /*private void OnDrawGizmos()
    {
        if(DrawNameInScene && Attributes != null)
            Handles.Label(transform.position, Attributes.ToString() + "\nOwner ID: " + Owner.GetPlayerId());
    }*/

    public void Damage(int damage)
    {
        damage = damage < 0 ? -damage : damage;
        Health -= (damage - Mathf.FloorToInt(damage * Attributes.invincibility));
        ClampHealth();
        SetHealthBarFill();
        SetHealthBarColor();
    }

    public void Heal(int heal)
    {
        heal = heal < 0 ? -heal : heal;
        Health += heal;
        ClampHealth();
        SetHealthBarFill();
        SetHealthBarColor();
    }

    protected void ClampHealth()
    {
        Health = Mathf.Clamp(Health, 0, Attributes.maxHealth);
    }

    public void SetHealthBarFill()
    {
        _healthBar.fillAmount = (float)Health / Attributes.maxHealth;
    }

    public void SetHealthBarColor()
    {
        Color nc = Color.Lerp(Attributes.healthBarCritical, Attributes.healthBarFit, _healthBar.fillAmount);
        nc.a = 1;
        _healthBar.color = nc;
    }

    public void SetCanvasName()
    {
        _healthCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(Attributes.name);
    }

    public void SetOwner(Player owner)
    {
        if(Owner == null)
        {
            Owner = owner;
        } 
        else
        {
            Debug.LogError("Nem lehet objektumot átruházni");
        }
    }
}
