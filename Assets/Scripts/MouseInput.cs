using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour, InputSystem_Actions.IMouseActions
{
    public InputSystem_Actions InputActions { get; private set; }

    public bool Click { get; private set; }
    public Vector2 Position { get; private set; }

    private void OnEnable()
    {
        InputActions = new InputSystem_Actions();

        InputActions.Mouse.Enable();
        InputActions.Mouse.SetCallbacks(this);
    }

    private void OnDisable()
    {
        InputActions.Mouse.Disable();
        InputActions.Mouse.RemoveCallbacks(this);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Click = context.ReadValueAsButton();
    }

    public void OnPosition(InputAction.CallbackContext context)
    {
        Position = context.ReadValue<Vector2>();
    }
}
