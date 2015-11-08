using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public enum eSynthesisMode
{
	Synthesis,
	Upgrade,
};

public enum eCostumeSynthesisResult
{
	Success,
	GreateSuccess,
	Compelete,
};

public class AsSynthesisCosDlg : AsSynthesisBaseDlg 
{ 
	public SpriteText txtInfo;
	public SpriteText txtMatNum;
	public SpriteText targetItemName;

	[SerializeField] UIProgressBar 	progressCurrent = null;
	[SerializeField] UIProgressBar 	progressExpect = null;
	[SerializeField] SpriteText 		txtProgress;
	private StringBuilder 				sbProgress = new StringBuilder();
	[SerializeField] GameObject 	goExpectExpMax = null;

	[SerializeField] GameObject	goEffectSuccessRolling = null;
	[SerializeField] GameObject	goEffectSuccess = null;

	[SerializeField] GameObject	goEffectGreateSuccessRolling = null;
	[SerializeField] GameObject	goEffectGreateSuccess = null;

	[SerializeField] SimpleSprite 	txtResultSuccess;
	[SerializeField] SimpleSprite 	txtResultGreateSuccess;
	[SerializeField] SimpleSprite 	txtResultComplete;
	protected List<SimpleSprite> 	listTxtResult = new List<SimpleSprite>();

	private AsSynthesisSlot			targetSynthesisSlot = null;
	private StringBuilder 				sbCommTemp = new System.Text.StringBuilder();
	private Color 						colorStrength = Color.white;

	private int							needMaxExp = 0;
	private int 							currentExp = 0;
	private int							expectExp = 0;
	private float						successProcessCurrentValue = 0;
	private float						successProcessDestValue = 0;

	private Color						colorResult = Color.white;
	private float 						fAlphaDecreaseSpeed = 2.0f;
	private float						fCurTime = 0;

	private int							upgradeCostGold = 0;
	private int							upgradeCostMiracle = 0;

	private eSynthesisMode			synthesisMode = eSynthesisMode.Synthesis;

	[SerializeField] GameObject 	goMaterialPopup;
	[SerializeField] GameObject 	goUpgradePopup;

	private List<int> 					listMaterialSlotIndex = new List<int>();

	public static bool 					isProcessing = false;
	public static bool 					isAuthorityButtonClick = true;

	public override void Open()
	{
		base.Open();

		if( txtMatNum != null )
			txtMatNum.Text = string.Empty;

		if( targetItemName != null )
			targetItemName.Text = string.Empty;
	}
	
	public override void Close()
	{
		base.Close();
		SetEnableApply(false);
		
	}
	
	protected void SetTargetItemName( RealItem _realItem )
	{
		if( null == targetItemName )
			return;
		
		sITEM _sitem = _realItem.sItem;
		Item _item = _realItem.item;

		sbCommTemp.Remove( 0, sbCommTemp.Length );
		if( null != _sitem && 0 < _sitem.nStrengthenCount )
		{			
			sbCommTemp.Append( colorStrength );
			sbCommTemp.Append( "+" );
			sbCommTemp.Append( _sitem.nStrengthenCount );
			sbCommTemp.Append( " " );
			sbCommTemp.Append( _item.ItemData.GetGradeColor() );
			sbCommTemp.Append( AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId ) );
		}
		else
		{
			sbCommTemp.Append( _item.ItemData.GetGradeColor() );
			sbCommTemp.Append( AsTableManager.Instance.GetTbl_String( _item.ItemData.nameId ) );
		}
		
