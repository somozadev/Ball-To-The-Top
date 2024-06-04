using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class Water : MonoBehaviour
{
    [Space(20)] [Header("Params")] [Space(5)] [SerializeField]
    private float dampening = 0.03f;

    [SerializeField] private float stiffness = 0.1f;

    [Range(0, 550)] [SerializeField] private int wavesAmount = 10;
    [Range(0.005f, .5f)] [SerializeField] private float displacement = 0.1f;


    [Space(20)] [Header("Refs")] [Space(5)]
    public Material waterFillMaterial;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _waterPointPrefab;
    public List<WaterPoint> points;
    private List<Vector3> pointsAux;

    private float[] phaseOffsets;

    [SerializeField] private float phaseOffsetScalar = 2f;
    [SerializeField] private float sinAmplitude = 0.1f;
    [SerializeField] private float sinFrequency = 2f;

    private void Awake()
    {
        points = new List<WaterPoint>();
        _lineRenderer = GetComponent<LineRenderer>();
        pointsAux = new List<Vector3>();
        InitializePhaseOffsets();
        GenerateWave();
    }

    [ContextMenu("GenerateLine")]
    private void GenerateWave()
    {
        for (int i = 1; i <= wavesAmount; i++)
        {
            var trf = transform;
            Vector3 position = new Vector3(trf.position.x + displacement * i, trf.position.y, 0);
            var go = Instantiate(_waterPointPrefab, position, quaternion.identity, trf);
            go.name = "WavePoint_" + i;
            var waterPoint = go.GetComponent<WaterPoint>();
            points.Add(waterPoint);
            waterPoint.Init(this);
        }

        points[0].isBorder = true;
        points[wavesAmount-1].isBorder = true;
        _lineRenderer.positionCount = wavesAmount;
    }

    private void FixedUpdate()
    {
        for (var i = 0; i < points.Count; i++)
            points[i].UpdateSpring(dampening, stiffness, phaseOffsets[i], sinAmplitude, sinFrequency);

        UpdateLine();
        // FillShader();
    }


    private void UpdateLine()
    {
        pointsAux.Clear();
        foreach (var point in points)
            pointsAux.Add(point.Pos);

        _lineRenderer.SetPositions(pointsAux.ToArray());
    }

    public void Propagate(float speedY, bool goLeft, bool goRight, bool isCenter, WaterPoint callerPoint)
    {
        var i = points.IndexOf(callerPoint);
        if (isCenter)
        {
            if (i - 1 >= 0) points[i - 1].ApplyHit(speedY, true, false, false,speedY < 0);
            if (i + 1 < points.Count) points[i + 1].ApplyHit(speedY, false, true, false,speedY < 0);
        }
        else if (goRight)
        {
            if (i + 1 < points.Count) points[i + 1].ApplyHit(speedY, false, true, false,speedY < 0);
        }
        else if (goLeft)
        {
            if (i - 1 >= 0) points[i - 1].ApplyHit(speedY, true, false, false,speedY < 0);
        }
    }

    private void FillShader()
    {
        UpdateLine();
        Vector4[] waterPointsArray = new Vector4[pointsAux.Count];
        for (int i = 0; i < points.Count; i++)
            waterPointsArray[i] = new Vector4(pointsAux[i].x, pointsAux[i].y);

        waterFillMaterial.SetVectorArray("_WaterPoints", waterPointsArray);
    }

    private void InitializePhaseOffsets()
    {
        phaseOffsets = new float[wavesAmount];
        float phaseIncrement = (phaseOffsetScalar * Mathf.PI) / wavesAmount;
        for (int i = 0; i < wavesAmount; i++)
        {
            phaseOffsets[i] = i * phaseIncrement;
        }
    }
}