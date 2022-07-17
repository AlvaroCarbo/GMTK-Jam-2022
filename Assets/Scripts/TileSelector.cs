using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public static TileSelector Instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap stepsTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private TileBase walkableSelectedTile;
    [SerializeField] private TileBase walkableTile;
    [SerializeField] private TileBase occupiedTile;

    [Header("Grid selected coordinates")]
    [SerializeField] private Vector3Int selectedTilePosition;
    [SerializeField] private Vector3Int lastSelectedTile;

    [Header("Grid player walkable coordinates")]
    [SerializeField] private List<Vector3> playerMovePosition;
    
    [Header("Entities")]
    [SerializeField] private EntityController player;
    [SerializeField] private List<EntityController> enemies;

    [Header("Helper")]
    [SerializeField] private GameObject mouseOver;
    [SerializeField] private TMP_Text mouseOverText;
    // [SerializeField] private GameObject selected;
    
    [Header("Inputs")]
    [SerializeField] private InputHandler inputHandler;
    
    [Header("Camera")]
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

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityController>();
        player.SetCell(walkableTilemap.WorldToCell(player.GetWorldPosition()));

        var enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyObjects)
        {
            enemies.Add(enemy.GetComponent<EntityController>());
        }

        foreach (var enemy in enemies)
        {
            enemy.SetCell(walkableTilemap.WorldToCell(enemy.GetWorldPosition()));
        }
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
            selectionTilemap.SetTile(selectedTilePosition,
                stepsTilemap.HasTile(selectedTilePosition) ? walkableSelectedTile : selectedTile);
        }


        if (player.GetCell() == selectedTilePosition)
        {
            mouseOver = player.gameObject;
        }
        else if (enemies.Any(enemy => enemy.GetCell() == selectedTilePosition))
        {
            mouseOver = enemies.First(enemy => enemy.GetCell() == selectedTilePosition).gameObject;
        }
        else
        {
            mouseOver = null;
        }
        
        // Helper
        mouseOverText.text = mouseOver == null ? "" : mouseOver.name;
    }

    private void SelectTile()
    {
        var gridPosition = walkableTilemap.WorldToCell(player.GetWorldPosition());
        // Vector2 playerPosition = walkableTilemap.GetCellCenterWorld(gridPosition);
        var playerPosition = new Vector2(gridPosition.x, gridPosition.y);
        var tempSelected = new Vector2Int(selectedTilePosition.x, selectedTilePosition.y);

        if (playerPosition == tempSelected)
        {
            player.Select();
            DisableTileRadius();
            ShowTileRadius(player.currentMovePoints);
        }
        else
        {
            if (player.GetSelected() && playerPosition != tempSelected)
            {
                MoveToPoint(new Vector3Int(tempSelected.x, tempSelected.y));
            }

            DisableTileRadius();
            player.Release();
        }
    }

    private void MoveToPoint(Vector3Int target)
    {
        foreach (var movePos in playerMovePosition.Where(movePos => target == movePos))
        {
            player.WalkedCellDistance(target);
            player.SetWorldPosition(walkableTilemap.CellToWorld(target));
            player.SetCell(target);
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
                    stepsTilemap.SetTile(tilePosition, walkableTile);
                    playerMovePosition.Add(tilePosition);
                }

                if (enemies.Any(enemy => enemy.GetCell() == tilePosition))
                {
                    stepsTilemap.SetTile(tilePosition, occupiedTile);
                    playerMovePosition.Remove(tilePosition);
                }
            }
        }
    }

    private void DisableTileRadius()
    {
        stepsTilemap.ClearAllTiles();
        playerMovePosition.Clear();
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