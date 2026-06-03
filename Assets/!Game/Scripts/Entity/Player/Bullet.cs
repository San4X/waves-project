using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;
    public float topSpeed = 5f;
    public AnimationCurve speedCurve;
    private float _timer;
    private Vector3 _startPosition;


    private void Awake()
    {
        _timer = lifeTime;
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        float distanceFromBarrel = Vector3.Distance(_startPosition, transform.position);
        float coef = Mathf.InverseLerp(0f, 10f, distanceFromBarrel);
        float distanceFactor = speedCurve.Evaluate(coef);
        
        transform.Translate(Vector3.forward * (topSpeed * distanceFactor * Time.deltaTime));
    }
}
