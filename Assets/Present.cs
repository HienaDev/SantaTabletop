using UnityEngine;

public class Present : MonoBehaviour
{

    [SerializeField, Range(1, 3)] private int health = 1;
    private int currentHealth;
    [SerializeField] private Material health1Material;
    [SerializeField] private Material health2Material;
    [SerializeField] private Material health3Material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = health;

        currentHealth = Random.Range(1, 4);

        UpdateMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int health)
    {

    }

    public int DamagePresent()
    {
        currentHealth--;

        if(currentHealth > 0)
        {
            UpdateMaterial();
        }
       
        return currentHealth;
    }

    public void BlowUpPresent()
    {
        Destroy(gameObject);
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
