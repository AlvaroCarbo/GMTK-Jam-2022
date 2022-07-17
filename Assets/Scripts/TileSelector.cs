using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public static TileSelector Instance;

    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap stepsTilemap;

    [SerializeField] private TileBase selectedTile;
    [SerializeField] private TileBase walkableTile;

    [SerializeField] private Vector3Int selectedTilePosition;
    [SerializeField] private Vector3Int lastSelectedTile;

    [SerializeField] private EntityController player;

    [SerializeField] private InputHandler inputHandler;
    private Camera _camera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _camera = Camera.main;
    }

    void Start()
    {
        inputHandler.OnMoveEvent += GetTilePosition;
        inputHandler.OnClickEvent += SelectTile;

        // Debug.Log(walkableTilemap.cellBounds);
        // Debug.Log(walkableTilemap.cellBounds.size);
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
        // Check if can show steps

        // Debug.Log("Selected tile: " + selectedTilePosition);

        var gridPosition = walkableTilemap.WorldToCell(player.GetPosition());
        // Vector2 playerPosition = walkableTilemap.GetCellCenterWorld(gridPosition);
        Vector2 playerPosition = new Vector2(gridPosition.x, gridPosition.y);
        var tempSelected = new Vector2Int(selectedTilePosition.x, selectedTilePosition.y);
        
        Debug.Log(playerPosition);
        Debug.Log(tempSelected);
        if (playerPosition == tempSelected)
        {
            player.Select();
            ShowTileRadius(player.GetStats().movePoints);
        }
        else
        {
            //Check if player is already selected and try to move there 
            //TO-DO: Evitar ir más lejos del rango
            if (player.GetSelected() && playerPosition != tempSelected) { var target = tempSelected; MoveToPoint(new Vector3Int(target.x, target.y)); }
            DisableTileRadius();
            player.Release();
        }
    }

    private void MoveToPoint(Vector3Int target)
    {
        player.SetPosition(walkableTilemap.CellToWorld(target));
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
                    stepsTilemap.SetTile(tilePosition, walkableTile);
                }
            }
        }
    }

    private void DisableTileRadius() => stepsTilemap.ClearAllTiles();

    private void OnDestroy()
    {
        inputHandler.OnMoveEvent -= GetTilePosition;
    }

    private void OnDisable()
    {
        inputHandler.OnMoveEvent -= GetTilePosition;
    }
}