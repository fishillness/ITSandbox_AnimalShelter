using UnityEngine;
using UnityEngine.Tilemaps;

public class HideTilemapOnPlay : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapRenderer.enabled = false;
    }
}
