using UnityEngine;

public class VisibilityTest : MonoBehaviour
{
    void Update()
    {
        // Press 'V' key to toggle visibility
        if (Input.GetKeyDown(KeyCode.V))
        {
            bool currentVisibility = gameObject.activeSelf;
            gameObject.SetActive(!currentVisibility);
            Debug.Log($"[VisibilityTest] Toggling {gameObject.name}. New activeSelf: {gameObject.activeSelf}, activeInHierarchy: {gameObject.activeInHierarchy}");
        }
    }
}
