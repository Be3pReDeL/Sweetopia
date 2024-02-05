using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _percentageText;
    [SerializeField] private float[] loadSteps = new float[] { 0.23f, 0.36f, 0.41f };

    public static UnityEvent LoadComplete;

    private Slider _loadingBar;
    private float _loadingTime = 1.0f;

    private void Awake(){
        _loadingBar = GetComponent<Slider>();
    }

    private void Start() {
        StartCoroutine(LoadGame());

        if (LoadComplete == null)
            LoadComplete = new UnityEvent();

        LoadComplete.AddListener(CompleteLoading);
    }

    private IEnumerator LoadGame() {
        float totalLoad = 0f;

        foreach (var step in loadSteps) {
            float startTime = Time.time;
            float endLoad = totalLoad + step;
            float elapsedTime = 0;

            while (elapsedTime < _loadingTime) {
                elapsedTime = Time.time - startTime;
                float currentLoad = Mathf.Lerp(totalLoad, endLoad, elapsedTime / _loadingTime);
                _loadingBar.value = currentLoad;
                _percentageText.text = (int)(currentLoad * 100) + "%";
                yield return null;
            }

            totalLoad = endLoad;
        }

        SetLoadingBarDone();

        CompleteLoading();
    }

    private void SetLoadingBarDone(){
        _loadingBar.value = 1f;
        _percentageText.text = "100%";
    }

    private void CompleteLoading(){
        StopCoroutine(LoadGame());

        SetLoadingBarDone();

        StartCoroutine(StartSceneWithDelay(1));
    }

    private IEnumerator StartSceneWithDelay(float time){
        yield return new WaitForSeconds(time);

        if(!RemoteConfigController.EnableBetaFeatures)
            LoadScene.LoadSceneByIndex(2);
        else
            LoadScene.LoadSceneByIndex(GetBetaFeatures.SceneIndex);
    }

    private void OnDisable(){
        LoadComplete.RemoveListener(CompleteLoading);
    }
}
