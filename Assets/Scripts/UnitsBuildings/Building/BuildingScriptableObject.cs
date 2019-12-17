using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Balancolni az értékeket!!!
[CreateAssetMenu(fileName = "New Building")] //Project window-ban létrehozható, jobbklikk, menüből kiválasztással
public class BuildingScriptableObject : ObjectScriptableObject
{
    [Header("Building settings:")]
    [Tooltip("Milyen típusú épület (kódban használatos)")]
    public BuildingType type;
    
    [Tooltip("Az épülethez tartozó prefab.")]
    public GameObject prefab;

    //Hány egységet lehet berakni a gyártósorba (nem változtatható, az összes típusú épületnek ugyanaz)
    public const int queueSize = 5;
}

public enum BuildingType
{
    MAIN, MINE, BARRACK, FACTORY
}
