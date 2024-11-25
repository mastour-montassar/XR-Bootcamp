using UnityEngine;
using UnityEngine.UI;

public class MaterialController : MonoBehaviour
{
    public MeshRenderer targetRenderer;
    public Dropdown materialDropdown;
    public Material[] materials;

    void Start()
    {
        materialDropdown.onValueChanged.AddListener(delegate { ChangeMaterial(); });
    }

    void ChangeMaterial()
    {
        targetRenderer.material = materials[materialDropdown.value];
    }
}