using UnityEngine;

public class TAG_OutlineSelected : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void EnableOutline(bool enable)
    {

        meshRenderer.enabled = enable;

    }
}
