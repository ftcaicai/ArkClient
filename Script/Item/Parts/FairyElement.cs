using UnityEngine;
using System.Collections;

public class FairyElement : PartsElement
{  
	//private Transform m_BoneTransform = null;  
	int m_EffectIdx = -1;
	
	
	public int getEffectIdx
	{
		get
		{
			return m_EffectIdx;
		}
	}
	
	public void SetEffectIdx( int iIdx )
	{
		m_EffectIdx = iIdx; 
	}
	
	public FairyElement()
	{
		m_eEquipType = Item.eEQUIP.Fairy;
		
	}   
	
	public override void SetEmpty()
	{
		base.SetEmpty();
		
		if( -1 != m_EffectIdx )
		{
			AsEffectManager.Instance.RemoveEffectEntity( m_EffectIdx );
		}
	}
	

    public override bool SetPartsItem( int _nItemTableIdx, byte _nStrengthenCount, eGENDER _eGender, eCLASS eClass, PartsRoot _partsRoot )
    {
		if( false == base.SetPartsItem( _nItemTableIdx, _nStrengthenCount, _eGender, eClass, _partsRoot ) )
			return false;
		
		if( null != AssetbundleManager.Instance && true == AssetbundleManager.Instance.useAssetbundle)
		{
			SetNeedResetParts(true);
			return true;
		}
		
		
		if( false == (getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || getItem.ItemData.getGender == _eGender) )
		{
			Debug.Log(" WeaponElement::SetPartsItem() [getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || getItem.ItemData.getGender == _eGender]");
			return false;
		}
		
		
		
		string strPartItem = string.Empty;
		if( eGENDER.eGENDER_MALE == _eGender )
		{
			strPartItem = getItem.ItemData.GetPartsItem_M();
			if (null == strPartItem)
	        {
	            Debug.LogError("PartsItem don't find [ null == strPartItem ] " );            
	            return false;
	        }
		}
		else
		{
			strPartItem = getItem.ItemData.GetPartsItem_W();			
			if (null == strPartItem)
	        {
	            Debug.LogError("PartsItem don't find [ null == strPartItem ] ");            
	            return false;
	        }
		}       
		
		if( -1 != m_EffectIdx )
		{
			AsEffectManager.Instance.RemoveEffectEntity( m_EffectIdx );
		}
		
		float fScale = 1f;
		if( null == _partsRoot )
			return false;		
		
		fScale = _partsRoot.transform.localScale.x;	
       	m_EffectIdx = AsEffectManager.Instance.PlayEffect( strPartItem, _partsRoot.transform, true, 0f, fScale);
		AsEffectManager.Instance.AddAttachEffectUser(m_EffectIdx, _partsRoot.getOwnerUniqKey );
		
		if( -1 == m_EffectIdx )
			return false;
		return true;
    }
}
