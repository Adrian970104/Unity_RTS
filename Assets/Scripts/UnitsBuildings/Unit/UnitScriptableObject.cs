using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Balancolni az értékeket!!!
[CreateAssetMenu(fileName = "New Unit")] //Project window-ban létrehozható, jobbklikk, menüből kiválasztással
public class UnitScriptableObject : ObjectScriptableObject 
{
    [Header("Unit Settings:")]
    [Tooltip("Milyen típusú egység (kódban használatos).")]
    public UnitType type;

    [Tooltip("Milyen épületből építhető.")]
    public BuildingType builtFrom;

    [Tooltip("Milyen messsziről tud támadni.")]
    [Range(3f, 15f)]
    public float range;

    [Tooltip("Mekkora a mozgási sebessége (tervezett: egységtávolság/másodperc)")]
    [Range(.1f, 10f)]
    public float speed;

    [Tooltip("Milyen gyorsan fordul meg (fok/másodperc)")]
    [Range(.1f, 3600f)]
    public float angularSpeed;

    [Tooltip("Támadásonként mennyit sebez (A különböző bónuszok alkalmazása előtt, pl. rpg többet sebez tankra)")]
    [Range(0,100)]
    public int damage;

    [Tooltip("Ennyi másodperc elteltével támad legközelebb újra a unit. (cooldown)")]
    [Range(0f,2f)]
    public float attackSpeed;

    //+ Modell, textúrák, animációk, hangok, stb

}

public enum UnitType
{
    MARINE, RPG, TANK, AV
}
