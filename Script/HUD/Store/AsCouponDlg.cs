using UnityEngine;
using System.Collections;

public class AsCouponDlg : MonoBehaviour {
	
	[SerializeField] SpriteText title_;
	[SerializeField] SpriteText contents_;
	[SerializeField] SpriteText txtOk_;
	[SerializeField] UITextField input_;
	[SerializeField] UIButton btnOk_;
	[SerializeField] UIButton btnClose_;
	
	static byte[] s_CouponKey = null; public static byte[] CouponKey{get{return s_CouponKey;}}
	
	AsMessageBox m_Popup = null;
	
	void Awake()
	{		
		btnOk_.SetInputDelegate(OnBtnOk_);
		btnClose_.SetInputDelegate(OnBtnClose_);
	}
	
	void Start () {
		
//1540	쿠폰 입력	쿠폰 입력창 제목
//1624	아크 스피어 쿠폰 16자리를 입력해주세요.	쿠폰 입력창 설명.
		
		title_.Text = AsTableManager.Instance.GetTbl_String(1540);
		contents_.Text = AsTableManager.Instance.GetTbl_String(1624);
		btnOk_.Text = "";
		txtOk_.Text = AsTableManager.Instance.GetTbl_String(1152);
		input_.Text = "";
		input_.SetValidationDelegate(InputValidator);
	}
	
	void Update () {
	
	}
	
	void Close()
	{
		if (AsHudDlgMgr.Instance.IsOpenCashStore)
			AsHudDlgMgr.Instance.cashStore.LockInput(false);

		if(m_Popup != null)
			Destroy(m_Popup.gameObject);
		
		Destroy(gameObject);
	}
	
	void OnBtnOk_(ref POINTER_INFO ptr)
	{
		if(m_Popup != null)
			return;
		
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
			bool succeed = AsNetworkIAPManager.Instance.ConnectToServer(AsNetworkDefine.IAPAGENT_SERVER_IP, AsNetworkDefine.IAPAGENT_SERVER_PORT, IAP_SOCKET_STATE.ISS_CONNECT);
		
			if(succeed == false)
				Debug.LogError("AsCouponDlg::Start: conncection is failed.");
				
			if(input_.Text.Length == 0)	
			{
//1625	쿠폰 코드를 입력해주시기 바랍니다.	UI 1_2 아무것도 입력하지 않고 확인 버튼 터치 시.
				string str = AsTableManager.Instance.GetTbl_String(1625);
				m_Popup = AsNotify.Instance.MessageBox("", str);
				return;
			}
			
			body_CI_COUPON_REQ req = new body_CI_COUPON_REQ();
			req.nUserUniqKey =AsUserInfo.Instance.LoginUserUniqueKey;
			req.nGameCode = AsGameDefine.GAME_CODE;
			
//			byte[] appID = new byte[AsGameDefine.eAPPID + 1];
//			byte[] id = System.Text.UTF8Encoding.UTF8.GetBytes("c95f54fa-3f68-4f00-95ec-d5ddd4fbab0d");
//			for(int i=0; i<id.Length; ++i)
//			{
//				appID[i] = id[i];
//			}
//			req.strAppID = appID;
			
			byte[] couponKey = new byte[AsGameDefine.eCOUPONKEY + 1];
			byte[] codes = System.Text.UTF8Encoding.UTF8.GetBytes(input_.Text);
			Debug.Log("AsCouponDlg:: OnBtnOk_: input_ codes's length is " + codes.Length);
			for(int i=0; i<codes.Length; ++i)
			{
				couponKey[i] = codes[i];
			}
			req.strCouponKey = s_CouponKey = couponKey;
			
			AsCommonSender.SendToIAP(req.ClassToPacketBytes());
			
			Close();
		}
	}
	
	void OnBtnClose_(ref POINTER_INFO ptr)
	{
		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
			Close();
			
			Debug.Log("AsCouponDlg::OnDestroy: IAP is diconnected.");
			AsNetworkIAPManager.Instance.DisConnect();
		}
	}
		
	string InputValidator( UITextField field, string text, ref int insPos)
	{		
		// #22671
		int index =  text.IndexOf('\'');
		if(-1 != index)
			text = text.Remove( index);	
		
		return text;	
	}
}
