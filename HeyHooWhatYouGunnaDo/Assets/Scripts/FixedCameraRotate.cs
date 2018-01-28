using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraRotate : MonoBehaviour {

    public float RotateDegrees = 90;
    public float RotateCoolDown = 1f;
    public float RotateTime = 1f;
    private bool _cooldown;
	
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space) && !_cooldown) {
            _cooldown = true;
            Rotate();
            StartCoroutine(Rotate());
            StartCoroutine(WaitForCooldown());
        }
	}

    private IEnumerator Rotate() {
        var elapsedTime = 0f;
        var startingRotation = transform.rotation.eulerAngles;
        var newRotation = startingRotation;
        newRotation.y += RotateDegrees;
        while (elapsedTime < RotateTime)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(startingRotation, newRotation, (elapsedTime / RotateTime)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitForCooldown() {
        yield return new WaitForSeconds(RotateCoolDown);
        _cooldown = false;
    }
}
