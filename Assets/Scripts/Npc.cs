using UnityEngine;

public class Npc : MonoBehaviour
{
    private struct Sides
    {
        public static float Left = -1f;
        public static float Right = 1f;
    }

    private static readonly Color RaycastColor = new(0.2627451f, 0.6666667f, 0.5450981f);
    private static readonly Color RaycastColorHit = new(00.9764706f, 0.254902f, 0.2666667f);

    private const float RaycastSidesDistance = 4f;
    private const float RaycastCenterDistance = 5f;
    private const float RaycastCenterMinDistance = 2f;

    private RaycastHit _hitLeft;
    private RaycastHit _hitCenter;
    private RaycastHit _hitRight;

    private bool _hasHitOnLeft;
    private bool _hasHitOnCenter;
    private bool _hasHitOnRight;

    private const int NoSpeed = 0;
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float rotationMinSpeed = 90f;
    [SerializeField] private float rotationMaxSpeed = 180f;
    [SerializeField] private float rotateSideCountDown;
    [SerializeField] private float rotationSide = Sides.Right;

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

        _hasHitOnLeft = Physics.Raycast(raycastOrigin, raycastLeftDirection, out _hitLeft, RaycastSidesDistance);
        Debug.DrawRay(raycastOrigin, raycastLeftDirection.normalized * RaycastSidesDistance, _hasHitOnLeft ? RaycastColorHit : RaycastColor);

        _hasHitOnCenter = Physics.Raycast(raycastOrigin, raycastCenterDirection, out _hitCenter, RaycastCenterDistance);
        Debug.DrawRay(raycastOrigin, raycastCenterDirection.normalized * RaycastCenterDistance, _hasHitOnCenter ? RaycastColorHit : RaycastColor);

        _hasHitOnRight = Physics.Raycast(raycastOrigin, raycastRightDirection, out _hitRight, RaycastSidesDistance);
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
        if (_hasHitOnCenter)
        {
            if (_hasHitOnLeft && _hasHitOnRight == false)
                transform.Rotate(0, rotationMinSpeed * Sides.Right * Time.deltaTime, 0, Space.Self);
            else if (_hasHitOnRight && _hasHitOnLeft == false)
                transform.Rotate(0, rotationMinSpeed * Sides.Left * Time.deltaTime, 0, Space.Self);
            else
            {
                rotationSide = DefineRotationSide();
                transform.Rotate(0, rotationSide * rotationMaxSpeed * Time.deltaTime, 0, Space.Self);
            }
        }
        else
        {
            if (_hasHitOnLeft && _hasHitOnRight == false)
                transform.Rotate(0, rotationMinSpeed * Sides.Right * Time.deltaTime, 0, Space.Self);
            else if (_hasHitOnRight && _hasHitOnLeft == false)
                transform.Rotate(0, rotationMinSpeed * Sides.Left * Time.deltaTime, 0, Space.Self);
        }
    }

    private float DefineRotationSide()
    {
        if (rotateSideCountDown <= Time.time)
        {
            rotateSideCountDown = Time.time + 5f;

            float r = Random.Range(Sides.Left, Sides.Right);
            rotationSide = r >= 0 ? Sides.Right : Sides.Left;
        }

        return rotationSide;
    }
}
