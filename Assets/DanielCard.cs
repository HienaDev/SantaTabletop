using UnityEngine;

public class DanielCard : Card
{
    [SerializeField] private Card[] specialCards;

    public override void UseEffect()
    {

        Card[] currentHand = new Card[playerController.PlayerHand.Count];
        playerController.PlayerHand.CopyTo(currentHand);
        playerController.ClearHand();

        for (int i = 0; i < currentHand.Length; i++)
        {
            playerController.RemoveCardFromDeck(currentHand[i]);
        }

        for (int i = 0; i < currentHand.Length; i++)
        {
            int randomCardIndex = Random.Range(0, specialCards.Length);

            playerController.AddCardToHand(specialCards[randomCardIndex]);

            playerController.AddCardToDeck(specialCards[randomCardIndex]);
        }

        playerController.ArrangeCardsInHand();
    }


}
