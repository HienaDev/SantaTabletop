using UnityEngine;

public class LookAtCameraZ : MonoBehaviour
{
    private Transform mainCameraTransform;

    [SerializeField] private bool onStartOnly = false;

    void Awake()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("LookAtCameraZ requires a GameObject tagged 'MainCamera'.");
            enabled = false;
        }

        if (onStartOnly)
        {
            FaceCamera();
        }
    }

    void LateUpdate()
    {
        if (!onStartOnly)
            FaceCamera();
    }

    private void FaceCamera()
    {
        if (mainCameraTransform == null) return;

        // Use the direction FROM the object TO the camera
        Vector3 direction = mainCameraTransform.position - transform.position;

        // Flatten using the correct axis (Y-axis only rotation)
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // This faces the object toward the camera PROPERLY
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
