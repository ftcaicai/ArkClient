using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public  class MeshElement : PartsElement
{	
	private Material[] m_Materials;
	private Mesh m_resMesh;    
    private List<string> m_bones;// = new List<string>();
	
	public void SetMaterials( Material[] mat )
	{
		m_Materials = mat;
	}
	
	public void SetResMesh( Mesh _combine )
	{
		m_resMesh = _combine;
	}
	
	public void SetBones( List<string> bones )
	{
		m_bones = bones;
	}
    
	
	private int nItemTableIdx;
	private byte nStrengthenCount;	

	public MeshElement( Item.eEQUIP _equip, int _nItemTableIdx, byte _nStrengthenCount, eGENDER _eGender, eCLASS eClass )
	{			
		m_eEquipType = _equip;
		SetPartsItem( _nItemTableIdx, _nStrengthenCount, _eGender, eClass, null );
	}	
	
	// Get
	public Mesh GetResMesh()
	{
		return m_resMesh;
	}
	
	public Material[] GetMaterials()
	{
		return m_Materials;
	}
	
	public List<string> GetBones()
	{
		return m_bones;
	}
	
	
	// override
	public override void SetEmpty()
	{
		base.SetEmpty();		
		
		SetNeedResetParts(true);		
		m_Materials = null;			
		//m_bones.Clear();
	}
	
	
	public override bool SetPartsItem( int _nItemTableIdx, byte _nStrengthenCount, eGENDER _eGender, eCLASS eClass, PartsRoot _partsRoot )
	{
		if( false == base.SetPartsItem( _nItemTableIdx, _nStrengthenCount, _eGender, eClass, _partsRoot ) )
			return false;	
		
		SetNeedResetParts(true);		
		return true;
	}
	
	
	public override void CreateData( eGENDER eGender )
	{
		if( false == isNeedResetParts )
			return;
		
		if( false == isHavePartsItem )				
			return;
		
		
		if( false == (getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || getItem.ItemData.getGender == eGender ) )
		{
			Debug.Log(" MeshElement::CreateData() [getItem.ItemData.getGender == eGENDER.eGENDER_NOTHING || getItem.ItemData.getGender == eGender");
			return;
		}		
		
		GameObject goPartsItem = null;
		Texture texDiff = null;
		if( eGENDER.eGENDER_MALE == eGender )
		{
			goPartsItem = getItem.GetPartsItem_M();
			texDiff = getItem.GetPartsDiff_M();
		}
		else
		{
			goPartsItem = getItem.GetPartsItem_W();
			texDiff = getItem.GetPartsDiff_W();
		}

        if (null == goPartsItem)
		{
			Debug.LogError("PartsItem don't find [ item id : " + getItem.ItemData.GetID() );
			return;
		}
			
		goPartsItem = GameObject.Instantiate(goPartsItem) as GameObject;	
        SkinnedMeshRenderer smr = goPartsItem.GetComponentInChildren<SkinnedMeshRenderer>();
        if (null == smr)
        {
            Debug.LogError(" MeshElement::CreateData()[ null == smr] itemid : " + getItem.ItemID + " itemtype : " + (Item.eEQUIP)getItem.ItemData.GetSubType() );
			GameObject.Destroy(goPartsItem);
            return;
        }
		
		if( 0 >= smr.materials.Length )
		{
			Debug.LogError(" MeshElement::CreateData()[ smr.materials empty ] itemid : " + getItem.ItemID + " itemtype : " + (Item.eEQUIP)getItem.ItemData.GetSubType());
			return;
		}
		
		// material 	
        smr.materials[0].SetTexture("_maintex", texDiff);        
        m_Materials = smr.materials;
		
		// mesh		
		m_resMesh = smr.sharedMesh;
		
		
		
		// bones 
		m_bones = new List<string>();
		//m_bones.Clear();
		foreach( Transform tr in smr.bones )
		{
			m_bones.Add( tr.name );
		}
		
		GameObject.DestroyObject(goPartsItem);		
		SetNeedResetParts( false );
	}
     
}
