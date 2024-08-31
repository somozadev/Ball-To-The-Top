using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] private TerrainType _type;
    [SerializeField] private bool _changeTypeOnHit;
    [SerializeField] private TerrainType _changedType;
    private TerrainType _currentType;

    [SerializeField] private PhysicsMaterial2D _iceMaterial;

    public bool isInContact;

    private void Awake()
    {
        if (_type == TerrainType.ICEY || _changedType == TerrainType.ICEY)
            _iceMaterial =
                Resources.Load<PhysicsMaterial2D>("PMat_Sliding");
    }

    private void Start()
    {
        _currentType = _type;
        UpdateVisualsType();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var cp = other.GetContact(0);
            
            ApplyEffect(other.gameObject.GetComponent<BallController>(), cp);
        }
    }

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     if (!other.gameObject.CompareTag("Player")) return;
    //     if (IsStillInContact(other.collider)) return;
    //     isInContact = false;
    //     RemoveFromChild(other.transform);
    //     other.gameObject.GetComponent<BallController>().EnableGravity();
    // }
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(firstFacePoint, 0.2f);
        Gizmos.DrawWireSphere(secondFacePoint, 0.2f);

        Gizmos.color = Color.cyan;
        var lineVector = secondFacePoint - firstFacePoint;
        var lineVectorXY = new Vector3(lineVector.x, lineVector.y, 0);
        var normalXY = new Vector3(-lineVectorXY.y, lineVectorXY.x, 0);
        normalXY.Normalize();
        Gizmos.DrawLine(transform.position, transform.position + normalXY);
    }

    [SerializeField] private bool drawGizmos;
    [SerializeField] private Vector3 firstFacePoint;
    [SerializeField] private Vector3 secondFacePoint;

    private void ApplyEffect(BallController ball, ContactPoint2D contactPoint2D)
    {
        isInContact = true;
        switch (_currentType)
        {
            case TerrainType.JUMPY:
                SoundManager.Instance.Play("Hit-jumpy",true, false, true);
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                var lineVector = secondFacePoint - firstFacePoint;
                var lineVectorXY = new Vector3(lineVector.x, lineVector.y, 0);
                var normalXY = new Vector3(-lineVectorXY.y, lineVectorXY.x, 0);
                normalXY.Normalize();
                ball.ApplyForce(normalXY , 15f);
                // SetAsChild(ball.transform);
                break;
            case TerrainType.POISONOUS:
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                ball.transform.position = ball.lastStablePosition;
                ball.ResetVelocity();
                // SetAsChild(ball.transform);
                break;
            case TerrainType.STICKY:
                SoundManager.Instance.Play("Hit-sticky",true, false, true);
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                ball.DisableGravity();
                ball.ResetVelocity();
                ball.transform.position = contactPoint2D.point;
                SetAsChild(ball.transform);
                break;
            case TerrainType.ICEY:
                GetComponent<PolygonCollider2D>().sharedMaterial = _iceMaterial;
                // SetAsChild(ball.transform);
                break;
            case TerrainType.DEFAULT:
            default:
                SoundManager.Instance.Play("Hit",true, false, true);
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                // SetAsChild(ball.transform);
                break;
        }

        if (_changeTypeOnHit) _currentType = _currentType == _type ? _changedType : _type;
        UpdateVisualsType();
    }

    private void SetAsChild(Transform ball)
    {
        ball.SetParent(transform, true);
    }

    public void RemoveFromChild(Transform ball)
    {
        ball.SetParent(null, true);
        isInContact = false;
    }

    private IEnumerator ApplyStickyEffect(BallController ball)
    {
        yield return new WaitForSeconds(0.1f);
        ball.DisableGravity();
        ball.ResetVelocity();
    }

    private void UpdateVisualsType()
    {
        switch (_currentType)
        {
            case TerrainType.JUMPY:
                GetComponent<SpriteRenderer>().color = new Color32(231, 197, 0, 255);
                break;
            case TerrainType.POISONOUS:
                GetComponent<SpriteRenderer>().color = new Color32(45, 255, 41, 255);
                break;
            case TerrainType.ICEY:
                GetComponent<SpriteRenderer>().color = new Color32(45, 151, 255, 255);
                break;
            case TerrainType.STICKY:
                GetComponent<SpriteRenderer>().color = new Color32(200, 45, 125, 255);
                break;
            case TerrainType.DEFAULT:
            default:
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                break;
        }
    }

    private bool IsStillInContact(Collider2D other)
    {
        Collider2D terrainCollider = GetComponent<Collider2D>();
        return terrainCollider.bounds.Intersects(other.bounds);
    }
}


public enum TerrainType
{
    DEFAULT,
    STICKY,
    ICEY,
    POISONOUS,
    JUMPY
}