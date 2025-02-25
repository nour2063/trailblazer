using UnityEngine;
using UnityEngine.Rendering;

public class ShaderPropertyDebug : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null || rend.sharedMaterial == null)
        {
            Debug.LogError("❌ Renderer or Material not found!");
            return;
        }

        Material mat = rend.sharedMaterial;
        Shader shader = mat.shader;

        Debug.Log($"🔍 Shader: {shader.name} has {shader.GetPropertyCount()} properties:");
        
        for (int i = 0; i < shader.GetPropertyCount(); i++)
        {
            string propName = shader.GetPropertyName(i);
            ShaderPropertyType type = shader.GetPropertyType(i);
            Debug.Log($"📌 Property {i}: {propName} ({type})");
        }
    }
}