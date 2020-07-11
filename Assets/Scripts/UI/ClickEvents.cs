using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickEvents : MonoBehaviour
{
    public Text textField;

    public void PlayButton() {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ControlsButton() {
        textField.text = $"WASD / Arrow Keys = Move\n" 
            + "Space / Left Mouse Click = Fire\n" 
            + "Move Mouse cursor = Aim / Rotate";
    }

    public void CreditsButton() {
        textField.text = $"Made by Owen Shevlin\n\n" 
            + "I thought I'd have more to put here tbh :|";
    }
}
