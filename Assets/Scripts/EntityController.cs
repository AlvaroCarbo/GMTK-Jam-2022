using System;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    [SerializeField] private Grid _grid;
    [SerializeField] private bool selected;

    [SerializeField] private CharacterStats stats;
    
    public int currentMovePoints;

    private void Start()
    {
        var position = transform.position;
        currentMovePoints = stats.movePoints;
    }

    public CharacterStats GetStats() { return stats; }
    public bool GetSelected() { return selected; }
    public void Select() => selected = true;
    
    public void Release() => selected = false;
    public void SetPosition(Vector3 position) {
        position.z = 1;
        this.transform.position = position;
        Debug.Log("Setting pos to:" + position);
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}