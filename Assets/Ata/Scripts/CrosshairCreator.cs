using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CrosshairCreator : MonoBehaviour
{
    // Use this script to create a simple crosshair prefab
    
#if UNITY_EDITOR
    [MenuItem("Tools/Create Simple Crosshair")]
    public static GameObject CreateCrosshair()
    {
        // Create parent object
        GameObject crosshairParent = new GameObject("Crosshair");
        
        // Create center
        GameObject center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        center.name = "Center";
        center.transform.SetParent(crosshairParent.transform);
        center.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        center.transform.localPosition = Vector3.zero;
        
        // Create crosshair lines
        CreateCrosshairLine(crosshairParent, "VerticalLine", new Vector3(0f, 0f, 0f), new Vector3(0.05f, 0.4f, 0.05f));
        CreateCrosshairLine(crosshairParent, "HorizontalLine", new Vector3(0f, 0f, 0f), new Vector3(0.4f, 0.05f, 0.05f));
        
        // Create outline
        GameObject outline = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        outline.name = "Outline";
        outline.transform.SetParent(crosshairParent.transform);
        outline.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
        outline.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        
        // Set materials for better visibility
        SetMaterialForChild(crosshairParent, "Center", Color.red);
        SetMaterialForChild(crosshairParent, "VerticalLine", Color.white);
        SetMaterialForChild(crosshairParent, "HorizontalLine", Color.white);
        
        // Make outline semi-transparent
        Material outlineMat = new Material(Shader.Find("Standard"));
        outlineMat.SetFloat("_Mode", 2); // Set to fade mode
        outlineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        outlineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        outlineMat.SetInt("_ZWrite", 0);
        outlineMat.DisableKeyword("_ALPHATEST_ON");
        outlineMat.EnableKeyword("_ALPHABLEND_ON");
        outlineMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        outlineMat.renderQueue = 3000;
        outlineMat.color = new Color(0f, 0f, 0f, 0.3f);
        
        MeshRenderer outlineRenderer = outline.GetComponent<MeshRenderer>();
        outlineRenderer.material = outlineMat;
        
        // Remove all colliders (we don't want the crosshair to interfere with raycasts or physics)
        Collider[] colliders = crosshairParent.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            DestroyImmediate(col);
        }
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/Crosshair.prefab";
        
        // Ensure directory exists
        string directoryPath = System.IO.Path.GetDirectoryName(prefabPath);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }
        
        // Create the prefab
        PrefabUtility.SaveAsPrefabAsset(crosshairParent, prefabPath);
        
        DestroyImmediate(crosshairParent);
        
        // Load the prefab asset
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log("Crosshair prefab created at: " + prefabPath);
        
        // Select the prefab in the project window
        Selection.activeObject = prefab;
        
        return prefab;
    }
    
    private static void CreateCrosshairLine(GameObject parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = name;
        line.transform.SetParent(parent.transform);
        line.transform.localPosition = position;
        line.transform.localScale = scale;
    }
    
    private static void SetMaterialForChild(GameObject parent, string childName, Color color)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material material = new Material(Shader.Find("Standard"));
                material.color = color;
                renderer.material = material;
            }
        }
    }
#endif
}