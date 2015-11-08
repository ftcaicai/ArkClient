using UnityEngine;
using System.Collections;
using System.Text;

public class AsCategoryAchieveInfo : MonoBehaviour
{
	[SerializeField] GameObject 	goInfoObtained = null;
	[SerializeField] UIProgressBar progressObtained = null;
	[SerializeField] SpriteText countInfoObtained = null;
	[SerializeField] SpriteText percentInfoObtained = null;
	[SerializeField] UIButton 	accrueRewardBtn = null;

	[SerializeField] GameObject 	goInfoNormal = null;
	[SerializeField] UIProgressBar progressNormal = null;
	[SerializeField] SpriteText countInfoNormal = null;
	[SerializeField] SpriteText percentInfoNormal = null;


	private StringBuilder sbCount = new StringBuilder();
	private StringBuilder sbPercent = new StringBuilder();
	
	// Use this for initialization
	void Start()
	{
		accrueRewardBtn.SetInputDelegate(AccrueRewardBtnDelegate);
		accrueRewardBtn.Text = AsTableManager.Instance.GetTbl_String(2728);
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( eDesignationCategory category)
	{
		AsUtil.SetButtonState( accrueRewardBtn , UIButton.CONTROL_STATE.DISABLED );

		goInfoObtained.SetActive (false);
		goInfoNormal.SetActive (true);

		switch( category)
		{
		case eDesignationCategory.Invalid:	_initObtainedList();	break;
		case eDesignationCategory.Character:	_initCharacterCategory();	break;
		case eDesignationCategory.Monster:	_initMonsterCategory();	break;
		case eDesignationCategory.Area:	_initAreaCategory();	break;
		case eDesignationCategory.Item:	_initItemCategory();	break;
		case eDesignationCategory.Quest:	_initQuestCategory();	break;
		case eDesignationCategory.Friends:	_initFriendsCategory();	break;
		case eDesignationCategory.Unique:	_initUniqueCategory();	break;
		case eDesignationCategory.Etc:	_initEtcCategory();	break;
		default:
			Debug.LogError( "AsCategoryAchieveInfo -> Invalid category");
			break;
		}
	}
	
	void _initObtainedList()
	{
		goInfoObtained.SetActive (true);
		goInfoNormal.SetActive (false);

		int nLastReceiveRewardRankPoint = AsDesignationRankRewardManager.Instance.LastReceiveRewardRankPoint;
		DesignationRankRewardData nextRankRewardData = AsDesignationRankRewardManager.Instance.GetNextRankRewardData (nLastReceiveRewardRankPoint);

		float progressValue = 0.0f;
		if (nextRankRewardData != null) 
		{
/*
			int nObtainedDesignationRankPoint = AsDesignationManager.Instance.TotalObtainedDesignationRankPoint;

			if (nObtainedDesignationRankPoint >= nextRankRewardData.nMaxRankRewardPoint)
					nObtainedDesignationRankPoint = nextRankRewardData.nMaxRankRewardPoint;

			progressValue = (float)nObtainedDesignationRankPoint / (float)nextRankRewardData.nMaxRankRewardPoint;

			sbCount.Remove (0, sbCount.Length);
			sbCount.AppendFormat ("{0}/{1}[{2:F1}%{3}]", nObtainedDesignationRankPoint, nextRankRewardData.nMaxRankRewardPoint,
	             progressValue * 100.0f, AsTableManager.Instance.GetTbl_String (2182));
			countInfoObtained.Text = sbCount.ToString ();
*/
			AsUtil.SetButtonState( accrueRewardBtn , UIButton.CONTROL_STATE.NORMAL );
		} 
		else 
		{
			countInfoObtained.Text = string.Empty;

			AsUtil.SetButtonState( accrueRewardBtn , UIButton.CONTROL_STATE.DISABLED );
		}




		int totalCount = AsDesignationManager.Instance.TotalCount - AsDesignationManager.Instance.CountByBlind;
		if( 0 > totalCount)
			totalCount = 0;

		if( 0 < AsDesignationManager.Instance.TotalCount)
			progressValue = (float)AsDesignationManager.Instance.ObtainedCount / (float)totalCount;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedCount, totalCount,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoObtained.Text = sbCount.ToString();




		sbPercent.Remove( 0, sbPercent.Length);
		sbPercent.AppendFormat( "{0}{1}", AsTableManager.Instance.GetTbl_String(4265), AsDesignationManager.Instance.TotalObtainedDesignationRankPoint);
		percentInfoObtained.Text = sbPercent.ToString();
		
		progressObtained.Value = progressValue;
	}
	
	void _initCharacterCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedCharacterDesignationCount, AsDesignationManager.Instance.CountByCharacter);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByCharacter)
			progressValue = (float)AsDesignationManager.Instance.ObtainedCharacterDesignationCount / (float)AsDesignationManager.Instance.CountByCharacter;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedCharacterDesignationCount, AsDesignationManager.Instance.CountByCharacter,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initMonsterCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedMonsterDesignationCount, AsDesignationManager.Instance.CountByMonster);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByMonster)
			progressValue = (float)AsDesignationManager.Instance.ObtainedMonsterDesignationCount / (float)AsDesignationManager.Instance.CountByMonster;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedMonsterDesignationCount, AsDesignationManager.Instance.CountByMonster,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initAreaCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedAreaDesignationCount, AsDesignationManager.Instance.CountByArea);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByArea)
			progressValue = (float)AsDesignationManager.Instance.ObtainedAreaDesignationCount / (float)AsDesignationManager.Instance.CountByArea;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedAreaDesignationCount, AsDesignationManager.Instance.CountByArea,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initItemCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedItemDesignationCount, AsDesignationManager.Instance.CountByItem);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByItem)
			progressValue = (float)AsDesignationManager.Instance.ObtainedItemDesignationCount / (float)AsDesignationManager.Instance.CountByItem;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedItemDesignationCount, AsDesignationManager.Instance.CountByItem,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initQuestCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedQuestDesignationCount, AsDesignationManager.Instance.CountByQuest);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByQuest)
			progressValue = (float)AsDesignationManager.Instance.ObtainedQuestDesignationCount / (float)AsDesignationManager.Instance.CountByQuest;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedQuestDesignationCount, AsDesignationManager.Instance.CountByQuest,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initFriendsCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedFriendsDesignationCount, AsDesignationManager.Instance.CountByFriends);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByFriends)
			progressValue = (float)AsDesignationManager.Instance.ObtainedFriendsDesignationCount / (float)AsDesignationManager.Instance.CountByFriends;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedFriendsDesignationCount, AsDesignationManager.Instance.CountByFriends,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initUniqueCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedUniqueDesignationCount, AsDesignationManager.Instance.CountByUnique);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByUnique)
			progressValue = (float)AsDesignationManager.Instance.ObtainedUniqueDesignationCount / (float)AsDesignationManager.Instance.CountByUnique;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedUniqueDesignationCount, AsDesignationManager.Instance.CountByUnique,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}
	
	void _initEtcCategory()
	{
		_initNormalInfo (AsDesignationManager.Instance.ObtainedEtcDesignationCount, AsDesignationManager.Instance.CountByEtc);
/*
		float progressValue = 0.0f;
		if( 0 < AsDesignationManager.Instance.CountByEtc)
			progressValue = (float)AsDesignationManager.Instance.ObtainedEtcDesignationCount / (float)AsDesignationManager.Instance.CountByEtc;

		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", AsDesignationManager.Instance.ObtainedEtcDesignationCount, AsDesignationManager.Instance.CountByEtc,
			progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
*/		
	}

	void _initNormalInfo( int _current , int _max )
	{
		float progressValue = 0.0f;
		if( 0 < _max )
			progressValue = (float)_current / (float)_max;
		
		sbCount.Remove( 0, sbCount.Length);
		sbCount.AppendFormat( "{0}/{1}[{2:F1}%{3}]", _current, _max, progressValue * 100.0f, AsTableManager.Instance.GetTbl_String(2182));
		countInfoNormal.Text = sbCount.ToString();
		
		percentInfoNormal.Text = string.Empty;
		
		progressNormal.Value = progressValue;
	}

	void AccrueRewardBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{		
			if( AsDesignationManager.Instance.SendRequestReward == true )
				return;

			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

			int nLastReceiveRewardRankPoint = AsDesignationRankRewardManager.Instance.LastReceiveRewardRankPoint;
			DesignationRankRewardData nextRankRewardData = AsDesignationRankRewardManager.Instance.GetNextRankRewardData (nLastReceiveRewardRankPoint);

			if( nextRankRewardData == null )
				return;

			int nItemId = -1;
			int nItemCount = 0;
			eCLASS _class = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS> (eComponentProperty.CLASS);
			switch (_class) 
			{
			case eCLASS.DIVINEKNIGHT:
				nItemId = nextRankRewardData.DivineKnight_Item_ID;
				nItemCount = nextRankRewardData.DivineKnight_Item_Count;
				break;
				
			case eCLASS.MAGICIAN:
				nItemId = nextRankRewardData.Magician_Item_ID;
				nItemCount = nextRankRewardData.Magician_Item_Count;
				break;
				
			case eCLASS.CLERIC:
				nItemId = nextRankRewardData.Cleric_Item_ID;
				nItemCount = nextRankRewardData.Cleric_Item_Count;
				break;
				
			case eCLASS.HUNTER:
				nItemId = nextRankRewardData.Hunter_Item_ID;
				nItemCount = nextRankRewardData.Hunter_Item_Count;
				break;
			}
			
			if (nItemId > 0) 
			{
				bool isConditionSatisfaction = false;
				if( AsDesignationManager.Instance.TotalObtainedDesignationRankPoint >= nextRankRewardData.nMaxRankRewardPoint )
					isConditionSatisfaction = true;

				GameObject dlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Designation_Popup")) as GameObject;
				Debug.Assert( null != dlg);
				AsDesignationRewardDlg rewardDlg = dlg.GetComponent<AsDesignationRewardDlg>();
				Debug.Assert( null != rewardDlg);
				rewardDlg.Open ( eDesignationRewardType.Accrue , nItemId  , nItemCount , isConditionSatisfaction , 0  , nextRankRewardData.nMaxRankRewardPoint );
			}

		}
	}

}










