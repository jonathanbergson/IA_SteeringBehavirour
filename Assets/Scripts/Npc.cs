using UnityEngine;

public class Npc : MonoBehaviour
{
    private const float RaycastCenterDistance = 5f;
    private const float RaycastCenterMinDistance = 2f;
    private const float RaycastSidesDistance = 3f;


    private static readonly Color RaycastColor = new(0.2627451f, 0.6666667f, 0.5450981f);
    private static readonly Color RaycastColorHit = new(00.9764706f, 0.254902f, 0.2666667f);

    private readonly Vector3 _raycastLeftDirection = new(-.5f, 0, 1);
    private readonly Vector3 _raycastCenterDirection = Vector3.forward;
    private readonly Vector3 _raycastRightDirection = new(.5f, 0, 1);

    private RaycastHit _hitLeft;
    private RaycastHit _hitCenter;
    private RaycastHit _hitRight;

    private bool _hasHitOnLeft;
    private bool _hasHitOnCenter;
    private bool _hasHitOnRight;

    [SerializeField] private float maxSpeed = 5f;

    private void Update()
    {
        HandleMove();
    }

    private void FixedUpdate()
    {
        Vector3 raycastStart = transform.position;

        _hasHitOnLeft = Physics.Raycast(raycastStart, _raycastLeftDirection, out _hitLeft, RaycastSidesDistance);
        Debug.DrawRay(raycastStart, _raycastLeftDirection * RaycastSidesDistance, _hasHitOnLeft ? RaycastColorHit : RaycastColor);

        _hasHitOnCenter = Physics.Raycast(raycastStart, _raycastCenterDirection, out _hitCenter, RaycastCenterDistance);
        Debug.DrawRay(raycastStart, _raycastCenterDirection * RaycastCenterDistance, _hasHitOnCenter ? RaycastColorHit : RaycastColor);

        _hasHitOnRight = Physics.Raycast(raycastStart, _raycastRightDirection, out _hitRight, RaycastSidesDistance);
        Debug.DrawRay(raycastStart, _raycastRightDirection * RaycastSidesDistance, _hasHitOnRight ? RaycastColorHit : RaycastColor);
    }

    private void HandleMove()
    {
        float speed = maxSpeed;
        Vector3 direction = Vector3.forward;

        if (_hasHitOnCenter)
        {
            bool isWallClose = _hitCenter.distance < RaycastCenterMinDistance;
            float speedDecreasing = maxSpeed * _hitCenter.distance / RaycastCenterDistance;

            speed = isWallClose ? 0f : speedDecreasing;
        }

        transform.position += direction * speed * Time.deltaTime;
    }
}
