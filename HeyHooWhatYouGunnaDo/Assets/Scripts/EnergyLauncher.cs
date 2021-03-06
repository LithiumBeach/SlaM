﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnergyLauncher : MonoBehaviour
{
    public static EnergyLauncher instance = null;
    public LineRenderer _lineRenderer;

    private GameObject RaycastSelectObject;
    public float RotateSpeed;
    public float LaunchDelay;
    public bool StartNode;
    public bool DBG_ControlWithMouse = true;
    private MeshRenderer _meshRenderer;
    private bool _launched;

    public bool Launched
    {
        set { _launched = value; }
        get { return _launched; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
            Initialize();
    }

    public void Initialize()
    {
        _lineRenderer = this.GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[] { transform.position, CursorControl.instance.CursorPosition });
        _meshRenderer = GetComponentInParent<MeshRenderer>();
    }

    private void Update()
    {
        if (!CursorControl.instance || _lineRenderer == null)
            return;

        UpdateRaycast();
        UpdateLinePosition();

        if (Input.GetButtonDown("Fire1") && !_launched)
        {
            Launch();
        }
    }

    private void UpdateRaycast()
    {
        var launchDirection = CursorControl.instance.CursorPosition - transform.position;
        //var launchDistance = launchDirection.magnitude;

        var ray = new Ray(transform.position, launchDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance: 1000f, layerMask: 1 << 9))
        {
            NodeBehaviour nodeObj = hit.transform.GetComponent<NodeBehaviour>();
            if (nodeObj != null && nodeObj.gameObject != RaycastSelectObject)
            {
                if (RaycastSelectObject != null)
                {
                    RaycastSelectObject.GetComponent<NodeBehaviour>().SetAsRaycastHit(false);
                }

                RaycastSelectObject = nodeObj.gameObject;
                RaycastSelectObject.GetComponent<NodeBehaviour>().SetAsRaycastHit(true);
            }
        }
        else
        {
            //  Reset reference so linerenderer defers to CursorPosition
            if (RaycastSelectObject != null)
            {
                RaycastSelectObject.GetComponent<NodeBehaviour>().SetAsRaycastHit(false);
            }

            RaycastSelectObject = null;
        }
    }

    protected virtual void UpdateLinePosition()
    {
        Vector3 endPosition = RaycastSelectObject == null ? CursorControl.instance.CursorPosition : RaycastSelectObject.transform.position;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, endPosition);
    }

    private void Launch()
    {
        _launched = false;

        //  Use object already set in UpdateRaycast
        if (RaycastSelectObject != null)
        {
            NodeBehaviour newNode = RaycastSelectObject.GetComponent<NodeBehaviour>();

            if (newNode != null)
            {
                if (newNode.IsCursorSelected && newNode.IsOpen)
                {
                    //  Update this NodeBehaviour
                    NodeBehaviour currentNode = GetComponentInParent<NodeBehaviour>();
                    currentNode.OnTravelFrom();

                    SetToNode(RaycastSelectObject);
                    newNode.OnTravelTo();
                    _launched = true;
                    
                    GameLoop.Instance.CurrentLevel.PushLightLine(currentNode.transform, newNode.transform);
                  
                }
                else
                {
                    newNode.FlashWarningColor();
                }
            }
        }
    }

    private void SetToNode(GameObject node)
    {
        //  Parent this object to the new node
        this.transform.SetParent(node.transform);
        this.transform.localPosition = Vector3.zero;

        //Reset and set a cooldown
        Recharge();
    }

    public void Recharge()
    {
        StartNode = false;
        Launched = true;
        Initialize();
        StartCoroutine(LaunchCooldown());
    }

    public void ResetToNode(GameObject node)
    {
        SetToNode(node);
    }

    private IEnumerator LaunchCooldown()
    {
        yield return new WaitForSeconds(LaunchDelay);
        _launched = false;
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(this);
    }
}
