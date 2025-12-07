using UnityEngine;

public class FachadaCard : Card
{
    public override void UseEffect()
    {
        gridManager.fachadaDebuff = true;
    }
}
