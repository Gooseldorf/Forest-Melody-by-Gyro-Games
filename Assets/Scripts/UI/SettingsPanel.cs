using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Analytics;

namespace UI
{
    public class SettingsPanel : MonoBehaviour
    {
        private const string privacyPolicyURL = "https://github.com/cnayh/Module-td2/wiki/Birds-Idle";

        private VisualElement settingsPanel;

        private SettingsSwitcher soundSwitcher;
        private SettingsSwitcher musicSwithcer;
        private SettingsSwitcher vibrationSwitcher;

        private VisualElement rateButton;
        private VisualElement privacyButton;
        private VisualElement unityPrivacyButton;

        private VisualElement russianButton;
        private VisualElement englishButton;

        public void SetUpSettingsPanel(VisualElement settingsPanel)
        {
            this.settingsPanel = settingsPanel;

            soundSwitcher = new SettingsSwitcher(settingsPanel.Q("SoundSettings"));
            soundSwitcher.RootVisualElement.RegisterCallback<ClickEvent>(OnSoundSwitcherClick);
            soundSwitcher.SetState(PlayerPrefs.GetInt(PrefKeys.Sound, 1) == 1);

            musicSwithcer = new SettingsSwitcher(settingsPanel.Q("MusicSettings"));
            musicSwithcer.RootVisualElement.RegisterCallback<ClickEvent>(OnMusicSwitcherClick);
            musicSwithcer.SetState(PlayerPrefs.GetInt(PrefKeys.Music, 1) == 1);

            vibrationSwitcher = new SettingsSwitcher(settingsPanel.Q("VibrationSettings"));
            vibrationSwitcher.RootVisualElement.RegisterCallback<ClickEvent>(OnVibrationSwitcherClick);
            vibrationSwitcher.SetState(PlayerPrefs.GetInt(PrefKeys.Vibro, 1) == 1);

            rateButton = settingsPanel.Q("RateButton");
            rateButton.RegisterCallback<ClickEvent>(OnRateButtonClick);

            privacyButton = settingsPanel.Q("PrivacyButton");
            privacyButton.RegisterCallback<ClickEvent>(OnPrivacyButtonClick);

            unityPrivacyButton = settingsPanel.Q("UnityPrivacyButton");
            unityPrivacyButton.RegisterCallback<ClickEvent>(OnUnityPrivacyButtonClick);

            russianButton = settingsPanel.Q("RussianButton");
            russianButton.RegisterCallback<ClickEvent>(OnLanguageButtonClick);
            englishButton = settingsPanel.Q("EnglishButton");
            englishButton.RegisterCallback<ClickEvent>(OnLanguageButtonClick);

            SetLanguageButtonColor(PlayerPrefs.GetString(PrefKeys.Language, "en"));
            OnUpdateLocalization();
            Show(false);
        }

        public void DisposeSettingsPanel()
        {
            soundSwitcher.RootVisualElement.UnregisterCallback<ClickEvent>(OnSoundSwitcherClick);
            musicSwithcer.RootVisualElement.UnregisterCallback<ClickEvent>(OnMusicSwitcherClick);
            vibrationSwitcher.RootVisualElement.UnregisterCallback<ClickEvent>(OnVibrationSwitcherClick);
            rateButton.UnregisterCallback<ClickEvent>(OnRateButtonClick);
            privacyButton.UnregisterCallback<ClickEvent>(OnPrivacyButtonClick);
            unityPrivacyButton.UnregisterCallback<ClickEvent>(OnUnityPrivacyButtonClick);
            russianButton.UnregisterCallback<ClickEvent>(OnLanguageButtonClick);
            englishButton.UnregisterCallback<ClickEvent>(OnLanguageButtonClick);
        }

        private void OnSoundSwitcherClick(ClickEvent evt)
        {
            soundSwitcher.SetState(!soundSwitcher.Active);
            PlayerPrefs.SetInt(PrefKeys.Sound, soundSwitcher.Active ? 1 : 0);
            SoundManager.Instance.ToggleSounds(soundSwitcher.Active);
            if (soundSwitcher.Active) SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OnMusicSwitcherClick(ClickEvent evt)
        {
            musicSwithcer.SetState(!musicSwithcer.Active);
            PlayerPrefs.SetInt(PrefKeys.Music, musicSwithcer.Active ? 1 : 0);
            SoundManager.Instance.ToggleMusic(musicSwithcer.Active);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OnVibrationSwitcherClick(ClickEvent evt)
        {
            vibrationSwitcher.SetState(!vibrationSwitcher.Active);
            PlayerPrefs.SetInt(PrefKeys.Vibro, vibrationSwitcher.Active ? 1 : 0);
            VibrationManager.ToggleVibration(vibrationSwitcher.Active);
            SoundManager.Instance.PlaySound("MenuClick");
            if (vibrationSwitcher.Active) VibrationManager.Vibrate(50, 150);
        }

        private void OnRateButtonClick(ClickEvent evt)
        {
            //TODO: open rate URL
            Debug.Log("on Rate button click");
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OnPrivacyButtonClick(ClickEvent evt)
        {
            OpenUrl(privacyPolicyURL);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OnUnityPrivacyButtonClick(ClickEvent evt)
        {
            DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        private void OnFailure(string reason)
        {
            Debug.LogWarning(System.String.Format("Failed to get data privacy url: {0}", reason));
        }

        private void OnLanguageButtonClick(ClickEvent evt)
        {
            string langCode = "";
            if (evt.currentTarget == russianButton) langCode = "ru";
            if (evt.currentTarget == englishButton) langCode = "en";// LocalizationManager.SetLanguageAndCode("English", "en");

            SetLanguage(langCode);
            SetLanguageButtonColor(langCode);

            Messenger.Broadcast(GameEvents.UpdateLocalization, MessengerMode.DONT_REQUIRE_LISTENER);
            OnUpdateLocalization();
            SoundManager.Instance.PlaySound("MenuClick");
        }

        private void OnUpdateLocalization()
        {
            soundSwitcher.SetText(LocalizationManager.GetTranslation("Sound"));
            musicSwithcer.SetText(LocalizationManager.GetTranslation("Music"));
            vibrationSwitcher.SetText(LocalizationManager.GetTranslation("Vibrations"));
            (rateButton.Q("RateButtonLabel") as Label).text = LocalizationManager.GetTranslation("Rate");
            (privacyButton.Q("PrivacyButtonLabel") as Label).text = LocalizationManager.GetTranslation("PrivacyPolicy");
            (settingsPanel.Q("TitleLabel") as Label).text = LocalizationManager.GetTranslation("Settings");
        }

        public void Show(bool show)
        {
            settingsPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetLanguage(string langCode)
        {
            if (LocalizationManager.CurrentLanguageCode == langCode) return;
            LocalizationManager.CurrentLanguageCode = langCode;
            PlayerPrefs.SetString(PrefKeys.Language, langCode);
        }

        private void SetLanguageButtonColor(string langCode)
        {
            russianButton.style.backgroundColor = langCode == "ru" ?
                new StyleColor(UIHelper.Instance.InactiveColor) :
                new StyleColor(UIHelper.Instance.UpgradeActiveColor);

            englishButton.style.backgroundColor = langCode == "en" ?
                new StyleColor(UIHelper.Instance.InactiveColor) :
                new StyleColor(UIHelper.Instance.UpgradeActiveColor);

            //switch (langCode)
            //{
            //    case "en": return englishButton;
            //    case "ru": return russianButton;
            //    default: return null;
            //}
        }
    }
}
