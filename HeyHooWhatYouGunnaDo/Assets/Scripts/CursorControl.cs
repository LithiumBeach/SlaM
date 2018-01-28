using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    public static CursorControl instance = null;

    public float CursorSpeed = 8f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public Vector3 CursorPosition { get { return transform.position; } }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 positionUpdate = transform.position;
        positionUpdate.x += CursorSpeed * Input.GetAxis("Horizontal");
        positionUpdate.y += CursorSpeed * Input.GetAxis("Vertical");
        positionUpdate.z = GameLoop.Instance.CurrentLevel.transform.position.z - 0.5f;
        transform.position = positionUpdate;

        ClampPosition();
	}

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.1f, 0.9f);
        pos.y = Mathf.Clamp(pos.y, 0.1f, 0.9f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

}
