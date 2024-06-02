using System;
using UnityEditor;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] private TerrainType _type;
    [SerializeField] private bool _changeTypeOnHit;
    [SerializeField] private TerrainType _changedType;
    private TerrainType _currentType;

    [SerializeField] private PhysicsMaterial2D _iceMaterial;

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
            ApplyEffect(other.gameObject.GetComponent<BallController>());
    }

    private void ApplyEffect(BallController ball)
    {
        switch (_currentType)
        {
            case TerrainType.JUMPY:
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                ball.ApplyForce(transform.up.normalized, 15f);
                break;
            case TerrainType.POISONOUS:
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                ball.transform.position = ball.lastStablePosition;
                ball.ResetVelocity();
                break;
            case TerrainType.STICKY:
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                ball.DisableGravity();
                ball.ResetVelocity();
                break;
            case TerrainType.ICEY:
                GetComponent<PolygonCollider2D>().sharedMaterial = _iceMaterial;
                break;
            case TerrainType.DEFAULT:
            default:
                GetComponent<PolygonCollider2D>().sharedMaterial = null;
                break;
        }

        if (_changeTypeOnHit) _currentType = _currentType == _type ? _changedType : _type;
        UpdateVisualsType();
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
}


public enum TerrainType
{
    DEFAULT,
    STICKY,
    ICEY,
    POISONOUS,
    JUMPY,
    PORTAL
}