using UnityEngine;

public class LeaderboardButton : MonoBehaviour
{
    public void Show()
    {
        GameManager.Instance.LeaderboardsRef.ShowLeaderboard();
    }
}