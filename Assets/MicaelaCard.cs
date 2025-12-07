using UnityEngine;

public class MicaelaCard : Card
{

    public override void UseEffect()
    {
        playerController.DoubleCastActive = true;

        //gridManager.TurnOffFirstRowIndicator();
    }
}
