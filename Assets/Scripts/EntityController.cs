using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{
    [SerializeField] private bool selected;

    [SerializeField] private CharacterStats stats;
    [SerializeField] private GameObject hpBar;

    public int currentMovePoints;
    public int currentHealthPoints;

    [SerializeField] private Vector3Int currentCell;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        ResetHealthPoints();
        hpBar.GetComponent<Slider>().maxValue = stats.healthPoints;
        hpBar.GetComponent<Slider>().value = stats.healthPoints;
        ResetMovePoints();
        Release();

        animator = GetComponentInChildren<Animator>();
    }

    public CharacterStats GetStats()
    {
        return stats;
    }

    public void ResetMovePoints()
    {
        currentMovePoints = stats.movePoints;
    }

    public void ResetHealthPoints()
    {
        hpBar.GetComponent<Slider>().value = stats.healthPoints;
        currentHealthPoints = stats.healthPoints;
    }

    public void DecreaseHealth(int healthToDecrease)
    {
        currentHealthPoints -= healthToDecrease;
        hpBar.GetComponent<Slider>().value = currentHealthPoints;
        checkIfDead(currentHealthPoints);
    }

    public void checkIfDead(int hp)
    {
        if (hp <= 0)
        {
            //Really dead
            this.gameObject.SetActive(false);
        }
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
        animator.SetBool("Outline", true);
        // spriteRenderer.sprite = selectedSprite;
    }

    public void ResetHover()
    {
        if (!selected)
        {
            animator.SetBool("Outline", false);
            // spriteRenderer.sprite = unselectedSprite;
        }
    }
}