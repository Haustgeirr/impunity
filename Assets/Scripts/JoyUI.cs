using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyUI : MonoBehaviour
{
    public Slider JoySlider;

    private void UpdateJoyBar(float currentJoy)
    {
        JoySlider.value = currentJoy / GameManager.instance.MaxJoy;
    }

    private void OnEnable()
    {
        GameManager.onUpdateJoyUI += UpdateJoyBar;
    }

    private void OnDisable()
    {
        GameManager.onUpdateJoyUI -= UpdateJoyBar;
    }
}
