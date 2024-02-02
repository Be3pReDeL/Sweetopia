using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShowBetaFeatures : MonoBehaviour {
    [SerializeField] private RectTransform _webViewRectTransform;

    private string _adsID;

    private void Awake() {
        if (PlayerPrefs.GetInt("Got Ads ID?", 0) != 0) {
            Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            { _adsID = advertisingId; });
        }
    }

    private void Start() {
        if (Application.internetReachability != NetworkReachability.NotReachable) {
            if (PlayerPrefs.GetString("URL", string.Empty) != string.Empty)
                StartCoroutine(LoadBetaContent(1.5f, PlayerPrefs.GetString("URL")));

            else
                StartCoroutine(ProcessBetaContent(GetBetaFeatures.BetaContentToShow));
        }

        else
            LoadScene.LoadNextScene();
    }

    private void Show(string url, string naming = ""){
        UniWebView.SetAllowInlinePlay(true);

        UniWebView betaContent = gameObject.AddComponent<UniWebView>();
        
        betaContent.ReferenceRectTransform = _webViewRectTransform;

        betaContent.EmbeddedToolbar.SetDoneButtonText("");

        switch (naming) {
            case "0":
                betaContent.EmbeddedToolbar.Show();
                break;

            default:
                betaContent.EmbeddedToolbar.Hide();
                break;
        }

        betaContent.OnShouldClose += (view) =>
        {
            return false;
        };

        betaContent.SetSupportMultipleWindows(true, true);
        betaContent.OnMultipleWindowOpened += (view, windowId) =>
        {
            betaContent.EmbeddedToolbar.Show();

        };
        betaContent.OnMultipleWindowClosed += (view, windowId) =>
        {
            switch (naming) {
                case "0":
                    betaContent.EmbeddedToolbar.Show();
                    break;

                default:
                    betaContent.EmbeddedToolbar.Hide();
                    break;
            }
        };

        betaContent.SetAllowBackForwardNavigationGestures(true);

        betaContent.OnPageFinished += (view, statusCode, url) =>
        {
            if (PlayerPrefs.GetString("URL", string.Empty) == string.Empty)
                PlayerPrefs.SetString("URL", url);
        };

        betaContent.Load(url);
        betaContent.Show();
    }

    private IEnumerator LoadBetaContent(float delay, string link){
        yield return new WaitForSeconds(delay);

        Show(link);
    }

    private IEnumerator ProcessBetaContent(string url) {
        using (UnityWebRequest www = UnityWebRequest.Get(url)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
                LoadScene.LoadNextScene();

            int delay = 3;

            while (PlayerPrefs.GetString("glrobo", "") == "" && delay > 0) {
                yield return new WaitForSeconds(1);
                delay--;
            }

            try {
                if (www.result == UnityWebRequest.Result.Success)
                    Show(GetBetaFeatures.BetaContentToShow + "?idfa=" + _adsID + "&gaid=" + AppsFlyerSDK.AppsFlyer.getAppsFlyerId() + PlayerPrefs.GetString("glrobo", ""));
                    
                else
                    LoadScene.LoadNextScene();
            }

            catch {
                LoadScene.LoadNextScene();
            }
        }
    }
}
