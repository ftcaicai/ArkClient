
#define _USER_SHADOW_
#define _NPC_SHADOW_ 
#define _USE_TEXTURE_MANAGER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropItemManagement
{
	private Dictionary<int, DropItemAction> m_DropItemList = new Dictionary<int, DropItemAction>();
	private GameObject m_ParentObject;	
	static public bool IsStarted = false;
	
	private List<int> m_ReceiveList= new List<int>();
	private List<int> m_NeedDeleteList= new List<int>();
	private List<AS_body2_SC_DROPITEM_APPEAR> receiveItemList = new List<AS_body2_SC_DROPITEM_APPEAR>();
	
	
	public void ReceiveInsertDropItems( AS_body1_SC_DROPITEM_APPEAR result) 
	{
		foreach( AS_body2_SC_DROPITEM_APPEAR data in result.body)
		{
			Debug.Log( "owner: " + data.nOwner + "idx: " + data.nDropItemIdx + "userid :" + AsEntityManager.Instance.UserEntity.UniqueId );
			
			if( 0 == data.nOwner || 
				( ( 0!=data.nOwner) && ( AsEntityManager.Instance.UserEntity.UniqueId == data.nOwner)) ||
				( 0!=AsPartyManager.Instance.PartyIdx && data.nPartyIdx == AsPartyManager.Instance.PartyIdx))
			{
				Debug.Log( "droped owner: " + data.nOwner + "idx: " + data.nDropItemIdx + "userid :" + AsEntityManager.Instance.UserEntity.UniqueId );
				Item item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx);
				if( null == item)
				{
					Debug.LogError( "DropItemAppear [Item id == null]. item id : " + data.nItemTableIdx);
					continue;
				}
				
				if( false == IsExistReceviedDrop( data.nDropItemIdx))
				{
					m_ReceiveList.Add( data.nDropItemIdx);
					receiveItemList.Add( data);
				}
				else
				{
					Debug.LogError( "DropItemAppear()[false != IsExistReceviedDrop] idx " + data.nDropItemIdx);
				}
			}
		}

#if UNITY_EDITOR
		if( ( null == AssetbundleManager.Instance) || ( true != AssetbundleManager.Instance.useAssetbundle))
		{
			foreach( AS_body2_SC_DROPITEM_APPEAR _data in receiveItemList)
			{
				InsertDropItem( _data);
			}
			
			receiveItemList.Clear();
		}
#endif
	}
	
	public IEnumerator Visualize()
	{
		IsStarted = true;
		
		while( true)
		{
			if( 0 >= receiveItemList.Count)
			{
				IsStarted = false;
				yield break;
			}
			
			AS_body2_SC_DROPITEM_APPEAR data = receiveItemList[0];
			
			if( true == IsExistNeedDeleteDrop( data.nDropItemIdx))
			{
				receiveItemList.RemoveAt( 0);
				RemoveNeedDeleteList( data.nDropItemIdx);
				continue;
			}
			
			if( true == IsExistDropItem( data.nDropItemIdx))
			{
				Debug.LogError( "InsertDropItems() IsExistDropItem[ idx : " + data.nDropItemIdx);
				receiveItemList.RemoveAt( 0);
				
				if( true == IsExistReceviedDrop( data.nDropItemIdx))
					RemoveReceiveList( data.nDropItemIdx);
				continue;
			}
			
			Item item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx);
			if( null == item)
			{
				receiveItemList.RemoveAt( 0);
				if( IsExistReceviedDrop( data.nDropItemIdx))
				{
					RemoveReceiveList( data.nDropItemIdx);
					RemoveNeedDeleteList( data.nDropItemIdx);
				}
				Debug.LogError( "DropItemManagement::InsertDropItem [Item id == null]. item id : " + data.nItemTableIdx);
				continue;
			}
			
			string path_model = item.ItemData.GetDropItem();
				
			// model
			string[] strSplit = path_model.Split( '/');
			string strLower_model = strSplit[strSplit.Length-1].ToLower();
			string strAssetbundlePath_model = AssetbundleManager.Instance.GetAssetbundleDownloadPath( strLower_model);
			int nVersion = AssetbundleManager.Instance.GetCachedVersion( strLower_model);
			
			if( false == AssetbundleManager.Instance.IsCachedVersion( strLower_model))
			{
				receiveItemList.RemoveAt( 0);
				Debug.LogError( "InsertDropItems() strLower_model not exist str: " + strLower_model);
				if( IsExistReceviedDrop( data.nDropItemIdx))
				{
					RemoveReceiveList( data.nDropItemIdx);
					RemoveNeedDeleteList( data.nDropItemIdx);
				}
				continue;
			}
	
			Debug.Log( "Model path : " + strAssetbundlePath_model);
			WWW www = WWW.LoadFromCacheOrDownload( strAssetbundlePath_model, nVersion);
			yield return www;

			AssetBundle bundle = www.assetBundle;
			AssetBundleRequest request = bundle.LoadAsync( strLower_model, typeof( GameObject));
			yield return request;
			
			GameObject obj = request.asset as GameObject;
			if( null == obj)
			{
				Debug.LogError( "PartsRoot::LoadParts: There is no [" + strLower_model + "] modeling resource.");
				obj = GameObject.CreatePrimitive( PrimitiveType.Sphere);
			}
			
			GameObject goObject = GameObject.Instantiate( obj) as GameObject;
			bundle.Unload( false);
			
			AssetbundleHelper helper = goObject.GetComponent<AssetbundleHelper>();
			if( null == helper)
			{
				receiveItemList.RemoveAt( 0);
				Debug.Log( "Not Found Assetbundle Helper: " + strLower_model);
				if( IsExistReceviedDrop( data.nDropItemIdx))
				{
					RemoveReceiveList( data.nDropItemIdx);
					RemoveNeedDeleteList( data.nDropItemIdx);
				}
				continue;
			}
			
			Renderer[] rens = goObject.GetComponentsInChildren<Renderer>();
			foreach( Renderer ren in rens)
			{
				int nIndex = helper.m_listMeshName.IndexOf( ren.name.ToLower());
				if( 0 > nIndex)
				{
					Debug.Log( "InitialNpcCharacter() NoMatchRenderMeshName: " + goObject.name + ", " + ren.name);
					continue;
				}
				
				if( nIndex > helper.m_listShaderName.Count - 1)
				{
					Debug.Log( "InitialNpcCharacter() index: " + nIndex + ", shader count: " + helper.m_listShaderName.Count);
					continue;
				}
				
				if( nIndex > helper.m_listTextureName.Count - 1)
				{
					Debug.Log( "InitialNpcCharacter() index: " + nIndex + ", texture count: " + helper.m_listTextureName.Count);
					continue;
				}
				
				string strShaderName = helper.m_listShaderName[nIndex];
				string sgtrNameLower = helper.m_listTextureName[nIndex].ToLower();
				string strAssetbundlePath = AssetbundleManager.Instance.GetAssetbundleDownloadPath( sgtrNameLower);
				nVersion = AssetbundleManager.Instance.GetCachedVersion( sgtrNameLower);
				
				Debug.Log( "Texture path : " + strAssetbundlePath);
				
	#if _USE_TEXTURE_MANAGER
				Texture tempTex = AsCharacterTextureManager.Instance.Get( sgtrNameLower);
				if( null == tempTex)
				{
					www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
					yield return www;
					
					bundle = www.assetBundle;
					tempTex = bundle.Load( sgtrNameLower) as Texture;
					
					bundle.Unload( false);
					
					AsCharacterTextureManager.Instance.Insert( sgtrNameLower, tempTex);
				}
	#else
				www = WWW.LoadFromCacheOrDownload( strAssetbundlePath, nVersion);
				yield return www;
				
				bundle = www.assetBundle;
				Texture tex = bundle.Load( sgtrNameLower) as Texture;
				
				Debug.Log( "Texture loaded: " + sgtrNameLower + ", " + tex);
				
				bundle.Unload( false);
	#endif
				
	#if _USE_TEXTURE_MANAGER
				ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tempTex);
	#else
				ren.material = AsShaderManager.Instance.CreateMaterial( strShaderName, tex);
	#endif
			}
			
			DropItemAction action = goObject.GetComponentInChildren<DropItemAction>() as DropItemAction;
			if( null == action)
			{
				Debug.LogError( " DropItemManagement::Create no have DropItemAction script ");
				action = goObject.AddComponent<DropItemAction>();
			}
			
			action.rootNode = goObject;
			
			if( null == m_ParentObject)
				m_ParentObject = new GameObject( "DropItem");
			
			Vector3 vec3Pos = data.sCurPosition;
			vec3Pos.y = TerrainMgr.GetTerrainHeight( vec3Pos);
			
			goObject.transform.parent = m_ParentObject.transform;
			goObject.transform.localPosition = vec3Pos;
			goObject.transform.localRotation = Quaternion.identity;
			goObject.transform.localScale = Vector3.one;
			
			ResourceLoad.SetLayerHierArchy( goObject.transform, LayerMask.NameToLayer( "Item"));
			
			action.Init( data);
			m_DropItemList.Add( data.nDropItemIdx, action);
			
			PlayDropSound( ItemMgr.ItemManagement.GetItem( data.nItemTableIdx));
			
			receiveItemList.RemoveAt( 0);
			
			yield return null;
		}
	}
	
	
	private bool IsExistReceviedDrop( int iDropIdx)
	{
		foreach( int _data in m_ReceiveList)
		{
			if( _data == iDropIdx)
				return true;
		}
		
		return false;
	}
	
	private bool IsExistNeedDeleteDrop( int iDropIdx)
	{
		foreach( int _data in m_NeedDeleteList)
		{
			if( _data == iDropIdx)
				return true;
		}
		
		return false;
	}
	
	public bool IsExistDropItem( int iDropID)
	{
		return m_DropItemList.ContainsKey( iDropID);
	}
	
	public void UpdateNeedDeleteList()
	{
		int i = 0;
		while( i < m_NeedDeleteList.Count)
		{
			if( true == IsExistDropItem( m_NeedDeleteList[i]))
			{
				RemoveDropItem( m_NeedDeleteList[i]);
				m_NeedDeleteList.RemoveAt(i);
				continue;
			}
			i++;
		}		
	}
	
	// Insert
	public void InsertDropItem( AS_body2_SC_DROPITEM_APPEAR data) 
	{
		if( null == data)
		{
			Debug.LogWarning( " Exist m_DropItemList[null == AS_body2_SC_DROPITEM_APPEAR]");
			return;
		}
		
		if( true == IsExistDropItem( data.nDropItemIdx))
		{
			Debug.LogWarning( " Exist m_DropItemList. iDropID : " + data.nDropItemIdx);
			return;
		}
		
		
		Item item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx);
		if( null == item)
		{
			Debug.LogError( "DropItemManagement::InsertDropItem [Item id == null]. item id : " + data.nItemTableIdx);
			return;
		}
		
		DropItemAction _action = CreateDropObject( data, item);
		if( null == _action)
		{
			Debug.LogError( "DropItemManagement::InsertDropItem [null == _action]. drop id : " + data.nDropItemIdx + " itemid : " + data.nItemTableIdx);
			return;
		}
		
		_action.Init( data);
		
		m_DropItemList.Add( data.nDropItemIdx, _action);
		
		PlayDropSound( ItemMgr.ItemManagement.GetItem( data.nItemTableIdx));
	}
	
	public void PlayDropSound( Item _item )	
	{
		if( null == _item)
		{
			Debug.LogError("UIIvenDlg::PlayDropSound() [ null == _item ]");
			return;
		}
		
		if(string.Compare("NONE", _item.ItemData.getStrDropSound, true) == 0)
		{
			Debug.LogError("UIInvenDlg::PlayDropSound()[getStrDropSound == NONE] id : " + _item.ItemID );
			return;
		}
		
		AsSoundManager.Instance.PlaySound( _item.ItemData.getStrDropSound, Vector3.zero, false  );
	}

	private DropItemAction CreateDropObject( AS_body2_SC_DROPITEM_APPEAR data, Item item)
	{
		if( null == item)
			return null;
		
		GameObject goDropItem = item.GetDropItem();
		
		if( null == goDropItem)
		{
			Debug.LogError( " DropItemManagement::Create [null == goDropItem]");
			return null;
		}
		
		GameObject goObject = GameObject.Instantiate( goDropItem, data.sCurPosition, Quaternion.identity) as GameObject;
		if( null == goObject)
		{
			Debug.LogError( "DropItemManagerMent::CreateDropObject()[ null == goObject ] item id : " + item.ItemID);
			return null;
		}
		
		DropItemAction action = goObject.GetComponentInChildren<DropItemAction>() as DropItemAction;
		if( null == action)
		{
			Debug.LogError( "DropItemManagement::Create no have DropItemAction script");
			GameObject.Destroy( goObject);
			return null;
		}
		
		action.rootNode = goObject;
		
		if( null == m_ParentObject)
			m_ParentObject = new GameObject( "DropItem");
		
		Vector3 vec3Pos = data.sCurPosition;
		vec3Pos.y = TerrainMgr.GetTerrainHeight( vec3Pos);
		
		goObject.transform.parent = m_ParentObject.transform;
		goObject.transform.localPosition = vec3Pos;
		goObject.transform.localRotation = Quaternion.identity;
		goObject.transform.localScale = Vector3.one;
		
		ResourceLoad.SetLayerHierArchy( goObject.transform, LayerMask.NameToLayer( "Item"));
		
		return action;
	}
	
	
	public void Clear()
	{
		AsCommonSender.isSendPickupDropItem = false;
		m_DropItemList.Clear();
		m_NeedDeleteList.Clear();
		m_ReceiveList.Clear();
		receiveItemList.Clear();
		IsStarted = false;
		
		if( null != m_ParentObject)
		{
			GameObject.Destroy( m_ParentObject);
			m_ParentObject = null;
		}
	}
	
	
	// remove
	public void ReceiveRemoveDropItem( int iDropItemId)
	{
		if( false == IsExistReceviedDrop( iDropItemId))
			return;
		
		if( false == IsExistDropItem( iDropItemId))
		{
			if( false == IsExistNeedDeleteDrop( iDropItemId))
				m_NeedDeleteList.Add( iDropItemId);
		}
		else
		{
			RemoveDropItem( iDropItemId);
		}
		
		
		RemoveReceiveList( iDropItemId);
	}
	
	private void RemoveReceiveList( int iDropItemId)
	{
		for( int i = 0; i < m_ReceiveList.Count; ++i)
		{
			if( m_ReceiveList[i] == iDropItemId)
			{
				m_ReceiveList.RemoveAt(i);
				break;
			}
		}
	}
	
	private void RemoveNeedDeleteList( int iDropItemId)
	{
		for( int i = 0; i < m_NeedDeleteList.Count; ++i)
		{
			if( m_NeedDeleteList[i] == iDropItemId)
			{
				m_NeedDeleteList.RemoveAt(i);
				break;
			}
		}
	}
	
	private void RemoveDropItem( int iDropItemID) 
	{
		
		if( false == IsExistDropItem( iDropItemID))
			return;
		
		if( null != m_DropItemList[iDropItemID])
		{
			if( null != m_DropItemList[iDropItemID].gameObject)
			{
				AS_body2_SC_DROPITEM_APPEAR data = m_DropItemList[iDropItemID].getDropItemData;
				Debug.Log( " remove owner: " + data.nOwner + "idx: " + data.nDropItemIdx );
				
				Item _item = ItemMgr.ItemManagement.GetItem( m_DropItemList[iDropItemID].getDropItemData.nItemTableIdx);
				ItemMgr.Instance.PlayGainEffect( _item, m_DropItemList[iDropItemID].gameObject.transform.position);
				
				PlayRemoveItemSound( m_DropItemList[iDropItemID].getDropItemData, iDropItemID);
				GameObject.Destroy( m_DropItemList[iDropItemID].rootNode); 
				m_DropItemList[iDropItemID] = null;				
			}
		}
		
		m_DropItemList.Remove( iDropItemID);
	}
	
	private void PlayRemoveItemSound( AS_body2_SC_DROPITEM_APPEAR data, int iDropItemID)
	{
		if( null == data)
			return;
		
		Item _item = ItemMgr.ItemManagement.GetItem( data.nItemTableIdx);
		if( null == _item)
			return ;
		
		Vector3 pos = m_DropItemList[iDropItemID].gameObject.transform.position;
		ItemMgr.Instance.PlayGainPlaySound( _item, pos);
	}
	
	// check collision 
	public void OnTriggerEnter( int iDropItemID)
	{
		AsCommonSender.SendPickupItem( iDropItemID);
	}
}
