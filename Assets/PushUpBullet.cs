using System.Collections.Generic;
using UnityEngine;

public class PushUpBullet : Bullet
{
    public override void MoveBullet()
    {
        gridManager.PushPresentCollumn(currentPosition.x);
        BlowBulletUp();
    }

    public override void DisplayPath()
    {
        if (moving)
            return;

        Vector2Int targetPos = currentPosition;

        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(currentPosition);

        while (true)
        {

            targetPos += new Vector2Int(0, -1);

            Debug.Log("Checking position: " + targetPos);
            if (gridManager.CheckIfPositionOutsideGrid(targetPos.x, targetPos.y))
            {
                break;
            }

            // Here you can add code to visually display the path if needed

            path.Add(targetPos);
        }

        gridManager.TurnOnPathIndicators(path);
    }
}
