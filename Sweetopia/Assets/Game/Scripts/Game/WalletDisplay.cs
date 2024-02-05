using UnityEngine;
using TMPro;

public class WalletDisplay : MonoBehaviour {
    [SerializeField] private TMP_Text walletText;

    private void Start() {
        UpdateDisplay();
    }

    private void Update() {
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if (walletText != null) {
            walletText.text = PlayerPrefs.GetInt("Coins", 10).ToString();
        }
    }
}
