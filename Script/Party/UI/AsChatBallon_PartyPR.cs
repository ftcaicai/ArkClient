using UnityEngine;
using System.Collections;

public class AsChatBallon_PartyPR : MonoBehaviour
{
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
	public UIButton m_PartyInfoViewBtn;

	[SerializeField] SimpleSprite top_partyimg = null;
	float m_PartyImgOffset = 0;

	private int m_nPartyIdx = 0;
	public int PartyIdx
	{
		get { return m_nPartyIdx; }
		set { m_nPartyIdx = value;}
	}
	private AsUserEntity entity = null;
	public AsUserEntity Owner
	{
		set	{ entity = value; }
	}

	[SerializeField] float m_Height = 3.0f;
	[SerializeField] float m_Depth = 11.0f;

	public float TotalHeight
	{
		get	{ return top.height + center.height + tail.height; }
	}

	public float TotalWidth
	{
		get	{ return left.width + center.width + right.width; }
	}

	void Awake()
	{
		PartyIdx = 0;
		m_PartyImgOffset = top_partyimg.transform.position.y - top.transform.position.y;
	}

	void Start()
	{
		m_PartyInfoViewBtn.SetInputDelegate( PartyInfoViewBtnDelegate);
	}

	void Update()
	{
		if( null == entity)
		{
			Debug.LogWarning( "AsChatBallon_PartyPR::Update: entity is null. destroy chat balloon.");

			GameObject.DestroyImmediate( gameObject);
			return;
		}

		if( entity.ModelObject != null)
		{
			Vector3 worldPos = entity.ModelObject.transform.position;
			worldPos.y += entity.characterController.height;
			Vector3 screenPos = CameraMgr.Instance.WorldToScreenPoint( worldPos);
			Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( screenPos);
			vRes.y += m_Height;
			vRes.z = m_Depth;
			gameObject.transform.position = vRes;
		}
		else
		{
			gameObject.transform.position = Vector3.zero;
		}
	}

	private void PartyInfoViewBtnDelegate( ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( 0 != m_nPartyIdx && !AsPartyManager.Instance.IsPartying)
			{
				AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
				AsPartyManager.Instance.SendNoticePartyIdx = m_nPartyIdx;
				AsPartySender.SendPartyDetailInfo( m_nPartyIdx);
			}
		}
	}

	void SetTitle( string partyName)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		title.Text = partyName;
	}

	void SetContent( string _content)
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( content);

		int idx = _content.IndexOf( '\0');
		content.Text = _content.Substring( 0, idx);
	}

	public void SetTitleAndContent( string partyName, string _content, int nPartyIdx)
	{
		if( partyName == null || _content == null)
			return;

		gameObject.transform.position = Vector3.zero;

		m_nPartyIdx = nPartyIdx;
		AsLanguageManager.Instance.SetFontFromSystemLanguage( title);
		title.Text = string.Format( AsTableManager.Instance.GetTbl_String( 1727), partyName);

		AsLanguageManager.Instance.SetFontFromSystemLanguage( content);

		content.Text = _content;
		if( content.Text == "")
			title.transform.localPosition = new Vector3( 0, -0.5f, -0.2f);

		UpdateShape();
	}

	private void UpdateShape()
	{
		Vector3 centerPt = center.transform.position;

		float miniWidth = ( MINIMAL_WIDTH > title.TotalWidth) ? MINIMAL_WIDTH : title.TotalWidth; //20646.
		center.width = ( miniWidth > content.TotalWidth) ? miniWidth : content.TotalWidth;
		if( content.Text == "")
		{
			center.height = content.lineSpacing;
		}
		else
			center.height = content.lineSpacing * content.GetDisplayLineCount() + content.lineSpacing;

		if( content.GetDisplayLineCount() > 1)
		{
			Vector3 titlepos = title.gameObject.transform.position;
			titlepos.y += ( content.lineSpacing * content.GetDisplayLineCount()) * 0.25f;
			title.gameObject.transform.position = titlepos;

			Vector3 contentpos = content.gameObject.transform.position;
			contentpos.y += ( content.lineSpacing * content.GetDisplayLineCount()) * 0.25f;
			content.gameObject.transform.position = contentpos;
		}
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
		float widthPartyImg = top_partyimg.width;
		Vector3 imgPos = top.transform.position; imgPos.y += m_PartyImgOffset; imgPos.z = top_partyimg.transform.position.z;
		top_partyimg.transform.position = imgPos;

		int portion = ( int)( maxWidthStoreImg / widthPartyImg);
		if( portion < 1)
			return;

		for( int i = -portion / 2 + 1; i<portion / 2; ++i)
		{
			if( i == 0)
				continue;

			GameObject obj = Instantiate( top_partyimg.gameObject) as GameObject;
			Vector3 pos = obj.transform.position;
			pos.x = i * widthPartyImg;
			obj.transform.position = pos;

			obj.transform.parent = top.transform;
		}

		m_PartyInfoViewBtn.width = TotalWidth;
		m_PartyInfoViewBtn.height = TotalHeight;
		m_PartyInfoViewBtn.UpdateCollider();
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