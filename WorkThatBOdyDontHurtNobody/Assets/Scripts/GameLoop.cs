using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public bool[] CorrectKeyFormation = new bool[(int)SymbolModel.eKey.COUNT];

    
	// Use this for initialization
	void Start ()
    {
        Initialize();
        RegisterEvents();
        RandomizeCorrectKey();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
	}

    private void Initialize()
    {
        for(int i = 0; i < (int)SymbolModel.eKey.COUNT; ++i)
        {
            CorrectKeyFormation[i] = false;
        }
    }

    private void RegisterEvents()
    {
        NodeBehaviour.OnNodeTraversed += HeardTraverseNode;
    }

    private void RandomizeCorrectKey()
    {
        for (int i = 0; i < (int)SymbolModel.eKey.COUNT; ++i)
        {
            CorrectKeyFormation[i] = (Random.Range(0, 100) > 50);
        }
    }

    private void HeardTraverseNode()
    {
        if(EnergyLauncher.instance.CurrentSymbolData.IsMatchingKey(CorrectKeyFormation))
        {
            Debug.Log("GAME WON!");
        }

        if(NodeBehaviour.AllNodesClosed())
        {
            ResetGame();
        }

    }

    private void ResetGame()
    {
        NodeBehaviour.ResetNodes();

        //  Move back to START NODE
    }


}
