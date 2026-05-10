using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;
    [SerializeField] private bool followZ = true;

    private void LateUpdate()
    {
        if (target == null)
        {
            return; 
        }

        Vector3 currentPosition = transform.position;
        Vector3 desiredPosition = target.position + offset;

        Vector3 targetPosition = new Vector3(
            followX ? desiredPosition.x : currentPosition.x,
            followY ? desiredPosition.y : currentPosition.y,
            followZ ? desiredPosition.z : currentPosition.z);

        transform.position = Vector3.Lerp(
            currentPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
