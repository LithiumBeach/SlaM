using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBoolOperator
{
    Union,
    Intersection,
    Complement,
    EndComparison
}

public class NodeBehaviour : MonoBehaviour
{
    private enum eNodeActivity
    {
        Idle,
        Flashing
    }
    
    public bool IsOpen;
    public bool IsCursorSelected;
    public bool IsRaycastSelected;
    private Color _blockingColor = Color.cyan;
    public Color _warningColor = Color.magenta;
    public Color _selectColor = Color.yellow;
    public NorseSymbol m_Symbol = null;
    private Color _currentColor;
    private eNodeActivity _currentActivity = eNodeActivity.Idle;

    public delegate void NodeTraversed();
    public static Action<NodeBehaviour> OnNodeTraversed;

    private static List<NodeBehaviour> AllNodes = new List<NodeBehaviour>();

    public EBoolOperator m_Operator = EBoolOperator.Union;

    // Use this for initialization
    void Start ()
    {
        AllNodes.Add(this);
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
        ChangeCurrentColor(Color.red);
    }

    public void OnTravelTo()
    {
        IsOpen = false;
        ChangeCurrentColor(Color.red);

        if (OnNodeTraversed != null)
        {
            OnNodeTraversed(this);
        }
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
