using UnityEngine;
using Leap;

public class LeapController : MonoBehaviour
{

    private PlayerController _controller;
    private Vector2 _handPosition;
    public HandController _handController;

    [Range(0, 1)]
    public float OnGrabStrength = 0.6f;

    [Range(0, 1)]
    public float OffGrabStrength = 0.4f;

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

        Globals.Update(Time.deltaTime);

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
        if (hand.GrabStrength > OnGrabStrength)
            Globals.SetSlowMotionTrue();
        if (hand.GrabStrength <= OffGrabStrength)
            Globals.SetSlowMotionFalse();
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
