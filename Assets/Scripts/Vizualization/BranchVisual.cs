using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BranchVisual : MonoBehaviour
{
    [SerializeField] private GameObject lockScreen;
    [SerializeField] private GameObject lockVisuals;
    [SerializeField] private TextMeshProUGUI conditionTMP;
    [SerializeField] private Image noteIcon; 

    public void ShowLock(bool isShown)
    {
        lockVisuals.SetActive(isShown);
    }

    public void ToggleLock(bool isActive)
    {
        lockScreen.gameObject.SetActive(isActive);
    }

    public void ShowLevelCondition(string text)
    {
        conditionTMP.text = text;
        noteIcon.enabled = false;
    }

    public void ShowCostCondition(string text)
    {
        conditionTMP.text = text;
        noteIcon.enabled = true;
    }
}
