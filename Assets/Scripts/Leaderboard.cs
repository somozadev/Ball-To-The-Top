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


    [ContextMenu("FetchLeaderboard")]
    public void FetchLeaderboard()
    {
        _loading.gameObject.SetActive(true);
        Leaderboards.BallItUp.GetEntries(true, Callbacks);
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
    }

    [ContextMenu("UploadScoreTest")]
    public void UploadToLeaderboardTest()
    {
        var scoreInt = ConvertToReversedInt(testScore);
        Leaderboards.BallItUp.UploadNewEntry(testUsername, scoreInt, success =>
        {
            if (success)
            {
                Debug.Log("Score uploaded successfully!");
            }
            else
            {
                Debug.LogError("Failed to upload score.");
            }
        });
    }

    //score will be 00:12:23 formatted as 001223 reversed , so 322100
    public void UploadToLeaderboard(string username, string score)
    {
        var scoreInt = ConvertToReversedInt(score);
        Leaderboards.BallItUp.UploadNewEntry(username, scoreInt, success =>
        {
            if (success)
            {
                Debug.Log("Score uploaded successfully!");
            }
            else
            {
                Debug.LogError("Failed to upload score.");
            }
        });
    }

    private void LoadUI()
    {
        foreach (var score in _scores)
        {
            var leaderboardUnitUI =
                Instantiate(leaderboardUnitPrefab, content.transform).GetComponent<LeaderboardUnitUI>();
            leaderboardUnitUI.Init(score);
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