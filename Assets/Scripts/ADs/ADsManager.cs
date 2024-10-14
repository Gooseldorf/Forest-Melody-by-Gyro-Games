using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public enum RewardedADUnits
{
    Envelope,
    LevelUpMultiply,
    OfflineIncomeMultiply,
    ReloadBoosters,
    Accelerate
}

public class ADsManager : MonoBehaviour
{
    private static ADsManager instance;

    [SerializeField] private float adRequestDelay;

    private const string levelUpInterstitialID = "ca-app-pub-4538912278178892/8101716916";

    private const string envelopeID = "ca-app-pub-4538912278178892/1843182904";
    private const string levelUpMultiplyID = "ca-app-pub-4538912278178892/8541572754";
    private const string offlineIncomeMultiplyID = "ca-app-pub-4538912278178892/1651611215";
    private const string reloadBoostersID = "ca-app-pub-4538912278178892/2784976907";
    private const string accelerateID = "ca-app-pub-4538912278178892/7228491082";

    private InterstitialAd levelUpInterstitialAD;
    public Dictionary<RewardedADUnits, AdUnit> RewardedUnitsDict;

    public bool InterstitialADLoaded;

    public static ADsManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        RewardedUnitsDict = new Dictionary<RewardedADUnits, AdUnit>()
        {
            { RewardedADUnits.OfflineIncomeMultiply, new AdUnit(RewardedADUnits.OfflineIncomeMultiply, offlineIncomeMultiplyID)},
            { RewardedADUnits.LevelUpMultiply, new AdUnit(RewardedADUnits.LevelUpMultiply, levelUpMultiplyID)},
            { RewardedADUnits.ReloadBoosters , new AdUnit(RewardedADUnits.ReloadBoosters, reloadBoostersID)},
            { RewardedADUnits.Envelope , new AdUnit(RewardedADUnits.Envelope, envelopeID)},
            { RewardedADUnits.Accelerate , new AdUnit(RewardedADUnits.Accelerate, accelerateID)}
        };
    }

    public void Start()
    {
        //MobileAds.SetiOSAppPauseOnBackground(true); For IOS

        List<String> deviceIds = new List<String>()
        {
            AdRequest.TestDeviceSimulator
        };

        // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
        deviceIds.Add("5308565CF8255BF34ACF156EE2E94CAD"); //Yar POCOx3 PRO
#endif

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(deviceIds).build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("Initialization complete.");
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // the main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Initialization complete.");
            RequestAndLoadInterstitialAd();
            foreach (var adUnit in RewardedUnitsDict)
            {
                StartCoroutine(RequestAndLoadRewardedAd(RewardedUnitsDict[adUnit.Key], 0));
            }

        });
    }

    private AdRequest CreateAdRequest()
    {
        AdRequest.Builder builder = new AdRequest.Builder();
        return builder.Build();
    }

    #region INTERSTITIAL ADS

    private void RequestAndLoadInterstitialAd()
    {
        Debug.Log("Requesting Interstitial ad.");

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = levelUpInterstitialID;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (levelUpInterstitialAD != null)
        {
            levelUpInterstitialAD.Destroy();
        }

        InterstitialAd.Load(adUnitId, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Interstitial ad failed to load with error: " + loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Interstitial ad failed to load.");
                    return;
                }

                Debug.Log("Interstitial ad loaded.");
                levelUpInterstitialAD = ad;
                InterstitialADLoaded = true;

                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Interstitial ad closed.");
                    InterstitialADLoaded = false;
                    RequestAndLoadInterstitialAd();
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("Interstitial ad failed to show with error: " + error.GetMessage());
                };
            });
    }

    public void ShowInterstitialAd()
    {
        if (levelUpInterstitialAD != null && levelUpInterstitialAD.CanShowAd())
            levelUpInterstitialAD.Show();
        else
            Debug.Log("Interstitial ad is not ready yet.");
    }

    #endregion

    #region REWARDED ADS
    private IEnumerator RequestAndLoadRewardedAd(AdUnit unit, float requestDelay)
    {
        yield return new WaitForSeconds(requestDelay);

        Debug.Log($"Requesting Rewarded {unit.ADUnit.ToString()} ad.");
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = unit.ID;
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log($"Rewarded {unit.ADUnit} failed to load with error: " + loadError.GetMessage());
                    StartCoroutine(RequestAndLoadRewardedAd(unit, requestDelay));
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log($"Rewarded {unit.ADUnit} failed to load.");
                    StartCoroutine(RequestAndLoadRewardedAd(unit, requestDelay));
                    return;
                }

                Debug.Log($"Rewarded {unit.ADUnit} loaded.");
                unit.AD = ad;
                unit.IsLoaded = true;
                Messenger<RewardedADUnits, bool>.Broadcast(GameEvents.IsRewardedADLoaded, unit.ADUnit, true, MessengerMode.DONT_REQUIRE_LISTENER);

                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log($"Rewarded {unit.ADUnit} closed.");
                    Messenger<RewardedADUnits, bool>.Broadcast(GameEvents.IsRewardedADLoaded, unit.ADUnit, false, MessengerMode.DONT_REQUIRE_LISTENER);
                    unit.IsLoaded = false;
                    StartCoroutine(RequestAndLoadRewardedAd(unit, 0));
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log($"Rewarded {unit.ADUnit} failed to show with error: " + error.GetMessage());
                };
            });
    }

    public void ShowRewardedAd(RewardedADUnits adUnits, Action rewardAction)
    {
        if (RewardedUnitsDict[adUnits] != null)
        {
            RewardedUnitsDict[adUnits].AD.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded {adUnits.ToString()} shown");
                rewardAction.Invoke();
            });
        }
        else
            Debug.Log("Rewarded ad is not ready yet.");
    }
    #endregion
}
public class AdUnit
{
    public RewardedADUnits ADUnit;
    public RewardedAd AD;
    public string ID;
    public bool IsLoaded;

    public AdUnit(RewardedADUnits unit, string id)
    {
        ID = id;
        ADUnit = unit;
    }
}
