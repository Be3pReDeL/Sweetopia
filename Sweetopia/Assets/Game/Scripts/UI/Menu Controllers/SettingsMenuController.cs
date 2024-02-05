using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class SettingsMenuController : UIController
{
    [SerializeField] private Button _musicButton, _soundButton, _vibrationButton;
    [SerializeField] private Sprite _toogleOn, _toogleOff;

    [SerializeField] private AudioMixer audioMixer;

    [OPS.Obfuscator.Attribute.DoNotRename]
    public override void Start(){
        base.Start();

        bool isMusicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music", 1));
        _musicButton.image.sprite = isMusicOn ? _toogleOn : _toogleOff;
        bool isSoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("SFX", 1));
        _soundButton.image.sprite = isSoundOn ? _toogleOn : _toogleOff;
        bool isVibrationEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("Vibration", 1));
        _vibrationButton.image.sprite = PlayerPrefs.GetInt("Vibration", 1) == 1 ? _toogleOn : _toogleOff;
    }
    
    [OPS.Obfuscator.Attribute.DoNotRename]
    public void ToggleMusic() 
    {
        bool isMusicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music", 1));
        PlayerPrefs.SetInt("Music", Convert.ToInt32(!isMusicOn));

        _musicButton.image.sprite = PlayerPrefs.GetInt("Music", 1) == 1 ? _toogleOn : _toogleOff;
        
        audioMixer.SetFloat("Soundtrack", isMusicOn ? -80f : 0f);
        audioMixer.SetFloat("Ambience", isMusicOn ? -80f : 0f);
    }

    [OPS.Obfuscator.Attribute.DoNotRename]
    public void ToggleSound() 
    {
        bool isSoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("SFX", 1));
        PlayerPrefs.SetInt("SFX", Convert.ToInt32(!isSoundOn));

        _soundButton.image.sprite = PlayerPrefs.GetInt("SFX", 1) == 1 ? _toogleOn : _toogleOff;
        
        audioMixer.SetFloat("SFX", isSoundOn ? -80f : 0f);
    }

    [OPS.Obfuscator.Attribute.DoNotRename]
    public void ToggleVibration() {
        bool isVibrationEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("Vibration", 1));
        PlayerPrefs.SetInt("Vibration", Convert.ToInt32(!isVibrationEnabled));

        _vibrationButton.image.sprite = PlayerPrefs.GetInt("Vibration", 1) == 1 ? _toogleOn : _toogleOff;
    }
}
