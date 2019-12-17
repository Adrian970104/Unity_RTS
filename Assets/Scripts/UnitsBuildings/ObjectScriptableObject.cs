using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Ebből az osztályból származnak a UnitSriptableObject és BuildingScriptableObject osztályok.
 * A mezői a közös adatok. Ezt használd, ha bármelyik SO kerülhet a kódodba. */
public abstract class ObjectScriptableObject : ScriptableObject
{
    [Header("General Settings:")]
    [Tooltip("A név, ami megjelenik a játékban.")]
    public new string name;

    [Tooltip("Az egység/épület maximális élete")]
    public int maxHealth;

    [Tooltip("Healthbar színe, ha maxos az élet")]
    public Color healthBarFit = Color.green;
    
    [Tooltip("Healthbar színe, ha kevés az élet")]
    public Color healthBarCritical = Color.red;

    [Tooltip("Mennyire tudja csillapítani az ellene irányuló sebzést")]
    [Range(0f, 1f)]
    public float invincibility;

    [Tooltip("Mennyi ásványba kerüljön az építés/gyártás")]
    public int cost;

    [Tooltip("Milyen kép jelenjen meg a UI-ban")]
    public Sprite uiGraphic;

    [Tooltip("Milyen kép jelenjen meg a UI-ban (4-3 képarány)")]
    public Sprite uiGraphic43;

    [Tooltip("DEBUG: Színe az egységet/épületet reprezentáló kockának")]
    public Color testColor;

    public new string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append(name);

        return sb.ToString();
    }
}
