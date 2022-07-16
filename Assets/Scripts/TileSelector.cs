using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap stepsTilemap;

    [SerializeField] private TileBase selectedTile;
    
    [SerializeField] private Vector3Int selectedTilePosition;
    [SerializeField] private Vector3Int lastSelectedTile;

    [SerializeField] private InputHandler inputHandler;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    void Start()
    {
        inputHandler.OnMoveEvent += GetTilePosition;
        inputHandler.OnClickEvent += SelectTile;
        
        Debug.Log(walkableTilemap.cellBounds);
        Debug.Log(walkableTilemap.cellBounds.size);
    }

    private void GetTilePosition(Vector2 pos)
    {
        if (_camera == null) return;
        selectedTilePosition = walkableTilemap.WorldToCell(_camera.ScreenToWorldPoint(pos));

        if (lastSelectedTile == selectedTilePosition) return;
        selectionTilemap.SetTile(lastSelectedTile, null);

        lastSelectedTile = selectedTilePosition;
        
        // tilemap.SetTile(selectedTilePosition, selectedTile);
        if (walkableTilemap.HasTile(selectedTilePosition))
        {
            selectionTilemap.SetTile(selectedTilePosition, selectedTile);
        }
        
    }

    private void SelectTile()
    {
        if (true)
        {
            ShowTileRadius(2);
        }
    }
    
    private void ShowTileRadius(int tileSteps)
    {
        // check for tiles around where there number between tiles and the distance to the selected tile is less than the number of tiles
        
        for (int x = -tileSteps; x <= tileSteps; x++)
        {
            for (int y = -tileSteps; y <= tileSteps; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > tileSteps) continue;
                Vector3Int tilePosition = new Vector3Int(selectedTilePosition.x + x, selectedTilePosition.y + y, 0);
                if (walkableTilemap.HasTile(tilePosition))
                {
                    stepsTilemap.SetTile(tilePosition, selectedTile);
                }
            }
        }
    }

    private void OnDestroy()
    {
        inputHandler.OnMoveEvent -= GetTilePosition;
    }

    private void OnDisable()
    {
        inputHandler.OnMoveEvent -= GetTilePosition;
    }
}