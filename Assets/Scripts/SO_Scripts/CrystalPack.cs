using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(CrystalPack), menuName = "ScriptableObjects/" + nameof(CrystalPack))]
    public class CrystalPack : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public int CrystalAmount { get; private set; }
        public void Activate()
        {
            PlayerData.Instance.ChangeCrystals(CrystalAmount);
        }
    }
}
