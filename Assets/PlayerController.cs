using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

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

    [SerializeField] private Bullet currentlySelectedCard;

    private Bullet currentBullet;

    private GridManager gridManager;

    [SerializeField] private int playerHandSize = 3;
    [SerializeField] private Card cardPrefab;
    private List<Card> playerHand = new List<Card>();
    [SerializeField] private Transform centerCardPosition;

    [SerializeField] private float cardZOffset = 0.5f;


    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();

        for (int i = 0; i < playerHandSize; i++)
        {
            AddCardToHand(cardPrefab);
        }

        ArrangeCardsInHand();
    }

    void Update()
    {
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
            if (currentBullet != null)
            {
                currentBullet.MoveBullet();
                currentBullet = null;
            }
        }
    }

    /// <summary>
    /// Called when the mouse starts hovering over a new object.
    /// </summary>
    /// <param name="hoveredObject">The GameObject the mouse is currently over.</param>
    private void HandleObjectHovered(GameObject hoveredObject)
    {

        if (hoveredObject.TryGetComponent<IndicatorCell>(out IndicatorCell cell))
        {
            if (currentBullet == null)
            {
                currentBullet = Instantiate(currentlySelectedCard, gridManager.ConvertPosition(cell.Position.x, cell.Position.y), Quaternion.identity);
                currentBullet.Initialize(cell.Position.x, cell.Position.y);
            }
            else
            {
                currentBullet.transform.position = gridManager.ConvertPosition(cell.Position.x, cell.Position.y);
                currentBullet.Initialize(cell.Position.x, cell.Position.y);
            }
        }

        if (hoveredObject.TryGetComponent<Card>(out Card card))
        {
            if (card.placed)
                card.EnterHover();
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
                card.ExitHover();
        }
    }

    public void SelectCard(Bullet card)
    {
        currentlySelectedCard = card;
        Debug.Log("Selected card: " + card.name);
    }

    public void AddCardToHand(Card card)
    {
        Card newCard = Instantiate(card, centerCardPosition.position, Quaternion.identity);
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
            Destroy(card.gameObject);
            ArrangeCardsInHand();
        }
    }

    public void ClearHand()
    {
        foreach (Card card in playerHand)
        {
            Destroy(card.gameObject);
        }
        playerHand.Clear();
    }

}