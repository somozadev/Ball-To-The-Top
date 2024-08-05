using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUnitUI : MonoBehaviour
{
    public LeaderboardScore leaderboardScore;
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Image leaderboardUnitBorder;


    public void Init(LeaderboardScore ls, int podiumPos)
    {
        leaderboardScore = ls;
        username.text = leaderboardScore.Username;
        score.text = leaderboardScore.Score;
        switch (podiumPos)
        {
            case 0: //gold
                leaderboardUnitBorder.color = new Color32(255, 255, 0, 255);
                username.color = new Color32(255, 255, 165, 255);
                score.color = new Color32(255, 255, 165, 255);
                break;
            case 1: //silver
                leaderboardUnitBorder.color = new Color32(102, 182, 197, 255);
                username.color = new Color32(157, 193, 200, 255);
                score.color = new Color32(157, 193, 200, 255);
                break;
            case 2: //bronce
                leaderboardUnitBorder.color = new Color32(133, 64, 27, 255);
                username.color = new Color32(142, 102, 80, 255);
                score.color = new Color32(142, 102, 80, 255);
                break;
            default: //else #767676
                leaderboardUnitBorder.color = new Color32(200, 200, 200, 255);
                username.color = new Color32(200, 200, 200, 255);
                score.color = new Color32(200, 200, 200, 255);
                break;
        }
    }
}