using UnityEngine;
using System.Collections;

using Zenject;
using Rewired;

public interface INonaInput<T>
{
    float GetAxis(T axisId);

    Vector2 GetAxes(T horizontalAxisId, T verticalAxisId, bool reverseHorizontal = false, bool reverseVertical = false);

    bool GetButtonPress(T buttonId);
    bool GetButtonRelease(T buttonId);
    bool GetButton(T buttonId);
}

public class NonaInput : INonaInput<string>
{
    private Player player;
    public NonaInput()
    {
        player = ReInput.players.GetPlayer(0);
    }

    public float GetAxis(string axisName)
    {
        return player.GetAxisRaw(GetId(axisName));
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

    public bool GetButtonPress(string buttonId)
    {
        return player.GetButtonDown(GetId(buttonId));
    }
    public bool GetButtonRelease(string buttonId)
    {
        return player.GetButtonUp(GetId(buttonId));
    }
    public bool GetButton(string buttonId)
    {
        return player.GetButton(GetId(buttonId));
    }

    private int GetId(string name)
    {
        return ReInput.mapping.GetActionId(name);
    }
}
