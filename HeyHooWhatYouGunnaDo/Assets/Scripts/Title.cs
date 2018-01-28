using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    public ParticleSystem StartEffect;
    public float StartDelay = 3f;

	void Update () {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartEffect.Play();
            SoundManager.Instance.PlayAudio(0);
            StartCoroutine(WaitToStart());
        }
	}

    private IEnumerator WaitToStart(){
        yield return new WaitForSeconds(StartDelay);
        SceneManager.LoadScene("Main");
        SceneManager.LoadScene("ArtTest", LoadSceneMode.Additive);
    }
}
