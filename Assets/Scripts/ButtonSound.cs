using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    private Button _button;
    private TMP_InputField _inputField;

    private void Awake()
    {
        TryGetComponent<Button>(out _button);
        TryGetComponent<TMP_InputField>(out _inputField);
        if (!_button)
            if (!_inputField)
                Destroy(this);
    }

    private void OnEnable()
    {
        if (_button)
            _button.onClick.AddListener(Play);
        if (_inputField)
            _inputField.onValueChanged.AddListener(Play);
    }

    private void OnDisable()
    {
        if (_button)
            _button.onClick.RemoveListener(Play);
        if (_inputField)
            _inputField.onValueChanged.RemoveListener(Play);

    }

    private void Play()
    {
        SoundManager.Instance.Play("Pop-ui", false,false, 1f, 1.5f);
    }

    private void Play(string v)
    {
        SoundManager.Instance.Play("Pop-ui", false,false, 1f, 1.5f);
    }
}