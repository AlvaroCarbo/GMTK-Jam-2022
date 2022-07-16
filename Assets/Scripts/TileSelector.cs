using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap walkableTilemap;
    public Tilemap selectionTilemap;
    
    public TileBase selectedTile;
    public Vector3Int lastSelectedTile;

    [SerializeField] private InputHandler inputHandler;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    void Start()
    {
        inputHandler.OnMoveMouseEvent += GetTilePosition;
        Debug.Log(walkableTilemap.cellBounds);

        Debug.Log(walkableTilemap.cellBounds.size);
    }

    private void GetTilePosition(Vector2 pos)
    {
        if (_camera == null) return;
        Vector3Int cellPosition = walkableTilemap.WorldToCell(_camera.ScreenToWorldPoint(pos));
        
        if (lastSelectedTile == cellPosition) return;
        selectionTilemap.SetTile(lastSelectedTile,null);
        
        lastSelectedTile = cellPosition;
        
        
        // tilemap.SetTile(cellPosition, selectedTile);
        if (walkableTilemap.HasTile(cellPosition))
        {
            selectionTilemap.SetTile(cellPosition, selectedTile);
        }


        Debug.Log(cellPosition);
    }

    private void OnDestroy()
    {
        inputHandler.OnMoveMouseEvent -= GetTilePosition;
    }
}