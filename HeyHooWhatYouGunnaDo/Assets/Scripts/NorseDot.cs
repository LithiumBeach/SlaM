using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

[System.Serializable]
[ExecuteInEditMode]
public class NorseDot : MonoBehaviour
{
    public SpriteRenderer m_SprDot;
    public SphereCollider m_Col;
    private bool m_Activated = false;
    public bool IsActivated
    {
        get { return m_Activated; }
        set
        {
            m_Activated = value;
            m_SprDot.sprite = m_Activated ? UIManager.Instance.m_Data.m_ActiveDot : UIManager.Instance.m_Data.m_InactiveDot;
        }
    }
    [HideInInspector]
    public int m_GridPositionX;
    [HideInInspector]
    public int m_GridPositionY;

    private void OnEnable()
    {
        m_SprDot.enabled = true;
        m_Col.enabled = true;
    }

    private void OnDisable()
    {
        m_SprDot.enabled = false;
        m_Col.enabled = false;
    }

    public Action<NorseDot> AOnActivate;
    public Action<NorseDot> AOnDeactivate;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            IsActivated = true;
            if (AOnActivate != null)
            {
                AOnActivate(this); 
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            IsActivated = false;
            if (AOnDeactivate != null)
            {
                AOnDeactivate(this); 
            }
        }
    }


    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.Layout || e.type == EventType.Repaint)
        {
            EditorUtility.SetDirty(this); // this is important, if omitted, "Mouse down" will not be display
        }
        else if (e.type == EventType.MouseDown)
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(r, out hit))
            {
                if (hit.transform.gameObject != gameObject)
                    return;
                if (e.button == 0)
                {
                    IsActivated = true;
                    if (AOnActivate != null)
                    {
                        AOnActivate(this);
                    }
                }
                else if (e.button == 1)
                {
                    IsActivated = false;
                    if (AOnDeactivate != null)
                    {
                        AOnDeactivate(this);
                    }
                }
            }
        }
    }
}
