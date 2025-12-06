using UnityEngine;
using DG.Tweening;

public class Present : MonoBehaviour
{

    [SerializeField, Range(1, 3)] private int health = 1;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    [SerializeField] private Material health1Material;
    [SerializeField] private Material health2Material;
    [SerializeField] private Material health3Material;

    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int health)
    {
        originalScale = transform.localScale;
        currentHealth = health;
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
            BlowUpPresent();
        }

            return currentHealth;
    }

    public void BlowUpPresent()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
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
