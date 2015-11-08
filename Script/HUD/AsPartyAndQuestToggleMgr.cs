using UnityEngine;
using System.Collections.Generic;

public class AsPartyAndQuestToggleMgr : MonoBehaviour {

	public enum PartyAndQuestToggleState
	{
		None,
		OpenQuestMini,
		CloseQuestMini,
		OpenParty,
		CloseParty,
	}

	public UIButton toggleBtn;
	public SimpleSprite[] spriteBtnBacks;
	public SimpleSprite[] spriteBtnIcons;
	PartyAndQuestToggleState nowState = PartyAndQuestToggleState.None;
	public bool		isChanging		= false;
	public float	changingTime	= 3.0f;
	public float	nowChangingTime	= 0.0f;
	public PartyAndQuestToggleState NowState { get { return nowState; } }

	// Use this for initialization
	void Start () 
	{
	
	}

	void Update()
	{
		if (isChanging == false)
			return;

		nowChangingTime -= Time.deltaTime;

		if (nowChangingTime > 0.0f)
			return;

		nowChangingTime = 0.0f;
		isChanging		= false;

		if (nowState == PartyAndQuestToggleState.OpenQuestMini)
		{
			SetBtnState(PartyAndQuestToggleState.OpenQuestMini);

			AsHudDlgMgr.Instance.OpenQuestMiniView();

			CloseParty();
		}
		else if (nowState == PartyAndQuestToggleState.OpenParty)
		{
			SetBtnState(PartyAndQuestToggleState.OpenParty);

			AsHudDlgMgr.Instance.CloseQuestMiniView();

			OpenParty();
		}
	}

	public void NothingQuestProcess()
	{
		if (isChanging == true)
			return;

		if (IsHaveParty() == true)
		{
			SetState(PartyAndQuestToggleState.OpenParty);
			SetBtnState(PartyAndQuestToggleState.OpenParty);

			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.CloseQuestMiniView();
			OpenParty();
		}
		else
		{
			SetState(PartyAndQuestToggleState.CloseQuestMini);
			SetBtnState(PartyAndQuestToggleState.CloseQuestMini);
			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.CloseQuestMiniView();
		}
	}

	public void AddPartyProcess()
	{
		if (nowState == PartyAndQuestToggleState.OpenQuestMini)
		{
			if (isChanging == false)
			{
				SetBtnState(PartyAndQuestToggleState.OpenParty);

				AsHudDlgMgr.Instance.CloseQuestMiniView();

				OpenParty();

				isChanging = true;

				nowChangingTime = changingTime;
			}
		}
		else if (nowState == PartyAndQuestToggleState.CloseQuestMini)
		{
			if (IsHaveParty() == true)
			{
				nowState = PartyAndQuestToggleState.OpenParty;
				SetBtnState(PartyAndQuestToggleState.OpenParty);
			}
		}
	}

	public void DelPartyProcess()
	{
		if (IsHaveQuest() == true)
		{
			if (AsHudDlgMgr.Instance.IsOpenQuestMiniView == false)
			{
				nowState = PartyAndQuestToggleState.OpenQuestMini;
				SetBtnState(PartyAndQuestToggleState.OpenQuestMini);

				if (AsHudDlgMgr.Instance != null)
					AsHudDlgMgr.Instance.OpenQuestMiniView();
			}
		}
		else
		{
			nowState = PartyAndQuestToggleState.CloseQuestMini;
			SetBtnState(PartyAndQuestToggleState.CloseQuestMini);
		}
	}

	public void ViewQuestMini()
	{
		nowState = PartyAndQuestToggleState.OpenQuestMini;
		SetBtnState(PartyAndQuestToggleState.OpenQuestMini);

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.OpenQuestMiniView();
	}
	
	/// <summary>
	/// for add only 1 count quest
	/// </summary>
	public void AddNewQuestProcess()
	{
		if (isChanging == true)
			return;

		if (nowState == PartyAndQuestToggleState.OpenParty)
		{
			SetBtnState(PartyAndQuestToggleState.OpenQuestMini);

			AsHudDlgMgr.Instance.OpenQuestMiniView();

			CloseParty();

			isChanging = true;

			nowChangingTime = changingTime;
		}
		else
		{
			nowState = PartyAndQuestToggleState.OpenQuestMini;
			SetBtnState(PartyAndQuestToggleState.OpenQuestMini);
			OpenQuestMiniView();
		}
	}

	void DisableAllImg()
	{
		foreach (SimpleSprite sprite in spriteBtnBacks)
			sprite.gameObject.SetActiveRecursively(false);

		foreach (SimpleSprite sprite in spriteBtnIcons)
			sprite.gameObject.SetActiveRecursively(false);
	}

	public void SetState(PartyAndQuestToggleState _state)
	{
		nowState = _state;
	}

	public void SetBtnState(PartyAndQuestToggleState _state)
	{
		DisableAllImg();

		if (_state == PartyAndQuestToggleState.OpenParty || _state == PartyAndQuestToggleState.OpenQuestMini)
			spriteBtnBacks[0].gameObject.SetActiveRecursively(true);
		else
			spriteBtnBacks[1].gameObject.SetActiveRecursively(true);

		if (_state == PartyAndQuestToggleState.OpenParty || _state == PartyAndQuestToggleState.CloseParty)
			spriteBtnIcons[1].gameObject.SetActiveRecursively(true);
		else
			spriteBtnIcons[0].gameObject.SetActiveRecursively(true);
	}

