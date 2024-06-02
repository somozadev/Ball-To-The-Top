using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundIcon : MonoBehaviour
{
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    [SerializeField] private Image _image;
    private Sprite currentSprite;

    private void Awake()
    {
        _image = GetComponent<Image>();
        currentSprite = soundOn;
    }

    public void Switch()
    {
        if (currentSprite == soundOn)
            currentSprite = soundOff;
        else
            currentSprite = soundOn;
        _image.sprite = currentSprite;
    }
}