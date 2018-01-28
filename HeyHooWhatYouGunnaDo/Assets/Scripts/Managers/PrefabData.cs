using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabData", menuName = "NorseMorseHorse/PrefabData", order = 0)]
public class PrefabData : ScriptableObject
{
    public NorseDot m_NorseDot;
    public LineRenderer m_DotConnectLR;
}
