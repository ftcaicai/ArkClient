using UnityEngine;
using System.Collections;

public class AsOtherUserState_PrivateShop : AsBaseFsmState<AsOtherUserFsm.eOtherUserFsmStateType, AsOtherUserFsm> {
	
	Msg_OpenPrivateShop m_OpenMsg = null;
	AsChatBallon_PersonalStore m_Title = null;
	
	public AsOtherUserState_PrivateShop(AsOtherUserFsm _fsm) : base(AsOtherUserFsm.eOtherUserFsmStateType.PRIVATESHOP, _fsm)
	{
		m_dicMessageTreat.Add(eMessageType.OPEN_PRIVATESHOP, OnOpenPrivateShop);
		m_dicMessageTreat.Add(eMessageType.CLOSE_PRIVATESHOP, OnClosePrivateShop);
		m_dicMessageTreat.Add(eMessageType.MODEL_LOADED, OnModelLoaded);
		m_dicMessageTreat.Add(eMessageType.MODEL_LOADED_DUMMY, OnModelLoaded_Dummy);
	}
	
	#region - member -
	#endregion
	
	#region - fsm function -
	public override void Enter(AsIMessage _msg)
	{
		m_OpenMsg = _msg as Msg_OpenPrivateShop;
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_MoveStopIndication());
		
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.SHOP_OPENING, true);
		m_OwnerFsm.EnterPStore();
		
		m_OwnerFsm.Entity.HandleMessage(new Msg_ModelChange());
		
		m_OwnerFsm.Entity.transform.eulerAngles = new Vector3(0, 180, 0);
		
		Debug.Log("AsOtherUserState_PrivateShop::Enter: enter private shop state");
		
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
			
			Debug.Log("AsOtherUserState_PrivateShop::Exit: title object destroyed normally");
		}
		
		m_OwnerFsm.ReleaseElements();
	}
	#endregion
	
	#region - msg -	
	void OnOpenPrivateShop(AsIMessage _msg)
	{
		m_OpenMsg = _msg as Msg_OpenPrivateShop;
		
		if(m_OwnerFsm.Entity.CheckModelLoadingState() == eModelLoadingState.Finished)
			OnModelLoaded(_msg);
	}
	
	void OnClosePrivateShop(AsIMessage _msg)
	{
		m_OwnerFsm.Entity.SetProperty(eComponentProperty.SHOP_OPENING, false);
		m_OwnerFsm.ExitPStore();
		
		if(m_OwnerFsm.Entity.CheckModelType() == eModelType.Shop)
			m_OwnerFsm.Entity.HandleMessage(new Msg_ModelChange());
		
//		m_OwnerFsm.Entity.InitComponents();
//		m_OwnerFsm.Entity.InterInitComponents();
//		m_OwnerFsm.Entity.LateInitComponents();
		
		m_OwnerFsm.SetOtherUserFsmState(AsOtherUserFsm.eOtherUserFsmStateType.IDLE, new Msg_PrevState_PrivateShop());
	}
	
	void OnModelLoaded(AsIMessage _msg)
	{
//		Debug.Log("AsOtherUserState_PrivateShop::OnModelLoaded: model is loaded");
		
		if(AsGameMain.s_gameState != GAME_STATE.STATE_INGAME)
		{
			Debug.Log("AsOtherUserState_PrivateShop::OnModelLoaded: game state is " + AsGameMain.s_gameState + ". shop name ui instantiating will be skip.");
			return;
		}
		
		if(m_Title != null)
		{
			GameObject.Destroy(m_Title.gameObject);
			m_Title = null;
			
			Debug.Log("AsOtherUserState_PrivateShop::OnModelLoaded: already instantiated is exist. destroy that.");
		}
		
		if(m_OpenMsg == null)
		{
			Debug.LogWarning("AsOtherUserState_PrivateShop::OnModelLoaded: m_OpenMsg = null. shop name ui instantiating will be skip");
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
			Debug.LogError("AsOtherUserState_PrivateShop:: Enter: Title<AsChatBalloonBase> is not found");
		else
		{			
//			m_Title.transform.parent = null;
			
			m_Title.transform.position = Vector3.zero;
			m_Title.transform.eulerAngles = Vector3.zero;
			m_Title.gameObject.SetActiveRecursively(true);
			m_Title.Owner = m_OwnerFsm.OtherUserEntity;
//			m_Title.SetText(m_OwnerFsm.OtherUserEntity.GetProperty<string>(eComponentProperty.NAME));
//			m_Title.SetContent(m_OwnerFsm.OtherUserEntity.shopContent);
			
			m_Title.SetTitleAndContent(m_OwnerFsm.OtherUserEntity.GetProperty<string>(eComponentProperty.NAME), m_OwnerFsm.OtherUserEntity.shopContent);
			
			Vector3 worldPos = m_OwnerFsm.Entity.ModelObject.transform.position;
			worldPos.y += m_OwnerFsm.Entity.characterController.height;
			Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
			Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
			vRes.y += 2.0f;
			vRes.z = 0.0f;
			m_Title.transform.position = vRes;
			
			Debug.Log("AsOtherUserState_PrivateShop::OnModelLoaded: title object shape updated");
		}
	}
	
	void OnModelLoaded_Dummy(AsIMessage _msg)
	{
		OnModelLoaded(_msg);
	}
	#endregion
}
