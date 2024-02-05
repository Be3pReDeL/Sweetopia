using UnityEngine;
using CandyCoded.HapticFeedback;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[OPS.Obfuscator.Attribute.DoNotObfuscateClass]
public class PlaySoundAndVibrate : MonoBehaviour {
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _audioSource;
    
    private Button _button;

    private void OnEnable(){
        _button.onClick.AddListener(() => PlaySoundAndVibrateButton(_audioClip));
    }

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();

        _button = GetComponent<Button>();

        PlayerPrefs.SetInt("Vibration", 1);
    }

    [OPS.Obfuscator.Attribute.DoNotRename]
    public void PlaySoundAndVibrateButton(AudioClip audioClip) {
        _audioSource.PlayOneShot(audioClip);

        if(PlayerPrefs.GetInt("Vibration", 0) == 1)
            HapticFeedback.MediumFeedback();
    }

    private void OnDisable(){
        _button.onClick.RemoveListener(() => PlaySoundAndVibrateButton(_audioClip));
    }
}
