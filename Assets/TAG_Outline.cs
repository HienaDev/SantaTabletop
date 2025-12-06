using UnityEngine;

public class TAG_Outline : MonoBehaviour
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
