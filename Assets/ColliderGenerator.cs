// Assets/Editor/ColliderGenerator.cs
using UnityEngine;
using UnityEditor; // This namespace is only available in Editor scripts

public class ColliderGenerator : EditorWindow
{
    // Add a menu item to the Unity Editor
    [MenuItem("Tools/Collider/Generate Mesh Colliders on Children")]
    public static void ShowWindow()
    {
        GetWindow<ColliderGenerator>("Collider Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Generate Mesh Colliders", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Select a parent GameObject (e.g., your imported map root). This will add a Mesh Collider to all its immediate children that have a Mesh Filter, and assign their mesh.", MessageType.Info);

        if (GUILayout.Button("Generate Colliders for Selected Children"))
        {
            GenerateColliders();
        }

        if (GUILayout.Button("Clear All Mesh Colliders from Selected Children"))
        {
            ClearColliders();
        }
    }

    static void GenerateColliders()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select the parent of your map meshes.");
            return;
        }

        GameObject parent = Selection.activeGameObject;
        int collidersAdded = 0;

        foreach (Transform child in parent.transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                // Ensure it doesn't already have one
                MeshCollider mc = child.GetComponent<MeshCollider>();
                if (mc == null)
                {
                    mc = child.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh; // Assign the mesh from the MeshFilter
                    Debug.Log($"Added Mesh Collider to: {child.name}");
                    collidersAdded++;
                }
                else
                {
                    Debug.Log($"Mesh Collider already exists on: {child.name}. Ensuring mesh is assigned.");
                    mc.sharedMesh = mf.sharedMesh; // Just re-assign to be safe
                }
            }
        }
        Debug.Log($"Generated {collidersAdded} Mesh Colliders on children of {parent.name}.");
        if (collidersAdded == 0)
        {
            Debug.LogWarning("No Mesh Colliders were added. Do your children have Mesh Filters?");
        }
    }

    static void ClearColliders()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select the parent of your map meshes.");
            return;
        }

        GameObject parent = Selection.activeGameObject;
        int collidersRemoved = 0;

        foreach (Transform child in parent.transform)
        {
            MeshCollider mc = child.GetComponent<MeshCollider>();
            if (mc != null)
            {
                DestroyImmediate(mc); // Destroy it immediately in editor
                Debug.Log($"Removed Mesh Collider from: {child.name}");
                collidersRemoved++;
            }
        }
        Debug.Log($"Removed {collidersRemoved} Mesh Colliders from children of {parent.name}.");
    }
}