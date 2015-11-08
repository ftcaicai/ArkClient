using UnityEngine;
using System.Collections;

public class AsHudDelegate : MonoBehaviour
{
	public UIButton menuButton = null;
	public UIButton btnLevUp = null;
	public UIButton btnDie = null;
	public UIButton btnMobRespawn = null;
	public UIButton btnKillMob = null;
	public UIButton btnWarpTown = null;
	public UIButton btnWarpBoss = null;

	// Use this for initialization
	void Start()
	{
		menuButton.SetInputDelegate( MenuBtnDelegate);
//		btnLevUp.SetInputDelegate( LevUpBtnDelegate);
//		btnDie.SetInputDelegate( DieBtnDelegate);
//		btnMobRespawn.SetInputDelegate( MobRespawnBtnDelegate);
//		btnKillMob.SetInputDelegate( KillMobBtnDelegate);
		btnWarpTown.SetInputDelegate( WarpTownBtnDelegate);
		btnWarpBoss.SetInputDelegate( WarpBossBtnDelegate);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

//	public void HideShieldBtn( bool flag)
//	{
//		shieldButton.Hide( flag );
//	}
	
	private void MenuBtnDelegate( ref POINTER_INFO ptr )
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "S0006_EFF_Window_Open", Vector3.zero, false);
            QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.TAP_MENU_BTN));
			AsMenuBtnAction action = menuButton.GetComponent<AsMenuBtnAction>();
			action.Expand();
           
		}
	}
	
//	private void LevUpBtnDelegate( ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
//			
//			body_CS_CHEAT_LEVELUP levelup = new body_CS_CHEAT_LEVELUP();
//			byte[] data = levelup.ClassToPacketBytes();
//			AsNetworkMessageHandler.Instance.Send( data);
//		}
//	}
	
//	private void DieBtnDelegate( ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
//			
//			AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_Cheat_Death());
//			
//			body_CS_CHEAT_DEATH death = new body_CS_CHEAT_DEATH();
//			byte[] data = death.ClassToPacketBytes();
//			AsNetworkMessageHandler.Instance.Send( data);
//		}
//	}
	
//	private void MobRespawnBtnDelegate( ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
//			
//			body_CS_CHEAT_NPCALL_REGEN regen = new body_CS_CHEAT_NPCALL_REGEN();
//			byte[] data = regen.ClassToPacketBytes();
//			AsNetworkMessageHandler.Instance.Send( data);
//		}
//	}
	
//	private void KillMobBtnDelegate( ref POINTER_INFO ptr)
//	{
//		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
//		{
//			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
//			
//			body_CS_CHEAT_NPCALL_DELETE kill = new body_CS_CHEAT_NPCALL_DELETE();
//			byte[] data = kill.ClassToPacketBytes();
//			AsNetworkMessageHandler.Instance.Send( data);
//		}
//	}
	
	private void WarpTownBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
			
			AsNotify.Instance.MessageBox( "System Message", "Would you like to go to\nthe town?",
				this, "ToTownScene", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}
	
	private void ToTownScene()
	{
		body_CS_GOTO_TOWN town = new body_CS_GOTO_TOWN();
		byte[] data = town.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data);
	}
	
	private void WarpBossBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
			
			AsCommonSender.SendWarp(3);
		}
	}
	
	private void ToCharSelBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
			
			AsNotify.Instance.MessageBox( "System Message", "Would you like to go to\nthe character selection\nscene?",
				this, "ToCharacterSelectScene", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}
				
	private void ToCharacterSelectScene()
	{
		Debug.Log( "ToCharacterSelectScene");
		
		AS_CG_RETURN_CHARSELECT retCharSelect = new AS_CG_RETURN_CHARSELECT();
		byte[] data = retCharSelect.ClassToPacketBytes();
		AsNetworkMessageHandler.Instance.Send( data );
	}
	
	private void ToLoginBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "S0008_EFF_Button", Vector3.zero, false);
			
			AsNotify.Instance.MessageBox( "System Message", "Would you like to go to\nthe login scene?",
				this, "ToLoginScene", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		}
	}
	
	private void ToLoginScene()
	{
		Debug.Log( "ToLoginScene");
		
		AsEntityManager.Instance.RemoveAllEntities();

		Application.LoadLevel( "Login");
		DDOL_Tracer.BeginTrace();//$ yde
		Resources.UnloadUnusedAssets();
	}

//	private void ShieldBtnDelegate( ref POINTER_INFO ptr )
//	{
//		switch( ptr.evt)
//		{
//		case POINTER_INFO.INPUT_EVENT.PRESS:
//			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Skill_Shield_Start());
//			break;
//		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
//		case POINTER_INFO.INPUT_EVENT.TAP:
//			AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Skill_Shield_End());
//			break;
//		}
//	}
}
