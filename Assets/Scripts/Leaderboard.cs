using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dan.Main;
using Dan.Models;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<LeaderboardScore> _scores;
    [SerializeField] private string testUsername = "Player1";
    [SerializeField] private string testScore = "01:42:01";
    [SerializeField] private GameObject leaderboardCanvas;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject leaderboardUnitPrefab;
    [SerializeField] private Loading _loading;
    [SerializeField] private GameObject errorFetchingText;

    public void TriggerSubmission()
    {
        GetComponent<LeaderboardSubmittingField>().EnableSubmission();
    }

    [ContextMenu("FetchLeaderboard")]
    public void FetchLeaderboard()
    {
        errorFetchingText.SetActive(false);
        _loading.gameObject.SetActive(true);
        Leaderboards.BallItUp.GetEntries(true, Callbacks, OnError);
        return;

        void Callbacks(Entry[] entries)
        {
            _loading.gameObject.SetActive(false);
            _scores.Clear();
            foreach (var entry in entries)
            {
                var scoreStr = ConvertToTimeFormat(entry.Score); 
                _scores.Add(new LeaderboardScore(entry.Username, scoreStr));
                Debug.Log($"Player: {entry.Username}, Score: {entry.Score}");
            }

            LoadUI();
        }

        void OnError(string error)
        {
            errorFetchingText.SetActive(true);
        }
    }

    [ContextMenu("UploadScoreTest")]
    public void UploadToLeaderboardTest()
    {
        var scoreInt = ConvertToReversedInt(testScore);
        Leaderboards.BallItUp.UploadNewEntry(testUsername, scoreInt, success =>
        {
            if (success)
            {
                Leaderboards.BallItUp.ResetPlayer(); //So players can upload more entries
                Debug.Log("Score uploaded successfully!");
            }
            else
            {
                Debug.LogError("Failed to upload score.");
            }
        });
    }

    //score will be 00:12:23 formatted as 001223 reversed , so 322100
    public void UploadToLeaderboard(string username, string score) //maybe activate profanity filter and listen to callback before transitioning to main scene (?)
    {
        var scoreInt = ConvertToReversedInt(score);
        Leaderboards.BallItUp.UploadNewEntry(username, scoreInt, success =>
            {
                if (success)
                {
                    Leaderboards.BallItUp.ResetPlayer();
                    Debug.Log("Score uploaded successfully!");
                }
                else
                {
                    Debug.LogWarning("Failed to upload score.");
                }
            },
            errorCallback => { Debug.LogError(errorCallback); }); //maybe another system to save the hs locally and try upload later (?)
    }

    private void LoadUI()
    {
        for (int i = 0; i < _scores.Count; i++)
        {
            var leaderboardUnitUI =
                Instantiate(leaderboardUnitPrefab, content.transform).GetComponent<LeaderboardUnitUI>();
            leaderboardUnitUI.Init(_scores[i],i);  
        }
    }

    public void ShowLeaderboard()
    {
        FetchLeaderboard();
        leaderboardCanvas.SetActive(true);
    }

    public void HideLeaderboard()
    {
        leaderboardCanvas.SetActive(false);
        foreach (var child in content.GetComponentsInChildren<LeaderboardUnitUI>())
        {
            Destroy(child.gameObject);
        }
    }

    private string ConvertToTimeFormat(int timeScore)
    {
        var scoreStr = timeScore.ToString().PadLeft(6, '0');
        var hours = scoreStr.Substring(0, 2);
        var minutes = scoreStr.Substring(2, 2);
        var seconds = scoreStr.Substring(4, 2);
        return $"{hours}:{minutes}:{seconds}";
    }

    private int ConvertToReversedInt(string timeFormat)
    {
        string scoreWithoutColons = timeFormat.Replace(":", "");
        char[] charArray = scoreWithoutColons.ToCharArray();
        return int.Parse(charArray);
    }
}

[Serializable]
public class LeaderboardScore
{
    public string Username;
    public string Score;

    public LeaderboardScore(string username, string score)
    {
        Username = username;
        Score = score;
    }
}