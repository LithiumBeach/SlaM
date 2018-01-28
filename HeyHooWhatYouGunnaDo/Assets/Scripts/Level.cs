using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public GameObject CompletedEffect;
    public NorseSymbol TargetSymbol;
    public float CompleteDelay = 3f;
    private bool _completed;

    public bool CheckForWinState(NorseSymbol mySymbol) {

        //Compare your symbol to the win state

        return false;
    }

    public void CompleteLevel(){
        _completed = true;
        CompletedEffect.SetActive(true);
    }

    private IEnumerator WaitForComplete(){
        yield return new WaitForSeconds(CompleteDelay);
        FindObjectOfType<GameLoop>().IncrementLevel();
    }
}
