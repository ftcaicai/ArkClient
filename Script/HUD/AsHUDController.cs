
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsHUDController : MonoBehaviour
{
	public AsHUD_TargetInfo targetInfo = null;
	public AsNpcMenu m_NpcMenu = null;
	public AsUserMenu m_UserMenu = null;
	public AsPanelManager panelManager = null;
    public QuestListControll m_questList = null;
	public WorldMapDlg m_WorldMap = null;
	public TooltipMgr m_TooltipMgr = null;
	[SerializeField]private SimpleSprite fogPlane = null;
	
	private static AsHUDController ms_kIstance = null;
	public static AsHUDController Instance
	{
		get { return ms_kIstance; }
	}
	
	
	public static void SetActiveRecursively( bool bAcitve)
	{
		if( null == Instance)
		{
			Debug.LogError( "null == Instance");
			return;
		}
		
		Instance.gameObject.SetActiveRecursively( bAcitve);
		
		if( true == bAcitve)
			Instance.targetInfo.OnEnable();
		else
			Instance.targetInfo.OnDisable();
		
		if( null != AsChatFullPanel.Instance)
			AsChatFullPanel.Instance.Close();
		
		//#14635
		AsSocialManager.Instance.CloseAllSocailUI();

		if (AsHudDlgMgr.Instance != null)
			AsHudDlgMgr.Instance.CloseQuestMiniView();
	}
	
	
	void Awake()
	{
		ms_kIstance = this; 			
		gameObject.SetActiveRecursively( false);		
		DontDestroyOnLoad( gameObject);
		DDOL_Tracer.RegisterDDOL( this, gameObject);//$ yde
	}
	
	
	// Use this for initialization
	void Start()
	{
	}
	
	public void Init()
	{
		AsHUDController.SetActiveRecursively( true);
		targetInfo.mobBuffMgr.ClearBuff();	
		m_NpcMenu.Close();
		m_UserMenu.Close();
        m_questList.Visible = false;
        //m_questAcceptInfo.Close();
		//m_QuestBook.Close(true);

		
	}
	
	
	public bool UpdateTargetSimpleInfo( AsBaseEntity target)
	{
		return targetInfo.UpdateTargetSimpleInfo( target);
	}
	
	
	public void SetTargetUser()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);

		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( null == playerFsm)
			return;
		
		AsUserEntity target = playerFsm.Target as AsUserEntity;
		
		m_UserMenu.Open( target.SessionId);
	}
	
	public void SetTargetNpc( AsBaseEntity target)
	{		
		if( target == null)
		{
		 	m_NpcMenu.Close();
			AsHudDlgMgr.Instance.CloseStrengthenDlg();
			//targetInfo.mobBuffMgr.ClearBuff();	
		}
		else
		{
			if( eNPCType.NPC	== target.GetProperty<eNPCType>( eComponentProperty.NPC_TYPE))
			{
				if( true == AsHudDlgMgr.Instance.IsOpenTradeRequestMsgBox)
					return;
				
				if( AsHudDlgMgr.Instance != null)
				{
					AsHudDlgMgr.Instance.Init( false); // all close HudDlg	
					//targetInfo.mobBuffMgr.SetCharBuff( target);
				}

				m_NpcMenu.Open( target);
			}
		}
	}

    public void ShowNPCTalk( int _npcID)
    {
        AsNpcEntity npc = AsEntityManager.Instance.GetNPCEntityByTableID( _npcID);

        if( npc != null)
            AsEntityManager.Instance.UserEntity.HandleMessage( new Msg_NpcClick( npc.sessionId_));
        else
            Debug.LogWarning( "not Find npc = " + _npcID);
    }

    public void StartTutorial()
    {
        // tutorial 
        GameObject objTutorial = GameObject.Find( "Tutorial");
        if( objTutorial != null)
        {
            ShowNPCTalk( 100114);
            GameObject.DestroyImmediate( objTutorial);
            objTutorial = null;
        }
    }
	
	
	public void SetTargetInfo( AsBaseEntity target)
	{
//		targetInfo.Enable = false;
//		targetInfo.SetInfo( target);
		targetInfo.Enable = false;
		//m_NpcMenu.RemoveNpcSelectEffect();
		
		if( target == null)
		{
		 	m_NpcMenu.Close();	
			m_UserMenu.Close();
			
			AsPartyManager.Instance.SetTargetByPartyMember(0);
			if( null != targetInfo.mobBuffMgr)
				targetInfo.mobBuffMgr.ClearBuff();
			
			if( null != targetInfo.mobBuffMgr)
				targetInfo.mobBuffMgr.EmptyOtherUserFsm();
		}
		else
		{
			targetInfo.Enable = true;
			targetInfo.SetInfo( target);		
			
			if( null != targetInfo.mobBuffMgr)
				targetInfo.mobBuffMgr.ReciveBuff( target);
			
			if( true == m_UserMenu.isVisible && target.GetInstanceID() != m_UserMenu.TargetUserEntity.GetInstanceID())
				m_UserMenu.Close();
			
			//party_check
			AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
			if( null != playerFsm)
			{		
				AsUserEntity party_user = playerFsm.Target as AsUserEntity;
				if( null != party_user)
					AsPartyManager.Instance.SetTargetByPartyMember( party_user.UniqueId);
				else
					AsPartyManager.Instance.SetTargetByPartyMember(0);
			}
			else
			{
				AsPartyManager.Instance.SetTargetByPartyMember(0);
			}
			
			#region - target mark -
			targetInfo.ActivateTargetMarkBtn( AsPartyManager.Instance.IsCaptain);
			#endregion
		}
	}
	
	
	/*public void ReceiveAddNpcBuff( body1_SC_NPC_BUFF data, AsBaseEntity curEntity)
	{
		if( false ==  targetInfo.Enable)
			return;
		 
		if( null == targetInfo.getCurTargetEntity)
			return;
		
		if( null == curEntity)
			return;
		
		if( targetInfo.getCurTargetEntity.GetInstanceID() != curEntity.GetInstanceID())
			return;
		  
		AsNpcEntity entity = targetInfo.getCurTargetEntity as AsNpcEntity;
		if( null == entity)
			return;
		
		if( entity.SessionId != data.nNpcIdx)
		{
			Debug.LogError( " NPC id Different [ target npc Id : " + entity.SessionId + " server npc Id : " + data.nNpcIdx);
			return;  
		}
		
		targetInfo.mobBuffMgr.ReciveCharBuff( data);
	}
	
	
	public void ReceiveRemoveNpcBuff( body_SC_NPC_DEBUFF data, AsBaseEntity curEntity)
	{
		if( false ==  targetInfo.Enable)
			return;
		
		if( null == targetInfo.getCurTargetEntity)
			return;
		
		if( null == curEntity)
			return;
		
		if( targetInfo.getCurTargetEntity.GetInstanceID() != curEntity.GetInstanceID())
			return;
		
		AsNpcEntity entity = targetInfo.getCurTargetEntity as AsNpcEntity;
		if( null == entity)
			return;
		
		if( entity.SessionId != data.nNpcIdx)
			return;
		
		targetInfo.mobBuffMgr.DeleteCharBuff( data);		
	}*/
	
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void OnEnable()
	{
	}
	
	public void Hide( bool flag)
	{
		if( true == flag)
			transform.position = new Vector3( -1000.0f, 0.0f, 0.0f);
		else
			transform.position = new Vector3( -500.0f, 0.0f, 0.0f);
	}
	
	#region -Designation
	bool isRunCoroutine = false;
	public void PromptDesignationAlarm( int designationID)
	{
		if( false == isRunCoroutine)
			StartCoroutine( "PromptAlarm");
	}
	
	IEnumerator PromptAlarm()
	{
		int designationID = 0;
		
		isRunCoroutine = true;
		
		while( true)
		{
			if( 0 == AsDesignationManager.Instance.AlarmCount)
			{
				isRunCoroutine = false;
				break;
			}
			
			designationID = AsDesignationManager.Instance.GetAlarm(0);
			GameObject go = Instantiate( Resources.Load( "UI/Designation/DesignationAlarm")) as GameObject;
			AsDesignationAlarm alarm = go.GetComponent<AsDesignationAlarm>();
			alarm.SetDesignationText( designationID);
			
			yield return new WaitForSeconds( 5.0f);
			
			AsDesignationManager.Instance.RemoveAlarm(0);
		}
	}
	#endregion
	
	public void SetMapFogColor( Color color)
	{
		fogPlane.renderer.material.SetColor( "_Color", color);
	}
	
	#region - target mark -
	public void ActivateTargetMarkBtn( bool _active)
	{
		if( targetInfo.Enable == true)
			targetInfo.ActivateTargetMarkBtn( _active);
	}
	#endregion
}
