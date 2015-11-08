using UnityEngine;
using System.Collections;

public class UIPetPerform : MonoBehaviour {
	
	[SerializeField] UIButton btn_Closing;
	[SerializeField] UIPetImg slot1;
	[SerializeField] SpriteText text_Index;
	
	void Awake()
	{
	}
	
	void Start()
	{
		btn_Closing.SetInputDelegate( Dlt_Close);
	}
	
	public void Open( PetInfo _info)
	{
		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord( _info.nPetTableIdx);
		if( petRec != null)
		{
			Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(_info.nPersonality);
			string personality = AsTableManager.Instance.GetTbl_String(scriptRec.PersonName);
			string name = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( _info.szPetName));
			
			string content = AsTableManager.Instance.GetTbl_String( 2218);
			content = string.Format( content, personality, name);
			text_Index.Text = content;
			
			slot1.SetSlotImg( petRec.Icon);
		}
		else
		{
			Debug.LogError( "UIPetPerform:: Open: there is no pet record. id = " + _info.nPetTableIdx);
			Destroy( gameObject);
		}
	}

	public void Open(body_SC_PET_CREATE_RESULT _create)
	{
		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord(_create.nPetTableIdx);
		if( petRec != null)
		{
			Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(_create.nPetPersonality);
			string personality = AsTableManager.Instance.GetTbl_String(scriptRec.PersonName);
			string name = petRec.Name;
			
			string content = AsTableManager.Instance.GetTbl_String( 2218);
			content = string.Format( content, personality, name);
			text_Index.Text = content;
			
			slot1.SetSlotImg( petRec.Icon);
		}
		else
		{
			Debug.LogError( "UIPetPerform:: Open: there is no pet record. id = " + _create.nPetTableIdx);
			Destroy( gameObject);
		}
	}

	public void Open(body_SC_PET_UPGRADE_RESULT _upgrade)
	{
		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord(_upgrade.nTableIdx);
		if( petRec != null)
		{
//			Tbl_PetScript_Record scriptRec = AsTableManager.Instance.GetPetScriptRecord(_upgrade.nPersonality);
//			string personality = AsTableManager.Instance.GetTbl_String(scriptRec.PersonName);
			string name = AsUtil.GetRealString(System.Text.UTF8Encoding.UTF8.GetString( _upgrade.szPetName));
			
			string content = AsTableManager.Instance.GetTbl_String( 2220);
			content = string.Format( content, name);
			text_Index.Text = content;
			
			slot1.SetSlotImg( petRec.Icon);
		}
		else
		{
			Debug.LogError( "UIPetPerform:: Open: there is no pet record. id = " + _upgrade.nTableIdx);
			Destroy( gameObject);
		}
	}

	public void Open( int _idx, string _content, int _lv)
	{
		Tbl_Pet_Record petRec = AsTableManager.Instance.GetPetRecord( _idx);
		if( petRec != null)
		{
			text_Index.Text = _content;
			
			slot1.SetSlotImg( petRec.Icon);
		}
		else
		{
			Debug.LogError( "UIPetPerform:: Open: there is no pet record. id = " + _idx);
			Destroy( gameObject);
		}
	}
	#region - delegate -
	void Dlt_Close(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
			
			Destroy( gameObject);
		}
	}
	#endregion
}
