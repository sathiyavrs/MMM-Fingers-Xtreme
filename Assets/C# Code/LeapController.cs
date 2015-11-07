using UnityEngine;
using System.Collections;
using Leap;

public class LeapController : MonoBehaviour
{

    Controller controller = new Controller();
    private PlayerController _controller;
    private Vector2 _handPosition;
    public HandController _handController;
    [Range(0, 1)]
    public float MinGrabStrength;

    public void Start()
    {
        _controller = GetComponent<PlayerController>();
        _handController = GetComponent<HandController>();
        Globals.Initialize();
        if (!_controller)
            Debug.LogError("Player Controller Component not found!");
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;
        Frame frame = _handController.GetFrame();
        Hand hand = frame.Hands.Rightmost;
        checkGrabbed(hand);
        getHandPosition(hand);
        var result = _controller.TrySetPosition(_handPosition);
        if (!result)
            Globals.GameOver = true;
    }

    void checkGrabbed(Hand hand)
    {
        if (hand.GrabStrength > MinGrabStrength)
            Globals.SlowMotion = true;
        if (hand.GrabStrength <= MinGrabStrength)
            Globals.SlowMotion = false;
    }

    void getHandPosition(Hand hand)
    {
        if (hand != null)
        {
            Vector handPosition = hand.StabilizedPalmPosition;
            _handPosition.x = handPosition.x / 22.86f;
            _handPosition.y = (handPosition.y - 145) / 12.23f;
        }
    }
}
