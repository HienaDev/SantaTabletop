using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescription : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cost;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Sprite[] manaIcons;

    public void SetCardDescription(int cardCost, Sprite image, string title, string description)
    {
        cost.sprite = manaIcons[cardCost];
        cardImage.sprite = image;
        titleText.text = title;
        descriptionText.text = description;
    }
}
