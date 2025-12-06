using UnityEngine;

public class MicaelaCard : Card
{
    public override void UseEffect()
    {
        playerController.DoubleCastActive = true;
        GridManager gridManager = FindAnyObjectByType<GridManager>();
        gridManager.TurnOffFirstRowIndicator();
    }
}
