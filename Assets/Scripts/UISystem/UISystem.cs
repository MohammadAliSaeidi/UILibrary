using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIManager
{
	public abstract class UISystem : MonoBehaviour
	{
		#region Vaiables

		internal UnityEvent e_OnSwitchedScreen = new UnityEvent();

		protected List<UIScreen> _screensList = new List<UIScreen>();

		public UIScreen FirstScreen;
		public UIScreen CurrentScreen { get; protected set; }
		public UIScreen PrevScreen { get; protected set; }

		public static readonly float DefaultShowAnimSpeed = 1f;
		public static readonly float DefaultHideAnimSpeed = 1f;

		#endregion

		#region Methods

		protected virtual void Start()
		{
			ShowFirstScreen();
		}

		public void SwitchTo(UIScreen screen)
		{
			if (screen && CurrentScreen != screen)
			{
				if (CurrentScreen)
				{
					CurrentScreen.gameObject.SetActive(true);
					CurrentScreen.Close();
					PrevScreen = CurrentScreen;
				}

				CurrentScreen = screen;
				CurrentScreen.gameObject.SetActive(true);
				CurrentScreen.Show();

				if (e_OnSwitchedScreen != null)
					e_OnSwitchedScreen.Invoke();
			}
		}

		public void Show(UIScreen screen)
		{
			screen.gameObject.SetActive(true);
			screen.Show();
		}

		public void Close(UIScreen screen)
		{
			screen.gameObject.SetActive(true);
			screen.Close();
		}

		public virtual void ShowFirstScreen()
		{
			if (FirstScreen)
			{
				SwitchTo(FirstScreen);
			}
		}

		public virtual void SwitchPrevScreen()
		{
			if (CurrentScreen.OverridePrevScreen != null)
			{
				SwitchTo(CurrentScreen.OverridePrevScreen);
			}

			else if (PrevScreen)
			{
				SwitchTo(PrevScreen);
			}

			else
			{
				Debug.LogError("The previous page is null");
			}
		}

		public void CloseAllScreens()
		{
			_screensList.ForEach(i =>
			{
				i.gameObject.SetActive(true);
				i.Close();
			});
		}

		protected abstract void GetAllScreens();

		#endregion
	}
}