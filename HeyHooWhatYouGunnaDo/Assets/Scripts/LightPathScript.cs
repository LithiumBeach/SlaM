using patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Attach this script to an empty gameobject
//  This script will recieve inputs for new Line segments, creating objects and appending them to root gameobject

public class LightPathScript : MonoBehaviour
{
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    public GameObject LinePrefab;

    public void ClearAllLines()
    {
        for(int i = 0; i < _lineRenderers.Count; ++i)
        {
            _lineRenderers[i].positionCount = 0;
            DestroyObject(_lineRenderers[i].gameObject, 2f);
        }
        _lineRenderers.Clear();
    }

    public void PushNewLine(Transform pointA, Transform pointB)
    {
        LineRenderer newLine = GameObject.Instantiate(LinePrefab).GetComponent<LineRenderer>();
        newLine.transform.SetParent(this.transform);

        newLine.positionCount = 2;
        newLine.SetPositions(new Vector3[] { pointA.position, pointB.position });

        _lineRenderers.Add(newLine);
    }
}
