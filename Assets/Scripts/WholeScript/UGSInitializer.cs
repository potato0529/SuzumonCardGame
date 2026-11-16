using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public class UGSInitializer : MonoBehaviour
{
    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //await AuthenticationService.Instance.SignInAnonymouslyAsync();

            await UnityServices.InitializeAsync();

            var consentState = EndUserConsent.GetConsentState();
            consentState.AnalyticsIntent = ConsentStatus.Granted;
            EndUserConsent.SetConsentState(consentState);
        }

        Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
    }
}