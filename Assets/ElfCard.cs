using UnityEngine;

public class ElfCard : Card
{
    [SerializeField] private Bullet elfBulletPrefab;

    public bool isActualElf = false;

    public override void UseEffect()
    {
        playerController.SelectCard(elfBulletPrefab);
    }
}
