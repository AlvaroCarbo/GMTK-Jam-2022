using System;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    [SerializeField] private Grid _grid;
    [SerializeField] private bool selected;

    [SerializeField] private CharacterStats stats;

    private void Start()
    {
        var position = transform.position;
    }

    public void Select() => selected = true;
    
    public void Release() => selected = false;

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}