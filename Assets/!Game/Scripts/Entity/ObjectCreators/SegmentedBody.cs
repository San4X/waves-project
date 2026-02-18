using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SegmentedBody : MonoBehaviour
{
    [SerializeField] private int segmentsAmount;
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private AnimationCurve segmentsScaleDistribution;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private Transform rig;
    [SerializeField] private Transform rigComponent;
    [SerializeField] private RigBuilder rigBuilder;
    private Transform _head;
    

    private void Awake()
    {
        _head = transform.GetChild(0);
        
        InstantiateBody(segmentsAmount);
    }
    
    private void InstantiateBody(int tailSegments)
    {
        var lastSegment = _head;
        
        for (int i = 0; i < tailSegments; i++)
        {
            var parameters = new InstantiateParameters
            {
                worldSpace = false,
                parent = lastSegment,
            };
            
            lastSegment = Instantiate(
                segmentPrefab, 
                GetSmallestZPointFromMesh(lastSegment), 
                Quaternion.identity,
                parameters);

            lastSegment.localScale = GetSegmentScale(i);
            SetRigForSegment(lastSegment);
        }
        
        rigBuilder.Build();
    }


    private float _lastScale = 1;
    private Vector3 GetSegmentScale(int segmentIndex)
    {
        float value = Mathf.InverseLerp(0, segmentsAmount-1, segmentIndex);
        float scale = segmentsScaleDistribution.Evaluate(value);
        scale *= scaleMultiplier;
        
        float actualScale = scale / _lastScale;
        _lastScale = scale;

        return Vector3.one * actualScale;
    }

    private void SetRigForSegment(Transform segment)
    {
        var component = Instantiate(rigComponent, rig).GetComponent<DampedTransform>();
        var data = component.data;
        data.constrainedObject = segment;

        var segmentsParent = segment.transform.parent;
        if (segmentsParent != null)
        {
            data.sourceObject = segmentsParent;
        }

        component.data = data;
    }

    private Vector3 GetSmallestZPointFromMesh(Transform mashTransform)
    {
        var mesh = mashTransform.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        float best = verts[0].z;

        for (int i = 0; i < verts.Length; i++)
        {
            //float globalZ = mashTransform.TransformPoint(verts[i]).z;
            float globalZ = verts[i].z;
            if (globalZ < best) best = globalZ;
        }
        
        return new Vector3(0f, 0f, best);
    }
}
