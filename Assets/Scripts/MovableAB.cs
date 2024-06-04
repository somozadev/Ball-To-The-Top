using System.Collections;
using UnityEngine;

public class MovableAB : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;

    [Range(0, 4)] [SerializeField] private float speed = 0.5f;

    private bool movingToA;

    private void Start()
    {
        movingToA = true;
        if(!A || !B) return;
        StartCoroutine(StartMovement());
    }

    private IEnumerator StartMovement()
    {
        while (true)
        {
            Transform target = movingToA ? A : B;

            while (Vector3.Distance(transform.position, target.position) > 0.01f)
            {
                transform.position = Vector3.Slerp(transform.position, target.position, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = target.position;
            movingToA = !movingToA;
            // yield return new WaitForSeconds(.25f); 
        }
    }
}