using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public delegate void OnMoveMouse(Vector2 pos);

    public event OnMoveMouse OnMoveMouseEvent;

    public void OnMoveCursor(InputAction.CallbackContext context) =>
        OnMoveMouseEvent?.Invoke(context.ReadValue<Vector2>());
}