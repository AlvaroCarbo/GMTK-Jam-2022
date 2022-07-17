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

    [Header("Tilemaps")] [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap selectionTilemap;
    [SerializeField] private Tilemap stepsTilemap;

    [Header("Tiles")] [SerializeField] private TileBase selectedTile;
    [SerializeField] private TileBase walkableSelectedTile;
    [SerializeField] private TileBase walkableTile;
    [SerializeField] private TileBase enemyWalkableTile;
    [SerializeField] private TileBase occupiedTile;

    [Header("Grid selected coordinates")] [SerializeField]
    private Vector3Int selectedTilePosition;

    [SerializeField] private Vector3Int lastSelectedTile;

    [Header("Grid player walkable coordinates")] [SerializeField]
    private List<Vector3> playerMovePosition;

    [Header("Enemy entities in player walkable coordinates")] [SerializeField]
    private List<Vector3> enemiesInMovePosition;

    [Header("Entities")] [SerializeField] private EntityController player;
    [SerializeField] private List<EntityController> enemies;

    [Header("Helper")] [SerializeField] private GameObject mouseHover;
    [SerializeField] private TMP_Text mouseOverText;
    [SerializeField] private GameObject selected;

    [Header("Inputs")] [SerializeField] private InputHandler inputHandler;

    [Header("Camera")] private Camera _camera;

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

    private void Start()
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
            mouseHover = player.gameObject;
            player.Hover();
            ShowPlayerTileRadius(player.currentMovePoints);
            // player.Select();
            // DisableTileRadius();
            // ShowPlayerTileRadius(player.currentMovePoints);
        }
        else if (enemies.Any(enemy => enemy.GetCell() == selectedTilePosition))
        {
            var enemy = enemies.First(enemy => enemy.GetCell() == selectedTilePosition);
            mouseHover = enemy.gameObject;
            enemy.Hover();
            if (!player.GetSelected())
            {
                DisableTileRadius();
                ShowEnemyTileRadius(enemy.currentMovePoints);
            }
            // enemy.Select();
        }
        else
        {
            mouseHover = null;
            player.ResetHover();
            foreach (var enemy in enemies)
            {
                enemy.ResetHover();
            }

            if (!player.GetSelected())
            {
                DisableTileRadius();
            }
        }

        // Helper
        mouseOverText.text = mouseHover == null ? "" : mouseHover.name;
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
            ShowPlayerTileRadius(player.currentMovePoints);
        }
        // else if (enemies.Any(enemy => enemy.GetCell() == selectedTilePosition))
        // {
        //     var enemy = enemies.First(enemy => enemy.GetCell() == selectedTilePosition);
        //     enemy.Select();
        //     DisableTileRadius();
        //     ShowEnemyTileRadius(enemy.currentMovePoints);
        // }
        // else
        // {
        //     DisableTileRadius();
        // }
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

        if (enemiesInMovePosition.Any(enemyPos => enemyPos == target))
        {
            var targetPos = new Vector2(target.x, target.y);
            var nearestPlayerMovePosition = playerMovePosition.OrderBy(pos => Vector2.Distance(pos, targetPos)).First();
            var cellToGo = new Vector3Int((int) nearestPlayerMovePosition.x, (int) nearestPlayerMovePosition.y,
                (int) nearestPlayerMovePosition.z);
            player.WalkedCellDistance(cellToGo);
            player.SetWorldPosition(walkableTilemap.CellToWorld(cellToGo));
            player.SetCell(cellToGo);

            // Attack to enemy
        }
    }

    private void ShowPlayerTileRadius(int tileSteps)
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
                    enemiesInMovePosition.Add(tilePosition);
                }
            }
        }
    }

    private void ShowEnemyTileRadius(int tileSteps)
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
                    stepsTilemap.SetTile(tilePosition, enemyWalkableTile);
                    playerMovePosition.Add(tilePosition);
                }

                if (player.GetCell() == tilePosition)
                {
                    stepsTilemap.SetTile(tilePosition, occupiedTile);
                    // playerMovePosition.Remove(tilePosition);
                    // enemiesInMovePosition.Add(tilePosition);
                }
            }
        }
    }

    private void DisableTileRadius()
    {
        stepsTilemap.ClearAllTiles();
        playerMovePosition.Clear();
        enemiesInMovePosition.Clear();
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