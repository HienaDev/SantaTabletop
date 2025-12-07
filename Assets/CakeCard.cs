using UnityEngine;

public class CakeCard : Card
{
    public override void UseEffect()
    {
        playerController.AddEnergy(2);
    }
}
