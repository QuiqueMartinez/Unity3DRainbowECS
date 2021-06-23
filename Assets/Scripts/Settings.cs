using Unity.Entities;
using UnityEngine;

[System.Serializable]
public class Settings : MonoBehaviour
{
    public Mesh droppletMesh;
    public Material droppletMaterial;
    public int side = 60;
    public Transform cam;
    public Transform sun;
}
