using UnityEngine;
using System.Collections;

public class UIStorageSortButton : MonoBehaviour 
{
	public AsIconCooltime coolTime;		
	private float m_fMaxCoolTime = 0.0f;
	private float m_fRemainCoolTime = 0.0f;
	
	
	// cool time
	private void SetCoolTime( float fMaxValue, float fRemainCooltime )
	{
		if( null == coolTime ) 
		{
			Debug.LogError("ResetDataButton::SetCoolTime() [ null == coolTime");
			return;
		}
		
		if( true == coolTime.gameObject.active )
		{
			return;
		}
		else
		{
			coolTime.gameObject.active = true;
		}		
		
		m_fRemainCoolTime = fRemainCooltime;
		m_fMaxCoolTime = fMaxValue;
		
		ResetCoolTime();
	}
	
	
	private void ResetCoolTime()
	{
		if (0.0f > m_fRemainCoolTime)
		{
			m_fRemainCoolTime = 0.0f;
			coolTime.gameObject.active = false;			
		}

		coolTime.Value = (m_fMaxCoolTime - m_fRemainCoolTime) / m_fMaxCoolTime;	
	}
	
	// input
	
	public void GuiInputUp(Ray inputRay, int iPage)
	{ 	
//		if( true == AsHudDlgMgr.Instance.IsDontMoveState)
//		{
//			Debug.Log(" true == AsHudDlgMgr.Instance.IsDontMoveState ");
//			return;
//		}
		if( true == coolTime.gameObject.active )
		{
			Debug.Log(" true == coolTime.gameObject.active ");
			return;
		}
		
		if( null == collider )
		{
			Debug.LogError("UIStorageSortButton::GuiInputUp() [ null == collider ] " );
			return;
		}
		
//		if( true == collider.bounds.IntersectRay( inputRay ) )
		if( true == AsUtil.PtInCollider( collider, inputRay))
		{
			if(AsHudDlgMgr.Instance.IsOpenStorage == true &&
				AsHudDlgMgr.Instance.storageDlg.PageLocked == false &&
				AsHudDlgMgr.Instance.storageDlg.CheckPageOpened() == true )
			{
				AsCommonSender.SendStoragePageSort( iPage );
				SetCoolTime( 30f, 30f );
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6006_EFF_ItemSort", Vector3.zero, false);
			}
		}
	}
	
	// storage reset button
	public void Init()
	{
		if( null == coolTime ) 
		{
			Debug.LogError("ResetDataButton::Init() [ null == coolTime");
			return;
		}
		coolTime.gameObject.active = false;
	}
	
	
	// Use this for initialization
	void Start () 
	{	
		Init();
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		if( null == coolTime )
			return;
		
		if( false == coolTime.gameObject.active )
			return;
		
		m_fRemainCoolTime -= Time.deltaTime;
		ResetCoolTime();
	}
}
