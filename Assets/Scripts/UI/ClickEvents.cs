using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickEvents : MonoBehaviour
{
    public Text textField;

    void Start() {
        if (textField != null) {
            ControlsButton();
        }
    }

    public void PlayButton() {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ControlsButton() {
        textField.text = "Time to fly!\n\n"
            + "Pilot your ship though waves of enemies while coping with faulty systems, and defeat the boss!\n\n"
            + "WASD / Arrow Keys = Move\n" 
            + "Space / Left Mouse Click = Fire\n" 
            + "Move Mouse cursor = Aim / Rotate";
    }

    public void CreditsButton() {
        textField.text = "Made by Owen Shevlin\n\n"
            + "Music:\n" 
            + "Son of a Rocket by Kevin MacLeod\n"
            + "Link: https://incompetech.filmmusic.io/song/4391-son-of-a-rocket\n"
            + "License: http://creativecommons.org/licenses/by/4.0/\n";
    }

    public void MenuButton() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
