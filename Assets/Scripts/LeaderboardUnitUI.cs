using TMPro;
using UnityEngine;

public class LeaderboardUnitUI : MonoBehaviour
{
    public LeaderboardScore leaderboardScore;
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text score;

    public void Init(LeaderboardScore ls)
    {
        leaderboardScore = ls;
        username.text = leaderboardScore.Username;
        score.text = leaderboardScore.Score.ToString();
    }
}
