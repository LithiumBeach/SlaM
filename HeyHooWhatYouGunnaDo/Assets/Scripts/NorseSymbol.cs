using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class NorseSymbol : MonoBehaviour
{
    public int dotsWidth = 3;
    public int dotsHeight = 4;

    private NorseDot[,] m_Dots;

    private Stack<NorseDot> m_ActivatedDotStack = new Stack<NorseDot>();
    [SerializeField]
    private List<NorseLine> m_Lines = new List<NorseLine>();

    void Awake()
    {
        if (!Application.isPlaying)
        {
            ReInitializeDots(); 
        }
    }

    private void OnDotActivate(NorseDot dot)
    {
        NorseDot prevDot = m_ActivatedDotStack.Count > 0 ? m_ActivatedDotStack.Peek() : null;
        dot.IsActivated = true;
        if (prevDot != null)
        {
            if (dot.m_GridPositionX == prevDot.m_GridPositionX && dot.m_GridPositionY == prevDot.m_GridPositionY)
            {
                return;
            }

            NorseLine nlCheck = m_Lines.Find(
                item => ((item.A.m_GridPositionX == dot.m_GridPositionX && item.A.m_GridPositionY == dot.m_GridPositionY) &&
                         (item.B.m_GridPositionX == prevDot.m_GridPositionX && item.B.m_GridPositionY == prevDot.m_GridPositionY)));
            if (nlCheck == null)
            {
                nlCheck = m_Lines.Find(
                item => ((item.A.m_GridPositionX == prevDot.m_GridPositionX && item.A.m_GridPositionY == prevDot.m_GridPositionY) &&
                         (item.B.m_GridPositionX == dot.m_GridPositionX && item.B.m_GridPositionY == dot.m_GridPositionY)));
                if (nlCheck == null)//if we haven't already made this line, bidirectionally, then add it.
                {
                    m_ActivatedDotStack.Push(dot);

                    //dot.m_LineRenderer.SetPositions(new Vector3[] { dot.transform.position, prevDot.transform.position });
                    NorseLine nl = new NorseLine();
                    LineRenderer l = Instantiate(PrefabManager.Instance.m_Data.m_DotConnectLR.gameObject, transform).GetComponent<LineRenderer>();
                    nl.line = l;
                    nl.A = prevDot;
                    nl.B = dot;
                    if (m_Lines.Contains(nl))
                    {
                        nl.CleanUp();
                        return;
                    }
                    m_Lines.Add(nl);
                }
            }
        }
        else
        {
            m_ActivatedDotStack.Push(dot);
        }
    }

    private void OnDotDeactivate(NorseDot dot)
    {
        while (m_ActivatedDotStack.Count > 0)
        {
            NorseDot iterPop = m_ActivatedDotStack.Pop();
            iterPop.IsActivated = false;

            if (m_Lines.Count > 0)
            {
                m_Lines[m_Lines.Count - 1].CleanUp();
                m_Lines.RemoveAt(m_Lines.Count - 1);
            }
            NorseLine l = m_Lines.Find(
                item => ((item.A.m_GridPositionX == iterPop.m_GridPositionX && item.A.m_GridPositionY == iterPop.m_GridPositionY) ||
                         (item.B.m_GridPositionX == iterPop.m_GridPositionX && item.B.m_GridPositionY == iterPop.m_GridPositionY)));
            iterPop.IsActivated = l != null;
            if (iterPop == dot)
            {
                return;
            }
        }
    }

    public void ReInitializeDots()
    {
        //final sweep of children
        while (transform.childCount > 0)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
#else
            GameObject.Destroy(transform.GetChild(transform.childCount - 1).gameObject);
#endif
        }

        //initialize all dots.
        m_Dots = new NorseDot[dotsWidth, dotsHeight];
        for (int y = 0; y < dotsHeight; y++)
        {
            for (int x = 0; x < dotsWidth; x++)
            {
                NorseDot iter = Instantiate(PrefabManager.Instance.m_Data.m_NorseDot.gameObject, transform).GetComponent<NorseDot>();
                //iter.m_SprDot.sprite = UIManager.Instance.m_Data.m_InactiveDot; //should be unnecessary, this should be set in the prefab
                iter.name = "dot (" + x.ToString() + " ," + y.ToString() + ")";
                iter.transform.position = new Vector3(x, y + (x % 2 == 1 ? .5f : 0f), 0);
                iter.m_GridPositionX = x;
                iter.m_GridPositionY = y;
                iter.AOnActivate = OnDotActivate;
                iter.AOnDeactivate = OnDotDeactivate;

                m_Dots[x, y] = iter;
            }
        }
    }
}
