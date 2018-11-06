using System;
using UnityEngine;

public class TapSwipeDetector : MonoBehaviour
{

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    private float minDistanceForSwipe = 150f;

    public static event Action<SwipeData> OnSwipe = delegate { };
    public static event Action OnTap = delegate { };
    public static event Action OnDoubleTap = delegate { };

    // PC ONLY
    private float tapCount;
    private static float CLICK_DELTA = 0.8f;
    private float clickQuota = 0;

    private void Update() {
        UpdateTouch();
        UpdateMouse();
    }

    private void UpdateTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                if (!DetectSwipe())
                {
                    if (touch.tapCount == 1 && touch.deltaTime <= 0.8f)
                    {
                        SendTap();
                    }
                    else if (touch.tapCount == 2)
                    {
                        SendDoubleTap();
                    }
                }
            }
        }
    }

    private void UpdateMouse() {
        // Removes taps over time by detecting loop arounds
        float newClickQuota = Mathf.Repeat(clickQuota + Time.deltaTime, CLICK_DELTA);
        if (newClickQuota < clickQuota) {
            tapCount = Mathf.Max(tapCount - 1, 0);
        }
        clickQuota = newClickQuota;

        // Process input
        Vector3 pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) {
            fingerUpPosition = pos;
            fingerDownPosition = pos;
        } else if (Input.GetMouseButtonUp(0)) {
            fingerDownPosition = pos;
            if (!DetectSwipe()) {
                tapCount += 1;
                if (tapCount == 1) {
                    SendTap();
                } else if (tapCount == 2) {
                    SendDoubleTap();
                    tapCount = 0;
                    clickQuota = 0;
                }
            }
        }
    }

    private Boolean DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            fingerUpPosition = fingerDownPosition;
            return true;
        }
        return false;
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    private void SendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPosition = fingerDownPosition,
            EndPosition = fingerUpPosition
        };
        OnSwipe(swipeData);
    }

    private void SendTap()
    {
        OnTap();
    }

    private void SendDoubleTap()
    {
        OnDoubleTap();
    }
}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public SwipeDirection Direction;
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}