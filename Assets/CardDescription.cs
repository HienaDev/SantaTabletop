using TMPro;
using UnityEngine;

public class CardDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetCardDescription(int cardCost, string title, string description)
    {
        cost.text = cardCost.ToString();
        titleText.text = title;
        descriptionText.text = description;
    }
}
