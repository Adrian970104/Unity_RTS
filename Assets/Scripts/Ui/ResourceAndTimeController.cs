using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceAndTimeController : MonoBehaviour
{
    // A játkban eddig eltelt idő
    private float _gameTime;
    // A játék kezdetének időpontja
    public float _startTime;
    // A UI-on megjelenő eltelt panelje
    private TextMeshProUGUI _timeText;

    
    void Start()
    {
        _timeText = gameObject.transform.Find("TimeTM").GetComponent<TextMeshProUGUI>();
        _gameTime = Time.time;
        _startTime = Time.time;
    }

    private void Update()
    {
        RefreshGameTime();
        RefreshPanel();
    }

    public void RefreshPanel()
    {

        _timeText.SetText(TimeToText(_gameTime));
    }

    // A játékban eltelt időt frissíti.
    public void RefreshGameTime()
    {
        _gameTime = Time.time - _startTime;
    }

    // Az eltelt időt megfelelő formátummá alakítja.
    public string TimeToText(float time)
    {
        string minutes = ((int)time / 60).ToString();
        string seconds = (Mathf.Floor(time % 60).ToString("f0"));
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(minutes);
        sb.Append(":");
        sb.Append(seconds);
        return sb.ToString();
    }
}
