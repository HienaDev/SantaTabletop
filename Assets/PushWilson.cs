using UnityEngine;

public class PushWilson : Card
{

    [SerializeField] private bool left = false;

    public override void UseEffect()
    {
        gridManager.PushWilson(left);
    }
}
