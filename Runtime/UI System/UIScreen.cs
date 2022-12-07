using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Chromium.Utilities.Animation;

namespace UILibrary
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(AnimationEventDispatcher))]

	public partial class UIScreen : MonoBehaviour
	{
		#region Vaiables

		#region Components

		protected Animator _animator;
		protected AnimationEventDispatcher _animationEventDispatcher;

		#endregion

		#region Events

		[Header("Events")]
		internal UnityEvent OnScreenStart = new UnityEvent();
		internal UnityEvent OnScreenClose = new UnityEvent();

		#endregion

		[SerializeField] protected Transform _content;

		[Tooltip("will override previous screen when it is not null")]
		[SerializeField] internal UIScreen OverridePrevScreen;
		[Tooltip("will override show animation speed when it is higher than \'0\'")]
		[SerializeField] internal float OverrideShowAnimSpeed;
		[Tooltip("will override hide animation speed when it is higher than \'0\'")]
		[SerializeField] internal float OverrideHideAnimSpeed;

		[Space(10)]
		public float DelayBeforeStartingScreen = 0;
		public float DelayBeforeClosingScreen = 0;
		public readonly float DefaultDelay = UISystem.DefaultShowAnimSpeed / 2;

		[Header("Screen Content Deactivation")]
		[Tooltip("Deactivates the \"Content\" gameobject under the screen for better performance")]
		public bool DeactivateContentOnHide = true;

		internal ScreenState ScreenState { get; private set; }
		public ScreenType screenType;
		protected Dictionary<AnimatorState, string> AnimatorStates = new Dictionary<AnimatorState, string>();

		#endregion

		#region Methods

		protected void OnValidate()
		{
			_animator = GetComponent<Animator>();
			_animationEventDispatcher = GetComponent<AnimationEventDispatcher>();

			if (_animator.runtimeAnimatorController == null)
			{
				var animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(@"Assets/Animations/UI/DefaultUIAnimations/UIScreen/UIScreen.controller");
				_animator.runtimeAnimatorController = animatorController;
			}
		}

		protected virtual void Awake()
		{
			_content = transform.Find("Content");

			_animationEventDispatcher.e_OnAnimationComplete.AddListener(
			delegate
			{
				if (ScreenState == ScreenState.IsBeingClosed)
				{
					ScreenState = ScreenState.Closed;

					if (_content != null && DeactivateContentOnHide)
					{
						_content.gameObject.SetActive(false);
					}
				}
				else if (ScreenState == ScreenState.IsBeingShown)
				{
					ScreenState = ScreenState.IsShowing;
				}
			});

			AnimatorStates.Add(AnimatorState.Idle, "UIScreen_Idle");
			AnimatorStates.Add(AnimatorState.Show, "UIScreen_Show");
			AnimatorStates.Add(AnimatorState.Hide, "UIScreen_Hide");
		}

		protected virtual void Start()
		{
			InitAnimationSpeed();
		}

		[ContextMenu("Show Screen")]
		public void Show()
		{
			if (ScreenState != ScreenState.IsShowing &&
				ScreenState != ScreenState.IsBeingShown)
			{
				if (ShowCoroutine != null)
				{
					StopCoroutine(ShowCoroutine);
				}

				ShowCoroutine = StartCoroutine(Co_Show());
			}
		}

		protected Coroutine CloseCoroutine;
		[ContextMenu("Close Screen")]
		public void Close()
		{
			if (ScreenState != ScreenState.IsBeingClosed ||
				ScreenState != ScreenState.Closed)
			{
				if (CloseCoroutine != null)
				{
					StopCoroutine(CloseCoroutine);
				}

				CloseCoroutine = StartCoroutine(Co_Close());
			}
		}

		#endregion

		#region Private Methods

		protected void InitAnimationSpeed()
		{
			float showAnimSpeed;
			float hideAnimSpeed;

			if (OverrideShowAnimSpeed > 0)
			{
				showAnimSpeed = 1 / OverrideShowAnimSpeed;
			}
			else
			{
				showAnimSpeed = 1 / UISystem.DefaultShowAnimSpeed;
			}

			if (OverrideHideAnimSpeed > 0)
			{
				hideAnimSpeed = 1 / OverrideHideAnimSpeed;
			}
			else
			{
				hideAnimSpeed = 1 / UISystem.DefaultHideAnimSpeed;
			}

			_animator.SetFloat("ShowTranstionDuration", showAnimSpeed);
			_animator.SetFloat("HideTranstionDuration", hideAnimSpeed);
		}

		protected Coroutine ShowCoroutine;

		protected IEnumerator Co_Show()
		{
			if (DelayBeforeStartingScreen > 0)
			{
				yield return new WaitForSeconds(DelayBeforeStartingScreen);
			}

			if (OnScreenStart != null)
			{
				OnScreenStart.Invoke();
			}

			_content.gameObject.SetActive(true);
			HandleAnimator(AnimatorState.Show);
		}

		protected IEnumerator Co_Close()
		{
			if (DelayBeforeClosingScreen > 0)
			{
				yield return new WaitForSeconds(DelayBeforeClosingScreen);
			}

			if (OnScreenClose != null)
			{
				OnScreenClose.Invoke();
			}

			HandleAnimator(AnimatorState.Hide);
		}

		protected void HandleAnimator(AnimatorState state)
		{
			InitAnimationSpeed();

			if (_animator)
			{
				_animator.CrossFadeInFixedTime(AnimatorStates.GetValueOrDefault(state), 0.75f);

				if (state == AnimatorState.Show)
				{
					ScreenState = ScreenState.IsBeingShown;
				}

				else if (state == AnimatorState.Hide)
				{
					ScreenState = ScreenState.IsBeingClosed;
				}
			}
		}

		#endregion
	}
}