using UnityEngine;

public class ClickDebugger : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button was pressed down this frame
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button click DETECTED by Unity!");
        }
    }
}