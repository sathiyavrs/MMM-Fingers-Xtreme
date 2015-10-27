using UnityEngine;

public class PulsationHandler : MonoBehaviour
{
    public Color InitialColor;
    public Color FinalColor;
    public float InitialScale;
    public float FinalScale;
    public float Multiplier;

    private SpriteRenderer _renderer;
    private float _currentScale;
    private Color _currentColor;
    private CustomResources.InterpolationDirection _direction;

    public void Start()
    {
        CustomResources.CheckForGlobalsInitialization();

        _renderer = GetComponent<SpriteRenderer>();
        if (!_renderer)
            Debug.LogError("Sprite Renderer Component not found!");

        _renderer.color = InitialColor;
        _currentColor = InitialColor;
        _currentScale = InitialScale;
        _direction = CustomResources.InterpolationDirection.Forward;
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        ComputeColor();
        ComputeScale();

        _renderer.color = _currentColor;
        transform.localScale = new Vector3(_currentScale, _currentScale, 1);
    }

    private void ComputeColor()
    {
        var color = _currentColor;
        var multiplier = Multiplier * (Globals.SlowMotion ? Globals.SlowMotionRatio : 1);
        var increment = multiplier * Time.deltaTime * (FinalColor - InitialColor);
        var direction = _direction == CustomResources.InterpolationDirection.Forward ? 1 : -1;

        color += direction * increment;
        _currentColor = color;
    }

    private void ComputeScale()
    {
        var scale = _currentScale;
        var multiplier = Multiplier * (Globals.SlowMotion ? Globals.SlowMotionRatio : 1);
        var increment = multiplier * Time.deltaTime * (FinalScale - InitialScale);

        switch (_direction)
        {
            case CustomResources.InterpolationDirection.Forward:
                scale += increment;
                if (scale > FinalScale)
                {
                    scale = FinalScale;
                    _direction = CustomResources.InterpolationDirection.Backward;
                    _currentColor = FinalColor;
                }

                break;

            case CustomResources.InterpolationDirection.Backward:
                scale -= increment;
                if (scale < InitialScale)
                {
                    scale = InitialScale;
                    _direction = CustomResources.InterpolationDirection.Forward;
                    _currentColor = InitialColor;
                }
                break;
        }

        _currentScale = scale;
    }


}
