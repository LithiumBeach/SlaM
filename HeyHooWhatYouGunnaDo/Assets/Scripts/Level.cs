using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject CompletedEffect;
    public GameObject StartingNode;
    public GameObject EndingNode;
    public LightPathScript LightPathRoot;
    public NorseSymbol m_SymbolToCompleteLevel;
    public float CompleteDelay = 3f;
    private bool _completed;
    public bool LevelComplete { get { return _completed; } }
    public Transform m_LevelBG = null;
    private float NodeDisplacement = -1;

    private void Start()
    {
        if(StartingNode == null)
        {
            Debug.LogError("Level::Start() -- No starting node has been set for level.");
        }
        if(LightPathRoot == null)
        {
            Debug.LogWarning("Level::Start() -- No root object for LightPathRoot set!");
        }


        NodeDisplacement = StartingNode.transform.position.z - m_LevelBG.transform.position.z;
    }

    public bool CheckForWinState(NorseSymbol mySymbol) {

        //Compare your symbol to the win state
        //  Unncessary method -- already taken care of in GameLoop::m_currentSymbol

        return false;
    }

    public void ResetLevel()
    {
        EnergyLauncher.instance.ResetToNode(StartingNode.transform.Find("EmptyNode").gameObject);

        if(LightPathRoot != null)
            LightPathRoot.ClearAllLines();
    }

    public void PushLightLine(Transform pointA, Transform pointB)
    {
        if(LightPathRoot != null)
        {
            LightPathRoot.PushNewLine(pointA, pointB);
        }
        else
        {
            Debug.LogWarning("There is no root object set for the Lights!");
        }
    }

    public void CompleteLevel()
    {
        _completed = true;
        CompletedEffect.SetActive(true);
        SoundManager.Instance.PlayAudio((int)SoundManager.AudioClipKeys.Correct);
        StartCoroutine(WaitForComplete());
    }

    private IEnumerator WaitForComplete()
    {
        yield return new WaitForSeconds(CompleteDelay);
        FindObjectOfType<FixedCameraRotate>().RotateToNextLevel();
        ResetLevel();
        GameLoop.Instance.IncrementLevel();
    }

    public float GetCursorZPlacement()
    {
        return m_LevelBG.transform.position.z + NodeDisplacement;
    }

    public Quaternion GetLevelRotation()
    {
        return m_LevelBG.parent.rotation;
    }
}
