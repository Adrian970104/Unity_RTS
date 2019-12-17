using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private int _money;
    private static readonly int _startMoney = 10000;

    public void Start()
    {
        _money = _startMoney;
    }

    public bool TakeMineral(int amount)
    {
        if(_money - amount < 0)
        {
            return false;
        } else
        {
            _money -= amount;
            return true;
        }
    }

    public void AddMineral(int amount)
    {
        _money += amount;
    }

    public int GetMineral()
    {
        int i = _money;

        return i;
    }

}
