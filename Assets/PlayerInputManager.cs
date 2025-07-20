// PlayerInputManager.cs
using UnityEngine;
using StarterAssets;

public class PlayerInputManager : MonoBehaviour
{
    public bool canPlayerMove = true;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator playerAnimator; // Reference to the Animator

    void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        playerAnimator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        // If the player shouldn't move...
        if (!canPlayerMove)
        {
            // ...zero out all their movement and look inputs.
            starterAssetsInputs.MoveInput(Vector2.zero);
            starterAssetsInputs.LookInput(Vector2.zero);
            starterAssetsInputs.JumpInput(false);
            starterAssetsInputs.SprintInput(false);

            // Stop the animations by setting the speed parameters to 0
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Speed", 0f);
                playerAnimator.SetFloat("MotionSpeed", 0f);
            }
        }
        
        // We also disable the main controller script to stop camera rotation.
        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = canPlayerMove;
        }
    }
}