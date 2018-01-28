using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    public static CursorControl instance = null;

    public float CursorSpeed = .4f;

    private Transform bullshit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Vector3 CursorPosition { get { return transform.position; } }

    // Use this for initialization
    void Start()
    {
        bullshit = Instantiate(new GameObject()).transform;
        bullshit.name = "cursorRoot";
        bullshit.transform.position = GameLoop.Instance.CurrentLevel.m_LevelBG.transform.position;
        bullshit.transform.rotation = GameLoop.Instance.CurrentLevel.m_LevelBG.transform.rotation;
        transform.parent = bullshit;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        GameLoop.Instance.AOnChangeLevel += OnChangeLevel;
    }

    private void OnChangeLevel()
    {
        bullshit.transform.position = GameLoop.Instance.CurrentLevel.m_LevelBG.transform.position;
        bullshit.transform.rotation = GameLoop.Instance.CurrentLevel.m_LevelBG.transform.rotation;
        transform.parent = bullshit;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 positionUpdate = transform.localPosition;
        positionUpdate.x += CursorSpeed * Input.GetAxis("Horizontal");
        positionUpdate.y += CursorSpeed * Input.GetAxis("Vertical");
        positionUpdate.z = -1f;// Camera.main.transform.TransformPoint(GameLoop.Instance.CurrentLevel.m_LevelBG.transform.position).z;

        transform.localPosition = positionUpdate;
        //transform.position = positionUpdate;

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
