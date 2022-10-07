using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using Utility.Animation;

namespace UIManager
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(AnimationEventDispatcher))]
	public class UIScreen : MonoBehaviour
	{
		#region Vaiables

		#region Components

		private Animator _animator;
		private AnimationEventDispatcher _animationEventDispatcher;

		#endregion

		#region Events

		[Header("Events")]
		internal UnityEvent OnScreenStart = new UnityEvent();
		internal UnityEvent OnScreenClose = new UnityEvent();

		#endregion

		private Transform _content;

		[Tooltip("will override previous screen when it is not null")]
		[SerializeField] internal UIScreen OverridePrevScreen;
		[Tooltip("will override show animation speed when it is higher than \'0\'")]
		[SerializeField] internal float OverrideShowAnimSpeed;
		[Tooltip("will override hide animation speed when it is higher than \'0\'")]
		[SerializeField] internal float OverrideHideAnimSpeed;

		[Space(10)]
		public float DelayBeforeStartingScreen = 0;
		public float DelayBeforeClosingScreen = 0;
		public float DefaultDelay
		{
			get
			{
				return UISystem.DefaultShowAnimSpeed / 2;
			}
		}

		[Space(10)]
		public bool DeactiveOnHide = true;

		internal ScreenState ScreenState { get; private set; }

		#endregion

		#region Methods

		private void OnValidate()
		{
			_animator = GetComponent<Animator>();
			_animationEventDispatcher = GetComponent<AnimationEventDispatcher>();

			if (_animator.runtimeAnimatorController == null)
			{
				var animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(@"Assets/Animation/UI/DefaultUIAnimations/UIScreen/UIScreen.controller");
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

					if (_content != null && DeactiveOnHide)
					{
						_content.gameObject.SetActive(false);
					}
				}
				else if (ScreenState == ScreenState.IsBeingShown)
				{
					ScreenState = ScreenState.IsShowing;
				}
			});
		}

		private void Start()
		{
			InitAnimationSpeed();
		}

		[ContextMenu("Show Screen")]
		public void Show()
		{
			if (ScreenState != ScreenState.IsShowing || ScreenState != ScreenState.IsBeingShown)
			{
				if (ShowCoroutine != null)
				{
					StopCoroutine(ShowCoroutine);
				}

				ShowCoroutine = StartCoroutine(Co_Show());
			}
		}

		private Coroutine CloseCoroutine;
		[ContextMenu("Close Screen")]
		public void Close()
		{
			if (ScreenState != ScreenState.IsBeingClosed || ScreenState != ScreenState.Closed)
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

		private void InitAnimationSpeed()
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

		private Coroutine ShowCoroutine;

		private IEnumerator Co_Show()
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
			HandleAnimator("Show");
		}

		private IEnumerator Co_Close()
		{
			if (DelayBeforeClosingScreen > 0)
			{
				yield return new WaitForSeconds(DelayBeforeClosingScreen);
			}

			if (OnScreenClose != null)
			{
				OnScreenClose.Invoke();
			}

			HandleAnimator("Hide");
		}

		private void HandleAnimator(string aTrigger)
		{
			InitAnimationSpeed();

			if (_animator)
			{
				_animator.SetTrigger(aTrigger);

				if (aTrigger == "Show")
				{
					ScreenState = ScreenState.IsBeingShown;
				}

				else if (aTrigger == "Hide")
				{
					ScreenState = ScreenState.IsBeingClosed;
				}
			}
		}

		#endregion
	}
}