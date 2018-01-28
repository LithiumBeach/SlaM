using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public bool[] CorrectKeyFormation = new bool[(int)SymbolModel.eKey.COUNT];
    public Level CurrentLevel;
    public NorseSymbol m_CurrentSymbol;
    public NorseSymbol m_CompleteConditionSymbol;

    public Level[] Levels;
    private int _currentLevelIndex;

    // Use this for initialization
    void Start()
    {
        Initialize();
        RegisterEvents();
        RandomizeCorrectKey();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CurrentLevel.CompleteLevel();
        }
    }

    private void Initialize()
    {
        if (Levels.Length > 0)
        {
            CurrentLevel = Levels[0];
            _currentLevelIndex = 0;
        }
        for (int i = 0; i < (int)SymbolModel.eKey.COUNT; ++i)
        {
            CorrectKeyFormation[i] = false;
        }
    }

    public void IncrementLevel()
    {
        if (_currentLevelIndex != Levels.Length - 1)
        {
            _currentLevelIndex++;
            CurrentLevel = Levels[_currentLevelIndex];
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

    private void HeardTraverseNode(NodeBehaviour node)
    {
        switch (node.m_Operator)
        {
            case EBoolOperator.Union:
                m_CurrentSymbol.UnionWith(node.m_Symbol);
                break;
            case EBoolOperator.Intersection:
                m_CurrentSymbol.IntersectionWith(node.m_Symbol);
                break;
            case EBoolOperator.Complement:

                m_CurrentSymbol.UnionWith(node.m_Symbol);
                break;
            default:
                break;
        }
        if (EnergyLauncher.instance.CurrentSymbolData.IsMatchingKey(CorrectKeyFormation))
        {
            Debug.Log("GAME WON!");
        }

        if (NodeBehaviour.AllNodesClosed())
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
