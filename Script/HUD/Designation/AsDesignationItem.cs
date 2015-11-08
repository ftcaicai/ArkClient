using UnityEngine;
using System.Collections;

public class AsDesignationItem : MonoBehaviour
{
	[SerializeField] SpriteText nameText = null;
	[SerializeField] SpriteText requireText = null;
	[SerializeField] SpriteText effectText = null;
	[SerializeField] SpriteText rankText = null;
	[SerializeField] Color assignColor = Color.white;
	[SerializeField] Color possessedColor = Color.white;
	[SerializeField] Color unPossessedColor = Color.white;

	[SerializeField] SimpleSprite 		spriteRewardBase = null;
	[SerializeField] SimpleSprite 		spriteRewardDone = null;
	[SerializeField] public Vector2		minusItemSize;

	[SerializeField] GameObject		goFocus;

	UISlotItem rewardSlotItem;

	private bool isPossessed = false;
	public bool IsPossessed
	{
		get	{ return isPossessed; }
	}
	private DesignationData data = null;
	public DesignationData Data
	{
		get	{ return data; }
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void Init( DesignationData data)
	{
		isPossessed = AsDesignationManager.Instance.IsObtainedDesignation( data.id);
		
		if( true == isPossessed)
		{
			nameText.Color = possessedColor;
			requireText.Color = possessedColor;
			effectText.Color = possessedColor;
			rankText.Color = possessedColor;
		}
		else
		{
			nameText.Color = unPossessedColor;
			requireText.Color = unPossessedColor;
			effectText.Color = unPossessedColor;
			rankText.Color = unPossessedColor;
			
			UIListButton listBtn = gameObject.GetComponent<UIListButton>();
			listBtn.SetControlState( UIButton.CONTROL_STATE.DISABLED);
		}
		
		if( data.id == AsDesignationManager.Instance.CurrentID)
		{
			nameText.Color = assignColor;
			requireText.Color = assignColor;
			effectText.Color = assignColor;
			rankText.Color = assignColor;
		}
		
		nameText.Text = AsTableManager.Instance.GetTbl_String( data.name);
		requireText.Text = AsTableManager.Instance.GetTbl_String( data.desc);
		effectText.Text = AsTableManager.Instance.GetTbl_String( data.effectDesc);
		rankText.Text = data.rankPoint.ToString();
		
		this.data = data;

		SetRewardItem ();

		ReleaseFocus ();
	}
	
	public void Assign()
	{
		nameText.Color = assignColor;
		requireText.Color = assignColor;
		effectText.Color = assignColor;
		rankText.Color = assignColor;
	}
	
	public void ReleaseAssign()
	{
		nameText.Color = possessedColor;
		requireText.Color = possessedColor;
		effectText.Color = possessedColor;
		rankText.Color = possessedColor;
	}

	public void Focus()
	{
		goFocus.SetActive (true);
	}

	public void ReleaseFocus()
	{
		goFocus.SetActive (false);
	}

	public void SetRewardItem()
	{
		if (data.isRewardReceived == true) 
		{
			spriteRewardBase.gameObject.SetActive(true);
			spriteRewardDone.gameObject.SetActive(true);
			if( rewardSlotItem != null )
				Destroy( rewardSlotItem.gameObject );
			return;
		}

		//	designation reward
		int nItemId = -1;
		int nItemCount = 0;
		eCLASS _class = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS> (eComponentProperty.CLASS);
		switch (_class) 
		{
		case eCLASS.DIVINEKNIGHT:
			nItemId = data.DivineKnight_Item_ID;
			nItemCount = data.DivineKnight_Item_Count;
			break;
			
		case eCLASS.MAGICIAN:
			nItemId = data.Magician_Item_ID;
			nItemCount = data.Magician_Item_Count;
			break;
			
		case eCLASS.CLERIC:
			nItemId = data.Cleric_Item_ID;
			nItemCount = data.Cleric_Item_Count;
			break;
			
		case eCLASS.HUNTER:
			nItemId = data.Hunter_Item_ID;
			nItemCount = data.Hunter_Item_Count;
			break;
		}

		if (nItemId <= 0) 
		{
			spriteRewardBase.gameObject.SetActive(true);
			spriteRewardDone.gameObject.SetActive(false);
			return;
		}

		spriteRewardBase.gameObject.SetActive(true);
		spriteRewardDone.gameObject.SetActive(false);

		Item _item = ItemMgr.ItemManagement.GetItem(nItemId);
		if (null == _item)
		{
			Debug.LogError("AsDesignationItem::Open()[ null == item ] id: " + nItemId);
			return;
		}

		if( rewardSlotItem != null )
			Destroy( rewardSlotItem.gameObject );


		rewardSlotItem = ResourceLoad.CreateItemIcon(_item.GetIcon(), spriteRewardBase, Vector3.back, minusItemSize, false);
		if (null == rewardSlotItem)
		{
			Debug.LogError("AsDesignationItem::Open()[ null == UISlotItem ] id: " + nItemId);
			return;
		}

/*
		if (nItemCount > 1)	rewardSlotItem.itemCountText.Text = nItemCount.ToString ();
		else 						rewardSlotItem.itemCountText.Text = string.Empty;
*/		
		//	designatio item don't display item count.
		rewardSlotItem.itemCountText.Text = string.Empty;

		Vector2 vItemSize = new Vector2( spriteRewardBase.gameObject.GetComponent<BoxCollider>().size.x, spriteRewardBase.gameObject.GetComponent<BoxCollider>().size.y);
		UIButton button = rewardSlotItem.gameObject.AddComponent<UIButton>();
		button.width = vItemSize.x;
		button.height = vItemSize.y;
		
		button.SetInputDelegate( RewardItemClick );

	}


	public void RewardItemClick( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( AsDesignationManager.Instance.SendRequestReward == true )
				return;

			int nItemId = -1;
			int nItemCount = 0;
			eCLASS _class = AsEntityManager.Instance.UserEntity.GetProperty<eCLASS> (eComponentProperty.CLASS);
			switch (_class) 
			{
			case eCLASS.DIVINEKNIGHT:
				nItemId = data.DivineKnight_Item_ID;
				nItemCount = data.DivineKnight_Item_Count;
				break;
				
			case eCLASS.MAGICIAN:
				nItemId = data.Magician_Item_ID;
				nItemCount = data.Magician_Item_Count;
				break;
				
			case eCLASS.CLERIC:
				nItemId = data.Cleric_Item_ID;
				nItemCount = data.Cleric_Item_Count;
				break;
				
			case eCLASS.HUNTER:
				nItemId = data.Hunter_Item_ID;
				nItemCount = data.Hunter_Item_Count;
				break;
			}

			if (nItemId > 0) 
			{
				GameObject dlg = GameObject.Instantiate( ResourceLoad.LoadGameObject( "UI/AsGUI/GUI_Designation_Popup")) as GameObject;
				Debug.Assert( null != dlg);
				AsDesignationRewardDlg rewardDlg = dlg.GetComponent<AsDesignationRewardDlg>();
				Debug.Assert( null != rewardDlg);
				rewardDlg.Open ( eDesignationRewardType.Normal , nItemId , nItemCount , isPossessed , data.id  , 100);
			}
		}
	}
}



























