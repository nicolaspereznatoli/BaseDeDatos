using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class shaderscript : MonoBehaviour
{

    public Material effectMaterial;    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, effectMaterial);
    }
    
}
