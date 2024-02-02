using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTween : MonoBehaviour, ITweanable {
    [SerializeField] private UIController _UIController;

    private Image _image;
    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;

    private Vector2 _startPos;

    [SerializeField] private bool _movable = true;
    [SerializeField] private float _finalAlpha = 1f;
    [SerializeField] private Vector2 _direction;

    private void Start() {
        TryGetComponent(out _image);
        TryGetComponent(out _text);

        _rectTransform = GetComponent<RectTransform>();
        _startPos = _rectTransform.anchoredPosition;

        _UIController.AddTweenObject(this);
    }

    public void Appear(float duration) {
        SetAlphaToZero();
        MoveAndFade(duration, _finalAlpha);
    }

    public void Disappear(float duration) {
        MoveAndFade(duration, 0f, deactivateOnComplete: true);
    }

    private void SetAlphaToZero() {
        if(_image != null) _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        if(_text != null) _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0f);
    }

    private void MoveAndFade(float duration, float targetAlpha, bool deactivateOnComplete = false) {
        if (_movable) {
            Vector2 targetPos = targetAlpha > 0 ? _startPos : _rectTransform.anchoredPosition + _direction * 25f;
            _rectTransform.DOAnchorPos(targetPos, duration);
        }

        if(_image != null) {
            _image.DOFade(targetAlpha, duration).OnComplete(() => OnAnimationComplete(deactivateOnComplete));
        } else if(_text != null) {
            _text.DOFade(targetAlpha, duration).OnComplete(() => OnAnimationComplete(deactivateOnComplete));
        }
    }

    private void OnAnimationComplete(bool deactivate) {
        if (deactivate) {
            _UIController.gameObject.SetActive(false);
        }
    }
}
