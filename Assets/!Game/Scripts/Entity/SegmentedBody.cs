using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SegmentedBody : MonoBehaviour
{
    [SerializeField] private int segmentsAmount;
    [SerializeField] private float biggestSegmentScale;
    [SerializeField] private AnimationCurve segmentsScaleDistribution;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private Transform rig;
    [SerializeField] private Transform rigComponent;
    [SerializeField] private RigBuilder rigBuilder;
    private Transform _head;
    
    // pick -z extreme point at mesh
    // instantiate segment as child
    
    // set scale based on max size and curve
    // set rig

    private void Start()
    {
        _head = transform.GetChild(0);
        
        InstantiateBody(segmentsAmount);
        rigBuilder.Build();
    }
    
    private void InstantiateBody(int tailSegments)
    {
        var lastSegment = _head;
        _head.localScale = GetSegmentScale(0);
        
        for (int i = 1; i <= tailSegments; i++) // accounting head
        {
            lastSegment = Instantiate(
                segmentPrefab, 
                GetSmallestZPointFromMesh(lastSegment), 
                Quaternion.identity,
                lastSegment);

            lastSegment.localScale = GetSegmentScale(i);
            SetRigForSegment(lastSegment);
        }
    }


    private float _lastScale = 1;
    private Vector3 GetSegmentScale(int segmentIndex)
    {
        float value = Mathf.InverseLerp(0, segmentsAmount, segmentIndex);
        float scale = segmentsScaleDistribution.Evaluate(value);
        scale *= biggestSegmentScale;
        
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

    private Vector3 GetSmallestZPointFromMesh(Transform parentSegment)
    {
        var mesh = parentSegment.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        float best = verts[0].z;

        for (int i = 0; i < verts.Length; i++)
        {
            float globalZ = parentSegment.TransformPoint(verts[i]).z;
            if (globalZ < best) best = globalZ;
        }
        
        return new Vector3(0f, 0f, best);
    }
}
