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
    public float RotationDelay = 1.8f;
    public float LevelSwitchDelay = .8f;
    private bool _completed;
    public bool LevelComplete { get { return _completed; } }
    public Transform m_LevelBG = null;
    private float NodeDisplacement = -1;

    private void OnEnable()
    {
        if (StartingNode == null)
        {
            Debug.LogError(gameObject.name + ": Level::Start() -- No starting node has been set for level.");
        }
        if (LightPathRoot == null)
        {
            Debug.LogWarning(gameObject.name + ": Level::Start() -- No root object for LightPathRoot set!");
        }
        NodeDisplacement = StartingNode.transform.position.z - m_LevelBG.transform.position.z;
    }

    private void Start()
    {
        
    }

    public bool CheckForWinState(NorseSymbol mySymbol) {

        //Compare your symbol to the win state
        //  Unncessary method -- already taken care of in GameLoop::m_currentSymbol

        return false;
    }

    public void ResetLevel()
    {
        EnergyLauncher.instance.ResetToNode(StartingNode.transform.Find("EmptyNode").gameObject);
        CleanLightLines();
    }

    public void CleanLightLines()
    {
        if (LightPathRoot != null)
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
        yield return new WaitForSeconds(RotationDelay);
        FindObjectOfType<FixedCameraRotate>().RotateToNextLevel();
        yield return new WaitForSeconds(LevelSwitchDelay);
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
