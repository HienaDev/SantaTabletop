using DG.Tweening;
using UnityEngine;
using System.Collections;

public class TAG_Cat : MonoBehaviour
{
    TAG_Cat cat;
    SpriteRenderer[] catSprites;

    private void Awake()
    {
        

        cat = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        Debug.Log("Cat found: " + cat);

        catSprites = cat.GetComponentsInChildren<SpriteRenderer>();

    }

    public void FlashSequence()
    {
        StartCoroutine(FlashSequence(cat, catSprites));
    }

    private IEnumerator FlashSequence(TAG_Cat cat, SpriteRenderer[] catSprites)
    {
        GetComponent<PopUpOnEnable>().PopUp();

        yield return new WaitForSeconds(0.02f);

        

        foreach (SpriteRenderer sprite in catSprites)
        {
            sprite.enabled = true;
            sprite.DOColor(Color.cyan, 0.1f).From(Color.yellow).SetLoops(15, LoopType.Yoyo).OnComplete(() =>
            {
                sprite.enabled = false;

            });
        }

    }
}
