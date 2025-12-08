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

    [SerializeField] private GameObject shotInfo;
    private Vector3 shotInfoOriginalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shotInfoOriginalPos = shotInfo.transform.position;

        gridManager = FindAnyObjectByType<GridManager>();
        currentShots = maxShots;

        shotCountdown.text = "Countdown to SHOOT!\n";

        switch (shotCoundown)
        {
            case 3:
                shotCountdown.text += $"<color=white>{shotCoundown.ToString()}</color>";
                break;
            case 2:
                shotCountdown.text += $"<color=yellow>{shotCoundown.ToString()}</color>";
                break;
            case 1:
                shotCountdown.text += $"<color=red>{shotCoundown.ToString()}</color>";
                break;
        }

        shotTextChance.text = $"Roll above \n<color=red>" + (1 * 100 / (currentShots)).ToString() + "</color>\n to not shoot!";
    }

    public void CountShotDown()
    {
        shotInfo.transform.DOMoveY(shotInfoOriginalPos.y + 4f, 1.5f).SetEase(Ease.InOutSine);

        mainCam.DOMove(cameraAnchorWilsonShooting.position, 1.5f).SetEase(Ease.InOutSine);
        mainCam.DORotateQuaternion(cameraAnchorWilsonShooting.rotation, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {

            DOVirtual.DelayedCall(1f, () =>
            {
                shotCoundown--;
                shotCountdown.text = "Countdown to SHOOT!\n";

                switch (shotCoundown)
                {
                    case 3:
                        shotCountdown.text += $"<color=white>{shotCoundown.ToString()}</color>";
                        break;
                    case 2:
                        shotCountdown.text += $"<color=yellow>{shotCoundown.ToString()}</color>";
                        break;
                    case 1:
                        shotCountdown.text += $"<color=red>{shotCoundown.ToString()}</color>";
                        break;
                }

                shotTextChance.text = $"Roll above \n<color=red>" + (1 * 100 / (currentShots)).ToString() + "</color>\n to not shoot!";

                if (shotCoundown < 1)
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
        float bulletChoice = Random.value;

        shotCountdown.text = "TIME TO SHOOT!";
        if (countDown)
        {
            Debug.Log("Shot because of countdown");
        }
        else
        {
            Debug.Log("Shot because presents destroyed");
        }

        bool gameOver = false;

        DOVirtual.DelayedCall(2f, () =>
        {
            Vector3 shotTextChanceTransformOriginalPos = shotTextChance.transform.position;
            shotTextChance.text = "Rolling...";
            shotTextChance.transform.DOShakePosition(1.5f, 0.01f, 10, 90, false, true).SetEase(Ease.OutQuart);
            DOVirtual.DelayedCall(2f, () =>
            {
                shotTextChance.transform.position = shotTextChanceTransformOriginalPos;
                if (bulletChoice <= (1f / currentShots))
                    shotTextChance.text = "Rolled a\n<color=red>" + ((int)(bulletChoice * 100)).ToString() + "</color>";
                else
                    shotTextChance.text = "Rolled a\n<color=green>" + ((int)(bulletChoice * 100)).ToString() + "</color>";

                DOVirtual.DelayedCall(2f, () =>
                    {
                        if (bulletChoice <= (1f / currentShots))
                        {
                            Debug.Log("<color=red>Bang!</color> <color=red>Wilson shot santa.</color>");
                            shotTextChance.text = "<color=yellow>BANG!</color> \n<color=red>Santa was shot!</color";
                            currentShots = maxShots;
                            gameOver = true; // Indicate that Wilson killed Santa
                            DOVirtual.DelayedCall(2f, () =>
                            {
                                gridManager.GameOver(false);
                            });
                            
                        }
                        else
                        {
                            Debug.Log("Click! Wilson spared santa.");
                            shotTextChance.text = "<color=yellow>CLICK!</color>\nSanta survives this time!";
                            currentShots--; // Decrease the number of remaining shots

                            gameOver = false; // Indicate that Wilson did not shoot himself
                        }

                        ReturnCamera(gameOver);
                    });
            });
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
                        shotInfo.transform.DOMoveY(shotInfoOriginalPos.y, 0.5f).SetEase(Ease.InOutSine);
                    });
                });
            });
        });
    }

    private void CameraToWilson()
    {

    }
}
