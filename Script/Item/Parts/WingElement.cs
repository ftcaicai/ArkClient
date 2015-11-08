using UnityEngine;
using System.Collections;

public class WingElement : PartsElement
{      
    private Transform m_BoneTransform = null;
    private GameObject m_goWeapon = null;
	
	public Transform getBoneTransform
	{
		get
		{
			return m_BoneTransform;
		}
	}
  
	
	public WingElement( Transform _transform, string strBoneName )
	{
		m_eEquipType = Item.eEQUIP.Wing;
		Create( _transform, strBoneName );
	}

    protected bool Create(Transform transform, string strBoneName)
    {
        if (null == transform)
        {
            Debug.LogError("WingElement::WingElement() [transform == null]");
            return false;
        }

        m_BoneTransform = ResourceLoad.SearchHierarchyTransform( transform, strBoneName );

        if (null == m_BoneTransform)
        {
            Debug.LogError("WingElement::WingElement() [m_BoneTransform == null]");
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
		m_goWeapon.transform.localScale = Vector3.one;			*/
		
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
		
		SetGameObject( ResourceLoad.CreateGameObject( path_model ), ResourceLoad.Loadtexture(path_texture) );

        return true;
    }
	
	public void SetGameObject( GameObject _obj, Texture _tex )
	{
		if( null == _obj  )
		{
			Debug.LogError("WingElement::SetWing() model null ");
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
			Debug.LogError("WingElement::SetWing() texture null ");
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
	}
}
