using System;
using UnityEngine;

public class WaterPoint : MonoBehaviour
{
    public float velocity = 0;
    [SerializeField] private float force = 0;
    public float height = 0f;
    [SerializeField] private float target_height = 0f;
    [SerializeField] private float resistance = 40f;
    private Water waterRef;
    public Vector3 Pos => transform.position;

    [SerializeField] private float sinAmplitude = 0.1f;
    [SerializeField] private float sinFrequency = 2f;
    [SerializeField] private float propagationDecreaseValue = 1.25f;

    public void Init(Water water)
    {
        velocity = 0;
        height = transform.localPosition.y;
        target_height = transform.localPosition.y;
        waterRef = water;
    }

    public void UpdateSpring(float springStiffness, float dampening, float phaseOffset, float amplitude,
        float frequency)
    {
        sinAmplitude = amplitude;
        sinFrequency = frequency;
        var trf = transform;
        var localPosition = trf.localPosition;
        target_height = Mathf.Sin((Time.time * sinFrequency) + phaseOffset) * sinAmplitude;

        height = localPosition.y;
        var x = height - target_height;
        var loss = -dampening * velocity;
        force = -springStiffness * x + loss;
        velocity += force;
        var y = localPosition.y;
        localPosition = new Vector3(localPosition.x, y + velocity, localPosition.z);
        trf.localPosition = localPosition;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        var ball = other.gameObject.GetComponent<BallController>();

        var speed = ball.velocityRb;
        Debug.Log(speed.y);
        var speedY = speed.y;
        if (speedY > 6) speedY = 6 * 3;
        else if (speedY < -6) speedY = -6 * 3;
        ApplyHit(speedY, false, false, true, speed.y < 0);
    }

    public void ApplyHit(float speedY, bool goLeft, bool goRight, bool isCenter, bool negative)
    {
        velocity += Mathf.Abs(speedY) / resistance;
        if (negative) velocity *= -1;

        if (waterRef == null) waterRef = GetComponentInParent<Water>();
        if (Mathf.Abs(speedY) > 0.02f)
            waterRef.Propagate(speedY / propagationDecreaseValue, goLeft, goRight, isCenter, this);
    }
}