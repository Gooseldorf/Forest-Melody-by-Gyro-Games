using System;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundButton : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action OnBackgroundButtonClicked;
    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            OnBackgroundButtonClicked?.Invoke();
        });
    }
    
    public void Toggle(bool value)
    {
        button.interactable = value;
    }
    
    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
