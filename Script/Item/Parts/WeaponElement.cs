using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WeaponElement : PartsElement
{      
    private Transform m_BoneTransform = null;
    private GameObject m_goWeapon = null;
	//private byte m_nStrengthenCount;
	private PartsRoot m_PartsRoot;
  
	
	public WeaponElement( Transform _transform, string strBoneName, PartsRoot _partsRoote )
	{
		m_eEquipType = Item.eEQUIP.Weapon;
		m_PartsRoot = _partsRoote;
		Create( _transform, strBoneName );
	}

    protected bool Create(Transform transform, string strBoneName)
    {
        if (null == transform)
        {
            Debug.LogError("WeaponRoot::WeaponRoot() [transform == null]");
            return false;
        }

        m_BoneTransform = ResourceLoad.SearchHierarchyTransform( transform, strBoneName );

        if (null == m_BoneTransform)
        {
            Debug.LogError("WeaponRoot::WeaponRoot() [m_BoneTransform == null]");
            return false;
        }

        return true;
    }
	
	public override void SetEmpty()
	{
		base.SetEmpty();
		
		if( null != m_goWeapon ) 
			GameObject.Destroy( m_goWeapon );		
	}
	

    public override bool SetPartsItem( int _nItemTableIdx, byte _nStrengthenCount, eGENDER _eGender, eCLASS eClass, PartsRoot _partsRoot )
    {
		if( false == base.SetPartsItem( _nItemTableIdx, _nStrengthenCount, _eGender, eClass, _partsRoot ) )
			return false;
		
		//m_nStrengthenCount = _nStrengthenCount;
		
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
		
		
		/*GameObject goPartsItem = null;
		if( eGENDER.eGENDER_MALE == _eGender )
		{
			goPartsItem = getItem.GetPartsItem_M();				
			if (null == goPartsItem)
	        {
	            Debug.LogError("PartsItem don't find [ name : " + getItem.ItemData.GetPartsItem_M() );            
	            return false;
	        }
		}
		else
		{
			goPartsItem = getItem.GetPartsItem_W();	
			
			if (null == goPartsItem)
	        {
	            Debug.LogError("PartsItem don't find [ name : " + getItem.ItemData.GetPartsItem_W() );            
	            return false;
	        }
		}       

       

        if (null != m_goWeapon)
            GameObject.Destroy(m_goWeapon);

        m_goWeapon = GameObject.Instantiate(goPartsItem, Vector3.zero, Quaternion.identity) as GameObject;	
		

        m_goWeapon.transform.parent = m_BoneTransform;
		m_goWeapon.transform.localPosition = Vector3.zero;
		m_goWeapon.transform.localRotation = Quaternion.identity;
		m_goWeapon.transform.localScale = Vector3.one;	
		
		if( null != m_PartsRoot )
		{
			_nStrengthenCount = m_PartsRoot.getStrengthenCount;
		}
		else	
		{
			Debug.LogError("WeaponElement::SetPartsItem()[ null != m_PartsRoot ]");
		}
		
		string strEffectName = GetString( eClass, _nStrengthenCount );	
		
		if( null != strEffectName && string.Empty != strEffectName )
		{
			float fScale = 1f;
			if( null != _partsRoot )
			{
				fScale = _partsRoot.transform.localScale.x;	
			}
			int id = AsEffectManager.Instance.PlayEffect( strEffectName, m_goWeapon.transform, true, 0f, fScale ); //dopamin #17440
			AsEffectManager.Instance.AddAttachEffectUser(id, _partsRoot.getOwnerUniqKey );
		}*/
		
		string path_model;
		string path_texture;
		
		if( eGENDER.eGENDER_MALE == _eGender )
		{
			path_model = getItem.ItemData.GetPartsItem_M();
			path_texture = getItem.ItemData.GetPartsItemDiff_M();
		}
		else
		{
			path_model = getItem.ItemData.GetPartsItem_W();
			path_texture = getItem.ItemData.GetPartsItemDiff_W();
		}
		
		SetGameObject( ResourceLoad.CreateGameObject( path_model ), ResourceLoad.Loadtexture(path_texture), eClass, _partsRoot );

        return true;
    }
	
	private string GetString( eCLASS eClass, byte _nStrengthenCount )
	{
		switch( eClass )
		{
		case eCLASS.DIVINEKNIGHT:
			if( 10 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengSwordHighFX";
			}
			else if( 7 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengSwordLowFX";
			}
			break;
			
		case eCLASS.MAGICIAN:
			if( 10 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengStaffHighFX";
			}
			else if( 7 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengStaffLowFX";
			}
			break;
			
		case eCLASS.CLERIC:
			if( 10 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengHammerHighFX";
			}
			else if( 7 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengHammerLowFX";
			}
			break;			
			
		case eCLASS.HUNTER:
			if( 10 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengBowHighFX";
			}
			else if( 7 <= _nStrengthenCount )
			{
				return "FX/Effect/Item/Weapon/Fx_Weapon_StrengBowLowFX";
			}
			break;			
		}
		
		return null;
	}
	
	
	public void SetGameObject( GameObject _obj, Texture _tex, eCLASS eClass, PartsRoot _partsRoot )
	{
		if( null == _obj  )
		{
			Debug.LogError("WeaponElement::SetWing() model null ");
			return;
		}	
		
		if (null != m_goWeapon)
            GameObject.Destroy(m_goWeapon);
		
		 m_goWeapon = _obj;	
		

        m_goWeapon.transform.parent = m_BoneTransform;
		m_goWeapon.transform.localPosition = Vector3.zero;
		m_goWeapon.transform.localRotation = Quaternion.identity;
		m_goWeapon.transform.localScale = Vector3.one;	
		
		
		if( null == _tex  )
		{		
			Debug.LogError("WeaponElement::SetWing() texture null ");
			return;
		}
		
		Renderer rens = m_goWeapon.GetComponentInChildren<Renderer>();
		if(null == rens )
		{
			Debug.LogError("GameObject Render == null ");
			return;
		}
		
//		Shader shader = AsShaderManager.Instance.Get( "newark_shader" );
//		Debug.Log( "Shader : " + shader);
//		rens.material = new Material( shader);
//		rens.material.SetTexture( "_maintex", _tex);
		rens.material = AsShaderManager.Instance.CreateMaterial( "newark_shader", _tex);
		
		byte _nStrengthenCount = 0;
		if( null != m_PartsRoot )
		{
			_nStrengthenCount = m_PartsRoot.getStrengthenCount;
		}
		else	
		{
			Debug.LogError("WeaponElement::SetGameObject()[ null != m_PartsRoot ]");
		}
		
		string strEffectName = GetString( eClass, _nStrengthenCount );	
		
		if( null != strEffectName && string.Empty != strEffectName )
		{
			float fScale = 1f;
			if( null != _partsRoot )
			{
				fScale = _partsRoot.transform.localScale.x;	
			}
			int id = AsEffectManager.Instance.PlayEffect( strEffectName, m_goWeapon.transform, true, 0f, fScale ); //dopamin #17440
			AsEffectManager.Instance.AddAttachEffectUser(id, _partsRoot.getOwnerUniqKey );
		}
	}
}
