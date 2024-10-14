using UnityEngine;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = nameof(BuyableSkipTimeData), menuName = "ScriptableObjects/" + nameof(BuyableSkipTimeData))]
    public class BuyableSkipTimeData : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public int CrystalPrice { get; private set; }
        [field: SerializeField] public int TimeSkip { get; private set; }

        public double Income
        {
            get
            {
                double income = 0;
                foreach (var tree in PlayerData.Instance.Trees)
                    income += tree.CalculateCleanIncomeInSecond() * TimeSkip;
                return income;
            }
        }


        public void Activate()
        {
            PlayerData.Instance.RemoveFromInventory(ID);
            PlayerData.Instance.ChangeNotes(Income);
        }
    }
}
