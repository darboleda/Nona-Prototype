using UnityEngine;
using System.Collections;

public struct NonaCollision
{

    public readonly NonaInformation Nona;
    public bool Grounded;

    public NonaCollision(NonaInformation nona)
    {
        this.Nona = nona;
        this.Grounded = false;
    }
}

public class NonaInformation
{
    private NonaController controller;

    public NonaInformation(NonaController controller)
    {
        this.controller = controller;
    }

    public Vector2 Position
    {
        get
        {
            return (Vector2)(this.controller.transform.position);
        }
    }

    public Vector2 Velocity
    {
        get
        {
            return (Vector2)(this.controller.velocity);
        }
    }
}
