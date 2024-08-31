using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private bool isTimerRunning = false;
    public string currentTime;

    private float elapsedTime;
    [SerializeField] private TMP_Text _timerText;

    private void OnEnable()
    {
        GameManager.Instance.TimerController = this;
    }

    void Start()
    {
        elapsedTime = 0;
        UpdateCurrentTime();
        LoadData(GameManager.Instance.currentData);
    }

    public void LoadData(Data data)
    {
        currentTime = data.Time;
        string[] timeParts = currentTime.Split(':');
        var hours = int.Parse(timeParts[0]);
        var minutes = int.Parse(timeParts[1]);
        var seconds = int.Parse(timeParts[2]);
        elapsedTime = (hours * 3600) + (minutes * 60) + seconds;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateCurrentTime();
            _timerText.text = currentTime;
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        UpdateCurrentTime();
        GameManager.Instance.SaveTime(currentTime);
    }

    private void UpdateCurrentTime()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        currentTime = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    private void OnDisable()
    {
        if (GameManager.Instance.finishedGame) return;
            GameManager.Instance.SaveTime(currentTime);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            GameManager.Instance.SaveTime(currentTime);
        else
            LoadData(GameManager.Instance.currentData);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            GameManager.Instance.SaveTime(currentTime);
        else
            LoadData(GameManager.Instance.currentData);
    }

    private void OnApplicationQuit()
    {
        GameManager.Instance.SaveTime(currentTime);
    }
}