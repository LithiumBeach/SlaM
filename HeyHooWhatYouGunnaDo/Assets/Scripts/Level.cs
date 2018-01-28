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

        //  imma say fuck this method -- we do this already in GameLoop w/ a given end node when we traverse to it

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
