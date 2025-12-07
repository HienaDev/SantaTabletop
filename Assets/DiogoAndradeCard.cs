using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DiogoAndradeCard : Card
{

    public override void UseEffect()
    {

        TAG_Cat cat = FindAnyObjectByType<TAG_Cat>();

        cat.FlashSequence();

        foreach (GameObject obj in gridManager.GridArray)
        {
            if (obj == null) continue;
            if (obj.TryGetComponent<Present>(out Present present))
            {
                present.DamagePresent();
            }
        }



    }




}
