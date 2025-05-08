using UnityEngine;
using CW.Common;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component allows you to translate the current GameObject relative to the camera using the finger drag gesture.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTranslate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate")]
	public class LeanDragTranslate : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera the translation will be calculated using.
		/// None/null = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>The movement speed will be multiplied by this.
		/// -1 = Inverted Controls.</summary>
		public float Sensitivity { set { sensitivity = value; } get { return sensitivity; } } [SerializeField] private float sensitivity = 1.0f;

		// /// <summary>The movement speed will be multiplied by this.
		// /// -1 = Inverted Controls.</summary>
		// public float IsFrontView { set { isFrontView = value; } get { return isFrontView; } } [SerializeField] private float isFrontView = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [SerializeField] protected float damping = -1.0f;

		/// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
		/// NOTE: This requires <b>Dampening</b> to be above 0.</summary>
		public float Inertia { set { inertia = value; } get { return inertia; } } [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

		[SerializeField]

		private Vector3 remainingTranslation;

		public bool isLeftBox;
		public bool isRightBox;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Start()
        {
            if (gameObject.CompareTag("left"))
			{
				isLeftBox = true;
				isRightBox = false;
			}
			else if (gameObject.CompareTag("right"))
			{
				isLeftBox = false;
				isRightBox = true;
			}
		}

		protected virtual void Update()
		{
			// maintain boundary constraint
			MaintainBoundary();

			// Store
			var oldPosition = transform.localPosition;

			// Get the fingers we want to use
			var fingers = Use.UpdateAndGetFingers();

			// // Filter out any fingers whose initial touch (StartScreenPosition) is not within our allowed area.
			// List<LeanFinger> validFingers = new List<LeanFinger>();
			// foreach (var finger in fingers)
			// {
			// 	if (IsWithinAllowedArea(finger.StartScreenPosition))
			// 	{
			// 		validFingers.Add(finger);
			// 	}
			// }

			// // If no valid fingers remain, do not process dragging.
			// if (validFingers.Count == 0)
			// 	return;

			// Calculate the screenDelta value based on these fingers
			var screenDelta = LeanGesture.GetScreenDelta(fingers);

			if (screenDelta != Vector2.zero)
			{
				Debug.Log("button moved");
				// Perform the translation
				if (transform is RectTransform)
				{
					TranslateUI(screenDelta);
				}
				else
				{
					Translate(screenDelta);
				}
			}

			// Increment
			remainingTranslation += transform.localPosition - oldPosition;

			// Get t value
			var factor = CwHelper.DampenFactor(Damping, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);

			// Shift this transform by the change in delta
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;

			if (fingers.Count == 0 && inertia > 0.0f && Damping > 0.0f)
			{
				newRemainingTranslation = Vector3.Lerp(newRemainingTranslation, remainingTranslation, inertia);
			}

			// Update remainingDelta with the dampened value
			remainingTranslation = newRemainingTranslation;
		}

		// private bool IsWithinAllowedArea(Vector2 screenPoint)
		// {
		// 	float edgeMargin = 100f;
		// 	float middleMargin = 100f;

		// 	Debug.Log(screenPoint);

		// 	// Check the overall edge margins.
		// 	if (screenPoint.x < edgeMargin || screenPoint.x > Screen.width - edgeMargin ||
		// 		screenPoint.y < edgeMargin || screenPoint.y > Screen.height - edgeMargin)
		// 	{
		// 		return false;
		// 	}

		// 	// Additionally, enforce a margin along the middle of the screen.
		// 	// For the left box, the tap must be clearly on the left side.
		// 	if (screenPoint.x > Screen.width / 2 - middleMargin && screenPoint.x < Screen.width / 2 + middleMargin)
		// 	{
		// 		return false;
		// 	}

		// 	return true;
		// }

		private void TranslateUI(Vector2 screenDelta)
		{
			Debug.Log("Moving buttons");
			var finalCamera = _camera;

			if (finalCamera == null)
			{
				var canvas = transform.GetComponentInParent<Canvas>();

				if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
				{
					finalCamera = canvas.worldCamera;
				}
			}

			// Screen position of the transform
			var screenPoint = RectTransformUtility.WorldToScreenPoint(finalCamera, transform.position);

			// Add the deltaPosition
			screenPoint += screenDelta * Sensitivity;
			// Debug.Log("making bounds for front view");

			float offset = 2.0f;

			// if (isLeftBox) {
			// 	screenPoint.x = Mathf.Clamp(screenPoint.x, 0 + offset, Screen.width / 2 - offset);
			// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0 + offset, Screen.height - offset);
			// } else if (isRightBox) {
			// 	screenPoint.x = Mathf.Clamp(screenPoint.x, Screen.width / 2 + offset, Screen.width - offset);
			// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0 + offset, Screen.height - offset);
			// }

			// Convert back to world space
			var worldPoint = default(Vector3);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, finalCamera, out worldPoint) == true)
			{
				transform.position = worldPoint;
			}

			// enforce on child as well 
			foreach (Transform child in transform) {
				if (child) {
					var childPoint = RectTransformUtility.WorldToScreenPoint(finalCamera, child.position);
					if (isLeftBox) {
						childPoint.x = Mathf.Clamp(childPoint.x, 0 + offset, Screen.width / 2 - offset);
						childPoint.y = Mathf.Clamp(childPoint.y, 0 + offset, Screen.height - offset);
					} else if (isRightBox) {
						childPoint.x = Mathf.Clamp(childPoint.x, Screen.width / 2 + offset, Screen.width - offset);
						childPoint.y = Mathf.Clamp(childPoint.y, 0 + offset, Screen.height - offset);
					}
					// Convert back to world space
					var childWorldPoint = default(Vector3);

					if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, childPoint, finalCamera, out childWorldPoint) == true)
					{
						child.position = childWorldPoint;
					}
				}
			}
		}

		private void Translate(Vector2 screenDelta)
		{
			// Make sure the camera exists
			var camera = CwHelper.GetCamera(this._camera, gameObject);
			Debug.Log("Moving buttons");

			if (camera != null)
			{
				// Screen position of the transform
				var screenPoint = camera.WorldToScreenPoint(transform.position);

				// Add the deltaPosition
				screenPoint += (Vector3)screenDelta * Sensitivity;

				// obtain current width and height of the box
				// UIEnforceConstraints constraints = GetComponent<UIEnforceConstraints>(); 
				// float width = constraints.width; 
				// float height = constraints.height; 

				// if (isLeftBox) {
				// 	screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width / 2);
				// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
				// } else if (isRightBox) {
				// 	screenPoint.x = Mathf.Clamp(screenPoint.x, Screen.width / 2, Screen.width);
				// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
				// }

				// Convert back to world space
				transform.position = camera.ScreenToWorldPoint(screenPoint);

				// find the children
				foreach (Transform child in transform) {
					if (child) {
						var childPoint = camera.WorldToScreenPoint(child.position);
						if (isLeftBox) {
							childPoint.x = Mathf.Clamp(childPoint.x, 0, Screen.width / 2);
							childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
						} else if (isRightBox) {
							childPoint.x = Mathf.Clamp(childPoint.x, Screen.width / 2, Screen.width);
							childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
						}
						child.position = camera.ScreenToWorldPoint(childPoint); 
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
			}
		}

		private void MaintainBoundary() {
			var camera = CwHelper.GetCamera(this._camera, gameObject);
			if (camera == null || camera.name != "MainCamera") {
				return; 
			}
			// // Screen position of the transform
			// var screenPoint = camera.WorldToScreenPoint(transform.position);
			// if (isLeftBox) {
			// 	screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width / 2);
			// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
			// } else if (isRightBox) {
			// 	screenPoint.x = Mathf.Clamp(screenPoint.x, Screen.width / 2, Screen.width);
			// 	screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
			// }

			// // Convert back to world space
			// transform.position = camera.ScreenToWorldPoint(screenPoint);

			// find the children
			foreach (Transform child in transform) {
				if (child) {
					var childPoint = camera.WorldToScreenPoint(child.position);
					if (isLeftBox) {
						if (childPoint.x < 0 || childPoint.x >= Screen.width / 2 || childPoint.y < 0 || childPoint.y >= Screen.height) {
							childPoint.x = Mathf.Clamp(childPoint.x, 0, Screen.width / 2);
							childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
							child.position = camera.ScreenToWorldPoint(childPoint); 
						}
					} else if (isRightBox) {
						if (childPoint.x < Screen.width / 2 || childPoint.x >= Screen.width || childPoint.y < 0 || childPoint.y >= Screen.height) {
							childPoint.x = Mathf.Clamp(childPoint.x, Screen.width / 2, Screen.width);
							childPoint.y = Mathf.Clamp(childPoint.y, 0, Screen.height);
							child.position = camera.ScreenToWorldPoint(childPoint); 

						}
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanDragTranslate;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET), true)]
	public class LeanDragTranslate_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("_camera", "The camera the translation will be calculated using.\n\nNone/null = MainCamera.");
			Draw("sensitivity", "The movement speed will be multiplied by this.\n\n-1 = Inverted Controls.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("inertia", "This allows you to control how much momentum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Damping</b> to be above 0.");
		}
	}
}
#endif