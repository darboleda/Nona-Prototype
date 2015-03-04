using UnityEngine;
using System.Collections;

using Zenject;

public interface INonaInput<T>
{
    float GetAxis(T axisId);
    float GetAxisRaw(T axisId);

    Vector2 GetAxes(T horizontalAxisId, T verticalAxisId, bool reverseHorizontal = false, bool reverseVertical = false);
    Vector2 GetAxesRaw(T horizontalAxisId, T verticalAxisId, bool reverseHorizontal = false, bool reverseVertical = false);

    bool GetButtonPress(T buttonId);
    bool GetButtonRelease(T buttonId);
    bool GetButton(T buttonId);
}

public class NonaInput : INonaInput<string>
{
    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    public float GetAxisRaw(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }

    public Vector2 GetAxes(string horizontalAxisId, string verticalAxisId, bool reverseHorizontal = false, bool reverseVertical = false)
    {
        float horizontalAxis = GetAxis(horizontalAxisId);
        float verticalAxis = GetAxis(verticalAxisId);

        return new Vector2(
            (reverseHorizontal ? -horizontalAxis : horizontalAxis),
            (reverseVertical ? -verticalAxis : verticalAxis)
        );
    }
    public Vector2 GetAxesRaw(string horizontalAxisId, string verticalAxisId, bool reverseHorizontal = false, bool reverseVertical = false)
    {
        float horizontalAxis = GetAxisRaw(horizontalAxisId);
        float verticalAxis = GetAxisRaw(verticalAxisId);

        return new Vector2(
            (reverseHorizontal ? -horizontalAxis : horizontalAxis),
            (reverseVertical ? -verticalAxis : verticalAxis)
        );
    }
    public bool GetButtonPress(string buttonId)
    {
        return Input.GetButtonDown(buttonId);
    }
    public bool GetButtonRelease(string buttonId)
    {
        return Input.GetButtonUp(buttonId);
    }
    public bool GetButton(string buttonId)
    {
        return Input.GetButton(buttonId);
    }
}
