using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public float tileSize = 1;
    public int width;
    public int height;
    public SpriteRenderer m_TilePrefab = null;
    private SpriteRenderer[,] tileGOs = null;

    public void Awake()
    {
        Populate();
    }

    public void Populate()
    {
        tileGOs = new SpriteRenderer[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SpriteRenderer go = Instantiate(m_TilePrefab.gameObject, transform).GetComponent<SpriteRenderer>();
                go.transform.position = new Vector3(x * tileSize, y * tileSize);
                go.color = Color.black;
                tileGOs[x, y] = go;
            }
        }
    }

    private int m_MouseTileX = 0;
    private int m_MouseTileY = 0;
    void Update()
    {
        if (m_MouseTileX >= width || m_MouseTileY > height || m_MouseTileX < 0 || m_MouseTileY < 0)
            return;

        tileGOs[m_MouseTileX, m_MouseTileY].color = Color.black;
        m_MouseTileX = (int)((Input.mousePosition.x/* + transform.position.x*/) / width);
        m_MouseTileY = (int)((Input.mousePosition.y/* + transform.position.y*/) / height);

        if (m_MouseTileX >= width || m_MouseTileY > height || m_MouseTileX < 0 || m_MouseTileY < 0)
            return;


        tileGOs[m_MouseTileX, m_MouseTileY].color = Color.cyan;
    }
}