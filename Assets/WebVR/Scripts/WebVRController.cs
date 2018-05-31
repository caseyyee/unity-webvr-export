using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class WebVRControllerButton
{
    public bool pressed;
    public bool touched;
    public float value;
}

public class WebVRController : MonoBehaviour
{
    [Tooltip("Map GameObject to controller hand name.")]
    public WebVRControllerHand hand = WebVRControllerHand.NONE;

    // public Enum hand;
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
//    public GameObject gameObject;

    private Dictionary<WebVRInputAction, bool[]> buttonStates = new Dictionary<WebVRInputAction, bool[]>();
    
    public void UpdateButtons(WebVRControllerButton[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            WebVRControllerButton button = buttons[i];
            foreach(WebVRInputAction action in Enum.GetValues(typeof(WebVRInputAction)))
            {
                if (i == (int)action) {
                    if (buttonStates.ContainsKey(action))
                        buttonStates[action][0] = button.pressed;
                    else
                        buttonStates.Add (action, new bool[]{ button.pressed, false });
                }
            }
        }
    }

    public bool GetButton(WebVRInputAction action)
    {
        if (!buttonStates.ContainsKey(action))
            return false;
        return buttonStates[action][0];
    }

    public bool GetButtonDown(WebVRInputAction action)
    {
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

    public bool GetButtonUp(WebVRInputAction action)
    {
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
        WebVRControllerHand updateHand;
        if (String.IsNullOrEmpty(h))
            updateHand = WebVRControllerHand.NONE;
        else
            updateHand = (WebVRControllerHand) Enum.Parse(typeof(WebVRControllerHand), h.ToUpper(), true);

        if (updateHand == hand)
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
		#if UNITY_EDITOR
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
        #endif
    }

    void Start()
    {
        WebVRManager.OnControllerUpdate += onControllerUpdate;
    }
}
