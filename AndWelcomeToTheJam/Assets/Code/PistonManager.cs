using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using patterns;
using System;

public class PistonManager : SingletonBehavior<PistonManager>
{
    [HideInInspector]
    public List<Piston> m_Pistons = new List<Piston>();
    public void AddPiston(Piston p) { m_Pistons.Add(p); m_Pistons.Sort(); }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExtendPushers();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            RetractPushers();
        }
    }

    #region Extend/Retract all Pushers
    private void ExtendPushers()
    {
        for (int i = 0; i < m_Pistons.Count; i++)
        {
            StartCoroutine("ExtendPusherDelayed", new object[] { .1f * i, m_Pistons[i] });
        }
    }
    private IEnumerator ExtendPusherDelayed(object[] o)
    {
        yield return new WaitForSeconds((float)o[0]);
        (o[1] as Piston).ExtendPusher();
    }

    private void RetractPushers()
    {
        for (int i = m_Pistons.Count-1; i >= 0; i--)
        {
            StartCoroutine("RetractPusherDelayed", new object[] { .1f * i, m_Pistons[m_Pistons.Count - 1 - i] });
        }
    }
    private IEnumerator RetractPusherDelayed(object[] o)
    {
        yield return new WaitForSeconds((float)o[0]);
        (o[1] as Piston).RetractPusher();
    }
    #endregion


}
