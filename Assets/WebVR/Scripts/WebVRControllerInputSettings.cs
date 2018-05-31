using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WebVRControllerInputSettings")]
public class WebVRControllerInputSettings : ScriptableObject {
	[Header("WebVR Controller Input Settings")]
	public List<WebVRControllerInput> inputs;
}

[System.Serializable]
public class WebVRControllerInput {
	[Tooltip("Name of input action to be performed.")]
	public string actionName;
	[Tooltip("WebVR Gamepad button ID.")]
	public int gamepadButtonId;
	[Tooltip("Button name defined in Unity Input Manager.")]
	public string unityInputName;
}