using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private Transform _playersContainer;
    private GameObject _localPlayer;
    private Player _localPlayerScript;
    private GameObject[] _spawnPoints;

    private void Start()
    {
        _playersContainer = transform.Find("Players").transform;

        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (_spawnPoints.Length < 2)
        {
            Debug.LogError("Nincs elég spawnpoint lerakva a pályán");
            return;
        }

        float minDistance = 9999f;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if(i < _spawnPoints.Length-1)
            {
                float distance = Vector3.Distance(_spawnPoints[i].transform.position, _spawnPoints[i + 1].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            
            GameObject player = Instantiate(PlayerPrefab, _spawnPoints[i].transform.position, Quaternion.identity, _playersContainer);
        }

        if (minDistance < 50f)
        {
            Debug.LogError("A spawnpointok túl közel vannak egymáshoz!");
            return;
        }

        ImpersonatePlayer(_spawnPoints.Length - 1);
    }

    private void Update()
    {
        if(IsWinCondition())
        {
            SceneManager.LoadScene("LoseScene");

            return;
        }
    }

    private bool IsWinCondition()
    {
        GameObject player;
        Player playerScript;
        Cell mainCell;
        for (int i = 0; i < _playersContainer.childCount; i++)
        {
            player = _playersContainer.GetChild(i).gameObject;
            playerScript = player.GetComponent<Player>();
            mainCell = playerScript.GetMainBuildingCell().GetComponent<Cell>();

            return !mainCell.Built;
        }

        return false;
    }

    private int _pid = 0;
    [Command("switchPlayer", "switchPlayer() 0 -> 1, 1 -> 0 játékosra váltás")]
    public void SwitchPlayer()
    {
        ImpersonatePlayer(_pid);
        _pid++;
        _pid = _pid % 2;
    }

    [Command("impersonate", "impersonate(id) váltás játékosra")]
    public void ImpersonatePlayer(int id)
    {
        GameObject player;
        Player playerScript;
        for (int i = 0; i < _playersContainer.childCount; i++)
        {
            player = _playersContainer.GetChild(i).gameObject;
            playerScript = player.GetComponent<Player>();

            if (playerScript.GetPlayerId() == id)
            {
                _localPlayer = player;
                _localPlayerScript = playerScript;
                _localPlayerScript.ActAsLocalPlayer();
            }
            else
            {
                playerScript.SuspendLocalPlayer();
            }
        }

        GameObject.FindGameObjectWithTag("Ui_Controller").GetComponent<UiController>().RefreshPlayer(_localPlayer.GetComponent<ResourceManager>());
    }

    public Player GetLocalPlayer()
    {
        return _localPlayerScript;
    }
}
