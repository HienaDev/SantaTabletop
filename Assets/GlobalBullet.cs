using UnityEngine;

public class GlobalBullet : Bullet
{



    public override void MoveBullet()
    {
        moving = false;
        BlowBulletUp();
    }

    
}
