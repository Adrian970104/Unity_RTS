using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ez a script azokon a ui elemeken van rajta, amelyek változnak a minimapScale változó szerint
public class UiResize : MonoBehaviour
{
    //Az elem alapmérete
    public float baseSizeX, baseSizeY;
    //X és/vagy Y tengelyen kövesse az értéket
    public bool resizableX, resizableY;

    public void Resize(float f)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if(rt != null)
        {
            if(resizableX)
            {
                rt.sizeDelta = new Vector2(baseSizeX * f, rt.sizeDelta.y);
            }
            if (resizableY)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, baseSizeY * f );
            }
        }
    }
}
