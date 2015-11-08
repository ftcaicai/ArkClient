using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPortalTrigger : MonoBehaviour
{
	public int PortalIndex = 0;
	private SpriteText textPortalName;
	public float height = 5f;
	private Vector3 m_vUIPosRevision = Vector3.zero;
	//$yde
	static List<AsPortalTrigger> m_listPortal = new List<AsPortalTrigger>();
	public static List<AsPortalTrigger> listPortal	{ get { return m_listPortal; } }

	//$yde
	void Awake()
	{
		m_listPortal.Add( this);
	}

	// Use this for initialization
	void Start()
	{
		SetPotalNameShow( PortalIndex);
	}

	public void SetPotalNameShow( int _iPotalIdx)
	{
		Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataRecord( _iPotalIdx);
		if( null == record)
			return;

		//if( record.getWarpMapId != TerrainMgr.Instance.GetCurMapID())
		//	return;

		if( false == record.IsNameExist())
			return;

		if( null == textPortalName)
		{
			GameObject temp = ResourceLoad.CreateGameObject( "UI/AsGUI/GUI_PortalNameShow");
			if( null == temp)
				return;

			textPortalName = temp.GetComponent<SpriteText>();
			if( null == textPortalName)
			{
				GameObject.DestroyObject( temp);
				return;
			}

			textPortalName.transform.parent = transform;
			//textPortalName.transform.localPosition = Vector3.zero;
			//textPortalName.transform.localRotation = Quaternion.identity;
			//textPortalName.transform.localScale = Vector3.one;

			textPortalName.Text = record.GetName();
			UpdateNamePosition();
		}
	}

	private void UpdateNamePosition()
	{
		if( null == textPortalName)
			return;

		m_vUIPosRevision.x = 0.0f;
		m_vUIPosRevision.y = textPortalName.BaseHeight + height;
		m_vUIPosRevision.z = 15f;
		textPortalName.transform.position = _WorldToUIPoint( transform.position, m_vUIPosRevision);
	}

	private Vector3 _WorldToUIPoint( Vector3 vWorldPos, Vector3 vUIPosRevision)
	{
		Vector3 vScreenPos = _WorldToScreenPoint( vWorldPos);
		Vector3 vRes = _ScreenPointToUIRay( vScreenPos, vUIPosRevision);
		return vRes;
	}

	private Vector3 _WorldToScreenPoint( Vector3 vWorldPos)
	{
		return CameraMgr.Instance.WorldToScreenPoint( vWorldPos);
	}

	private Vector3 _ScreenPointToUIRay( Vector3 vScreenPos, Vector3 vUIPosRevision)
	{
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		vRes.x += vUIPosRevision.x;
		vRes.y += vUIPosRevision.y;
		vRes.z = vUIPosRevision.z;
		return vRes;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateNamePosition();
	}

	//$yde
	void OnDestroy()
	{
		m_listPortal.Remove( this);
	}

	public void OnTriggerEnter( Collider collider)
	{
		if( null == collider || null == collider.gameObject)
			return;

		Tbl_WarpData_Record record = AsTableManager.Instance.GetWarpDataRecord( PortalIndex);
		if( null == record)
			return;

		if( false == record.isActive)
		{
			if( false == AsHudDlgMgr.Instance.isOpenMsgBox)
				AsChatManager.Instance.InsertChat( AsTableManager.Instance.GetTbl_String( 873), eCHATTYPE.eCHATTYPE_SYSTEM);

			return;
		}

		AsUserEntity userEntity = AsEntityManager.Instance.GetEntityByInstanceId( collider.gameObject.GetInstanceID()) as AsUserEntity;
		if( null == userEntity)
			return;

		if( eFsmType.PLAYER != userEntity.FsmType)
			return;
		
		if( true == AsCommonSender.isSendWarp)
		{
			Debug.Log( "true == AsCommonSender.isSendWarp");
			return;
		}

		AsHudDlgMgr.Instance.CollapseMenuBtn();	// #10694

		AsCommonSender.SendWarp( PortalIndex);

		#region - input -
		if( AsCommonSender.isSendGuild == false && AsCommonSender.isSendPostList == false)
			AsInputManager.Instance.m_Activate = false;

		AsInputManager.Instance.Warped();
		#endregion
		#region - chat -
		AsEmotionManager.Instance.CloseEmoticonPanel();
		AsEmotionManager.Instance.BlockPanel();
		#endregion
		#region - auto combat -
		AutoCombatManager.Instance.ZoneWarped();
		#endregion
	}

	public static float ClosestPortalDistance()
	{
		float dist = float.MaxValue;
		Vector3 pos = AsUserInfo.Instance.GetCurrentUserEntity().transform.position;

		foreach( AsPortalTrigger portal in m_listPortal)
		{
			float curDist = Vector3.Distance( portal.transform.position, pos);
			if( curDist < dist)
				dist = curDist;
		}

		return dist;
	}
}
