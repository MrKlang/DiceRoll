using UnityEngine;

public interface IThrowable
{
    bool IsNotMoving();
    Transform GetTransform();
    Rigidbody GetRigidbody();
    void SetGravity(bool useGravity);
    void SetVelocity(Vector3 newVelocity);
}
