using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnergyLauncher : MonoBehaviour
{
    public static EnergyLauncher instance = null;
    
    private GameObject RaycastSelectObject;
    public SymbolModel CurrentSymbolData;
    public float RotateSpeed;
    public float LaunchDelay;
    public bool StartNode;
    public bool DBG_ControlWithMouse = true;
    private MeshRenderer _meshRenderer;
    private LineRenderer _lineRenderer;
    private bool _launched;

    public bool Launched{
        set {_launched = value; }
        get { return _launched; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (StartNode)
            Initialize();
    }

    public void Initialize() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[] { transform.position, CursorControl.instance.CursorPosition});
        _meshRenderer = GetComponentInParent<MeshRenderer>();
        CurrentSymbolData = new SymbolModel();
    }

    private void Update() {
        if (!CursorControl.instance || _lineRenderer.positionCount < 2)
            return;

        UpdateRaycast();
        UpdateLinePosition();
        
        if (Input.GetKeyDown(KeyCode.Space) && !_launched){
            Launch();
        }
    }

    private void UpdateRaycast()
    {
        var launchDirection = CursorControl.instance.CursorPosition - transform.position;
        var launchDistance = launchDirection.magnitude;

        var ray = new Ray(transform.position, launchDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, launchDistance))
        {
            NodeBehaviour nodeObj = hit.transform.GetComponent<NodeBehaviour>();
            if (nodeObj != null)
            {
                RaycastSelectObject = nodeObj.gameObject;
            }
        }
        else
        {
            //  Reset reference so linerenderer defers to CursorPosition
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
        _launched = true;
        var launchDirection = CursorControl.instance.CursorPosition - transform.position;

        bool resetLaunch = true;
        
        //  Use object already set in UpdateRaycast
        if(RaycastSelectObject != null)
        {
            NodeBehaviour nodeObj = RaycastSelectObject.GetComponent<NodeBehaviour>();

            if (nodeObj != null)
            {
                if(nodeObj.IsSelected && nodeObj.IsOpen)
                {
                    EnergizeNode(RaycastSelectObject);
                    CurrentSymbolData.ProcessUpdate(nodeObj._keyData);
                    nodeObj.OnTravelTo();
                    resetLaunch = false;
                }
                else 
                {
                    nodeObj.FlashWarningColor();
                }
            }
        }
        
        if(resetLaunch)
        {
            _launched = false;
        }
    }

    private void EnergizeNode(GameObject node) {

        //  Parent this object to the new node
        this.transform.SetParent(node.transform);
        this.transform.localPosition = Vector3.zero;

        //  Update this NodeBehaviour
        NodeBehaviour nodeBehaviour = GetComponentInParent<NodeBehaviour>();
        nodeBehaviour.OnTravelFrom();

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

    private IEnumerator LaunchCooldown(){
        yield return new WaitForSeconds(LaunchDelay);
        _launched = false;
    }

    private IEnumerator SelfDestruct(){
        yield return new WaitForSeconds(0.01f);
        Destroy(this);
    }
}
