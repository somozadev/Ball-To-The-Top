using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EndGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.gameObject.tag.Equals("Player")) return;

        GameManager.Instance.TimerController.StopTimer();
        GameManager.Instance.LeaderboardsRef.TriggerSubmission();
        GameManager.Instance.DisableInput();
        Destroy(gameObject);

    }
}