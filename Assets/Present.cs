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

    [SerializeField] private Card[] cards;

    [SerializeField] private Vector2[] spawnChances;

    private PlayerController playerController;

    [SerializeField] private Present coalPrefab;

    [SerializeField] private bool isCoal = false;

    [SerializeField] private SpriteRenderer wonCardPrefab;

    private bool spawnedCoal = false;

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
            coalTemp.Initialize(1, gridPosition);
            spawnedCoal = true;
            FindAnyObjectByType<GridManager>().AddObjectToGrid(gridPosition.x, gridPosition.y, coalTemp.gameObject);
        }
        else
        {
            // Spawn common card
            int index = Random.Range(0, cards.Length);

            SpriteRenderer wonCard = Instantiate(wonCardPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);

            wonCard.gameObject.SetActive(true);
            wonCard.sprite = cards[index].CardImage;
            playerController.AddCardToDeck(cards[index]);
        }

    }

    public void PushPositionDown()
    {
        gridPosition.y += 1;
    }

    public void PushPositionUp()
    {
        gridPosition.y -= 1;
    }

    public void BlowUpPresent()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            GridManager gridManager = FindAnyObjectByType<GridManager>();

            if(!spawnedCoal)
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
