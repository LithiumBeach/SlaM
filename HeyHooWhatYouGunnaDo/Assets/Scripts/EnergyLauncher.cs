using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyLauncher : MonoBehaviour
{
    public static EnergyLauncher instance = null;
    
    public GameObject LaunchDirector;
    public Material FocusMaterial;
    public Material UnfocusMaterial;
    public SymbolModel CurrentSymbolData;
    public float RotateSpeed;
    public float LaunchDistance;
    public float LaunchDelay;
    public bool StartNode;
    public bool DBG_ControlWithMouse = true;
    private MeshRenderer _meshRenderer;
  //  private LineRenderer _lineRenderer;
    private bool _launched;

    public bool Launched{
        set {_launched = value; }
        get { return _launched; }
    }

    private void Awake() {
        if (StartNode)
            Initialize();

        instance = this;
    }

    public void Initialize() {
   //     _lineRenderer = GetComponentInParent<LineRenderer>();
   //     _lineRenderer.positionCount = 2;
   //     _lineRenderer.SetPositions(new Vector3[] { transform.position, LaunchDirector.transform.position });
        _meshRenderer = GetComponentInParent<MeshRenderer>();
        _meshRenderer.material = FocusMaterial;
        CurrentSymbolData = new SymbolModel();
    }

    private void Update() {
        if (!LaunchDirector)// || _lineRenderer.positionCount < 2)
            return;

        UpdateLinePosition();
        
        if (Input.GetKeyDown(KeyCode.Space) && !_launched){
            Launch();
        }
    }

    private void Launch() {
        _launched = true;
        var launchDirection = LaunchDirector.transform.position - transform.position;

        var ray = new Ray(transform.position, launchDirection);
        bool resetLaunch = true;
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, LaunchDistance))
        {
            NodeBehaviour nodeObj = hit.transform.GetComponent<NodeBehaviour>();

            if (nodeObj != null)
            {
                if (nodeObj.IsSelected)
                {
                    if(nodeObj.IsOpen)
                    {
                        EnergizeNode(hit.transform.gameObject);
                        CurrentSymbolData.ProcessUpdate(nodeObj._keyData);
                        nodeObj.OnTravelTo();
                        resetLaunch = false;
                    }
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

    //    _lineRenderer.positionCount = 0;

        //  Update this NodeBehaviour
        NodeBehaviour nodeBehaviour = GetComponentInParent<NodeBehaviour>();
        nodeBehaviour.OnTravelFrom();

        //Energize the next node
        Energize(LaunchDirector);

        {
            ////Copy the EnergyLauncher component to next node
            //System.Type EnergyLauncher = this.GetType();
            //var nextNodeEnergyLauncher = node.AddComponent(EnergyLauncher) as EnergyLauncher;
            //System.Reflection.FieldInfo[] fields = EnergyLauncher.GetFields();
            //foreach (var field in fields) {
            //    field.SetValue(nextNodeEnergyLauncher, field.GetValue(this));
            //}

            ////Reposition the LaunchDirector relative to next node
            //var directorDistance = (transform.position - LaunchDirector.transform.position).magnitude;
            //var translateDirection = (node.transform.position - LaunchDirector.transform.position).normalized;
            //LaunchDirector.transform.position = node.transform.position + (translateDirection * directorDistance);

            ////Set material to unfocused
            //_meshRenderer.material = UnfocusMaterial;

            ////  Update this NodeBehaviour
            //NodeBehaviour nodeBehaviour = this.GetComponent<NodeBehaviour>();
            //nodeBehaviour.OnTravelFrom();

            ////Energize the next node and destroy this energyLauncher
            //nextNodeEnergyLauncher.Energize(LaunchDirector);
            //StartCoroutine(SelfDestruct());
        }
    }

    public void Energize(GameObject launchDirector) {
        StartNode = false;
        Launched = true;
        LaunchDirector = launchDirector;
        Initialize();
        _meshRenderer.material = FocusMaterial;
        StartCoroutine(LaunchCooldown());
    }

    private IEnumerator LaunchCooldown(){
        yield return new WaitForSeconds(LaunchDelay);
        _launched = false;
    }

    private IEnumerator SelfDestruct(){
        yield return new WaitForSeconds(0.01f);
   //     _lineRenderer.positionCount = 0;
        Destroy(this);
    }

    protected virtual void UpdateLinePosition()
    {
        if(!DBG_ControlWithMouse)
        {
            float axisFactor = Input.GetAxis("Horizontal");

            LaunchDirector.transform.RotateAround(transform.position, Vector3.forward, axisFactor * -RotateSpeed);
   //         _lineRenderer.SetPosition(1, LaunchDirector.transform.position);
        }
        else
        {
   //         _lineRenderer.SetPosition(0, transform.position);
            Vector3 targetPos = CursorControl.instance.CursorPosition;// Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;
            Vector3 endPoint = transform.position + (targetPos - transform.position).normalized * 2f;
            endPoint.z = 0;
   //         _lineRenderer.SetPosition(1, endPoint);

            LaunchDirector.transform.position = endPoint;
        }
    }
}
