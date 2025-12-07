using UnityEngine;
using DG.Tweening;

public class Present : MonoBehaviour
{

    private Vector2Int gridPosition;

    [SerializeField, Range(1, 3)] private int health = 1;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    [SerializeField] private Material health1Material;
    [SerializeField] private Material health2Material;
    [SerializeField] private Material health3Material;

    private Vector3 originalScale;

    [SerializeField] private Card[] commonCard;
    [SerializeField] private Card[] godCards;

    [SerializeField] private Vector3[] spawnChances;

    private PlayerController playerController;

    [SerializeField] private Present coalPrefab;

    [SerializeField] private bool isCoal = false;

    [SerializeField] private SpriteRenderer wonCardPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int health, Vector2Int pos)
    {
        originalScale = transform.localScale;
        gridPosition = pos;

        if (isCoal)
        {

            this.health = 1;
            currentHealth = 1;
            PopUpAnimation();
            return;
        }
        currentHealth = health;
        this.health = health;
        UpdateMaterial();

        PopUpAnimation();
    }

    public void PopUpAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);
    }

    public int DamagePresent()
    {
        currentHealth--;

        if(currentHealth > 0)
        {
            UpdateMaterial();
            ShakePresent();
        }
        else
        {
            if(!isCoal)
                GiveRewards();

            BlowUpPresent();
        }

        return currentHealth;
    }

    public void GiveRewards()
    {
        float roll = Random.value;

        Debug.Log("Present roll: " + roll); 

        if (roll <= spawnChances[health - 1].x)
        {
            // Coal
            Present coalTemp = Instantiate(coalPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            FindAnyObjectByType<GridManager>().AddObjectToGrid(gridPosition.x, gridPosition.y, coalTemp.gameObject);
        }
        else if (roll <= spawnChances[health - 1].x + spawnChances[health - 1].y)
        {
            // Spawn common card
            int index = Random.Range(0, commonCard.Length);

            SpriteRenderer wonCard = Instantiate(wonCardPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);

            wonCard.gameObject.SetActive(true);
            wonCard.sprite = commonCard[index].CardImage;
            playerController.AddCardToDeck(commonCard[index]);
        }
        else
        {
            int index = Random.Range(0, godCards.Length);

            SpriteRenderer wonCard = Instantiate(wonCardPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);

            wonCard.gameObject.SetActive(true);
            wonCard.sprite = godCards[index].CardImage;
            playerController.AddCardToDeck(godCards[index]);
        }
    }

    public void PushPositionDown()
    {
        gridPosition.y += 1;
    }

    public void BlowUpPresent()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            GridManager gridManager = FindAnyObjectByType<GridManager>();
            gridManager.AddObjectToGrid(gridPosition.x, gridPosition.y, null);
            Destroy(gameObject);
        });
    }

    private void ShakePresent()
    {
        transform.DOShakePosition(0.7f, 0.2f, 10, 90, false, true);
    }

    private void UpdateMaterial()
    {
        switch (currentHealth)
        {
            case 3:
                GetComponent<Renderer>().material = health3Material;
                break;
            case 2:
                GetComponent<Renderer>().material = health2Material;
                break;
            case 1:
                GetComponent<Renderer>().material = health1Material;
                break;
            default:
                break;
        }
    }
}
