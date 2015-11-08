using UnityEngine;
using System.Collections;

public class AsPromotionDlg : MonoBehaviour {
	
	public delegate void CancelClicked();
	
	[SerializeField] SimpleSprite m_spPromotion;
	
	[SerializeField] UIButton m_btnBuy;
	[SerializeField] SpriteText m_textBuy;
	
	[SerializeField] SimpleSprite m_spIndex;
	[SerializeField] SpriteText m_textIndex;
	
	[SerializeField] SimpleSprite m_spSlot;
	[SerializeField] SimpleSprite m_spIcon;
	
	[SerializeField] SpriteText m_textTitle;
	
	float m_FadeInTime = 1f;
	float m_PromotionTime = 3f;
	float m_FadeOutTime = 1f;
	
	Tbl_Promotion_Record m_Record = null;
	
	enum eDlgState {NONE, FadeIn, Normal, FadeOut}
	eDlgState m_State = eDlgState.NONE;

	// Use this for initialization
	void Start () {
		
		Open_FadeIn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Init(Tbl_Promotion_Record _record)
	{
		m_Record = _record;
		
		m_FadeInTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(101).Value * 0.001f;
		m_PromotionTime = _record.PromotionTime * 0.001f;
		m_FadeOutTime = AsTableManager.Instance.GetTbl_GlobalWeight_Record(102).Value * 0.001f;
		
		Item item = ItemMgr.ItemManagement.GetItem(_record.Item_Index);
		
		// icon
		GameObject obj = Instantiate(item.GetIcon()) as GameObject;
		UISlotItem slot = obj.GetComponent<UISlotItem>();
		SimpleSprite sprite = slot.iconImg;
		sprite.transform.parent = m_spSlot.transform;
		sprite.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		Destroy(m_spIcon.gameObject);
//		sprite.transform.position = m_spIcon.transform.position;
//		sprite.transform.rotation = m_spIcon.transform.rotation;
//		Destroy(m_spIcon.gameObject);
//		m_spIcon.gameObject.SetActiveRecursively(false);
//		sprite.transform.parent = m_spIcon.transform;
//		sprite.transform.position = Vector3.zero;
//		sprite.transform.rotation = Quaternion.identity;
		m_spIcon = sprite;
		
		string name = AsTableManager.Instance.GetTbl_String(item.ItemData.nameId);
		string str = AsTableManager.Instance.GetTbl_String(_record.String_Index);
		string buttonStr = AsTableManager.Instance.GetTbl_String(1769);
		
		m_textTitle.Text = name;
		m_textIndex.Text = str;		
		m_btnBuy.Text = buttonStr;
		
		// alpha
		SetAlpha(0f);
		
		m_btnBuy.SetInputDelegate(OnBuyBtnClicked_Del);
		m_spPromotion.GetComponent<AsPromotionBtn_Cancel>().SetInput_Del(OnCancelClicked_Del);
	}
	
	void Open_FadeIn()
	{
		if(m_State != eDlgState.FadeIn)
		{
			m_State = eDlgState.FadeIn;
			
			StopAllCoroutines();
			StartCoroutine(FadeInProcess());
		}
	}
	
	IEnumerator FadeInProcess()
	{
		float alpha = 0f;
		
		while(true)
		{
			alpha += Time.deltaTime / m_FadeInTime;
			
			SetAlpha(alpha);
			
			if(alpha >= 1f)
				break;
			
			yield return null;
		}
		
		OpenCompletion();
	}
	
	void OpenCompletion()
	{
		if(m_State != eDlgState.Normal)
		{
			m_State = eDlgState.Normal;
			
			StopAllCoroutines();
			StartCoroutine(OpenCompletionProcess());
		}
	}
	
	IEnumerator OpenCompletionProcess()
	{
		StartCoroutine(BlinkButtonProcess());
		
		yield return new WaitForSeconds(m_PromotionTime);
		
		Close_FadeOut();
	}
	
	IEnumerator BlinkButtonProcess()
	{
		Color origColor = m_textBuy.Color;
		string str = m_textBuy.Text;
		
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			
			m_textBuy.Text = Color.gray + str;
			
			yield return new WaitForSeconds(0.5f);
			
			m_textBuy.Text = origColor + str;
		}
	}
	
	void Close_FadeOut()
	{
		if(m_State != eDlgState.FadeOut)
		{
			m_State = eDlgState.FadeOut;
			
			StopAllCoroutines();
			StartCoroutine(FadeOutProcess());
		}
	}
	
	IEnumerator FadeOutProcess()
	{
		float alpha = 1f;
		
		while(true)
		{
			alpha -= Time.deltaTime / m_FadeOutTime;
			
			SetAlpha(alpha);
			
			if(alpha <= 0f)
				break;
			
			yield return null;
		}
		
		Destroy(gameObject);
	}
	
	void SetAlpha(float _alpha)
	{
		Color fadeColor = new Color(1f, 1f, 1f, _alpha);
			
		m_spPromotion.renderer.material.color = fadeColor;
		
		m_btnBuy.renderer.material.color = fadeColor;
		m_textBuy.renderer.material.color = fadeColor;
		
		m_spIndex.renderer.material.color = fadeColor;
		m_textIndex.renderer.material.color = fadeColor;
		
		m_spSlot.renderer.material.color = fadeColor;
		m_spIcon.renderer.material.color = fadeColor;
		
		m_textTitle.renderer.material.color = fadeColor;
	}
	
	void OnBuyBtnClicked_Del(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Debug.Log("AsPromotionDlg::OnBuyBtnClicked_Del: button clicked. m_Record.Miracle_Page " + m_Record.Miracle_Page);
			
			if( AsPStoreManager.Instance.UnableActionByPStore() == true)
			{
				AsHudDlgMgr.Instance.SetMsgBox( AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(365),
					null, "", AsNotify.MSG_BOX_TYPE.MBT_OK, AsNotify.MSG_BOX_ICON.MBI_NOTICE));
				return;
			}
			
			m_spPromotion.collider.enabled = false;
			m_btnBuy.collider.enabled = false;
			
			Close_FadeOut();
			
			//AsCashStore.CreateCashStore(m_Record.Miracle_Page, 0);
			AsHudDlgMgr.Instance.OpenCashStore(0, AsEntityManager.Instance.UserEntity.GetProperty<eCLASS>(eComponentProperty.CLASS), m_Record.Miracle_Page, m_Record.Sub_Category, 0);
			
//			switch(m_Record.Miracle_Page)
//			{
//			case eCashStoreMenuMode.MAIN:
//				break;
//			case eCashStoreMenuMode.CHARGE_MIRACLE:
//				break;
//			case eCashStoreMenuMode.CHARGE_GOLD:
//				break;
//			case eCashStoreMenuMode.COSTUME:
//				break;
//			case eCashStoreMenuMode.USEITEM:
//				break;
//			case eCashStoreMenuMode.BOOSTER:
//				break;
//			case eCashStoreMenuMode.JEWEL:
//				break;
//			case eCashStoreMenuMode.RANDOM:
//				break;
//			case eCashStoreMenuMode.PACKAGE:
//				break;
//			case eCashStoreMenuMode.ETC:
//				break;
//			}
		}
	}
	
	void OnCancelClicked_Del()
	{
		m_spPromotion.collider.enabled = false;
		m_btnBuy.collider.enabled = false;
		
		Close_FadeOut();
	}
}
