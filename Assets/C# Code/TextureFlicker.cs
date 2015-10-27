using UnityEngine;

public class TextureFlicker : MonoBehaviour
{
    public float TimePeriod;
    public float TimeActive;

    private CustomResources.FlickerState _state;
    private float _timePassed;
    private bool _activated;
    private SpriteRenderer _renderer;

    public void Activate()
    {
        _activated = true;
    }

    public void Start()
    {
        _activated = false;

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
            Debug.LogError("Sprite Renderer Component not found!");

        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0);
        _state = CustomResources.FlickerState.On;
        _timePassed = 0;
    }

    public void Update()
    {
        if (!_activated)
            return;

        UpdateStateAndTime();
        UpdateTexture();
    }

    private void UpdateStateAndTime()
    {
        _timePassed += Time.deltaTime;
        switch (_state)
        {
            case CustomResources.FlickerState.On:
                if (_timePassed > TimeActive)
                {
                    _state = CustomResources.FlickerState.Off;
                    _timePassed = 0;
                }
                break;

            case CustomResources.FlickerState.Off:
                if(_timePassed > TimePeriod)
                {
                    _state = CustomResources.FlickerState.On;
                    _timePassed = 0;
                }
                break;
        }
    }

    private void UpdateTexture()
    {
        switch (_state)
        {
            case CustomResources.FlickerState.On:
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1);
                break;

            case CustomResources.FlickerState.Off:
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0);
                break;
        }
    }
}
