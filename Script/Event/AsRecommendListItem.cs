using UnityEngine;
using System.Collections;
using System.Text;
public class AsRecommendListItem : MonoBehaviour
{
	public SpriteText m_TextCount = null;
	public SpriteText m_TextRewardItem = null;
	public UIButton m_BtnComplete = null;
	public UIButton m_BtnReward = null;
	private int m_Index;
	private int m_nCount;
	private Item m_Item;
/* Old Recommend	
   private body2_SC_RECOMMEND_INFO m_Data;
	
	void Init()
	{
		m_BtnReward.SetInputDelegate( RewardBtnDelegate);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnComplete.spriteText);//완료	
		m_BtnComplete.Text =  AsTableManager.Instance.GetTbl_String(1594);
		
		m_BtnComplete.SetControlState( UIButton.CONTROL_STATE.DISABLED);	
		m_BtnComplete.controlIsEnabled = false;	
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_BtnReward.spriteText);	//받기
		m_BtnReward.Text =  AsTableManager.Instance.GetTbl_String(1595);		
	}
	
	// Use this for initialization
	void Start () {
		
	
		
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnEnable()
	{
	
		if(m_Data == null) 	return;
		
		if( m_nCount >= m_Data.nRecommendCount)
		{			
			if( m_Data.bReceive )
			{
				m_BtnComplete.gameObject.SetActiveRecursively( true);
				m_BtnReward.gameObject.SetActiveRecursively( false);						
			}
			else
			{
				m_BtnComplete.gameObject.SetActiveRecursively( false);
				m_BtnReward.gameObject.SetActiveRecursively( true);		
			}
		}
		else
		{
			m_BtnComplete.gameObject.SetActiveRecursively( false);
			m_BtnReward.gameObject.SetActiveRecursively( false);
		}
	}
	
	public void SetData(body2_SC_RECOMMEND_INFO data, int count, int index)
	{
		Init();
		m_BtnComplete.gameObject.SetActiveRecursively( false);
		m_BtnReward.gameObject.SetActiveRecursively( false);	
		
		m_Data = data;
		m_TextCount.Text = data.nRecommendCount.ToString();
		m_nCount = count;
	    m_Index  = index;
			
		m_Item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx);
		if( null == m_Item)
			return;	
		
		m_TextRewardItem.Text = AsTableManager.Instance.GetTbl_String( m_Item.ItemData.nameId );
		if(data.nItemCount > 1)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(m_TextRewardItem.Text);
			sb.Append("(");
			sb.Append(data.nItemCount.ToString());
			sb.Append(")");
			m_TextRewardItem.Text = sb.ToString();
		//	m_TextRewardItem.Text = m_TextRewardItem.Text + "("+ data.nItemCount.ToString() + ")";
		}
		
		if( count >= data.nRecommendCount)
		{
			if( data.bReceive )
			{
				m_BtnComplete.gameObject.SetActiveRecursively( true);
				m_BtnReward.gameObject.SetActiveRecursively( false);						
			}
			else
			{
				m_BtnComplete.gameObject.SetActiveRecursively( false);
				m_BtnReward.gameObject.SetActiveRecursively( true);		
			}
		}
	}
		
	private void RewardBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{	
			Debug.Log("body_CS_RECOMMEND_RECEIVE :: RewardBtnDelegate");
			body_CS_RECOMMEND_RECEIVE packetData = new body_CS_RECOMMEND_RECEIVE( );
			packetData.nReceiveCount = m_Index;
     	    byte[] data = packetData.ClassToPacketBytes();
            AsNetworkMessageHandler.Instance.Send( data);
			
			m_BtnReward.SetControlState( UIButton.CONTROL_STATE.DISABLED);	
			m_BtnReward.controlIsEnabled = false;	
		
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		}
	}
	*/
}
