using patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameLoop : SingletonBehavior<GameLoop>
{
    public Level CurrentLevel;
    public NorseSymbol m_YourSymbol;

    public Level[] Levels;
    private int _currentLevelIndex;

    private Stack<NodeBehaviour> m_history = new Stack<NodeBehaviour>();

    public Action AOnChangeLevel;

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
        MoveYourSymbolTo(nb.m_Symbol);
    }

    public void IncrementLevel()
    {
        if (_currentLevelIndex != Levels.Length - 1)
        {
            _currentLevelIndex++;

            //  Disable past level
            CurrentLevel.CleanLightLines();
            NodeBehaviour.ResetNodes();
            CurrentLevel.gameObject.SetActive(false);

            CurrentLevel = Levels[_currentLevelIndex];

            CurrentLevel.gameObject.SetActive(true);
            CurrentLevel.ResetLevel();

            if (AOnChangeLevel != null)
            {
                AOnChangeLevel();
            }
        }
    }

    private void RegisterEvents()
    {
        NodeBehaviour.OnNodeTraversed += HeardTraverseNode;
    }

    private void HeardTraverseNode(NodeBehaviour node)
    {
        if (m_history.Count > 0)
        {
            m_history.Peek().m_Symbol.gameObject.SetActive(true);
        }
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
                Debug.Log("You shouldn't be here bud");
                break;
        }

        MoveYourSymbolTo(node.m_Symbol);
        node.m_Symbol.gameObject.SetActive(false);

        if ((NodeBehaviour.AllNodesClosed() && !CurrentLevel.LevelComplete) || hardReset)
        {
            node.m_Symbol.gameObject.SetActive(true);
            ResetGame();
        }

    }

    private void MoveYourSymbolTo(NorseSymbol symb)
    {
        Vector3 delta = symb.transform.position - m_YourSymbol.transform.position;
        for (int i = 0; i < m_YourSymbol.m_Lines.Count; i++)
        {
            m_YourSymbol.m_Lines[i].line.SetPositions(new Vector3[] { m_YourSymbol.m_Lines[i].line.GetPosition(0) + delta, m_YourSymbol.m_Lines[i].line.GetPosition(1) + delta });
        }
        m_YourSymbol.transform.position = symb.transform.position;
    }

    private void OnCorrectSymbol()
    {
        Debug.Log("Game Won!");
        CurrentLevel.CompleteLevel();
    }

    private void ResetGame()
    {
        NodeBehaviour.ResetNodes();
        if (CurrentLevel)
            CurrentLevel.ResetLevel();

        //  Intersect
        m_YourSymbol.IntersectionWith(m_YourSymbol);
        //  Unionize
        m_YourSymbol.UnionWith(CurrentLevel.StartingNode.GetComponentInChildren<NodeBehaviour>().m_Symbol);
    }

    private void PopThroughHistory()
    {
        while (m_history.Count > 0)
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
