using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using GLTFast.Schema;

public class WilsonLogic : MonoBehaviour
{

    [SerializeField] private int shotCoundown = 3;

    [SerializeField] private int maxShots = 6;
    private int currentShots;

    private GridManager gridManager;

    [SerializeField] private TextMeshProUGUI shotCountdown;
    [SerializeField] private TextMeshProUGUI shotTextChance;

    private bool isStunned = false;

    [SerializeField] private Transform cameraAnchorPlaying;
    [SerializeField] private Transform cameraAnchorWilsonShooting;
    [SerializeField] private Transform cameraAnchorWilsonLever;
    [SerializeField] private Transform mainCam;

    [SerializeField] private Animator leverAnimator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        currentShots = maxShots;
    }

    public void CountShotDown()
    {

        mainCam.DOMove(cameraAnchorWilsonShooting.position, 1.5f).SetEase(Ease.InOutSine);
        mainCam.DORotateQuaternion(cameraAnchorWilsonShooting.rotation, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            shotCountdown.gameObject.SetActive(true);
            shotTextChance.gameObject.SetActive(true);

            DOVirtual.DelayedCall(1f, () =>
            {
                shotCoundown--;
                shotCountdown.text = "Countdown to SHOOT!\n" + shotCoundown.ToString();

                shotTextChance.text = $"{1}/{currentShots}\n" + (1f / currentShots * 100).ToString("F2") + "%";

                if (shotCoundown <= 1)
                {
                    if (!isStunned)
                        Shoot();

                    shotCoundown = 3;

                    return;
                }

                

                isStunned = false;

                ReturnCamera();
            });


        });

        
    }

    public void StunWilson()
    {
        isStunned = true;
    }

    public bool Shoot(bool countDown = true, int currentCountdown = -1)
    {
        int bulletChoice = Random.Range(0, currentShots);


        if(countDown)
        {
            Debug.Log("Shot because of countdown");
        }
        else
        {
            Debug.Log("Shot because presents destroyed");
        }

        bool gameOver = false;

        DOVirtual.DelayedCall(1f, () =>
        {
            if (bulletChoice == 0)
            {
                Debug.Log("Bang! Wilson shot santa.");
                currentShots = maxShots;
                gridManager.GameOver(false);
                gameOver = true; // Indicate that Wilson killed Santa
            }
            else
            {
                Debug.Log("Click! Wilson spared santa.");
                currentShots--; // Decrease the number of remaining shots
                
                gameOver =  false; // Indicate that Wilson did not shoot himself
            }

            ReturnCamera(gameOver);
        });

        return gameOver;
    }

    public void ReturnCamera(bool gameOver = false)
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            mainCam.DOMove(cameraAnchorWilsonLever.position, 1.5f).SetEase(Ease.InOutSine);
            mainCam.DORotateQuaternion(cameraAnchorWilsonLever.rotation, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                leverAnimator.SetTrigger("PullLever");


                DOVirtual.DelayedCall(1f, () =>
                {
                    if (!gameOver)
                        gridManager.SpawnNewRow();

                    mainCam.DOMove(cameraAnchorPlaying.position, 1.5f).SetEase(Ease.InOutSine);
                    mainCam.DORotateQuaternion(cameraAnchorPlaying.rotation, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        shotCountdown.gameObject.SetActive(false);
                        shotTextChance.gameObject.SetActive(false);
                    });
                });
            });
        });
    }

    private void CameraToWilson()
    {

    }
}
