using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(BuyableMultiplierData), menuName = "ScriptableObjects/" + nameof(BuyableMultiplierData))]
    public class BuyableMultiplierData : ScriptableObject
    {
        [field:SerializeField] public string ID { get; private set; }
        [field:SerializeField] public int CrystalPrice { get; private set; }
        [field:SerializeField] public int Multiplier { get; private set; }
        [field:SerializeField] public int Duration { get; private set; }

    }
}   
