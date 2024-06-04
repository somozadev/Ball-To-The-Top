using UnityEngine;

public class Debugger : MonoBehaviour
{
    [Range(0,4)] [SerializeField] private float range;
    [SerializeField] private Color32 color;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}