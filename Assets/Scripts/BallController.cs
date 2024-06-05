using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Header("References")] [Space(10)] public Rigidbody2D rb;
    private LineRenderer _draggingLineRenderer;
    private LineRenderer _lineRenderer;
    [SerializeField] private CameraController _cameraController;
    private Camera _cameraRef;
    [SerializeField] private RectTransform _canvasRectTransform;

    [Header("Conditionals")] [Space(10)] public bool canInteract;
    public bool isMouseClicked;
    public bool modifyCamera = true;
    public bool isInWater = false;
    public bool isInMovingObject = false;

    [Header("Trajectory variables")] [Space(10)] [SerializeField]
    private float launchForce = 15f;

    [SerializeField] private float trajectoryTimeStamp = 0.05f;
    [SerializeField] private int trajectoryStepCount = 40;
    [SerializeField] private LayerMask ignoreLayerMask;

    [Header("Vectors")] [Space(10)] private Vector2 _velocity;
    private Vector2 _startPosition;
    private Vector2 _fingerStartPosition;
    private Vector2 _currentPosition;
    public Vector3 lastStablePosition;
    public Vector2 velocityRb;

    [Header("LastTerrainsData")] [Space(10)] [SerializeField]
    private List<TerrainDataHolder> _lastTerrains;

    [SerializeField] private TerrainDataHolder _currentTerrain;

    [Header("Particles Object Pooling")] [Space(10)] [SerializeField]
    private ParticleSystem _hitParticle;

    [SerializeField] private List<ParticleSystem> _particlePool;


    #region InitialMethods

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _lineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
        _draggingLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        _cameraRef = _cameraController.cameraRef;
        LoadData(GameManager.Instance.currentData);
    }

    public void LoadData(Data data)
    {
        Vector3 position = new Vector3(data.XPos, data.YPos, data.ZPos);
        Vector3 rotation = new Vector3(data.XRot, data.YRot, data.ZRot);
        transform.position = position;
        transform.rotation = quaternion.Euler(rotation);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteract) return;
            if (!GameManager.Instance.detectMouseGameInput) return;
            MouseDown();
            isMouseClicked = true;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isMouseClicked) return;
            if (canInteract)
                if (GameManager.Instance.detectMouseGameInput)
                    MouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!canInteract) return;
            if (!GameManager.Instance.detectMouseGameInput) return;
            MouseUp();
            isMouseClicked = false;
        }
    }

    private void FixedUpdate()
    {
        velocityRb = rb.velocity;
        if (isInWater) canInteract = true;
        else if (isInMovingObject) canInteract = true;
        else
            canInteract = !(rb.velocity.magnitude > 0.1f);
    }

    #endregion

    #region Collisions

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
            isInWater = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
            isInWater = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isInMovingObject = other.gameObject.CompareTag("MovingObject");

        Instantiate(_hitParticle, other.collider.ClosestPoint(transform.position),
            quaternion.identity); //Update to use object pooling
        // Debug.LogWarning($"COLISSION ENTERED IN BALL WITH {other.gameObject.name}");
        ManageTerrainList(other);
    }

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     Debug.LogWarning($"COLISSION EXITED IN BALL WITH {other.gameObject.name}");
    // }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (_currentTerrain == null)
            ManageTerrainList(other);
    }

    #endregion

    #region Terrain

    private void ManageTerrainList(Collision2D other)
    {
        if (other.gameObject.CompareTag("Terrain") || other.gameObject.CompareTag("MovingObject"))
        {
            var dataHolder = other.gameObject.GetComponent<TerrainDataHolder>();
            if (!_lastTerrains.Contains(dataHolder)) _lastTerrains.Add(dataHolder);
            _currentTerrain = dataHolder;
        }
    }

    public Vector3 GetLastStablePositionFromLastTerrain()
    {
        for (int i = _lastTerrains.Count - 1; i >= 0; i--)
        {
            if (_lastTerrains[i].HasPositions)
            {
                return _lastTerrains[i].GetLastStablePosition();
            }
        }

        throw new InvalidOperationException("No stable position found in last terrains.");
    }

    #endregion

    #region MouseEvents

    private void MouseDown()
    {
        lastStablePosition = transform.position;
        if (_currentTerrain)
            _currentTerrain.AddStablePosition(lastStablePosition);
        _startPosition = GetViewportPosition(Input.mousePosition);
        _currentPosition = GetViewportPosition(Input.mousePosition);
    }

    private void MouseDrag()
    {
        _currentPosition = GetViewportPosition(Input.mousePosition);
        Vector2 direction = -(_currentPosition - _startPosition);
        direction = direction.normalized;
        float distance = Vector2.Distance(_startPosition, _currentPosition) * 5;
        if (distance >= 1f)
            distance = 1f;
        if (modifyCamera)
            _cameraController.LerpZoomOut(distance);
        _currentPosition = _startPosition + direction * distance;
        _velocity = direction * distance * launchForce;
        DrawDraggingLine();
        DrawTrajectory();
    }

    private void MouseUp()
    {
        _currentPosition = GetViewportPosition(Input.mousePosition);
        if (modifyCamera)
            _cameraController.LerpZoomIn();
        Launch();
        ResetDraggingLine();
        ResetDrawTrajectory();
    }

    #endregion

    #region OtherMethods

    public void ApplyForce(Vector2 direction, float power)
    {
        rb.AddForce(direction * power, ForceMode2D.Impulse);
    }

    public void DisableGravity() => rb.gravityScale = 0;
    public void EnableGravity() => rb.gravityScale = 1;

    public void ResetVelocity()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Launch()
    {
        if (_currentTerrain)
        {
            _currentTerrain.GetComponent<Terrain>().RemoveFromChild(transform);
            _currentTerrain = null;
        }

        EnableGravity();
        canInteract = false;
        rb.AddForce(_velocity, ForceMode2D.Impulse);
        _velocity = Vector2.zero;
    }

    private Vector2 GetViewportPosition(Vector3 mousePosition) => _cameraRef.ScreenToViewportPoint(mousePosition);

    #endregion

    #region TrajectoryLine

    private void DrawTrajectory()
    {
        // Limpiar la trayectoria anterior
        _lineRenderer.positionCount = 0;

        // Calcular la trayectoria
        Vector3[] positions = new Vector3[trajectoryStepCount];
        Vector2 currentPosition = transform.position;
        Vector2 currentVelocity = _velocity;

        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float time = i * trajectoryTimeStamp;
            Vector2 newPosition = currentPosition + currentVelocity * time + 0.5f * Physics2D.gravity * time * time;
            positions[i] = (new Vector3(newPosition.x, newPosition.y, 0f));

            if (i > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(positions[i - 1], positions[i] - positions[i - 1],
                    Vector2.Distance(positions[i], positions[i - 1]), ~ignoreLayerMask);
                if (hit.collider != null && !hit.collider.CompareTag("Player"))
                {
                    positions[i] = hit.point;
                    Array.Resize(ref positions, i + 1);
                    break;
                }
            }
        }

        // Dibujar la trayectoria
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.SetPositions(positions);
    }

    private void ResetDrawTrajectory()
    {
        _lineRenderer.positionCount = 0;
    }

    #endregion

    #region DraggingLine

    private void DrawDraggingLine()
    {
        _draggingLineRenderer.positionCount = 2;
        Vector2 startPoint = transform.position;
        Vector2 endPoint = startPoint + (_startPosition - _currentPosition);

        _draggingLineRenderer.SetPosition(0, startPoint);
        _draggingLineRenderer.SetPosition(1, endPoint);
    }

    private void ResetDraggingLine()
    {
        _draggingLineRenderer.positionCount = 0;
    }

    #endregion

    private void OnDisable()
    {
        GameManager.Instance.SaveBall(transform);
    }

    #region Application

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            GameManager.Instance.SaveBall(transform);
        else
            LoadData(GameManager.Instance.currentData);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            GameManager.Instance.SaveBall(transform);
        else
            LoadData(GameManager.Instance.currentData);
    }

    private void OnApplicationQuit()
    {
        GameManager.Instance.SaveBall(transform);
    }

    #endregion
}