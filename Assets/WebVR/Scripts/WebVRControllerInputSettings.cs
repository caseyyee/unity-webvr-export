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
	public string actionName;
	public int gamepadId;
	public string unityInputAction;
}