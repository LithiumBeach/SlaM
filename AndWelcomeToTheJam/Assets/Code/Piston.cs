using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour, IComparable
{
    public GameObject m_Pusher = null;

    public void Awake()
    {
        PistonManager.Instance.AddPiston(this);
    }

    public int CompareTo(object _rhs)
    {
        Piston rhs = _rhs as Piston;
        Debug.Assert(rhs != null);
        return transform.IsChildOf(rhs.transform) ? 1 : -1;
    }

    public void ExtendPusher()
    {
        m_Pusher.transform.position += m_Pusher.transform.up;
    }

    public void RetractPusher()
    {
        //m_Pusher.MovePosition(m_Pusher.transform.position + -m_Pusher.transform.up);
        m_Pusher.transform.position -= m_Pusher.transform.up;
    }
}
