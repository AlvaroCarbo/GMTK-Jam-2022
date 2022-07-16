using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Vector2 _mousePos;
    
    public delegate void OnMove(Vector2 pos);
    public event OnMove OnMoveEvent;

    public delegate void OnClick();
    public event OnClick OnClickEvent;
    
    public void OnMoveCursor(InputAction.CallbackContext context)
    {
        _mousePos = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(_mousePos);
    }
    
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Debug.Log("Select"  + _mousePos);
            OnClickEvent?.Invoke();
        }
    }
}