	void ProcessBtnClick()
	{
		if (isChanging == true)
			return;

		if (nowState == PartyAndQuestToggleState.OpenQuestMini) // close quest mini
		{
			if (IsHaveParty() == true)
			{
				nowState = PartyAndQuestToggleState.OpenParty;
				
				SetBtnState(PartyAndQuestToggleState.OpenParty);
				
				OpenParty();

				if (AsHudDlgMgr.Instance != null)
					AsHudDlgMgr.Instance.CloseQuestMiniView();
			}
			else
			{
				nowState = PartyAndQuestToggleState.CloseQuestMini;
				
				SetBtnState(PartyAndQuestToggleState.CloseQuestMini);
				
				if (AsHudDlgMgr.Instance != null)
					AsHudDlgMgr.Instance.CloseQuestMiniView();
			}
		}
		else if (nowState == PartyAndQuestToggleState.CloseQuestMini)
		{
			if (ArkQuestmanager.instance.GetSortedQuestListForQuestMini().Count == 0)
			{
				// add chat message
				AsEventNotifyMgr.Instance.CenterNotify.AddGMMessage(AsTableManager.Instance.GetTbl_String(421));
				return;
			}

			nowState = PartyAndQuestToggleState.OpenQuestMini;
			
			SetBtnState(PartyAndQuestToggleState.OpenQuestMini);

			if (AsHudDlgMgr.Instance != null)
				AsHudDlgMgr.Instance.OpenQuestMiniView();
		}
		else if (nowState == PartyAndQuestToggleState.OpenParty)  // close party
		{
			if (IsHaveQuest() == true)
			{
				CloseParty();

				if (AsHudDlgMgr.Instance != null)
					AsHudDlgMgr.Instance.OpenQuestMiniView();

				nowState = PartyAndQuestToggleState.OpenQuestMini;
				
				SetBtnState(PartyAndQuestToggleState.OpenQuestMini);
			}
			else
			{
				nowState = PartyAndQuestToggleState.CloseParty;
				
				SetBtnState(PartyAndQuestToggleState.CloseParty);

				CloseParty();
			}
		}
		else if (nowState == PartyAndQuestToggleState.CloseParty)
		{
			nowState = PartyAndQuestToggleState.OpenParty;
			
			SetBtnState(PartyAndQuestToggleState.OpenParty);
			
			OpenParty();
		}
	}

	public void OpenParty()
	{
		// open party
		if (AsPartyManager.Instance != null)
			if (AsPartyManager.Instance.PartyUI != null)
				if (AsPartyManager.Instance.PartyUI.PartyMemberUI != null)
					AsPartyManager.Instance.PartyUI.PartyMemberUI.ReSetPartyMember();
	}

	void CloseParty()
	{
		// close party
		if (AsPartyManager.Instance != null)
			if (AsPartyManager.Instance.PartyUI != null)
				AsPartyManager.Instance.PartyUI.PartyMemberUI.ClosePartyMember();
	}

	bool IsHaveParty()
	{
		return AsPartyManager.Instance == null ? false : AsPartyManager.Instance.GetPartyMemberList().Count > 1;
	}

	bool IsHaveQuest()
	{
		List<ArkQuest> listQuest = ArkQuestmanager.instance.GetSortedQuestListForQuestMini();
		return listQuest.Count >= 1;
	}

	public void BtnInputDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound(AsSoundPath.Tap_Quest_MiniView, Vector3.zero, false);
			ProcessBtnClick();
		}
	}

	public void Initilize()
	{
		toggleBtn.SetInputDelegate(BtnInputDelegate);
		OpenQuestMiniView();
	}

	void OpenQuestMiniView()
	{
		bool bHaveQuest = IsHaveQuest();
		bool bHaveParty = IsHaveParty();

		if (nowState == PartyAndQuestToggleState.None) // for first init
		{
			if (bHaveQuest == true)
			{
				nowState = PartyAndQuestToggleState.OpenQuestMini;
				SetBtnState(PartyAndQuestToggleState.OpenQuestMini);
				if (AsHudDlgMgr.Instance != null)
					AsHudDlgMgr.Instance.OpenQuestMiniView();
			}
			else
			{
				nowState = PartyAndQuestToggleState.CloseQuestMini;
				SetBtnState(PartyAndQuestToggleState.CloseQuestMini);
			}
		}
		else // for enter world
		{
			switch (nowState)
			{
				case PartyAndQuestToggleState.OpenParty:
					OpenParty();
					break;
				case PartyAndQuestToggleState.OpenQuestMini:
					{
						if (AsHudDlgMgr.Instance != null)
							AsHudDlgMgr.Instance.OpenQuestMiniView();
						CloseParty();
					}
					break;

				case PartyAndQuestToggleState.CloseParty:
					CloseParty();
					break;
				case PartyAndQuestToggleState.CloseQuestMini:
					if (AsHudDlgMgr.Instance != null)
						AsHudDlgMgr.Instance.CloseQuestMiniView();
					break;
			}

			SetBtnState(nowState);
		}
	}
}
