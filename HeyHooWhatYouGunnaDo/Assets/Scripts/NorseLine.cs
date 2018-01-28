using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class NorseLine
{
    private NorseDot m_A;
    private NorseDot m_B;

    public NorseDot A { get { return m_A; } set { m_A = value; line.SetPosition(0, value.transform.position); } }
    public NorseDot B { get { return m_B; } set { m_B = value; line.SetPosition(1, value.transform.position); } }

    public LineRenderer line;

    public void CleanUp()
    {
#if UNITY_EDITOR
        GameObject.DestroyImmediate(line.gameObject);
#else
        GameObject.Destroy(line.gameObject);
#endif
    }
}
