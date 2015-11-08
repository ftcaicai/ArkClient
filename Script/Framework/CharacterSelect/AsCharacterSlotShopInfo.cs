using UnityEngine;
using System.Collections;

public class AsCharacterSlotShopInfo : MonoBehaviour {
	
	public enum eState {Off, On, Expired}
	[SerializeField] eState m_State = eState.Off;
	
	[SerializeField] SpriteText personalStore;
	[SerializeField] SpriteText personalStoreTime;
	
	float m_RemainTime = 0;
	
	void Awake()
	{
		m_RemainTime = 0;
		
		personalStore.Text = "";
		personalStoreTime.Text = "";
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(m_State)
		{
		case eState.On:
			if(m_RemainTime > 0)
			{
				m_RemainTime -= Time.deltaTime;
				
				float hr = m_RemainTime / 3600.0f;
				float min = ( m_RemainTime % 3600.0f) / 60.0f;
				float sec = ( m_RemainTime % 3600.0f) % 60.0f;
				
				personalStoreTime.Text = string.Format( "{0:D2}:{1:D2}:{2:D2}", (int)hr, (int)min, (int)sec);
			}
//			else
//			{
//				m_State = eState.Expired;
//				
//				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_ClosePrivateShop());
//				
//				AsUserInfo.Instance.nPrivateShopOpenCharUniqKey = 0;
//				Clear();
//				
//				if(m_Expire != null)
//					m_Expire();
//				else
//					Debug.LogError("AsCharacterSlotShopInfo::Update: Expire delegate is null.");
//			}
			break;
		case eState.Off:
			break;
		case eState.Expired:
			break;
		}
	}
	
//	public delegate void Dlt_Expire();
//	Dlt_Expire m_Expire;
//	public void Dlt_SetExpire(Dlt_Expire _dlt)
//	{
//		m_Expire = _dlt;
//	}
	
	public void BeginTimeCount(float _remain)
	{
		m_State = eState.On;
		
		personalStore.Text = AsTableManager.Instance.GetTbl_String(1319);
		m_RemainTime = _remain;
		
//		long endTime = _remain;
//		System.DateTime dt = new System.DateTime(1970, 1, 1, 9, 0, 0);
//		dt = dt.AddSeconds(endTime);
//		System.TimeSpan timeSpan = new System.TimeSpan(dt.Ticks - System.DateTime.Now.Ticks);
//		m_RemainTime = (float)timeSpan.TotalSeconds;
		
//		Debug.Log("AsCharacterSlotShopInfo::BeginTimeCount: _remain + _total = " + endTime);
//		Debug.Log("AsCharacterSlotShopInfo::BeginTimeCount: timeSpan.Ticks = " + timeSpan.Ticks);
//		Debug.Log("AsCharacterSlotShopInfo::BeginTimeCount: " + m_RemainTime);
	}
	
	public void Clear()
	{
		m_State = eState.Off;
		
		personalStore.Text = "";
		personalStoreTime.Text = "";
		
		m_RemainTime = 0;
	}
}
