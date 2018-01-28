using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject CompletedEffect;
    public GameObject StartingNode;
    public GameObject EndingNode;
    public float CompleteDelay = 3f;
    private bool _completed;

    private void Start()
    {
        if(StartingNode == null)
        {
            Debug.LogError("Level::Start() -- No starting node has been set for level.");
        }
    }

    public bool CheckForWinState(NorseSymbol mySymbol) {

        //Compare your symbol to the win state
        //  Unncessary method -- already taken care of in GameLoop::m_currentSymbol

        return false;
    }

    public void ResetLevel()
    {
        EnergyLauncher.instance.ResetToNode(StartingNode.transform.Find("EmptyNode").gameObject);
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
        GameLoop.Instance.IncrementLevel();
    }
}
