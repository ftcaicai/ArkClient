using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AsPvpKillMessage : MonoBehaviour
{
	public SimpleSprite bgLeft;
	public SimpleSprite bgCenter;
	public SimpleSprite bgRight;
	
	public SimpleSprite[] classLeft;
	public SimpleSprite[] classRight;
	
	public SpriteText textLeft;
	public SpriteText textRight;
	
	public Color colorDeathName = Color.red;
	public Color colorAttackName = Color.blue;
	
	private Vector3 m_vPos = Vector3.zero;
	
	void Start()
	{
		m_vPos.y = 7.0f;
		transform.localPosition = m_vPos;
	}
	
	void Update()
	{
	}
	
	IEnumerator Remove()
	{
		yield return new WaitForSeconds( 5.0f);
		
		AsPvpManager.Instance.m_listKillMsg.Remove( this);
		Destroy( gameObject);
	}
	
	public void ShowKillMessage(int nCharUniqKey_Death, int nCharUniqKey_AttackUser)
	{
		ShowKillMessage( (uint)nCharUniqKey_Death, (uint)nCharUniqKey_AttackUser);
	}
	
	public void ShowKillMessage(uint nCharUniqKey_Death, uint nCharUniqKey_AttackUser)
	{
		int nMyTeam = AsPvpManager.Instance.GetMyTeamType();
		StringBuilder sb = new StringBuilder();
		int nClassLeft = 0;
		int nClassRight = 0;

		// name
		textLeft.Text = "";
		textRight.Text = "";
		foreach( KeyValuePair<uint, sARENAUSERINFO> pair in AsPvpManager.Instance.GetUserInfoList)
		{
			if( nCharUniqKey_AttackUser == pair.Value.nCharUniqKey)
			{
				sb.Remove( 0, sb.Length);
				
				if( nMyTeam == pair.Value.nTeamType)
					sb.Append( colorAttackName.ToString());
				else
					sb.Append( colorDeathName.ToString());
				
				sb.Append( pair.Value.szCharName);
				sb.Append( Color.white.ToString());
				sb.Append( AsTableManager.Instance.GetTbl_String( 2137));
				textLeft.Text = sb.ToString();
				
				nClassLeft = (int)pair.Value.eClass - 1;
			}
			
			if( nCharUniqKey_Death == pair.Value.nCharUniqKey)
			{
				sb.Remove( 0, sb.Length);
				
				if( nMyTeam == pair.Value.nTeamType)
					sb.Append( colorAttackName.ToString());
				else
					sb.Append( colorDeathName.ToString());
				
				sb.Append( pair.Value.szCharName);
				sb.Append( Color.white.ToString());
				
				if( 0 == nCharUniqKey_AttackUser)
					sb.Append( AsTableManager.Instance.GetTbl_String( 2139));
				else
					sb.Append( AsTableManager.Instance.GetTbl_String( 2138));
				
				textRight.Text = sb.ToString();
				
				nClassRight = (int)pair.Value.eClass - 1;
			}
		}
		
		// class all hide
		foreach( SimpleSprite img in classLeft)
			img.renderer.enabled = false;
		
		foreach( SimpleSprite img in classRight)
			img.renderer.enabled = false;

		// bg center
		if( 0 == nCharUniqKey_AttackUser)
			bgCenter.width = textRight.TotalWidth + classLeft[0].width;
		else
			bgCenter.width = textLeft.TotalWidth + textRight.TotalWidth + classLeft[0].width * 2.0f;
		
		// position setting
		float fBg = ( bgCenter.width * 0.5f);
		float fBg_L = ( bgLeft.width * 0.5f);
		float fBg_R = ( bgRight.width * 0.5f);
		float fClass = ( classLeft[0].width * 0.5f);
		float fText_L = ( textLeft.TotalWidth * 0.5f);
		float fText_R = ( textRight.TotalWidth * 0.5f);
		float x = 0.0f;
		Vector3 v = Vector3.zero;
		
		// bg left
		x = fBg + fBg_L;
		v = new Vector3( -x, 0, 0);
		bgLeft.transform.localPosition = v;
		
		// bg right
		x = fBg + fBg_R;
		v = new Vector3( x, 0, 0);
		bgRight.transform.localPosition = v;
		
		if( 0 != nCharUniqKey_AttackUser)
		{
			// class left
			x = fBg - fClass;
			v = new Vector3( -x, 0, 0);
			classLeft[nClassLeft].renderer.enabled = true;
			classLeft[nClassLeft].transform.localPosition = v;
			
			// name left
			x = fBg - fClass * 2 - fText_L;
			v = textLeft.transform.localPosition;
			v.x = -x;
			textLeft.transform.localPosition = v;
			
			// class right
			x = fBg - fClass * 2 - fText_L * 2 - fClass;
			v = new Vector3( -x, 0, 0);
			classRight[nClassRight].renderer.enabled = true;
			classRight[nClassRight].transform.localPosition = v;
			
			// name right
			x = fBg - fClass * 2 - fText_L * 2 - fClass * 2 - fText_R;
			v = textRight.transform.localPosition;
			v.x = -x;
			textRight.transform.localPosition = v;
		}
		else
		{
			// class right
			x = fBg - fClass;
			v = new Vector3( -x, 0, 0);
			classRight[nClassRight].renderer.enabled = true;
			classRight[nClassRight].transform.localPosition = v;
			
			// name right
			x = fBg - fClass * 2 - fText_R;
			v = textRight.transform.localPosition;
			v.x = -x;
			textRight.transform.localPosition = v;
		}
		
		StartCoroutine( Remove());
	}
	
	public void PositionUp()
	{
		m_vPos.y += bgCenter.height;
		transform.localPosition = m_vPos;
	}
}
