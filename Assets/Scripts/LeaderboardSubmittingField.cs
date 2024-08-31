using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardSubmittingField : MonoBehaviour
{
    private Leaderboard _leaderboard;

    [SerializeField] private GameObject submittingFieldGroup;

    [SerializeField] private TMP_InputField usernameInputF;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text finalTimer;
    private string username;
    private string score;

    private void Awake()
    {
        _leaderboard = GetComponent<Leaderboard>();
    }

    public void EnableSubmission()
    {
        score = GameManager.Instance.TimerController.currentTime;
        finalTimer.text = score;
        submittingFieldGroup.SetActive(true);
    }

    private void OnEnable()
    {
        usernameInputF.onSubmit.AddListener(InputFieldSubmit);
        submitButton.onClick.AddListener(Submit);
    }

    private void OnDisable()
    {
        username = score = "";
        usernameInputF.onSubmit.RemoveListener(InputFieldSubmit);
        submitButton.onClick.RemoveListener(Submit);
        submittingFieldGroup.SetActive(false);
    }

    private void InputFieldSubmit(string arg0)
    {
        if (arg0.Length <= 0) return;
        submitButton.enabled = true;
        username = arg0;
    }


    private void Update()
    {
        submitButton.enabled = usernameInputF.text.Length is > 0 and <= 11;
    }

    private void Submit()
    {
        // if (username.Equals(string.Empty) || score.Equals(string.Empty)) return;
        username = usernameInputF.text;
        score = GameManager.Instance.TimerController.currentTime;
        
        _leaderboard.UploadToLeaderboard(username, score);
        GameManager.Instance.finishedGame = true;
        submittingFieldGroup.SetActive(false);
        GameManager.Instance.LoadHomeScene();
        GameManager.Instance.TimerController.ResetTimer();
        usernameInputF.text = "";
        

    }
}