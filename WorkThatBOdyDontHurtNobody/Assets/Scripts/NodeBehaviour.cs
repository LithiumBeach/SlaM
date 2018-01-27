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
            ActiveKeys[i] = (Random.Range(0, 100) > 50);
        }
    }

    public void RandomizeMethod()
    {
        _currentMethod = (eUpdateMethod)Random.Range(0, (int)eUpdateMethod.COUNT);
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

    public string DebugKeys()
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
    public SymbolModel _keyData;
    public bool IsOpen;

	// Use this for initialization
	void Start ()
    {
        _keyData = new SymbolModel();
        _keyData.RandomizeActiveKeys();
        _keyData.RandomizeMethod();

        ResetNode();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnTravelTo()
    {
        IsOpen = false;
    }

    public SymbolModel GetSymbolUpdate()
    {
        return null;
    }

    private void ResetNode()
    {
        IsOpen = true;
    }

}
