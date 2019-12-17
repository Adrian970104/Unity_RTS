using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject MainBuildingPrefab;

    private static int playerCount = 0;

    [SerializeField]
    private bool IsLocalPlayer = false;
    [SerializeField]
    private int _playerId;

    private Cell _mainBuildingCell;

    private void Awake()
    {
        _mainBuildingCell = GameObject.Find("Grid").GetComponent<Grid>()
            .GetNearestCell(transform.position, true)
            .GetComponent<Cell>();

        _mainBuildingCell.Build(MainBuildingPrefab, this);

        _playerId = playerCount++;
    }

    public int GetPlayerId()
    {
        return _playerId;
    }

    public Cell GetMainBuildingCell()
    {
        return _mainBuildingCell;
    }

    public void ActAsLocalPlayer()
    {
        IsLocalPlayer = true;
        Camera camera = Camera.main;
        camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, transform.position.z);
        camera.transform.SetParent(transform, true);
        camera.transform.GetComponent<MouseController>().PurgeSelection();
        Debug.Log("Local Player is now PlayerID:" + _playerId);
    }

    public void SuspendLocalPlayer()
    {
        IsLocalPlayer = false;
    }

    public bool IsActivePlayer()
    {
        return IsLocalPlayer;
    }
}
