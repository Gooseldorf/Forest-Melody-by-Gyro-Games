using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(DecorData), menuName = "ScriptableObjects/" + nameof(DecorData))]
    public class DecorData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Condition condition;
        [SerializeField] private float percent;
        [SerializeField] private int crystalCost;
    
        public string ID => id;
        public Condition Condition => condition;
        public float Percent => percent;
        public int CrystalCost => crystalCost;
    }
}
