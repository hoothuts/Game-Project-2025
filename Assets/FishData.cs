using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("Fish Information")]
    public string fishName = "New Fish";
    public Sprite icon;
    // public GameObject fish3DModelPrefab; // This line is removed in this version
    public int basePrice = 10;

    [Header("Rarity")]
    public FishRarity rarity = FishRarity.Common;
}

public enum FishRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
