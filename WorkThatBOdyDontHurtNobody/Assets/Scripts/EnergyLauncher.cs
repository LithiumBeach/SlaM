using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(MeshRenderer), typeof(MeshCollider))]
public class EnergyLauncher : MonoBehaviour {

    public GameObject LaunchDirector;
    public Material FocusMaterial;
    public Material UnfocusMaterial;
    public float RotateSpeed;
    public float LaunchDistance;
    public float LaunchDelay;
    public bool StartNode;
    private MeshRenderer _meshRenderer;
    private LineRenderer _lineRenderer;
    private bool _launched;

    private SymbolModel _currentSymbolData;

    public bool Launched{
        set {_launched = value; }
        get { return _launched; }
    }

    private void Awake() {
        if (StartNode)
            Initialize();
    }

    public void Initialize() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[] { transform.position, LaunchDirector.transform.position });
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = FocusMaterial;
        _currentSymbolData = new SymbolModel();
    }

    private void Update() {
        if (!LaunchDirector || _lineRenderer.positionCount < 2)
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
            if (nodeObj != null && nodeObj.IsOpen)
            {
                EnergizeNode(hit.transform.gameObject);
                nodeObj.OnTravelTo();
                _currentSymbolData.ProcessUpdate(nodeObj._keyData);
                Debug.Log("Player Keys: " + _currentSymbolData.DebugKeys());
                resetLaunch = false;
            }
        }
        
        if(resetLaunch)
        {
            _launched = false;
        }
    }

    private void EnergizeNode(GameObject node) {
        //Copy the EnergyLauncher component to next node
        System.Type EnergyLauncher = this.GetType();
        var nextNodeEnergyLauncher = node.AddComponent(EnergyLauncher) as EnergyLauncher;
        System.Reflection.FieldInfo[] fields = EnergyLauncher.GetFields();
        foreach (var field in fields) {
            field.SetValue(nextNodeEnergyLauncher, field.GetValue(this));
        }

        //Reposition the LaunchDirector relative to next node
        var directorDistance = (transform.position - LaunchDirector.transform.position).magnitude;
        var translateDirection = (node.transform.position - LaunchDirector.transform.position).normalized;
        LaunchDirector.transform.position = node.transform.position + (translateDirection * directorDistance);

        //Set material to unfocused
        _meshRenderer.material = UnfocusMaterial;

        //Energize the next node and destroy this energyLauncher
        nextNodeEnergyLauncher.Energize(LaunchDirector);
        StartCoroutine(SelfDestruct());
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
        _lineRenderer.positionCount = 0;
        Destroy(this);
    }

    protected virtual void UpdateLinePosition()
    {
        // LaunchDirector.transform.RotateAround(transform.position, Vector3.forward, (-RotateSpeed * Time.deltaTime));


        // Vector3 mouseDelta = Camera.main.ScreenToViewportPoint(Input.mousePosition) - Camera.main.WorldToViewportPoint(this.transform.position);
        // float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;
        // if (angle < 0) angle += 360;
        //
        // while((angle / 360) > 1)
        // {
        //     angle -= 360;
        // }
        //
        // Debug.Log(angle);
        //
        // LaunchDirector.transform.RotateAround(transform.position, Vector3.forward, angle);

        _lineRenderer.SetPosition(0, transform.position);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 endPoint = transform.position + (mousePos - transform.position).normalized * 2f;
        endPoint.z = 0;
        _lineRenderer.SetPosition(1, endPoint);

        LaunchDirector.transform.position = endPoint;
    }
}
