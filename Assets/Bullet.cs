using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{

    protected bool moving = false;

    protected GridManager gridManager;
    protected Vector3 originalScale;

    protected Vector2Int currentPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        originalScale = transform.localScale;
    }

    public void Initialize(int x, int y)
    {
        currentPosition = new Vector2Int(x, y);
        DisplayPath();  
    }

    public void StartMoving()
    {
        MoveBullet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void MoveBullet()
    {
        // Implement bullet movement logic here
    }

    public virtual void OnStopMoveAction()
    {
        // Implement logic for when the bullet stops moving
    }

    public virtual void BlowBulletUp()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            if(currentPosition.y == 0)
            {
                CheckIfHitWilson(currentPosition.x);
            }

                Destroy(gameObject);
        });

    }

    public virtual void CheckIfHitWilson(int x)
    {
        gridManager.CheckIfHitWilson(x);
    }

    public virtual void DisplayPath()
    {
        // Implement path display logic here
    }
}
