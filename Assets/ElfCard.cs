using UnityEngine;

public class ElfCard : Card
{
    [SerializeField] private Bullet elfBulletPrefab;

    public override void UseEffect()
    {
        playerController.SelectCard(elfBulletPrefab);
    }
}