		targetItemName.Text = sbCommTemp.ToString();
	}
	
	private void CancelBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			AsHudDlgMgr.Instance.CloseSynCosDlg();
		}
	}
	
	private void ApplyBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( true == isMsgBox)
				return;

			if (isProcessing == true || isAuthorityButtonClick == false)
				return;

			isAuthorityButtonClick = false;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			if (synthesisMode == eSynthesisMode.Synthesis) 
			{
				ProcessSynthesis();
			}
			else if( synthesisMode == eSynthesisMode.Upgrade )
			{
				ProcessUpgrade();
			}
		}
	}


	private void ProcessSynthesis()
	{
		listMaterialSlotIndex.Clear();
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			if( _synSlot.slotType == eSynthesisSlotType.RawMaterial )
			{
				if( _synSlot.invenSlot.slotItem != null )
					listMaterialSlotIndex.Add( _synSlot.invenSlot.slotItem.realItem.getSlot );
				else
					listMaterialSlotIndex.Add( 0 );
			}
		}
		
		//	is there epic material?
		if( IsEpicMaterial() == true )
		{
			if( listMaterialSlotIndex.Count > 0 )
			{
				GameObject go = GameObject.Instantiate( goMaterialPopup ) as GameObject;
				AsSynthesisMaterialPopup	popup = go.GetComponentInChildren<AsSynthesisMaterialPopup>();
				popup.Open( this , eSynthesisMaterialAskMode.Grade , listMaterialSlotIndex[0] , listMaterialSlotIndex[1] , listMaterialSlotIndex[2] );
			}
		}
		//	if there already progress material?
		else if( IsAlreadyProgress() == true )
		{
			if( listMaterialSlotIndex.Count > 0 )
			{
				GameObject go = GameObject.Instantiate( goMaterialPopup ) as GameObject;
				AsSynthesisMaterialPopup	popup = go.GetComponentInChildren<AsSynthesisMaterialPopup>();
				popup.Open( this , eSynthesisMaterialAskMode.AlreadyProgress , listMaterialSlotIndex[0] , listMaterialSlotIndex[1] , listMaterialSlotIndex[2] );
			}
		}
		else
		{
			ExcuteSynthesis();
		}
	}
	
	private void ProcessUpgrade()
	{
		if (targetSynthesisSlot == null || targetSynthesisSlot.invenSlot.slotItem == null)
			return;

		RealItem _realItem = targetSynthesisSlot.invenSlot.slotItem.realItem;

		Tbl_SynCosMix_Record	recordCosMix = AsTableManager.Instance.GetSynCosMixRecord( _realItem.item.ItemData.grade , (Item.eEQUIP)_realItem.item.ItemData.GetSubType()  );

		int _costNormal = recordCosMix.upgradeCostNormal;
		int _costSpecial = recordCosMix.upgradeCostSpecial;

		if (_realItem.item.ItemData.isItem_OptionType == true) 
		{
			_costSpecial = 0;
		}

		GameObject go = GameObject.Instantiate( goUpgradePopup ) as GameObject;
		AsSynthesisUpgradePopup	popup = go.GetComponentInChildren<AsSynthesisUpgradePopup>();
		popup.Open( this , _costNormal , _costSpecial );
	}

	public void AskAlreadyProgress()
	{
		if( IsAlreadyProgress() == true )
		{
			if( listMaterialSlotIndex.Count > 0 )
			{
				GameObject go = GameObject.Instantiate( goMaterialPopup ) as GameObject;
				AsSynthesisMaterialPopup	popup = go.GetComponentInChildren<AsSynthesisMaterialPopup>();
				popup.Open( this , eSynthesisMaterialAskMode.AlreadyProgress , listMaterialSlotIndex[0] , listMaterialSlotIndex[1] , listMaterialSlotIndex[2] );
			}
		}
		else
		{
			ExcuteSynthesis();
		}
	}
	
	public void ExcuteSynthesis()
	{
		goEffectGreateSuccessRolling.SetActive(false);
		goEffectGreateSuccessRolling.SetActive(true);
		Invoke( "SendPacket", 2.0f);

		isProcessing = true;

		PlaySoundProgress (false);
	}

	public void ExcuteUpgrade(int _gold , int _miracle)
	{
		upgradeCostGold 		= _gold;
		upgradeCostMiracle 	= _miracle;

		goEffectGreateSuccessRolling.SetActive(false);
		goEffectGreateSuccessRolling.SetActive(true);
		Invoke( "SendPacket", 2.0f);

		isProcessing = true;

		PlaySoundProgress (true);
	}

	void PlaySoundProgress(bool isUpgrade)
	{
		Transform charTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		if( null == charTransform)
			return;

		if( isUpgrade == false )
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6037_EFF_MixCostume_Progress", charTransform.position, false);
		else
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6037_EFF_MixCostume_Complete", charTransform.position, false);
	}

	public void SetApplyAction()
	{
		if( actionSlider != null )
			actionSlider.ActionStart();

		SetEnableApply(false);
		SetPlayEffect(false);		
	}
	
	private void ResetBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			if( true == isMsgBox)
				return;
			
			if( actionSlider != null && false == actionSlider.isStop )
				return;
			
			if( true == AsCommonSender.isSendItemMix )
				return;
			
			ResetSlotMoveLock();
			txtMatNum.Text = string.Empty;
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			SetEnableApply(false);
		}
	}

	protected bool IsSameRealItem( RealItem _realItem )
	{
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			if( _synSlot.invenSlot == null || _synSlot.invenSlot.slotItem == null )
				continue;

			if( _synSlot.invenSlot.slotItem.realItem.getSlot == _realItem.getSlot )
				return true;
		}

		return false;
	}

	protected bool IsEpicMaterial()
	{
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			if( _synSlot.invenSlot.slotItem != null && _synSlot.slotType == eSynthesisSlotType.RawMaterial &&   _synSlot.invenSlot.slotItem.realItem.item.ItemData.grade >= Item.eGRADE.Epic )
				return true;
		}
		
		return false;
	}

	protected bool IsAlreadyProgress()
	{
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			if( _synSlot.invenSlot.slotItem != null && _synSlot.slotType == eSynthesisSlotType.RawMaterial && _synSlot.invenSlot.slotItem.realItem.sItem.nAccreCount > 0 )
				return true;
		}
		
		return false;
	}

	protected bool IsNowExpectExpMax()
	{
		if (currentExp + expectExp >= needMaxExp)
			return true;

		return false;
	}

	public override bool SetInputUpRealItem( RealItem _realItem, Ray inputRay )
	{
		if (isProcessing == true)
			return false;

		if( true == isMsgBox)
			return false;
		
		if( null == _realItem )
			return false;
		
		if( actionSlider != null && false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;

		bool isSynSlotChange = false;

		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;

			if( _invenSlot == null )
				continue;

			if( synthesisMode == eSynthesisMode.Upgrade && _synSlot.slotType == eSynthesisSlotType.RawMaterial )
			{
				AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2417), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
				return false;
			}

			if( _invenSlot.IsIntersect( inputRay ) == true )
			{
				int _checkResult = IsCheckItemType( _synSlot , _realItem );
				if( _checkResult != 0 )
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(_checkResult), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return false;
				}

				if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && targetSynthesisSlot == null )
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2426) , AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return false;
				}

				if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && IsNowExpectExpMax() == true )
				{
					AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2424) , AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
					return false;
				}

				if( _invenSlot.slotItem != null )
				{
					ResetSlotMoveLock(_invenSlot.slotItem.realItem.getSlot , false);
					isSynSlotChange = true;
				}

				AsSynthesisSlot	_realActionSlot = _synSlot;

				if( isSynSlotChange == false && _realActionSlot.slotType == eSynthesisSlotType.RawMaterial )
				{
					_realActionSlot = GetEmptyMaterialSlot();
					_invenSlot = _realActionSlot.invenSlot;
				}
				
				SetItemInSlot( _invenSlot, _realItem );
				if( null != _invenSlot.slotItem && _realActionSlot.slotType == eSynthesisSlotType.Target )
				{
					_invenSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
					SetTargetItemName( _realItem );
					targetSynthesisSlot = _realActionSlot;
				}

				AsSoundManager.Instance.PlaySound( _realItem.item.ItemData.getStrDropSound, Vector3.zero, false);
				UpdateItemChangeInfo();

				return true;
			}
		}
		return false;
	}
	
	public override bool SetDClickRealItem( RealItem _realItem )
	{
		if (isProcessing == true)
			return false;

		if( true == isMsgBox)
			return false;
		
		if( null == _realItem )
			return false;
		
		if( actionSlider != null && false == actionSlider.isStop )
			return false;
			
		if( true == AsCommonSender.isSendItemMix )
			return false;

		int _checkResult = 2417;
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;

			_checkResult = IsCheckItemType( _synSlot , _realItem );
			if( _checkResult == 0 )
			{
				if( _invenSlot != null && _invenSlot.slotItem == null )
				{
					if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && synthesisMode == eSynthesisMode.Upgrade )
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2417), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
						return false;
					}

					if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && targetSynthesisSlot == null )
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2426) , AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
						return false;
					}

					if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && IsNowExpectExpMax() == true )
					{
						AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2424) , AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);
						return false;
					}

					SetItemInSlot( _invenSlot, _realItem );
					if( null != _invenSlot.slotItem )
					{
						if( _synSlot.slotType == eSynthesisSlotType.Target )
						{
							_invenSlot.slotItem.iconImg.SetSize( m_iconSize, m_iconSize );			
							SetTargetItemName( _realItem );
							targetSynthesisSlot = _synSlot;
						}
						else if( _synSlot.slotType == eSynthesisSlotType.RawMaterial )
						{

						}
					}
					UpdateItemChangeInfo();

					return true;
				}
			}
		}

		if(  _checkResult == 0 )
			_checkResult = 2417;

		//	fail
		AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(_checkResult), AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE);

		return false;
	}
	
	protected int IsCheckItemType( AsSynthesisSlot _synSlot , RealItem _realItem )
	{
		if (Item.eITEM_TYPE.CosEquipItem != _realItem.item.ItemData.GetItemType ()) 
		{
			if (_synSlot.slotType == eSynthesisSlotType.Target) 
				return 2408;
			else if (_synSlot.slotType == eSynthesisSlotType.RawMaterial) 
				return 2412;
		}
		
		if (Item.eUSE_TIME_TYPE.NONE != _realItem.item.ItemData.m_eUseTimeType) 
		{
			if (_synSlot.slotType == eSynthesisSlotType.Target) 
				return 2410;
			else if (_synSlot.slotType == eSynthesisSlotType.RawMaterial) 
				return 2413;
		}

		if( IsSameRealItem( _realItem ) )
		{
			return 101;
		}
		
		if( false == _realItem.item.ItemData.m_Item_MixEnchant )
		{
			return 2411;
		}

		if (_synSlot.slotType == eSynthesisSlotType.Target) 
		{
			if (_realItem.item.ItemData.grade == Item.eGRADE.Normal || _realItem.item.ItemData.grade == Item.eGRADE.Ark)
				return 2409;
		}

		return 0;
	}

	private AsSynthesisSlot	GetEmptyMaterialSlot()
	{
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;
			if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && _invenSlot != null && _invenSlot.slotItem == null )
				return _synSlot;
		}
		return null;
	}

	void UpdateItemChangeInfo()
	{
		needMaxExp 	= 0;
		currentExp 		= 0;
		expectExp 		= 0;

		if (targetSynthesisSlot == null) 
		{
			progressCurrent.Value = 0;
			progressExpect.Value = 0;
			txtProgress.Text = string.Empty;
			goExpectExpMax.SetActive(false);
			CheckApplyBtn( eSynthesisMode.Synthesis , false );
			return;
		}

		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;

			if( _invenSlot == null || _invenSlot.slotItem == null )
				continue;

			RealItem	_realItem =  _invenSlot.slotItem.realItem;

			if( _synSlot.slotType == eSynthesisSlotType.Target )
			{
				Tbl_SynCosMix_Record	recordCosMix = AsTableManager.Instance.GetSynCosMixRecord( _realItem.item.ItemData.grade , (Item.eEQUIP)_realItem.item.ItemData.GetSubType()  );

				Tbl_GlobalWeight_Record recordGlobal = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "CosMixExpFactor" );

				if( recordCosMix != null && recordGlobal != null )
				{
//					needMaxExp = (int)((float)recordCosMix.needExp * ( 1.0f - (  recordGlobal.Value * (float)_realItem.sItem.nStrengthenCount / 1000.0f ) ));
					needMaxExp = (int)(((float)recordCosMix.needExp * ( 1000.0f - (  recordGlobal.Value * (float)_realItem.sItem.nStrengthenCount ) ))/1000.0f);
				}

				currentExp = _synSlot.invenSlot.slotItem.realItem.sItem.nAccreCount;
			}
			else if( _synSlot.slotType == eSynthesisSlotType.RawMaterial )
			{
				ItemData	targetItemData = null;
				ItemData	currentItemData = _synSlot.invenSlot.slotItem.realItem.item.ItemData;
				bool 			isSameKind			= false;
				
				if( targetSynthesisSlot != null )
				{
					targetItemData = targetSynthesisSlot.invenSlot.slotItem.realItem.item.ItemData;
					if( (Item.eEQUIP)targetItemData.GetSubType() ==  (Item.eEQUIP)currentItemData.GetSubType() )
						isSameKind = true;
				}


				int		nRealRankPoint = ItemMgr.GetRealRankPoint( _synSlot.invenSlot.slotItem.realItem.sItem , _synSlot.invenSlot.slotItem.realItem.item );
				if( isSameKind == true )
				{
					Tbl_GlobalWeight_Record recordGlobal = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "CosMixSameKindBonusRatio" );
					expectExp += (int)((float)nRealRankPoint * recordGlobal.Value * 0.001f);
				}
				else
				{
					expectExp += nRealRankPoint;
				}
			}
		}

		//	target item
		float fRatioCurrent = (float)currentExp / (float)needMaxExp;
		if( fRatioCurrent >= 1.0f ) fRatioCurrent = 1.0f;
		progressCurrent.Value = fRatioCurrent;

		//	raw material item
		float fRatioExpect = (float)(currentExp + expectExp) / (float)needMaxExp;
		if( fRatioExpect >= 1.0f ) fRatioExpect = 1.0f;
		progressExpect.Value = fRatioExpect;

		//	progressCurrent EmptySprite disable
		AutoSprite emptySprite = progressCurrent.gameObject.GetComponentInChildren<AutoSprite> ();
		if (emptySprite != null)	
			emptySprite.gameObject.SetActive (false);

		
		//	prograss text
		if( currentExp + expectExp >= needMaxExp )
		{
			txtProgress.Text = AsTableManager.Instance.GetTbl_String(2414);
		}
		else
		{
			sbProgress.Remove( 0, sbProgress.Length);

			if( expectExp != 0 )
				sbProgress.AppendFormat( "RGBA( 1.0,1.0,1.0,1.0){0} / {1} RGBA( 0.1,1.0,0.1,1.0)+ {2}", currentExp, needMaxExp , expectExp);
			else
				sbProgress.AppendFormat( "RGBA( 1.0,1.0,1.0,1.0){0} / {1}", currentExp, needMaxExp);

			txtProgress.Text = sbProgress.ToString();
		}

		if (currentExp >= needMaxExp) 
		{
			CheckApplyBtn( eSynthesisMode.Upgrade , true );

			CancelMaterialItem();
		} 
		else 
		{
			if( expectExp == 0 )
				CheckApplyBtn( eSynthesisMode.Synthesis , false );
			else
				CheckApplyBtn( eSynthesisMode.Synthesis , true );
		}

		if (expectExp > 0 && currentExp + expectExp >= needMaxExp ) 
		{
			goExpectExpMax.SetActive(true);
		}
		else
		{
			goExpectExpMax.SetActive(false);
		}
	}

	void CancelMaterialItem()
	{
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;
			
			if( _invenSlot == null)
				continue;

			if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && _invenSlot.slotItem	!= null )
			{
				RealItem	_realItem =  _invenSlot.slotItem.realItem;

				ResetSlotMoveLock(_invenSlot.slotItem.realItem.getSlot , false);
				_invenSlot.DeleteSlotItem();
			}
		}
	}

	void CheckApplyBtn(eSynthesisMode _mode , bool _enable)
	{
		synthesisMode = _mode;

		if( _mode == eSynthesisMode.Synthesis )
			btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String (2407);	//	synthesis
		else
			btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String (2406);	// upgrade

		SetEnableApply(_enable);
	}

	protected override void SendPacket()
	{
		if (synthesisMode == eSynthesisMode.Synthesis) 
		{
			if( targetSynthesisSlot != null && targetSynthesisSlot.invenSlot!= null && listMaterialSlotIndex.Count >= 3 )
				AsCommonSender.SendCosItemMixUp( (Int16)targetSynthesisSlot.invenSlot.slotItem.realItem.getSlot , (Int16)listMaterialSlotIndex[0] , (Int16)listMaterialSlotIndex[1] , (Int16)listMaterialSlotIndex[2] );
			else
				Debug.LogError( "AsSynthesisCosDlg SendPacket : eSynthesisMode.Synthesis fail!!" );
		} 
		else 
		{
			bool isCash = false;

			if( upgradeCostMiracle > 0 )
				isCash = true;

			if( targetSynthesisSlot != null && targetSynthesisSlot.invenSlot!= null  )
				AsCommonSender.SendCosItemMixUpgrade( (short)targetSynthesisSlot.invenSlot.slotItem.realItem.getSlot , isCash );
			else
				Debug.LogError( "AsSynthesisCosDlg SendPacket : eSynthesisMode.Upgrade fail!!" );
		}
	}

	public void ReceivePacket_CosItemMixResult( body_SC_COS_ITEM_MIX_UP_RESULT _result)
	{
		CancelMaterialItem();

		TargetSynthesisSlotResetting (_result.nResultSlot);

		SuccessEffect ( _result.bGreat );
	}

	public void ReceivePacket_CosItemMixUpgradeResult( body_SC_COS_ITEM_MIX_UPGRADE_RESULT _result)
	{
		CompleteEffect ();

		TargetSynthesisSlotResetting (_result.nResultSlot);

		InvenSlot[]	slots = ItemMgr.HadItemManagement.Inven.invenSlots;
		if (_result.nResultSlot < slots.Length) 
		{
			InvenSlot	_invenSlot = slots[_result.nResultSlot];
			
			if( _invenSlot.realItem.item.ItemData.grade >= Item.eGRADE.Rare )
			{
				AsEventNotifyMgr.Instance.ItemGetAlarmNotify.AddListItem ( _invenSlot.realItem.sItem );	
			}
		}
	}

	private void TargetSynthesisSlotResetting(int _targetSlot)
	{
		if (targetSynthesisSlot != null && targetSynthesisSlot.invenSlot.slotItem != null) 
		{
			ResetSlotMoveLock( targetSynthesisSlot.invenSlot.slotItem.realItem.getSlot , false);
			targetSynthesisSlot.invenSlot.DeleteSlotItem();
			
			targetItemName.Text = string.Empty;
			targetSynthesisSlot = null;
		}
		
		//	result target item set
		InvenSlot[]	slots = ItemMgr.HadItemManagement.Inven.invenSlots;
		if (_targetSlot < slots.Length) 
		{
			InvenSlot	_invenSlot = slots[_targetSlot];

			if( _invenSlot.realItem.item.ItemData.grade < Item.eGRADE.Ark )
			{
				SetDClickRealItem( _invenSlot.realItem );
				ResetSlotMoveLock( _targetSlot , true);
			}
			else
			{
				UpdateItemChangeInfo();
			}
		}
	}

	public void SuccessEffect(bool _isGreateSuccess)
	{
		successProcessCurrentValue = (float)currentExp;
		successProcessDestValue = (float)(currentExp+expectExp);
		
		colorResult.a = 1;
		fCurTime = Time.time;
		
		if (_isGreateSuccess == true) 
		{
			goEffectGreateSuccess.SetActive (false);
			goEffectGreateSuccess.SetActive (true);
			
			txtResultGreateSuccess.gameObject.SetActive(true);
			MeshRenderer	renderer = txtResultGreateSuccess.gameObject.GetComponent<MeshRenderer>();
			if( renderer != null )
				renderer.material.color = colorResult;

			CostumeSynthesisResultPanel (eCostumeSynthesisResult.GreateSuccess);
		} 
		else 
		{
			goEffectSuccess.SetActive (false);
			goEffectSuccess.SetActive (true);
			
			txtResultSuccess.gameObject.SetActive(true);
			MeshRenderer	renderer = txtResultSuccess.gameObject.GetComponent<MeshRenderer>();
			if( renderer != null )
				renderer.material.color = colorResult;

			CostumeSynthesisResultPanel (eCostumeSynthesisResult.Success);
		}

		SuccessMagicEffect ( false );
	}
	
	public void CompleteEffect()
	{
		colorResult.a = 1;
		fCurTime = Time.time;
		
		txtResultComplete.gameObject.SetActive(true);
		MeshRenderer	renderer = txtResultComplete.gameObject.GetComponent<MeshRenderer>();
		if( renderer != null )
			renderer.material.color = colorResult;

		CostumeSynthesisResultPanel (eCostumeSynthesisResult.Compelete);

		SuccessMagicEffect ( true );

		goEffectGreateSuccess.SetActive (false);
		goEffectGreateSuccess.SetActive (true);
	}

	public void CostumeSynthesisResultPanel( eCostumeSynthesisResult _type )
	{
		AsHUDController hud = null;
		GameObject go = AsHUDController.Instance.gameObject;
		if( null != go)
		{
			hud = go.GetComponent<AsHUDController>();
			switch( _type )
			{
			case eCostumeSynthesisResult.Success:
				hud.panelManager.ShowCosMixResult( "z" );
				break;

			case eCostumeSynthesisResult.GreateSuccess:
				hud.panelManager.ShowCosMixResult( "y" );
				break;

			case eCostumeSynthesisResult.Compelete:
				hud.panelManager.ShowCosMixResult( "|" );
				break;
			}
		}
	}

	public void SuccessMagicEffect( bool isUpgrade )
	{
		Transform charTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		if( null == charTransform)
			return;

		AsEffectManager.Instance.PlayEffect( "FX/Effect/COMMON/Fx_Common_StrengthenSuccess", charTransform , false, 0f, 1.0f);

		if( isUpgrade == false )
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6037_EFF_MixCostume_Success", charTransform.position, false);
	}

	public override void GuiInputUp(Ray inputRay)
	{ 	
		if( false == m_bInputDown )
			return;
		
		base.GuiInputUp(inputRay);	
	}

	public override void GuiInputDClickUp(Ray inputRay)
	{
		if (isProcessing == true)
			return;

		if( false == m_bInputDown )
			return;
		
		base.GuiInputDClickUp(inputRay);	

		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;
			if( _invenSlot != null && _invenSlot.IsIntersect( inputRay ) == true )
			{
				if( _invenSlot.slotItem != null )
					ResetSlotMoveLock(_invenSlot.slotItem.realItem.getSlot , false);

				_invenSlot.DeleteSlotItem();

				if( _synSlot.slotType == eSynthesisSlotType.Target )
				{
					targetItemName.Text = string.Empty;
					targetSynthesisSlot = null;

					CancelMaterialItem();
					UpdateItemChangeInfo();
				}
				else
				{
					UpdateItemChangeInfo();
					ReRangeMaterial();
				}

				return;
			}
		}
	}

	private void ReRangeMaterial()
	{
		bool 	isEmptyMaterial = false;
		int 	nSeq = 0;

		//	1. check
		AsSynthesisSlot _beforeSynMaterialSlot = null;
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;
			if( _synSlot.slotType == eSynthesisSlotType.RawMaterial )
			{
				if( _beforeSynMaterialSlot == null )
				{
					if( _invenSlot.slotItem == null )
						isEmptyMaterial = true;
				}
				else
				{
					if( _beforeSynMaterialSlot.invenSlot.slotItem == null && _invenSlot.slotItem != null )
						isEmptyMaterial = true;
				}

				_beforeSynMaterialSlot = _synSlot;
			}
		}

		if (isEmptyMaterial == false)
			return;

		//	2. rerange
		List<RealItem> listRealItem = new List<RealItem> ();
		foreach (AsSynthesisSlot	_synSlot in listSynthesisSlot) 
		{
			UIInvenSlot		_invenSlot = _synSlot.invenSlot;
			if( _synSlot.slotType == eSynthesisSlotType.RawMaterial && _invenSlot.slotItem != null )
			{
				listRealItem.Add( _invenSlot.slotItem.realItem );

				_invenSlot.DeleteSlotItem();
			}
		}

		UpdateItemChangeInfo();

		foreach (RealItem	_realItem in listRealItem) 
		{
			SetDClickRealItem (_realItem);
		}

	}
	 


	// Use this for initialization
	void Start () 
	{		
		txtTitle.Text = AsTableManager.Instance.GetTbl_String (1788);
		txtInfo.Text = AsTableManager.Instance.GetTbl_String (2401);
		txtEnchantSlotName [0].Text = AsTableManager.Instance.GetTbl_String (1224);
		txtEnchantSlotName [1].Text = AsTableManager.Instance.GetTbl_String (1225);
		txtEnchantSlotName [2].Text = AsTableManager.Instance.GetTbl_String (2405);	

		//	add SynthesisSlotList
		int nSeq = 0;
		AddSynthesisSlot (targetEnchantSlot, eSynthesisSlotType.Target);
		foreach (UIInvenSlot _slot in enchantSlots) 
		{
			AddSynthesisSlot( _slot , eSynthesisSlotType.RawMaterial , nSeq );
			nSeq++;
		}

		progressCurrent.Value = 0;
		progressExpect.Value = 0;
		txtProgress.Text = string.Empty;

		goExpectExpMax.SetActive(false);

		goEffectSuccessRolling.SetActive (false);
		goEffectSuccess.SetActive (false);

		goEffectGreateSuccessRolling.SetActive (false);
		goEffectGreateSuccess.SetActive (false);

		isProcessing = false;
		isAuthorityButtonClick = true;

		listTxtResult.Add (txtResultSuccess);
		listTxtResult.Add (txtResultGreateSuccess);
		listTxtResult.Add (txtResultComplete);

		foreach (SimpleSprite sp in listTxtResult)
			sp.gameObject.SetActive (false);


		synthesisMode = eSynthesisMode.Synthesis;
		if (btnApply != null)
		{
			btnApply.spriteText.Text = AsTableManager.Instance.GetTbl_String (2407);
			btnApply.SetInputDelegate (ApplyBtnDelegate);
		}

		btnClose.SetInputDelegate(CancelBtnDelegate);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (successProcessCurrentValue > 0 && successProcessDestValue > 0 && successProcessCurrentValue < successProcessDestValue ) 
		{
			successProcessCurrentValue += ((float)needMaxExp * Time.deltaTime);

			if( successProcessCurrentValue >= successProcessDestValue )
				successProcessCurrentValue = successProcessDestValue;

			float fRatioCurrent = successProcessCurrentValue / (float)needMaxExp;
			if( fRatioCurrent >= 1.0f ) fRatioCurrent = 1.0f;
			progressCurrent.Value = fRatioCurrent;

			if( fRatioCurrent >= 1.0f )
			{
				txtProgress.Text = AsTableManager.Instance.GetTbl_String(2414);
			}
			else
			{
				sbProgress.Remove( 0, sbProgress.Length);
				sbProgress.AppendFormat( "RGBA( 1.0,1.0,1.0,1.0){0} / {1}", (int)successProcessCurrentValue, needMaxExp);
				txtProgress.Text = sbProgress.ToString();
			}
		}


		foreach (SimpleSprite sp in listTxtResult) 
		{
			if ( sp.gameObject.activeSelf == true ) 
			{
				if( Time.time > fCurTime + 0.5f)
				{
					MeshRenderer	renderer = sp.gameObject.GetComponent<MeshRenderer>();
					if( renderer != null )
					{
						colorResult = renderer.material.color;
						colorResult.a -= ( Time.deltaTime * fAlphaDecreaseSpeed);
						if( colorResult.a <= 0 )
						{
							colorResult.a = 0;
							sp.gameObject.SetActive(false);
						}

						renderer.material.color = colorResult;
					}
				}
			}
		}

	}
}








