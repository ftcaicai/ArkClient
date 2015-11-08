using UnityEngine;
using System.Collections;

public class AsPlayerState_PrivateShop : AsBaseFsmState<AsPlayerFsm.ePlayerFsmStateType, AsPlayerFsm> {
	
	Msg_OpenPrivateShop m_OpenMsg = null;
	AsChatBallon_PersonalStore m_Title = null;
		
	public AsPlayerState_PrivateShop(AsPlayerFsm _fsm) : base(AsPlayerFsm.ePlayerFsmStateType.PRIVATESHOP, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.CLOSE_PRIVATESHOP, OnClosePrivateShop);
		m_dicMessageTreat.Add(eMessageType.PLAYER_CLICK, OnOpenPrivateShopUI);
		m_dicMessageTreat.Add(eMessageType.OPEN_PRIVATESHOP_UI, OnOpenPrivateShopUI);
		m_dicMessageTreat.Add(eMessageType.MODEL_LOADED, OnModelLoaded);
		m_dicMessageTreat.Add(eMessageType.MODEL_LOADED_DUMMY, OnModelLoaded_Dummy);
	}
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OpenMsg = _msg as Msg_OpenPrivateShop;
		
		m_OwnerFsm.UserEntity.HandleMessage(new Msg_MoveStopIndication());
		
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.SHOP_OPENING, true);
		m_OwnerFsm.EnterPStore();
		
		if(m_OwnerFsm.Entity.CheckModelType() != eModelType.Shop)
			m_OwnerFsm.Entity.HandleMessage(new Msg_ModelChange());
		
		m_OwnerFsm.Entity.transform.eulerAngles = new Vector3(0, 180, 0);
		
		if(m_OwnerFsm.Entity.CheckModelLoadingState() == eModelLoadingState.Finished)
			OnModelLoaded(_msg);
	}
	public override void Update()
	{
	}
	public override void Exit()
	{
		if(m_Title != null)
		{
			GameObject.Destroy(m_Title.gameObject);
			m_Title = null;
			
			Debug.Log("AsPlayerState_PrivateShop::Exit: title object destroyed normally");
		}
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - msg -
	void OnClosePrivateShop(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.SHOP_OPENING, false);
		m_OwnerFsm.ExitPStore();
		
		if(m_OwnerFsm.Entity.CheckModelType() == eModelType.Shop)
			m_OwnerFsm.Entity.HandleMessage(new Msg_ModelChange());
		
//		m_OwnerFsm.Entity.InitComponents();
//		m_OwnerFsm.Entity.InterInitComponents();
//		m_OwnerFsm.Entity.LateInitComponents();
		
		m_OwnerFsm.SetPlayerFsmState(AsPlayerFsm.ePlayerFsmStateType.IDLE, new Msg_PrevState_PrivateShop());
		
		m_OwnerFsm.ExitPStore();
	}
	
	void OnOpenPrivateShopUI(AsIMessage _msg)
	{
		if(AsPStoreManager.Instance.storeState == ePStoreState.User_Folded)
			AsPStoreManager.Instance.OpenFoldedUserPrivateShop();
	}
	
	void OnModelLoaded(AsIMessage _msg)
	{
		if(m_OwnerFsm.Entity.GetProperty<bool>(eComponentProperty.SHOP_OPENING) == false)
		{
			Debug.LogWarning("AsPlayerState_PrivateShop::OnModelLoaded: SHOP_OPENING = false. shop name ui instantiating will be skip");
			return;
		}
		
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME && AsGameMain.s_gameState != GAME_STATE.STATE_LOADING)
		{
			Debug.LogWarning("AsPlayerState_PrivateShop::OnModelLoaded: game state is " + AsGameMain.s_gameState + ". shop name ui instantiating will be skip.");
			return;
		}
		
		if(m_Title != null)
		{
			GameObject.Destroy(m_Title.gameObject);
			m_Title = null;
			
			Debug.Log("AsPlayerState_PrivateShop::OnModelLoaded: already instantiated is exist. destroy that.");
		}
		
		if(m_OpenMsg == null)
		{
			Debug.LogWarning("AsPlayerState_PrivateShop::OnModelLoaded: m_OpenMsg = null. shop name ui instantiating will be skip");
			return;
		}
		
		GameObject obj = null;
		switch((Item.eUSE_ITEM)m_OpenMsg.item_.ItemData.GetSubType())
		{
		case Item.eUSE_ITEM.PrivateStore1:
			obj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/GUI_Balloon_PersonalStore_normal")) as GameObject;
			break;
		case Item.eUSE_ITEM.PrivateStore2:
			obj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/GUI_Balloon_PersonalStore_magic")) as GameObject;
			break;
		case Item.eUSE_ITEM.PrivateStore3:
			obj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/GUI_Balloon_PersonalStore_rare")) as GameObject;
			break;
		case Item.eUSE_ITEM.PrivateStore4:
			obj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/GUI_Balloon_PersonalStore_epic")) as GameObject;
			break;
		case Item.eUSE_ITEM.PrivateStore5:
			obj = GameObject.Instantiate(Resources.Load("UI/Optimization/Prefab/GUI_Balloon_PersonalStore")) as GameObject;
			break;
		}
		
		if(obj != null) m_Title = obj.GetComponent<AsChatBallon_PersonalStore>();
		if(m_Title == null)
			Debug.LogError("AsPlayerState_PrivateShop:: Enter: Title<AsChatBalloonBase> is not found");
		else
		{			
//			m_Title.transform.parent = null;
			
			m_Title.transform.position = Vector3.zero;
			m_Title.transform.eulerAngles = Vector3.zero;
			m_Title.gameObject.SetActiveRecursively(true);
			m_Title.Owner = m_OwnerFsm.UserEntity;
			
			m_Title.SetTitleAndContent(m_OwnerFsm.UserEntity.GetProperty<string>(eComponentProperty.NAME), m_OwnerFsm.UserEntity.shopContent);
			
//			m_Title.SetText(m_OwnerFsm.UserEntity.GetProperty<string>(eComponentProperty.NAME));
//			m_Title.SetContent(m_OwnerFsm.UserEntity.shopContent);
			
			Vector3 worldPos = m_OwnerFsm.Entity.ModelObject.transform.position;
			worldPos.y += m_OwnerFsm.Entity.characterController.height;
			Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
			Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
			vRes.y += 2.0f;
			vRes.z = 0.0f;
			m_Title.transform.position = vRes;
			
			Debug.Log("AsPlayerState_PrivateShop::OnModelLoaded: title object shape updated");
		}
	}
	
	void OnModelLoaded_Dummy(AsIMessage _msg)
	{
		OnModelLoaded(_msg);
	}
	#endregion
}
