using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // The maximum distance the raycast will check for objects
    [Tooltip("The maximum distance the raycast will check for objects.")]
    [SerializeField] private float hoverDistance = 100f;

    // Optional: A LayerMask to ensure the raycast only hits certain objects (e.g., 'Interactable')
    [Tooltip("Layers to include in the raycast check.")]
    [SerializeField] private LayerMask hoverMask;

    // Stores the last object we were hovering over
    private GameObject lastHoveredObject = null;

    [SerializeField] private Bullet currentlySelectedBullet;

    private Bullet bulletInstance;
    private Bullet bulletMoving;

    private GridManager gridManager;

    [SerializeField] private Transform handParent;
    private Vector3 handOriginalPosition;
    private bool handHidden = false;    

    [SerializeField] private int drawPerTurn = 3;
    [SerializeField] private Card cardPrefab;
    private List<Card> playerHand = new List<Card>();
    [SerializeField] private Transform centerCardPosition;

    [SerializeField] private float cardZOffset = 0.5f;

    [SerializeField] private Card[] initialDeck;
    private List<Card> deck = new List<Card>();
    private int currentCardIndex = 0;


    private Card currentlyHoveredCard;
    private Vector3 draggedStartSpawnPosition;
    private Card currentlyDraggedCard;

    [SerializeField] private float distanceToPlayCard = 1f;

    [SerializeField] private int energyPerTurn = 3;
    private int currentEnergy;

    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private GameObject endTurnButton;

    private void Start()
    {
        handOriginalPosition = handParent.position;
        currentlyDraggedCard = null;
        deck = new List<Card>(initialDeck);

        currentEnergy = energyPerTurn;

        gridManager = FindAnyObjectByType<GridManager>();

       
    }

    public void StartTurn()
    {
        currentEnergy = energyPerTurn;
        UpdateUICost();
        ClearHand();
        DrawCards(drawPerTurn);
        endTurnButton.SetActive(true);
    }

    public void EndTurn()
    {
        ClearHand();
        
    }

    public void DrawCards(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            if(currentCardIndex >= deck.Count)
            {
                ShuffleDiscardDeck();
            }

            Card drawnCard = deck[currentCardIndex];

            AddCardToHand(drawnCard);

            currentCardIndex++;
        }
        ArrangeCardsInHand();
    }

    public void AddCardToDeck(Card card)
    {
        deck.Add(card);
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (deck.Contains(card))
        {
            deck.Remove(card);
        }
    }

    public void ShuffleDiscardDeck()
    {
        ShuffleDeck();
        currentCardIndex = 0;
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void Update()
    {

        if(currentlySelectedBullet != null || gridManager.WilsonDoingStuff)
        {
            HideHand(true);
        }
        else if(bulletMoving != null)
        {
            if(!bulletMoving.stoppedActing)
            {
                HideHand(true);
            }
            else
            {
                bulletMoving = null;
                HideHand(false);
            }
        }
        else
        {
            HideHand(false);
        }

        // 1. Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 2. Perform the raycast check
        if (Physics.Raycast(ray, out hit, hoverDistance, hoverMask))
        {
            // We hit an object!
            GameObject currentHitObject = hit.collider.gameObject;

            // Check if this is a NEW object being hovered
            if (currentHitObject != lastHoveredObject)
            {
                // Unhover the old object (optional, for cleanup)
                if (lastHoveredObject != null)
                {
                    HandleObjectUnhovered(lastHoveredObject);
                }

                // Hover the new object
                HandleObjectHovered(currentHitObject);
                lastHoveredObject = currentHitObject;
            }
            // If it's the SAME object, do nothing (or continue hovering logic here)
        }
        else
        {
            // No object was hit
            if (lastHoveredObject != null)
            {
                // Unhover the object that was previously being hovered
                HandleObjectUnhovered(lastHoveredObject);
                lastHoveredObject = null;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (bulletInstance != null)
            {
                gridManager.TurnOffAllIndicatorCells();
                bulletInstance.MoveBullet();

                bulletMoving = bulletInstance;

                bulletInstance = null;
                currentlySelectedBullet = null;
                gridManager.TurnOffFirstRowIndicator();
            }

            if(currentlyDraggedCard == null && currentlyHoveredCard != null)
            {
                currentlyDraggedCard = currentlyHoveredCard;
                draggedStartSpawnPosition = currentlyDraggedCard.transform.position;
            }
        }


        if(currentlyDraggedCard != null && Input.GetMouseButton(0))
            MoveDraggedCardWithMouse();


        if (Input.GetMouseButtonUp(0))
        {
            if (currentlyDraggedCard != null)
            {
                if(Vector3.Distance(currentlyDraggedCard.transform.position, draggedStartSpawnPosition) >= distanceToPlayCard)
                {
                    if(currentlyDraggedCard.Cost > currentEnergy)
                    {
                        ReturnToPosition(draggedStartSpawnPosition);
                        currentlyDraggedCard.UnreadyToUse();
                        currentlyDraggedCard = null;
                        return;
                    }
                    gridManager.TurnOnFirstRowIndicator();
                    PlayCard();
                    RemoveCardFromHand(currentlyDraggedCard);
                }
                else
                    ReturnToPosition(draggedStartSpawnPosition);

                currentlyDraggedCard = null;
            }
        }

        if(currentlyDraggedCard != null && Input.GetKeyDown(KeyCode.Escape))
        {
            
            ReturnToPosition(draggedStartSpawnPosition);
            currentlyDraggedCard.UnreadyToUse();
            currentlyDraggedCard = null;
            currentlyHoveredCard = null;
        }
    }

    private void HideHand(bool hide)
    {
        if(handHidden && !hide)
        {
            foreach (Card card in playerHand)
            {
                card.HideCard(false);
            }
            endTurnButton.SetActive(true);
            handHidden = false; 
            return;
        }
        else if (!handHidden && hide)
        {
            foreach (Card card in playerHand)
            {
                card.HideCard(true);
            }
            endTurnButton.SetActive(false);
            handHidden = true;
            return;
        }

    }

    private void MoveDraggedCardWithMouse()
    {
        // The normal vector of the local XZ plane is the object's local Y-axis (transform.up).
        Vector3 planeNormal = currentlyDraggedCard.transform.up;

        // The point the plane passes through is the object's current world position.
        Vector3 planeWorldPoint = currentlyDraggedCard.transform.position + transform.TransformVector(Vector3.zero);

        // 2. Define the local XZ plane in world space.
        Plane localPlane = new Plane(planeNormal, planeWorldPoint);

        // 3. Create a ray from the camera through the mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distance;

        // 4. Check for intersection between the ray and the local plane.
        if (localPlane.Raycast(ray, out distance))
        {
            // 5. Get the exact world point of the intersection.
            Vector3 targetPoint = ray.GetPoint(distance);

            // 6. Move the object to that position.
            currentlyDraggedCard.transform.position = targetPoint;
        }

        if (Vector3.Distance(currentlyDraggedCard.transform.position, draggedStartSpawnPosition) >= distanceToPlayCard)
        {
            currentlyDraggedCard.ReadyToUse();
        }
        else
        {
            currentlyDraggedCard.UnreadyToUse();
        }
    }

    private void PlayCard()
    {
        currentlyDraggedCard.UseEffect();
        currentEnergy -= currentlyDraggedCard.Cost;
        UpdateUICost();
    }

    private void UpdateUICost()
    {
        energyText.text = currentEnergy.ToString() + "/" + energyPerTurn;
    }

    public void ReturnToPosition(Vector3 targetPosition)
    {
        currentlyDraggedCard.MoveCardToPosition(targetPosition, 5f, false);
    }

    /// <summary>
    /// Called when the mouse starts hovering over a new object.
    /// </summary>
    /// <param name="hoveredObject">The GameObject the mouse is currently over.</param>
    private void HandleObjectHovered(GameObject hoveredObject)
    {

        if (currentlySelectedBullet != null && hoveredObject.TryGetComponent<IndicatorCell>(out IndicatorCell cell))
        {
            if (bulletInstance == null)
            {
                bulletInstance = Instantiate(currentlySelectedBullet, gridManager.ConvertPosition(cell.Position.x, cell.Position.y), Quaternion.identity);
                bulletInstance.Initialize(cell.Position.x, cell.Position.y);
            }
            else
            {
                bulletInstance.transform.position = gridManager.ConvertPosition(cell.Position.x, cell.Position.y);
                bulletInstance.Initialize(cell.Position.x, cell.Position.y);
            }
        }

        if(currentlySelectedBullet == null)
            if (hoveredObject.TryGetComponent<Card>(out Card card))
            {
                if (card.placed)
                {
                    currentlyHoveredCard = card;
                    card.EnterHover();
                }
                
            }
    }

    /// <summary>
    /// Called when the mouse moves off the last hovered object.
    /// </summary>
    /// <param name="unhoveredObject">The GameObject the mouse just left.</param>
    private void HandleObjectUnhovered(GameObject unhoveredObject)
    {
        if(unhoveredObject.TryGetComponent<Card>(out Card card))
        {
            if(card.placed)
            {
                currentlyHoveredCard = null;
                card.ExitHover();
            }
        }
    }

    public void SelectCard(Bullet card)
    {
        currentlySelectedBullet = card;
        Debug.Log("Selected card: " + card.name);
    }

    public void AddCardToHand(Card card)
    {
        Card newCard = Instantiate(card, centerCardPosition.position, Quaternion.identity);
        newCard.transform.parent = handParent;
        playerHand.Add(newCard);
    }

    public void ArrangeCardsInHand()
    {
        for (int i = 0; i < playerHand.Count; i++)
        {
            float zOffset = (i - (playerHand.Count - 1) / 2f) * cardZOffset;
            Vector3 targetPosition = centerCardPosition.position + new Vector3(0, i * 0.02f, zOffset);
            playerHand[i].transform.eulerAngles = centerCardPosition.eulerAngles;
            playerHand[i].transform.localEulerAngles += new Vector3(0, (i - (playerHand.Count - 1) / 2f) * 2f, 0f);
            playerHand[i].MoveCardToPosition(targetPosition, 5f);
        }
    }

    public void RemoveCardFromHand(Card card)
    {
        if (playerHand.Contains(card))
        {
            playerHand.Remove(card);
            card.DestroyCard();
        }
    }

    public void ClearHand()
    {
        foreach (Card card in playerHand)
        {
            card.DestroyCard();
        }
        playerHand.Clear();
    }

}