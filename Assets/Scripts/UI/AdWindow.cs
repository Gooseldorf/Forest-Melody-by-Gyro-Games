using I2.Loc;
using UnityEngine;
using UnityEngine.UIElements;

public class AdWindow : MonoBehaviour
{
    [SerializeField] private ADEnvelopeController envelope;
    private VisualElement root;

    private Label titleLabel;
    private Label descLabel;
    private Label textLabel;

    private Label cancelButtonLabel;

    private VisualElement adButton;
    private Label adButtonLabel;

    private double notes;

    public void SetUpAdWindow(VisualElement root)
    {
        this.root = root;
        titleLabel = root.Q("TitleLabel") as Label;
        descLabel = root.Q("DescLabel") as Label;
        textLabel = root.Q("TextLabel") as Label;

        cancelButtonLabel = root.Q("CancelButtonLabel") as Label;

        adButton = root.Q("AdButton");
        adButtonLabel = adButton.Q("AdButtonLabel") as Label;

        adButton.RegisterCallback<ClickEvent>(OnAdButtonClick);

        Show(false);
    }

    public void DisposeAdWindow()
    {
        adButton.UnregisterCallback<ClickEvent>(OnAdButtonClick);
    }

    public void Init(double notes)
    {
        this.notes = notes;
        textLabel.text = Utilities.GetNotesString(notes);
        titleLabel.text = LocalizationManager.GetTranslation("AdOffer");
        descLabel.text = LocalizationManager.GetTranslation("AdDesc");
        cancelButtonLabel.text = LocalizationManager.GetTranslation("NoThanks");
        adButtonLabel.text = LocalizationManager.GetTranslation("WatchAd");
        Show(true);
    }

    private void OnAdButtonClick(ClickEvent evt)
    {
        ADsManager.Instance.ShowRewardedAd(RewardedADUnits.Envelope, () =>
        {
            envelope.Toggle(false);
            PlayerData.Instance.ChangeNotes(notes);
        });
        Show(false);
    }

    public void Show(bool show) => root.visible = show;
}
