using UnityEngine;
using System.Collections;
using System.Text;

public class AsPartyDiceDlg : MonoBehaviour
{
	public SpriteText m_TimeText = null;
	public UIButton m_RollDiceBtn;
	public UIButton m_CloseBtn;
	public UIInvenSlot m_Itemslot;
	public UIProgressBar m_GiveUpTimeProgress = null;
	Animation m_Animation;
	//AnimationState m_Ani_state = null;
	private float m_fMaxTime = 60.0f;
	private float m_fStartTime = 0.0f;
	private int m_nDropItemIdx;
	public int DropItemIdx
	{
		get { return m_nDropItemIdx; }
		set { m_nDropItemIdx = value; }
	}
	private bool m_bIsShake = false;
	private float m_DiceAniTime = 0;
	private string m_strDiceAniName = "dice_ani";

	private bool m_IsUseing = false;
	public bool IsUseing
	{
		get { return m_IsUseing; }
		set { m_IsUseing = value; }
	}

	private Vector3 m_beforeAcce;

	void Clear()
	{
		m_bIsShake = false;
		IsUseing = false;
	}

	public void Close()
	{
		Clear();
		AsPartyManager.Instance.ClosePartyDiceDlg();

		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void GiveUp()
	{
		Clear();

		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void Hidden()
	{
		gameObject.SetActiveRecursively( false);
		gameObject.active = false;
	}

	public void Open()
	{
		gameObject.SetActiveRecursively( true);
		gameObject.active = true;
		//#13136
		if( null != m_Itemslot.slotItem)
				m_Itemslot.slotItem.ShowCoolTime( false);

		m_CloseBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL); //#18188
		m_CloseBtn.controlIsEnabled = true;
		
		m_RollDiceBtn.SetControlState( UIButton.CONTROL_STATE.NORMAL); //#22058
		m_RollDiceBtn.controlIsEnabled = true;
	}

	public void SetData( AS_SC_PARTY_DICE_ITEM_INFO data)
	{
		Clear();

		m_fStartTime = Time.time;
		if( data != null)
		{
			RealItem realItem = new RealItem( data.sItem, 0);
			m_Itemslot.DeleteSlotItem();

			if( null != realItem)
			{
				m_Itemslot.CreateSlotItem( realItem, m_Itemslot.gameObject.transform);
				if( null != m_Itemslot.slotItem)
					m_Itemslot.slotItem.ShowCoolTime( false);
			}
		}

		m_nDropItemIdx = data.nDropItemIdx;
		IsUseing = true;

		Hidden();
	}

	void Start()
	{
		m_Animation = m_RollDiceBtn.gameObject.GetComponentInChildren<Animation>();
		m_CloseBtn.SetInputDelegate( CloseBtnDelegate);
		m_RollDiceBtn.SetInputDelegate( RollDiceBtnDelegate);

		AnimationState ani_state;
		ani_state = m_Animation[m_strDiceAniName];
		m_DiceAniTime += ani_state.length;

		m_beforeAcce = Input.acceleration;
		m_fMaxTime = ( AsTableManager.Instance.GetTbl_GlobalWeight_Record( "DiceSelectTime").Value * 0.001f) - m_DiceAniTime;
	}

	// Update is called once per frame
	void Update()
	{
		if( !m_bIsShake)
		{
			SetGiveUpTime( ( m_fMaxTime + m_fStartTime - Time.time)/ m_fMaxTime);
			//#24142
			/*if( m_beforeAcce.x - Input.acceleration.x > 0.8f)
			{
				Debug.LogError( "acceleration!!!");
				m_bIsShake = true;
				m_Animation.Play( m_strDiceAniName);//#13437
				#if UNITY_IPHONE || UNITY_ANDROID
				if( true == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_Vibrate))
					Handheld.Vibrate();
				#endif
			}
			*/
		}
		else
		{
			if( !m_Animation.IsPlaying( m_strDiceAniName))
			{
				AsPartySender.SendPartyDiceShake( true,m_nDropItemIdx);
				Close();
			}
		}
	}

	public bool UpdateGiveUpTime()
	{
		if( !IsUseing || m_bIsShake)
			return false;

		float giveUpTime = ( m_fMaxTime + m_fStartTime - Time.time)/ m_fMaxTime;
		if( giveUpTime < 0.0f)
		{
			AsPartySender.SendPartyDiceShake( false,m_nDropItemIdx);
			Close();
			//Debug.LogError( "UpdateGiveUpTime!!!");
			return true;
		}

		return false;
	}

	public void SetGiveUpTime( float giveUpTime)
	{
		StringBuilder sb = new StringBuilder();
		int time = ( int)( m_fMaxTime + m_fStartTime - Time.time);
		sb.Insert( 0, time);
		sb.AppendFormat( "{0}", AsTableManager.Instance.GetTbl_String( 90));
		m_TimeText.Text = sb.ToString();
		if( giveUpTime < 0.0f)
		{
			AsPartySender.SendPartyDiceShake( false,m_nDropItemIdx);
			Close();
		}
		m_GiveUpTimeProgress.Value = giveUpTime;
	}

	private void CloseBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPartySender.SendPartyDiceShake( false,m_nDropItemIdx);
			Close();
		}
	}

	private void RollDiceBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			m_bIsShake = true;
			m_Animation.Play( m_strDiceAniName);//#13437
			m_RollDiceBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED); //#22058
			m_CloseBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED); //##17118
			#if UNITY_IPHONE || UNITY_ANDROID
			if( true == AsGameMain.GetOptionState( OptionBtnType.OptionBtnType_Vibrate))
				Handheld.Vibrate();
			#endif
		}
	}
}
