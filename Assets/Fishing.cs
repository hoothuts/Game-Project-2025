using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    // --- Public Variables (visible in Inspector) ---
    [Header("Fishing State")]
    public bool isFishingAvailable;
    public bool isCasted;
    public bool isPulling;
    public bool hasBite;

    [Header("References")]
    public GameObject baitPrefab;
    public Transform lineStartPoint;
    private LineRenderer lineRenderer;
    private Animator animator;

    [Header("Fishing Parameters")]
    public float castDelay = 1f;
    public float minTimeToBite = 2f;
    public float maxTimeToBite = 5f;

    [Header("Inventory Integration")]
    public InventoryManager inventoryManager;
    public string fishingRodItemName = "Fishing Rod"; // The exact name of your fishing rod item

    [Header("Fish Data by Water Body")]
    public List<FishData> oceanFishList; // Assign your Ocean FishData assets here
    public List<FishData> lakeFishList;  // Assign your Lake FishData assets here

    [Header("Rarity Chances (Sum should be 1.0 or 100%)")]
    [Range(0, 1)] public float commonChance = 0.5f;
    [Range(0, 1)] public float uncommonChance = 0.3f;
    [Range(0, 1)] public float rareChance = 0.15f;
    [Range(0, 1)] public float epicChance = 0.04f;
    [Range(0, 1)] public float legendaryChance = 0.01f;

    // --- Private Variables ---
    private GameObject currentBaitInstance;
    private FishData caughtFishData; // Store the data of the fish that was caught


    private void Start()
    {
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // Basic null checks for essential references
        if (animator == null) Debug.LogWarning("Fishing: Animator not found on this GameObject or its parent/children!");
        if (lineRenderer == null) Debug.LogWarning("Fishing: LineRenderer not found on this GameObject!");
        if (inventoryManager == null) Debug.LogError("Fishing: InventoryManager reference is not assigned! Fishing will not work correctly.");

        // Check if fish lists are populated
        if (oceanFishList == null || oceanFishList.Count == 0) Debug.LogWarning("Fishing: Ocean Fish List is empty!");
        if (lakeFishList == null || lakeFishList.Count == 0) Debug.LogWarning("Fishing: Lake Fish List is empty!");
    }


    void Update()
    {
        bool isRodEquipped = false;
        if (inventoryManager != null)
        {
            Item equippedItem = inventoryManager.GetEquippedItem();
            if (equippedItem != null && equippedItem.itemName == fishingRodItemName)
            {
                isRodEquipped = true;
            }
        }
        else
        {
            Debug.LogError("InventoryManager reference is missing in Fishing script. Cannot check equipped item.");
            return;
        }

        // Debug.Log($"[Fishing] isRodEquipped: {isRodEquipped}, isFishingAvailable: {isFishingAvailable}, isCasted: {isCasted}, isPulling: {isPulling}");


        if (isRodEquipped)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            // Use LayerMasks for specific water bodies
            int oceanLayer = LayerMask.GetMask("Ocean"); // New Layer for Ocean
            int lakeLayer = LayerMask.GetMask("Lake");   // New Layer for Lake
            int waterLayers = oceanLayer | lakeLayer; // Combine them

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, waterLayers)) // Raycast only hits water layers
            {
                // Determine which water body was hit
                List<FishData> currentFishList = null;
                if (hit.collider.CompareTag("Ocean")) // New Tag for Ocean
                {
                    isFishingAvailable = true;
                    currentFishList = oceanFishList;
                }
                else if (hit.collider.CompareTag("Lake")) // New Tag for Lake
                {
                    isFishingAvailable = true;
                    currentFishList = lakeFishList;
                }
                else
                {
                    isFishingAvailable = false;
                }

                if (isFishingAvailable && Input.GetMouseButtonDown(0) && !isCasted && !isPulling)
                {
                    if (currentFishList != null && currentFishList.Count > 0)
                    {
                        // Pass the correct fish list to CastRod
                        StartCoroutine(CastRod(hit.point, currentFishList));
                    }
                    else
                    {
                        Debug.LogWarning("No fish defined for this water body!");
                        isFishingAvailable = false; // Cannot fish if no fish are defined
                    }
                }
            }
            else
            {
                isFishingAvailable = false;
            }
        }
        else
        {
            if (isCasted || isPulling)
            {
                Debug.Log("Fishing rod unequipped while fishing. Reeling in.");
                ForceReelIn();
            }
            isFishingAvailable = false;
        }

        // --- Line Renderer Update ---
        if ((isCasted || isPulling) && isRodEquipped)
        {
            if (lineRenderer != null && lineStartPoint != null && currentBaitInstance != null)
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, lineStartPoint.position);
                lineRenderer.SetPosition(1, currentBaitInstance.transform.position);
            }
        }
        else
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }

        // --- Reeling Input ---
        if (isCasted && Input.GetMouseButtonDown(1) && isRodEquipped)
        {
            PullRod();
        }
    }


    IEnumerator CastRod(Vector3 targetPosition, List<FishData> fishListToUse)
    {
        isCasted = true;
        isPulling = false;
        hasBite = false;
        caughtFishData = null; // Reset caught fish data

        if (animator != null)
        {
            animator.SetTrigger("Cast");
        }

        yield return new WaitForSeconds(castDelay);

        if (baitPrefab != null)
        {
            currentBaitInstance = Instantiate(baitPrefab);
            currentBaitInstance.transform.position = targetPosition;
        }
        else
        {
            Debug.LogError("Fishing: Bait Prefab is not assigned! Cannot cast.");
            isCasted = false;
            yield break;
        }

        // Start Fish Bite Logic, passing the relevant fish list
        StartCoroutine(FishBiteLogic(fishListToUse));
    }

    IEnumerator FishBiteLogic(List<FishData> fishListToUse)
    {
        float waitTime = UnityEngine.Random.Range(minTimeToBite, maxTimeToBite);
        yield return new WaitForSeconds(waitTime);

        // Determine if a fish bites
        if (UnityEngine.Random.value < GetOverallBiteChance(fishListToUse)) // You can adjust this based on fish rarity or just use a fixed biteChance
        {
            hasBite = true;
            caughtFishData = GetRandomFish(fishListToUse); // Get a random fish based on rarity
            Debug.Log($"FISH BITE! It's a {caughtFishData?.fishName ?? "Unknown Fish"}!");
            // TODO: Add visual feedback (e.g., bobber dips, particle effect)
            // TODO: Add audio feedback (e.g., a splash sound)
        }
        else
        {
            Debug.Log("No bite this time.");
            hasBite = false;
        }
    }

    // Helper method to get a random fish based on rarity chances
    private FishData GetRandomFish(List<FishData> availableFish)
    {
        if (availableFish == null || availableFish.Count == 0) return null;

        // Create a list of fish filtered by rarity
        List<FishData> commonFish = availableFish.FindAll(f => f.rarity == FishRarity.Common);
        List<FishData> uncommonFish = availableFish.FindAll(f => f.rarity == FishRarity.Uncommon);
        List<FishData> rareFish = availableFish.FindAll(f => f.rarity == FishRarity.Rare);
        List<FishData> epicFish = availableFish.FindAll(f => f.rarity == FishRarity.Epic);
        List<FishData> legendaryFish = availableFish.FindAll(f => f.rarity == FishRarity.Legendary);

        float totalChance = commonChance + uncommonChance + rareChance + epicChance + legendaryChance;
        if (totalChance == 0) totalChance = 1; // Prevent division by zero if all chances are 0

        float randomValue = UnityEngine.Random.value * totalChance; // Scale random value by total chance

        // Select rarity based on chances
        if (randomValue < commonChance && commonFish.Count > 0)
        {
            return commonFish[UnityEngine.Random.Range(0, commonFish.Count)];
        }
        randomValue -= commonChance; // Subtract chance to move to next tier

        if (randomValue < uncommonChance && uncommonFish.Count > 0)
        {
            return uncommonFish[UnityEngine.Random.Range(0, uncommonFish.Count)];
        }
        randomValue -= uncommonChance;

        if (randomValue < rareChance && rareFish.Count > 0)
        {
            return rareFish[UnityEngine.Random.Range(0, rareFish.Count)];
        }
        randomValue -= rareChance;

        if (randomValue < epicChance && epicFish.Count > 0)
        {
            return epicFish[UnityEngine.Random.Range(0, epicFish.Count)];
        }
        randomValue -= epicChance;

        if (randomValue < legendaryChance && legendaryFish.Count > 0)
        {
            return legendaryFish[UnityEngine.Random.Range(0, legendaryFish.Count)];
        }

        // Fallback: if no fish found by rarity, just pick any random fish from the full list
        Debug.LogWarning("No fish found for selected rarity or rarity list empty. Picking random fish from all available.");
        return availableFish[UnityEngine.Random.Range(0, availableFish.Count)];
    }

    // You can adjust the overall bite chance if needed, or just use a fixed value.
    private float GetOverallBiteChance(List<FishData> fishListToUse)
    {
        // For simplicity, let's just use a fixed biteChance for now.
        // You could make this more complex, e.g., higher chance for common fish.
        return 0.7f; // This is the public biteChance from the inspector
    }


    private void PullRod()
    {
        StopAllCoroutines();

        if (animator != null)
        {
            animator.SetTrigger("Pull");
        }

        isCasted = false;
        isPulling = true;

        if (currentBaitInstance != null)
        {
            Destroy(currentBaitInstance);
            currentBaitInstance = null;
        }

        if (hasBite && caughtFishData != null) // Ensure we have a fish data to add
        {
            Debug.Log($"FISH CAUGHT! It's a {caughtFishData.fishName}!");
            if (inventoryManager != null)
            {
                // Create a new Item using the properties from the caught FishData
                Item caughtItem = new Item(caughtFishData.fishName, caughtFishData.icon, true, 1); // Fish are stackable, quantity 1
                inventoryManager.AddItem(caughtItem);
            }
            else
            {
                Debug.LogError("InventoryManager reference missing! Cannot add fish.");
            }
        }
        else
        {
            Debug.Log("Reeled in, but no fish caught.");
        }

        StartCoroutine(ResetPullingStateAfterDelay(1f));
    }

    private void ForceReelIn()
    {
        StopAllCoroutines();
        if (animator != null)
        {
            animator.SetTrigger("Pull");
        }

        isCasted = false;
        isPulling = true;

        if (currentBaitInstance != null)
        {
            Destroy(currentBaitInstance);
            currentBaitInstance = null;
        }
        hasBite = false;

        StartCoroutine(ResetPullingStateAfterDelay(1f));
    }

    IEnumerator ResetPullingStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPulling = false;
        hasBite = false;
        caughtFishData = null; // Clear caught fish data after reset
    }
}
