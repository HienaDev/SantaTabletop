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

        if(onStartOnly)
        {
            // 1. Calculate the direction vector from the object TO the camera.
            Vector3 directionToCamera = mainCameraTransform.position - transform.position;

            // 2. CRITICAL STEP: Flatten the direction vector.
            //    By setting the Y-component to zero, we ensure the object only rotates
            //    around its local Y-axis and ignores pitching up or down.
            directionToCamera.z = 0f;

            // 3. Ensure the vector isn't zero (e.g., if the object and camera are at the same Y level)
            if (directionToCamera == Vector3.zero)
            {
                // If the camera is perfectly level with the object, do nothing to prevent errors.
                return;
            }

            // 4. Use Quaternion.LookRotation to create the rotation.
            //    Since the Y component is zero, the resulting rotation will only contain Y-axis rotation.
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // 5. Apply the rotation directly to the object.
            transform.rotation = targetRotation;
        }
    }

    void LateUpdate()
    {
        if (onStartOnly) return;
        if (mainCameraTransform == null) return;

        // 1. Calculate the direction vector from the object TO the camera.
        Vector3 directionToCamera = mainCameraTransform.position - transform.position;

        // 2. CRITICAL STEP: Flatten the direction vector.
        //    By setting the Y-component to zero, we ensure the object only rotates
        //    around its local Y-axis and ignores pitching up or down.
        directionToCamera.z = 0f;

        // 3. Ensure the vector isn't zero (e.g., if the object and camera are at the same Y level)
        if (directionToCamera == Vector3.zero)
        {
            // If the camera is perfectly level with the object, do nothing to prevent errors.
            return;
        }

        // 4. Use Quaternion.LookRotation to create the rotation.
        //    Since the Y component is zero, the resulting rotation will only contain Y-axis rotation.
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // 5. Apply the rotation directly to the object.
        transform.rotation = targetRotation;
    }
}