using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class HolyCrossBullet : Bullet
{
    [SerializeField] private Transform pivot;
    public override void MoveBullet()
    {
        
        Vector3 corssPosition = gridManager.ConvertPosition(currentPosition.x, currentPosition.y);
        pivot.transform.position = new Vector3(corssPosition.x, pivot.transform.position.y, corssPosition.z);

        Debug.Log("wtf???");

        pivot.DOLocalMoveY(0f, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            Debug.Log("Hello cross??");
            List<GameObject> presentsToDamage = new List<GameObject>();

            Vector2Int targetPos = currentPosition - new Vector2Int(0, 6);


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x, targetPos.y));


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x + 1, targetPos.y));


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x - 1, targetPos.y));


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x, targetPos.y + 1));


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x, targetPos.y + 2));


            presentsToDamage.Add(gridManager.GetObjectAtPosition(targetPos.x, targetPos.y - 1));

            Debug.Log("presentsToDamage count: " + presentsToDamage.Count);

            foreach (GameObject presentObj in presentsToDamage)
            {
                Debug.Log("Checking present at position: " + presentObj);
                if (presentObj != null)
                {
                    Present present = presentObj.GetComponent<Present>();
                    if (present != null)
                    {
                        Debug.Log("Damaging present with current health: " + present.CurrentHealth);
                        present.gameObject.transform.DOMoveY(present.transform.position.y + 5f, 0.1f).SetLoops(2, LoopType.Yoyo);
                        if (present.CurrentHealth == 2)
                        {
                            present.DamagePresent();
                            present.DamagePresent();
                        }
                    }
                }
            }

            DOVirtual.DelayedCall(1f, () =>
            {
                BlowBulletUp();
            });

        });

    }

    public override void DisplayPath()
    {
        if (moving)
            return;

        Vector2Int targetPos = currentPosition - new Vector2Int(0, 6);

        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(targetPos);

        path.Add(new Vector2Int(targetPos.x + 1, targetPos.y));
        path.Add(new Vector2Int(targetPos.x - 1, targetPos.y));
        path.Add(new Vector2Int(targetPos.x, targetPos.y + 1));
        path.Add(new Vector2Int(targetPos.x, targetPos.y + 2));
        path.Add(new Vector2Int(targetPos.x, targetPos.y - 1));


        foreach (Vector2Int pos in path)
        {
            Debug.Log("HolyCrossBullet path position: " + pos);
        }

        gridManager.TurnOnPathIndicators(path);
    }

}
