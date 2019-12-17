using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MineralDisplay : MonoBehaviour
{
    ResourceManager _playerResources;

    void Update()
    {
        if(_playerResources != null)
        {
            GetComponent<TextMeshProUGUI>().SetText(_playerResources.GetMineral().ToString());
        } else
        {
            GetComponent<TextMeshProUGUI>().SetText("No player");
        }
    }

    public void SetPlayer(ResourceManager rm)
    {
        _playerResources = rm;
    }
}
