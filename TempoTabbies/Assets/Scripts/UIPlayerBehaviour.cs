using System.Collections;
using UnityEngine;

public class UIPlayerBehaviour : MonoBehaviour
{
    // Which button is selected
    public enum ButtonSelection
    {
        // Rename these to be based on the button in use
        section1,
        section2,
        section3
    }
    public ButtonSelection state;

    // The number associated with each number
    int stateChoice;

    // The vertical input of the joystick
    float verticalInput;

    // All the buttons
    [SerializeField] private UnityEngine.UI.Button Button1;
    [SerializeField] private UnityEngine.UI.Button Button2;
    [SerializeField] private UnityEngine.UI.Button Button3;

    private void Awake()
    {
        stateChoice = 1;
        ChangeSelection();
    }

    private void Update()
    {
        // Checks the input
        verticalInput = Input.GetAxis("Vertical");

        // Changes the selected state based on player controller movement
        if (verticalInput > 0.1 && stateChoice! >= 3)
        {
            stateChoice += 1;
            ChangeSelection();
            Wait(0.1f);
        }
        else if (verticalInput < -0.1 && stateChoice! <= 0)
        {
            stateChoice -= 1;
            ChangeSelection();
            Wait(0.1f);
        }

        // Clicks the currently selected button
        if (Input.GetButtonDown("Jump"))
        {
            switch (state)
            {
                case ButtonSelection.section1:
                    Button1.onClick.Invoke();
                    break;
                case ButtonSelection.section2:
                    Button2.onClick.Invoke();
                    break;
                case ButtonSelection.section3:
                    Button3.onClick.Invoke();
                    break;
            }
        }
    }

    // Actually changes the state of the state, to make all buttons real
    private void ChangeSelection()
    {
        switch (stateChoice)
        {
            case 1:
                state = ButtonSelection.section1;
                // The color changes make it more obvovious what the player has currently selected
                Button1.image.color = new Color32(22, 22, 22, 255);
                Button2.image.color = new Color32(66, 66, 66, 255);
                Button3.image.color = new Color32(66, 66, 66, 255);
                break;
            case 2:
                state = ButtonSelection.section2;
                Button1.image.color = new Color32(66, 66, 66, 255);
                Button2.image.color = new Color32(22, 22, 22, 255);
                Button3.image.color = new Color32(66, 66, 66, 255);
                break;
            case 3:
                state = ButtonSelection.section3;
                Button1.image.color = new Color32(66, 66, 66, 255);
                Button2.image.color = new Color32(66, 66, 66, 255);
                Button3.image.color = new Color32(22, 22, 22, 255);
                break;
        }
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
