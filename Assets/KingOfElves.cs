using UnityEngine;

public class KingOfElves : Card
{
    public override void UseEffect()
    {
        playerController.BuffAllElfCards();
    }
}
