using System;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField] private bool selected;

    [SerializeField] private CharacterStats stats;

    public int currentMovePoints;

    [SerializeField] private Vector3Int currentCell;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private void Awake()
    {
        ResetMovePoints();
        Release();
    }

    public CharacterStats GetStats()
    {
        return stats;
    }
    
    public void ResetMovePoints()
    {
        currentMovePoints = stats.movePoints;
    }

    public bool GetSelected()
    {
        return selected;
    }

    public void Select()
    {
        selected = true;
    }

    public void Release()
    {
        selected = false;
    }

    public void SetWorldPosition(Vector3 worldPos)
    {
        worldPos.z = 1;
        transform.position = worldPos;
    }

    public void SetCell(Vector3Int cell)
    {
        currentCell = cell;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public Vector3Int GetCell()
    {
        return currentCell;
    }

    public void WalkedCellDistance(Vector3Int target)
    {
        var distance = Mathf.CeilToInt(Vector3Int.Distance(currentCell, target));
        currentMovePoints -= distance;
    }

    public void Hover()
    {
        spriteRenderer.sprite = selectedSprite;
    }

    public void ResetHover()
    {
        if (!selected)
        {
            spriteRenderer.sprite = unselectedSprite;
        }
    }
}