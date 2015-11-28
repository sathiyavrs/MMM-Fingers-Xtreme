using UnityEngine;

public class MouseController : MonoBehaviour
{
    private PlayerController _controller;
    private Vector2 _mousePosition;

    public void Start()
    {
        _controller = GetComponent<PlayerController>();
        if (!_controller)
            Debug.LogError("Player Controller Component not found!");

        ComputeMousePosition();
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        Globals.Update(Time.deltaTime);

        ComputeMousePosition();
        ClickListener();

        var result = _controller.TrySetPosition(_mousePosition);
        if (!result)
            Globals.GameOver = true;
    }

    private void ClickListener()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Globals.SlowMotion)
            {
                Globals.SetSlowMotionFalse();
            } 
            else
            {
                Globals.SetSlowMotionTrue();
            }
        }

    }

    private void ComputeMousePosition()
    {
        var screenPosition = Input.mousePosition;
        var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        _mousePosition = new Vector2(worldPosition.x, worldPosition.y);
    }
}