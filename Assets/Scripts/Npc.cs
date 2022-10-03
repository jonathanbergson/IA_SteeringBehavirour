using UnityEngine;

public class Npc : MonoBehaviour
{
    private enum Sides
    {
        Left = -1,
        Right = 1,
    }

    private static readonly Color RaycastColor = new(0.2627451f, 0.6666667f, 0.5450981f);
    private static readonly Color RaycastColorHit = new(00.9764706f, 0.254902f, 0.2666667f);

    private const float RaycastSidesDistance = 4f;
    private const float RaycastCenterDistance = 5f;
    private const float RaycastCenterMinDistance = 2f;

    private RaycastHit _hitCenter;

    private bool _hasHitOnLeft;
    private bool _hasHitOnCenter;
    private bool _hasHitOnRight;

    private const int NoSpeed = 0;
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float rotationMaxSpeed = 180f;
    [SerializeField] private float rotateSideCountDown;
    [SerializeField] private Sides rotationSide = Sides.Right;

    private void Update()
    {
        HandleRotation();
        HandleMove();
    }

    private void FixedUpdate()
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastCenterDirection = transform.forward;
        Vector3 raycastLeftDirection = raycastCenterDirection * 2 + transform.right * -1;
        Vector3 raycastRightDirection = raycastCenterDirection * 2 + transform.right;

        _hasHitOnLeft = Physics.Raycast(raycastOrigin, raycastLeftDirection, out _, RaycastSidesDistance);
        Debug.DrawRay(raycastOrigin, raycastLeftDirection.normalized * RaycastSidesDistance, _hasHitOnLeft ? RaycastColorHit : RaycastColor);

        _hasHitOnCenter = Physics.Raycast(raycastOrigin, raycastCenterDirection, out _hitCenter, RaycastCenterDistance);
        Debug.DrawRay(raycastOrigin, raycastCenterDirection.normalized * RaycastCenterDistance, _hasHitOnCenter ? RaycastColorHit : RaycastColor);

        _hasHitOnRight = Physics.Raycast(raycastOrigin, raycastRightDirection, out _, RaycastSidesDistance);
        Debug.DrawRay(raycastOrigin, raycastRightDirection.normalized * RaycastSidesDistance, _hasHitOnRight ? RaycastColorHit : RaycastColor);
    }

    private void HandleMove()
    {
        float speed = CalcTranslateSpeed();
        Vector3 direction = transform.forward;

        transform.position += direction * speed * Time.deltaTime;
    }

    private float CalcTranslateSpeed()
    {
        if (_hasHitOnCenter == false) return maxSpeed;

        var isWallClose = _hitCenter.distance < RaycastCenterMinDistance;
        float speedDecreasing = maxSpeed * _hitCenter.distance / RaycastCenterDistance;

        return isWallClose ? NoSpeed : speedDecreasing;
    }

    private void HandleRotation()
    {
        bool allHits = _hasHitOnCenter && _hasHitOnLeft && _hasHitOnRight;
        bool justCenterHit = _hasHitOnCenter && !_hasHitOnLeft && !_hasHitOnRight;

        if (allHits || justCenterHit)
        {
            Sides side = DefineRotationSide();
            Rotate(side, rotationMaxSpeed);
        }
        else if (_hasHitOnLeft && _hasHitOnRight == false) Rotate(Sides.Right);
        else if (_hasHitOnRight && _hasHitOnLeft == false) Rotate(Sides.Left);
    }

    private Sides DefineRotationSide()
    {
        if (rotateSideCountDown <= Time.time)
        {
            rotateSideCountDown = Time.time + 5f;

            // TODO: Checar qual hit lateral é menor antes de fazer um random
            float r = Random.Range(-1f, 1f);
            rotationSide = r >= 0 ? Sides.Right : Sides.Left;
        }

        return rotationSide;
    }

    private void Rotate(Sides side, float speed = 90f)
    {
        rotationSide = side; // NOTE: Salva o último lado que o NPC virou
        float yAngle = speed * (float) side * Time.deltaTime;
        transform.Rotate(0, yAngle, 0, Space.Self);
    }
}
