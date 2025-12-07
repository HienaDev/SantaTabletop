using TMPro;
using UnityEngine;

public class WilsonLogic : MonoBehaviour
{

    [SerializeField] private int shotCoundown = 3;

    [SerializeField] private int maxShots = 6;
    private int currentShots;

    private GridManager gridManager;

    [SerializeField] private TextMeshProUGUI shotCountdown;
    [SerializeField] private TextMeshProUGUI shotTextChance;

    private bool isStunned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        currentShots = maxShots;
    }

    public void CountShotDown()
    {

        shotCountdown.gameObject.SetActive(true);
        shotTextChance.gameObject.SetActive(true);

        shotCoundown--;
        shotCountdown.text = "Countdown to SHOOT!\n" + shotCoundown.ToString();

        shotTextChance.text = $"{1}/{currentShots}\n" + (1f / currentShots * 100).ToString("F2") + "%";

        if (shotCoundown <= 0)
        {
            if(!isStunned)
                Shoot();

            shotCoundown = 3;
        }

        isStunned = false;
    }

    public void StunWilson()
    {
        isStunned = true;
    }

    public bool Shoot()
    {
        int bulletChoice = Random.Range(0, currentShots);

        if (bulletChoice == 0)
        {
            Debug.Log("Bang! Wilson shot santa.");
            currentShots = maxShots;
            gridManager.GameOver(false);
            return true; // Indicate that Wilson killed Santa
        }
        else
        {
            Debug.Log("Click! Wilson spared santa.");
            currentShots--; // Decrease the number of remaining shots
            return false; // Indicate that Wilson did not shoot himself
        }
    }
}
