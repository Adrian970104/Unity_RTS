using UnityEngine;

public interface IMovable
{
    void MoveToPositionByRay(object[] param);//Ray actionPosition);

    void MoveTo(Vector3 targetPos);
}