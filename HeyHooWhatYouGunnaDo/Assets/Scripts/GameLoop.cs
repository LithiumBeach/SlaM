using patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : SingletonBehavior<GameLoop>
{
    public Level CurrentLevel;
    public NorseSymbol m_YourSymbol;

    public Level[] Levels;
    private int _currentLevelIndex;

    private Stack<NodeBehaviour> m_history = new Stack<NodeBehaviour>();

    // Use this for initialization
    void Start()
    {
        Initialize();
        RegisterEvents();
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

        NodeBehaviour nb = CurrentLevel.StartingNode.GetComponentInChildren<NodeBehaviour>();
        m_YourSymbol.UnionWith(nb.m_Symbol);
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

    private void HeardTraverseNode(NodeBehaviour node)
    {
        m_history.Push(node);

        bool hardReset = false;
        switch (node.m_Operator)
        {
            case EBoolOperator.Union:
                m_YourSymbol.UnionWith(node.m_Symbol);
                break;
            case EBoolOperator.Intersection:
                m_YourSymbol.IntersectionWith(node.m_Symbol);
                break;
            case EBoolOperator.Complement:

                m_YourSymbol.UnionWith(node.m_Symbol);
                break;
            case EBoolOperator.EndComparison:
                m_history.Pop();
                if (m_YourSymbol.IsEquivalentSymbol(CurrentLevel.m_SymbolToCompleteLevel))
                {
                    OnCorrectSymbol();
                }
                else
                {
                    hardReset = true;
                }
                break;
            default:
                break;
        }

        if (NodeBehaviour.AllNodesClosed() || hardReset)
        {
            ResetGame();
        }

    }

    private void OnCorrectSymbol()
    {
        Debug.Log("Game Won!");
        CurrentLevel.CompleteLevel();
    }

    private void ResetGame()
    {
        NodeBehaviour.ResetNodes();
        if(CurrentLevel)
            CurrentLevel.ResetLevel();

        //  Intersect
        m_YourSymbol.IntersectionWith(m_YourSymbol);
        //  Unionize
        m_YourSymbol.UnionWith(CurrentLevel.StartingNode.GetComponentInChildren<NodeBehaviour>().m_Symbol);
    }

    private void PopThroughHistory()
    {
        while(m_history.Count > 0)
        {
            NodeBehaviour node = m_history.Pop();

            switch (node.m_Operator)
            {
                case EBoolOperator.Union:
                    m_YourSymbol.UnionWith(node.m_Symbol);
                    break;
                case EBoolOperator.Intersection:
                    m_YourSymbol.IntersectionWith(node.m_Symbol);
                    break;
                case EBoolOperator.Complement:
                    break;
                case EBoolOperator.EndComparison:
                default:
                    Debug.Log("YOu shouldnt be here...");
                    break;
            }
        }
    }


}
