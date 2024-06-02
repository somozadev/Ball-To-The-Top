using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    
    [SerializeField] private GetHeightController _heightController;
    private TMP_Text _meters;

    private void Awake()
    {
        _meters = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        _meters.text = string.Format("{0:D4} m", (int)_heightController.targetHeight);
    }
}
