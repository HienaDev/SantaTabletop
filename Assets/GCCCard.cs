using UnityEngine;

public class GCCCard : Card
{
    public override void UseEffect()
    {
        playerController.DrawCards(3);
    }
}
