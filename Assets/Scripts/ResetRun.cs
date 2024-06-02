using UnityEngine;
using UnityEngine.UI;

public class ResetRun : MonoBehaviour
{
    private Button _button;
    [SerializeField] private GameObject _canvasResetConfirm;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.interactable = GameManager.Instance.currentData.HasSavedData();
    }

    public void Cancel()
    {
        _canvasResetConfirm.SetActive(false);
    }
    public void Reset(bool firstButton)
    {
        if (firstButton)
        {
            _canvasResetConfirm.SetActive(true);
            return;
        }
        DataController.SaveNew(new Data());
        GameManager.Instance.currentData.ResetData();
        _button.interactable = false;
        Cancel();
    }
}