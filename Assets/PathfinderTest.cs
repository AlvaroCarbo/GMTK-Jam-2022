using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

[RequireComponent(typeof(EntityController))]
public class PathfinderTest : MonoBehaviour
{
    private EntityController _enemy;
    
    [Header("Target to find")]
    [SerializeField] private EntityController player;
    
    [Header("Other enemies")]
    [SerializeField] private List<EntityController> enemies;
    
    [Header("Tilemap to find path on")]
    [SerializeField] private Tilemap walkableTilemap;
    
    [Header("Enemy positions to move")]
    [SerializeField] private List<Vector3> enemyMovement;
    
    [Header("Player inside enemy move range")]
    [SerializeField] private List<Vector3> playerInMovePosition;
    
    [Header("Other enemies inside enemy move range")]
    [SerializeField] private List<Vector3> enemiesInMovePosition;

    private void Awake()
    {
        _enemy = GetComponent<EntityController>();
        
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList().Select(e => e.GetComponent<EntityController>()).ToList();
    }
    
    private void Start()
    {
        ShowEnemyTileRadius(_enemy.currentMovePoints);
        
        MoveToPoint(player.GetCell());
    }
    
    private void ShowEnemyTileRadius(int tileSteps)
    {
        // check for tiles around where there number between tiles and the distance to the selected tile is less than the number of tiles

        for (int x = -tileSteps; x <= tileSteps; x++)
        {
            for (int y = -tileSteps; y <= tileSteps; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > tileSteps) continue;
                Vector3Int tilePosition = new Vector3Int(_enemy.GetCell().x + x, _enemy.GetCell().y + y, 0);
                if (walkableTilemap.HasTile(tilePosition))
                {
                    
                    // stepsTilemap.SetTile(tilePosition, enemyWalkableTile);
                    enemyMovement.Add(tilePosition);
                }

                if (player.GetCell() == tilePosition)
                {
                    // stepsTilemap.SetTile(tilePosition, occupiedTile);
                    enemyMovement.Remove(tilePosition);
                    playerInMovePosition.Add(tilePosition);
                }
            }
        }
    }
    
    private void MoveToPoint(Vector3Int target)
    {
        foreach (var movePos in enemyMovement.Where(movePos => target == movePos))
        {
            _enemy.WalkedCellDistance(target);
            _enemy.SetWorldPosition(walkableTilemap.CellToWorld(target));
            _enemy.SetCell(target);
        }

        if (playerInMovePosition.Any(enemyPos => enemyPos == target))
        {
            var targetPos = new Vector2(target.x, target.y);
            var nearestPlayerMovePosition = enemyMovement.OrderBy(pos => Vector2.Distance(pos, targetPos)).First();
            var cellToGo = new Vector3Int((int) nearestPlayerMovePosition.x, (int) nearestPlayerMovePosition.y,
                (int) nearestPlayerMovePosition.z);
            _enemy.WalkedCellDistance(cellToGo);
            _enemy.SetWorldPosition(walkableTilemap.CellToWorld(cellToGo));
            _enemy.SetCell(cellToGo);

            // Attack to enemy
        }
    }
}