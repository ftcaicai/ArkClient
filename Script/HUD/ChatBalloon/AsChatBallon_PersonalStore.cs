using UnityEngine;
using System.Collections;

public class AsChatBallon_PersonalStore : MonoBehaviour {
	
	private const float MINIMAL_WIDTH = 8.0f;
	public SimpleSprite left_top = null;
	public SimpleSprite top = null;
	public SimpleSprite right_top = null;
	public SimpleSprite left = null;
	public SimpleSprite center = null;
	public SimpleSprite right = null;
	public SimpleSprite left_bottom = null;
	public SimpleSprite bottom_left = null;
	public SimpleSprite tail = null;
	public SimpleSprite bottom_right = null;
	public SimpleSprite right_bottom = null;
	public SpriteText title = null;
	public SpriteText content = null;
	
	[SerializeField] SimpleSprite top_storeimg = null;
	float m_StoreImgOffset = 0;
	
	float m_VerticalOffset = 0.5f;

	private AsUserEntity entity = null;
	public AsUserEntity Owner
	{
		set
		{
			entity = value;
//			Debug.Log("AsChatBallon_PersonalStore::Owner: entity.FsmType = " + entity.FsmType + ", entity.UniqueId = " + entity.UniqueId);
		}
	}
	
	[SerializeField] float m_Height = 1.0f;
//	float m_Height = 2.0f;
	[SerializeField] float m_Depth = 11.0f;
	
	void Awake()
	{
		m_StoreImgOffset = top_storeimg.transform.position.y - top.transform.position.y;
	}
	
	void Start()
	{
//		StartCoroutine( RemoveBalloon());
	}
	
	void Update()
	{
		if( null == entity)
		{
			Debug.LogWarning("AsChatBallon_PersonalStore::Update: entity is null. destroy chat balloon.");
			
			GameObject.DestroyImmediate( gameObject);
			return;
		}
		
		if(entity.ModelObject != null)
		{
			Vector3 worldPos = entity.ModelObject.transform.position;
			worldPos.y += entity.characterController.height;
			Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
			Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
			vRes.y += m_Height;
			vRes.z = m_Depth;
			gameObject.transform.position = vRes;
		}
	}
	
	void SetText( string msg)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		title.Text = string.Format(AsTableManager.Instance.GetTbl_String(1231), msg);

//		UpdateShape();
	}
	
	void SetContent(byte[] _content)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( content);
		string str = System.Text.Encoding.UTF8.GetString(_content);
		//content.Text = str.TrimEnd('\0');
        int idx = str.IndexOf('\0');
        content.Text = str.Substring(0, idx);
//		UpdateShape();
	}
	
	public void SetTitleAndContent(string _title, byte[] _content)
	{
		if(_title == null || _content == null)
			return;
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage(title);
		title.Text = string.Format(AsTableManager.Instance.GetTbl_String(1231), _title);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( content);
		string str = System.Text.Encoding.UTF8.GetString(_content);
        int idx = str.IndexOf('\0');
        content.Text = str.Substring(0, idx);
		
		if(content.Text == "")
		{
			title.transform.localPosition = new Vector3(0, -0.5f, -0.2f);
		}
		
		UpdateShape();
	}
	
	private void UpdateShape()
	{
		Vector3 centerPt = center.transform.position;
		
		float maxWidth = (title.TotalWidth > content.TotalWidth) ? title.TotalWidth: content.TotalWidth;
		center.width = ( MINIMAL_WIDTH > maxWidth) ? MINIMAL_WIDTH : maxWidth;
		
		float maxHeight = content.lineSpacing * title.GetDisplayLineCount();
		if(content.Text != "")
			maxHeight += content.lineSpacing * content.GetDisplayLineCount();
		center.height = maxHeight;
		
		switch(title.GetDisplayLineCount())
		{
		case 1:
			switch(content.GetDisplayLineCount())
			{
			case 0:
			case 1:
				break;
			case 2:
				centerPt.y -= m_VerticalOffset;
				break;
			}
			break;
		case 2:
			switch(content.GetDisplayLineCount())
			{
			case 0:
			case 1:
				centerPt.y += m_VerticalOffset;
				break;
			case 2:
				break;
			}
			break;
		}
		center.transform.position = centerPt;
		
//		if(title.GetDisplayLineCount() > 1 && title.GetDisplayLineCount() > content.GetDisplayLineCount())
//		{
//			centerPt.y += m_VerticalOffset;
//			center.transform.position = centerPt;
//		}
//		else if(title.GetDisplayLineCount() == 1 && content.GetDisplayLineCount() == 2)
//		{
//			centerPt.y -= m_VerticalOffset;
//			center.transform.position = centerPt;
//		}
		
//		if(content.Text == "")
//		{
//			center.height = content.lineSpacing;
//		}
//		else
//			center.height = content.lineSpacing * content.GetDisplayLineCount() * 2;
		
		left_top.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), 	centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.transform.position = new Vector3( centerPt.x, centerPt.y + ( center.height * 0.5f), centerPt.z);
		top.width = center.width;
		right_top.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y + ( center.height * 0.5f), centerPt.z);
		left.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y, centerPt.z);
		left.height = center.height;
		right.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y, centerPt.z);
		right.height = center.height;
		left_bottom.transform.position = new Vector3( centerPt.x - ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);

		tail.transform.position = new Vector3( centerPt.x, centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_left.transform.position = new Vector3( tail.transform.position.x - ( tail.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_left.width = ( center.width - tail.width) * 0.5f;
		bottom_right.transform.position = new Vector3( tail.transform.position.x + ( tail.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		bottom_right.width = ( center.width - tail.width) * 0.5f;
		
		right_bottom.transform.position = new Vector3( centerPt.x + ( center.width * 0.5f), centerPt.y - ( center.height * 0.5f), centerPt.z);
		
		top.CalcSize();
		left.CalcSize();
		center.CalcSize();
		right.CalcSize();
		bottom_left.CalcSize();
		bottom_right.CalcSize();
		
		float maxWidthStoreImg = top.width;
		float widthStoreImg = top_storeimg.width;
		Vector3 imgPos = top.transform.position; imgPos.y += m_StoreImgOffset; imgPos.z = top_storeimg.transform.position.z;
		top_storeimg.transform.position = imgPos;
		
		int portion = (int)(maxWidthStoreImg / widthStoreImg);
		if(portion < 1)
			return;
		
		for(int i = -portion / 2 + 1; i<portion / 2; ++i)
		{
			if(i == 0)
				continue;
			
			GameObject obj = Instantiate(top_storeimg.gameObject) as GameObject;
			Vector3 pos = obj.transform.position;
			pos.x = i * widthStoreImg;
			obj.transform.position = pos;
			
			obj.transform.parent = top.transform;
		}
	}
	
	private IEnumerator RemoveBalloon()
	{
		while( true)
		{
			yield return new WaitForSeconds( 4.0f);
			GameObject.DestroyImmediate( gameObject);
		}
	}
}
