// using EntityStates;
// using RoR2;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.Networking;
// using EntityStates.VoidRaidCrab.Weapon;
// using EntityStates.Railgunner.Backpack;
// using RoR2.HudOverlay;
// using RoR2.UI;
// using System.Collections.Generic;

// namespace Voidcrid {
// public class LaserCountdown : BaseBackpack
// {[SerializeField]
// 	public float baseDuration;

// 	[SerializeField]
// 	public GameObject crosshairOverridePrefab;

// 	[SerializeField]
// 	public GameObject overlayPrefab;

// 	[SerializeField]
// 	public string overlayChildLocatorEntry;

// 	[SerializeField]
// 	public string animationLayerName;

// 	[SerializeField]
// 	public string animationStateName;

// 	[SerializeField]
// 	public string animationPlaybackRateParam;

// 	private OverlayController overlayController;

// 	private List<ImageFillController> fillUiList = new List<ImageFillController>();

// 	private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

// 	private float duration;

// 	public override void OnEnter()
// 	{
// 		isSoundScaledByAttackSpeed = true;
// 		base.OnEnter();
// 		duration = baseDuration / attackSpeedStat;
// 		PlayAnimation(animationLayerName, animationStateName, animationPlaybackRateParam, duration);
// 		OverlayCreationParams overlayCreationParams = default(OverlayCreationParams);
// 		overlayCreationParams.prefab = overlayPrefab;
// 		overlayCreationParams.childLocatorEntry = overlayChildLocatorEntry;
// 		OverlayCreationParams overlayCreationParams2 = overlayCreationParams;
// 		overlayController = HudOverlayManager.AddOverlay(base.gameObject, overlayCreationParams2);
// 		overlayController.onInstanceAdded += OnOverlayInstanceAdded;
// 		overlayController.onInstanceRemove += OnOverlayInstanceRemoved;
// 		if ((bool)crosshairOverridePrefab)
// 		{
// 			crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.PrioritySkill);
// 		}
// 	}

// 	public override void OnExit()
// 	{
// 		crosshairOverrideRequest?.Dispose();
// 		if (overlayController != null)
// 		{
// 			overlayController.onInstanceAdded -= OnOverlayInstanceAdded;
// 			overlayController.onInstanceRemove -= OnOverlayInstanceRemoved;
// 			fillUiList.Clear();
// 			HudOverlayManager.RemoveOverlay(overlayController);
// 		}
// 		base.OnExit();
// 	}

// 	public override void FixedUpdate()
// 	{
// 		base.FixedUpdate();
// 		if (duration > 0f)
// 		{
// 			foreach (ImageFillController fillUi in fillUiList)
// 			{
// 				fillUi.SetTValue(base.fixedAge / duration);
// 			}
// 		}
// 		if (base.fixedAge >= duration)
// 		{
// 			outer.SetNextStateToMain();
// 		}
// 	}

// 	// protected abstract EntityState LaserBeam();

// 	private void OnOverlayInstanceAdded(OverlayController controller, GameObject instance)
// 	{
// 		fillUiList.Add(instance.GetComponent<ImageFillController>());
// 	}

// 	private void OnOverlayInstanceRemoved(OverlayController controller, GameObject instance)
// 	{
// 		fillUiList.Remove(instance.GetComponent<ImageFillController>());
// 	}
//     }
// }