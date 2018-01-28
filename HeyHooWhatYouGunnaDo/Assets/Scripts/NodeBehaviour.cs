using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolModel
{
    public enum eKey
    {
        Key1,
        Key2,
        Key3,
        Key4,
        Key5,

        COUNT
    }
    public bool[] ActiveKeys = new bool[(int)eKey.COUNT];

    public enum eUpdateMethod
    {
        Union,
        Exclusive,
        Remove,

        COUNT
    }
    public eUpdateMethod _currentMethod;

    public SymbolModel()
    {

    }

    private void Initialize()
    {
        for (int i = 0; i < (int)eKey.COUNT; ++i)
        {
            ActiveKeys[i] = false;
        }
    }

    public void RandomizeActiveKeys()
    {
        for(int i = 0; i < (int)eKey.COUNT; ++i)
        {
            ActiveKeys[i] = (UnityEngine.Random.Range(0, 100) > 50);
        }
    }

    public void RandomizeMethod()
    {
        _currentMethod = (eUpdateMethod)UnityEngine.Random.Range(0, (int)eUpdateMethod.COUNT);
    }

    //  I don't like using ref too much, so a SymbolModel obj will update itself based on information from another SymbolModel obj... I'm so sorry
    public void ProcessUpdate(SymbolModel dataUpdate)
    {
        for(int i = 0; i < (int)eKey.COUNT; ++i)
        {
            switch(_currentMethod)
            {
                case eUpdateMethod.Union:
                    {
                        if (dataUpdate.ActiveKeys[i])
                        {
                            ActiveKeys[i] = true;
                        }
                    }
                    break;
                case eUpdateMethod.Exclusive:
                    {
                        if (dataUpdate.ActiveKeys[i] && ActiveKeys[i])
                        {
                            ActiveKeys[i] = true;
                        }
                        else
                        {
                            ActiveKeys[i] = false;
                        }
                    }
                    break;
                case eUpdateMethod.Remove:
                    {
                        if (dataUpdate.ActiveKeys[i])
                        {
                            ActiveKeys[i] = false;
                        }
                    }
                    break;
            }
        }
    }

    public bool IsMatchingKey(bool[] correctKey)
    {
        for(int i = 0; i < (int)eKey.COUNT; ++i)
        {
            if(ActiveKeys[i] != correctKey[i])
            {
                return false;
            }
        }

        return true;
    }

    public string GetActiveKeysDebug()
    {
        string debugString = "";
        for(int i = 0; i < (int)eKey.COUNT; ++i)
        {
            if(ActiveKeys[i])
                debugString += i.ToString() + ",";
        }
        return debugString;
    }


}

public class NodeBehaviour : MonoBehaviour
{
    private enum eNodeActivity
    {
        Idle,
        Flashing
    }

    public SymbolModel _keyData;
    public bool IsOpen;
    public bool IsCursorSelected;
    public bool IsRaycastSelected;
    public float WarningColorTime = 2f;
    private Color _blockingColor = Color.cyan;
    public Color _warningColor = Color.magenta;
    public Color _selectColor = Color.yellow;
    public NorseSymbol m_Symbol = null;
    private Color _currentColor;
    private eNodeActivity _currentActivity = eNodeActivity.Idle;

    public delegate void NodeTraversed();
    public static Action<NodeBehaviour> OnNodeTraversed;

    private static List<NodeBehaviour> AllNodes = new List<NodeBehaviour>();

	// Use this for initialization
	void Start ()
    {
        AllNodes.Add(this);

        _keyData = new SymbolModel();
        _keyData.RandomizeActiveKeys();
        _keyData.RandomizeMethod();

        Debug.Assert(m_Symbol != null);

        ResetNode();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(_currentActivity == eNodeActivity.Idle)
        {
            UpdateIdleColor();
        }
	}

    void UpdateIdleColor()
    {
        if(IsRaycastSelected && !IsCursorSelected)
        {
            SetBlockColor();
        }
        else if(IsCursorSelected && IsOpen)
        {
            SetSelectColor();
        }
        else
        {
            SetCurrentColor();
        }
    }

    public void OnTravelFrom()
    {
        IsOpen = false;
    }

    public void OnTravelTo()
    {
        IsOpen = false;
        if(OnNodeTraversed != null)
        {
            OnNodeTraversed(this);
        }

        ChangeCurrentColor(Color.red);
    }

    private void ResetNode()
    {
        IsOpen = true;
        IsCursorSelected = false;
        IsRaycastSelected = false;

        ChangeCurrentColor(Color.green);
    }

    public void ChangeCurrentColor(Color newColor)
    {
        _currentColor = newColor;
    }

    private void SetCurrentColor()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.color = _currentColor;
    }

    public void SetSelectColor()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.color = _selectColor;
    }

    public void SetBlockColor()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.color = _blockingColor;
    }

    public void SetAsRaycastHit(bool isHit)
    {
        IsRaycastSelected = isHit;    
    }

    public void FlashWarningColor()
    {
        _currentActivity = eNodeActivity.Flashing;
        StartCoroutine(ShowWarningColor());
    }

    private IEnumerator ShowWarningColor()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        renderer.material.color = _warningColor;
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = _currentColor;
        yield return new WaitForSeconds(0.2f);
        renderer.material.color = _warningColor;
        yield return new WaitForSeconds(0.1f);
        
        _currentActivity = eNodeActivity.Idle;
    }

    public static bool AllNodesClosed()
    {
        for(int i = 0; i < AllNodes.Count; ++i)
        {
            if(AllNodes[i].IsOpen)
            {
                return false;
            }
        }
        return true;
    }

    public static void ResetNodes()
    {
        for (int i = 0; i < AllNodes.Count; ++i)
        {
            AllNodes[i].ResetNode();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cursor") && IsOpen)
        {
            IsCursorSelected = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Cursor") && IsOpen)
        {
            IsCursorSelected = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Cursor"))
        {
            IsCursorSelected = false;
        }
    }


}
