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
    [SerializeField]
    private List<NorseDot> m_Dots;
    [SerializeField]
    private List<NorseDot> m_ActivatedDotStack = new List<NorseDot>();
    [SerializeField]
    public List<NorseLine> m_Lines = new List<NorseLine>();


    public Color m_DotsColor = Color.black;
    public Gradient m_LinesColorGradient;

    public float m_DotSpacingScale = .25f;

    private void OnDotActivate(NorseDot dot)
    {
        NorseDot prevDot = m_ActivatedDotStack.Count > 0 ? m_ActivatedDotStack[m_ActivatedDotStack.Count - 1] : null;
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
                    m_ActivatedDotStack.Add(dot);

                    //dot.m_LineRenderer.SetPositions(new Vector3[] { dot.transform.position, prevDot.transform.position });
                    NorseLine nl = new NorseLine();
                    LineRenderer l = Instantiate(PrefabManager.Instance.m_Data.m_DotConnectLR.gameObject, transform).GetComponent<LineRenderer>();
                    l.colorGradient = m_LinesColorGradient;
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
            m_ActivatedDotStack.Add(dot);
        }
    }

    public void UnionWith(NorseSymbol otherSymbol)
    {
        List<NorseLine> linesToAdd = new List<NorseLine>();

        for (int i = 0; i < otherSymbol.m_Lines.Count; i++)
        {
            //check if line already exists (bidirectionally)
            NorseLine foundLine = m_Lines.Find(mline =>
            ((mline.A.m_GridPositionX == otherSymbol.m_Lines[i].A.m_GridPositionX && mline.A.m_GridPositionY == otherSymbol.m_Lines[i].A.m_GridPositionY) &&
            (mline.B.m_GridPositionX == otherSymbol.m_Lines[i].B.m_GridPositionX && mline.B.m_GridPositionY == otherSymbol.m_Lines[i].B.m_GridPositionY)) ||
            ((mline.A.m_GridPositionX == otherSymbol.m_Lines[i].B.m_GridPositionX && mline.A.m_GridPositionY == otherSymbol.m_Lines[i].B.m_GridPositionY) &&
            (mline.B.m_GridPositionX == otherSymbol.m_Lines[i].A.m_GridPositionX && mline.B.m_GridPositionY == otherSymbol.m_Lines[i].A.m_GridPositionY)) );

            //add line if doesn't exist.
            if (foundLine == null)
            {
                NorseLine nl = new NorseLine();
                LineRenderer l = Instantiate(PrefabManager.Instance.m_Data.m_DotConnectLR.gameObject, transform).GetComponent<LineRenderer>();
                nl.line = l;
                nl.A = m_Dots.Find(item => item.m_GridPositionX == otherSymbol.m_Lines[i].A.m_GridPositionX && item.m_GridPositionY == otherSymbol.m_Lines[i].A.m_GridPositionY);
                nl.A.IsActivated = true;
                nl.B = m_Dots.Find(item => item.m_GridPositionX == otherSymbol.m_Lines[i].B.m_GridPositionX && item.m_GridPositionY == otherSymbol.m_Lines[i].B.m_GridPositionY);
                nl.B.IsActivated = true;
                m_Lines.Add(nl);
            }
        }
    }

    public void IntersectionWith(NorseSymbol otherSymbol)
    {
        List<NorseLine> linesToRemove = new List<NorseLine>();
        for (int i = 0; i < m_Lines.Count; i++)
        {
            NorseLine nlCheck = otherSymbol.m_Lines.Find(
                   item => ((item.A.m_GridPositionX == m_Lines[i].A.m_GridPositionX && item.A.m_GridPositionY == m_Lines[i].A.m_GridPositionY) &&
                            (item.B.m_GridPositionX == m_Lines[i].B.m_GridPositionX && item.B.m_GridPositionY == m_Lines[i].B.m_GridPositionY)));
            if (nlCheck == null)
            {
                nlCheck = otherSymbol.m_Lines.Find(
                item => ((item.A.m_GridPositionX == m_Lines[i].B.m_GridPositionX && item.A.m_GridPositionY == m_Lines[i].B.m_GridPositionY) &&
                         (item.B.m_GridPositionX == m_Lines[i].A.m_GridPositionX && item.B.m_GridPositionY == m_Lines[i].A.m_GridPositionY)));
            }

            if (nlCheck != null)
            {
                //we got em boys
                linesToRemove.Add(m_Lines[i]);
            }
        }

        //check for IsActive on dots
        for (int i = 0; i < linesToRemove.Count; i++)
        {
            linesToRemove[i].CleanUp();

            m_Lines.Remove(linesToRemove[i]);

            //set IsActivated to false if there are no more lines connected to this dot.
            NorseLine lA = m_Lines.Find(
                item => (item.A.m_GridPositionX == linesToRemove[i].A.m_GridPositionX && item.A.m_GridPositionY == linesToRemove[i].A.m_GridPositionY) ||
                        (item.B.m_GridPositionX == linesToRemove[i].A.m_GridPositionX && item.B.m_GridPositionY == linesToRemove[i].A.m_GridPositionY)
                );
            NorseLine lB = m_Lines.Find(
                item => (item.A.m_GridPositionX == linesToRemove[i].B.m_GridPositionX && item.A.m_GridPositionY == linesToRemove[i].B.m_GridPositionY) ||
                        (item.B.m_GridPositionX == linesToRemove[i].B.m_GridPositionX && item.B.m_GridPositionY == linesToRemove[i].B.m_GridPositionY)
                );

            linesToRemove[i].A.IsActivated = lA != null;
            linesToRemove[i].B.IsActivated = lB != null;
        }

    }

    public void ComplementWith(NorseSymbol otherSymbol)
    {// TODO: stub
        throw new NotImplementedException();
    }

    public bool IsEquivalentSymbol(NorseSymbol otherSymbol)
    {
        for (int i = 0; i < otherSymbol.m_Lines.Count; i++)
        {
            //check if line already exists (bidirectionally)
            NorseLine foundLine = m_Lines.Find(mline =>
            ((mline.A.m_GridPositionX == otherSymbol.m_Lines[i].A.m_GridPositionX && mline.A.m_GridPositionY == otherSymbol.m_Lines[i].A.m_GridPositionY) &&
            (mline.B.m_GridPositionX == otherSymbol.m_Lines[i].B.m_GridPositionX && mline.B.m_GridPositionY == otherSymbol.m_Lines[i].B.m_GridPositionY)) ||
            ((mline.A.m_GridPositionX == otherSymbol.m_Lines[i].B.m_GridPositionX && mline.A.m_GridPositionY == otherSymbol.m_Lines[i].B.m_GridPositionY) &&
            (mline.B.m_GridPositionX == otherSymbol.m_Lines[i].A.m_GridPositionX && mline.B.m_GridPositionY == otherSymbol.m_Lines[i].A.m_GridPositionY)));

            if(foundLine == null)
            {
                return false;
            }

        }
        return true;
    }

    private void OnDotDeactivate(NorseDot dot)
    {
        while (m_ActivatedDotStack.Count > 0)
        {
            NorseDot iterPop = m_ActivatedDotStack[m_ActivatedDotStack.Count - 1];
            m_ActivatedDotStack.RemoveAt(m_ActivatedDotStack.Count - 1);
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

    internal void UpdateAllDotDelegates()
    {
        for (int i = 0; i < m_Dots.Count; i++)
        {
            m_Dots[i].AOnActivate += OnDotActivate;
            m_Dots[i].AOnDeactivate += OnDotDeactivate;
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
        m_Lines.Clear();
        m_ActivatedDotStack.Clear();
        //initialize all dots.
        m_Dots = new List<NorseDot>();// new NorseDot[dotsWidth, dotsHeight];
        for (int y = 0; y < dotsHeight; y++)
        {
            for (int x = 0; x < dotsWidth; x++)
            {
                NorseDot iter = Instantiate(PrefabManager.Instance.m_Data.m_NorseDot.gameObject, transform).GetComponent<NorseDot>();
                //iter.m_SprDot.sprite = UIManager.Instance.m_Data.m_InactiveDot; //should be unnecessary, this should be set in the prefab
                iter.name = "dot (" + x.ToString() + " ," + y.ToString() + ")";
                iter.transform.localPosition = new Vector3((x * m_DotSpacingScale), (y * m_DotSpacingScale) + (x % 2 == 1 ? (.5f * m_DotSpacingScale) : 0f), 0);
                iter.m_GridPositionX = x;
                iter.m_GridPositionY = y;
                iter.AOnActivate = OnDotActivate;
                iter.AOnDeactivate = OnDotDeactivate;
                iter.m_SprDot.color = m_DotsColor;

                //m_Dots[x][y] = iter;
                m_Dots.Add(iter);
            }
        }
    }
}
