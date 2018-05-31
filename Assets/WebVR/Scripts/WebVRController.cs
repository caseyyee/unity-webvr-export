using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum WebVRControllerHand { NONE, LEFT, RIGHT };

[System.Serializable]
public class WebVRControllerButton
{
    public bool pressed;
    public bool touched;
    public float value;
}

public class WebVRController : MonoBehaviour
{
    [Tooltip("Controller hand to use.")]
    public WebVRControllerHand hand = WebVRControllerHand.NONE;
    [Tooltip("Controller input settings.")]
    public WebVRControllerInputSettings inputSettings;
    [HideInInspector]
    public int index;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Quaternion rotation;
    [HideInInspector]
    public Matrix4x4 sitStand;
    [HideInInspector]
    public WebVRControllerButton[] buttons = null;

    private Dictionary<string, bool[]> buttonStates = new Dictionary<string, bool[]>();
    
    public void UpdateButtons(WebVRControllerButton[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            WebVRControllerButton button = buttons[i];
            foreach (WebVRControllerInput input in inputSettings.inputs)
            {
                if (input.gamepadButtonId == i)
                {
                    if (buttonStates.ContainsKey(input.actionName))
                        buttonStates[input.actionName][0] = button.pressed;
                    else
                        buttonStates.Add(input.actionName, new bool[]{ button.pressed, false });
                }
            }
        }
    }

    public bool GetButton(string action)
    {
        if (!buttonStates.ContainsKey(action))
            return false;
        return buttonStates[action][0];
    }

    public bool GetButtonDown(string action)
    {
        // Use Unity Input Manager when XR is enabled and WebVR is not being used (eg: standalone or from within editor).
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            foreach(WebVRControllerInput input in inputSettings.inputs)
            {
                if (action == input.actionName)
                    return Input.GetButtonDown(input.unityInputName);
            }
            return false;
        }

        if (!buttonStates.ContainsKey(action))
            return false;

        bool isDown = false;
        bool buttonPressed = buttonStates[action][0];
        bool prevButtonState = buttonStates[action][1];

        if (buttonPressed && prevButtonState != buttonPressed)
        {
            buttonStates[action][1] = true;
            isDown = true;
        }
        return isDown;
    }

    public bool GetButtonUp(string action)
    {
        // Use Unity Input Manager when XR is enabled and WebVR is not being used (eg: standalone or from within editor).
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            foreach(WebVRControllerInput input in inputSettings.inputs)
            {
                if (action == input.actionName)
                    return Input.GetButtonUp(input.unityInputName);
            }
            return false;
        }

        if (!buttonStates.ContainsKey(action))
            return false;
        
        bool isUp = false;
        bool buttonPressed = buttonStates[action][0];
        bool prevButtonState = buttonStates[action][1];

        if (!buttonPressed && prevButtonState) {
            buttonStates[action][1] = false;
            isUp = true;
        }
        return isUp;
    }

    private void onControllerUpdate(
        int index, string h, Vector3 position, Quaternion rotation, Matrix4x4 sitStand, WebVRControllerButton[] b)
    {
        // convert string to enum
        WebVRControllerHand handValue = WebVRControllerHand.NONE;
        if (!String.IsNullOrEmpty(h)) {
            try
            {
                handValue = (WebVRControllerHand) Enum.Parse(typeof(WebVRControllerHand), h.ToUpper(), true);
            }
            catch
            {
            }
        }

        if (handValue == hand)
        {
            // Apply controller orientation and position.
            Quaternion sitStandRotation = Quaternion.LookRotation (
                sitStand.GetColumn (2),
                sitStand.GetColumn (1)
            );
            transform.rotation = sitStandRotation * rotation;
            transform.position = sitStand.MultiplyPoint(position);

            UpdateButtons(b);
        }	
    }

    void Update()
    {
        // Use Unity XR Input when enabled. When using WebVR, updates are performed onControllerUpdate.
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            if (hand == WebVRControllerHand.LEFT)
            {
                transform.position = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
                transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftHand);
            }

            if (hand == WebVRControllerHand.RIGHT)
            {
                transform.position = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand);
                transform.rotation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightHand);
            }
        }
    }

    void Start()
    {
        WebVRManager.OnControllerUpdate += onControllerUpdate;
    }
}
