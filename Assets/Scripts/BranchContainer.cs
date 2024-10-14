using UnityEngine;
using UnityEngine.UI;

public class BranchContainer : MonoBehaviour, IPosition
{
    [SerializeField] private Image emptyContainerImage;
    [SerializeField] private Button button;
    
    public bool IsEmpty = true;
    public bool IsMirrored;
    public ContainerType ContainerType;
    
    public int BranchPosition { get; set; }
    public int PositionIndex { get; set; }

    public void InitWithPosition(int branchPosition, int positionIndex)
    {
        BranchPosition = branchPosition;
        PositionIndex = positionIndex;
        button.onClick.AddListener(() => Messenger<BranchContainer>.Broadcast(GameEvents.OnContainerClick, this));
    }

    public void ToggleContainer(bool isActive)
    {
        emptyContainerImage.enabled = isActive;
        button.interactable = isActive;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
