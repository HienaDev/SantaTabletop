using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DiogoAndradeCard : Card
{

    public override void UseEffect()
    {

        TAG_Cat cat = FindAnyObjectByType<TAG_Cat>();
        Debug.Log("Cat found: " + cat);

        SpriteRenderer[] catSprites = cat.GetComponentsInChildren<SpriteRenderer>();

        foreach (GameObject obj in gridManager.GridArray)
        {
            if (obj == null) continue;
            if (obj.TryGetComponent<Present>(out Present present))
            {
                present.DamagePresent();
            }
        }

        StartCoroutine(FlashSequence(cat, catSprites));

    }

    private IEnumerator FlashSequence(TAG_Cat cat, SpriteRenderer[] catSprites)
    {
        cat.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.02f);

        cat.gameObject.SetActive(true);

        foreach (SpriteRenderer sprite in catSprites)
        {
            sprite.enabled = true;
            sprite.DOColor(Color.cyan, 0.15f).SetLoops(15, LoopType.Yoyo).OnComplete(() =>
            {
                sprite.enabled = false;

            });
        }
    }


}
