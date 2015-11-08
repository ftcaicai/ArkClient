using UnityEngine;
using System.Collections;

public class AsPanel_Damage : MonoBehaviour
{
	public SpriteText DamageText = null;
	private bool m_bShowCommand = false;
	private GameObject m_go = null;
	private Color m_curColor;
	private Vector3 m_vCurPos;
	private float m_fCurScale;
	private float m_fCurTime;
	private Vector3 m_vWorldPosRevision;
	private Vector3 m_vUIPosRevision;
	private float m_fAlphaDecreaseSpeed = 2.0f; // default: 2.0f
	private float m_fMoveSpeedY = 5.0f; // default: 5.0f
	
	// Use this for initialization
	void Start()
	{
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	// Update is called once per frame
	void Update()
	{
		if( false == m_bShowCommand)
			return;
		
		if( 0.0f >= m_curColor.a)
		{
			_Remove();
			return;
		}
		
		if( Time.time > m_fCurTime + 0.5f)
		{
			m_curColor.a -= ( Time.deltaTime * m_fAlphaDecreaseSpeed);
			DamageText.SetColor( m_curColor);
		}
		
		if(m_go == null)
			return;
		
		Vector3 vBuf = _WorldToUIPoint( m_go.transform.position + m_vWorldPosRevision, m_vUIPosRevision);
		m_vCurPos.x = vBuf.x;
		m_vCurPos.y += ( Time.deltaTime * m_fMoveSpeedY);
		m_vCurPos.z = vBuf.z;
		transform.position = m_vCurPos;
	}
	
	void OnEnable()
	{
		if( false == m_bShowCommand)
			gameObject.SetActiveRecursively( false);
	}
	
	void OnDisable()
	{
		if( m_curColor.a > 0.0f)
			DestroyObject( gameObject);
	}
	
	public void Show(GameObject go, string Damage, Color textColor, float fScale, int nUIYPosRevisionFlag, bool isUseHeightScale, bool isUseInteractivePanel, float fSpeed)
	{
		m_bShowCommand = true;
		gameObject.SetActiveRecursively( true);
		DamageText.text = Damage;
		m_fCurScale = fScale;
		m_fCurTime = Time.time;

		CharacterController ctrl = go.GetComponentInChildren<CharacterController>();
		m_vWorldPosRevision = Vector3.zero;
		if( true == isUseHeightScale)
			m_vWorldPosRevision.y = _GetHeightRevision( ctrl.height);
		else
			m_vWorldPosRevision.y = ctrl.height;

		float fTextWidth = DamageText.GetWidth( Damage);
		float fTextHeight = DamageText.BaseHeight;
		m_vUIPosRevision.x = -(fTextWidth * m_fCurScale) * 0.5f;
		m_vUIPosRevision.y = (fTextHeight * m_fCurScale) * 0.5f + ( fTextHeight * (float)nUIYPosRevisionFlag);
		m_vUIPosRevision.z = 1.0f;
		
		m_vCurPos = _WorldToUIPoint( go.transform.position + m_vWorldPosRevision, m_vUIPosRevision);
		transform.position = m_vCurPos;
		transform.localScale = new Vector3( m_fCurScale, m_fCurScale, m_fCurScale);
		
		m_curColor = textColor;
		DamageText.SetColor( m_curColor);
		
		if( true == isUseInteractivePanel)
			_InteractivePanel_Reveal();
		
		m_fAlphaDecreaseSpeed = 2.0f * fSpeed; // default: 2.0f
		m_fMoveSpeedY = 5.0f * fSpeed; // default: 5.0f
		m_go = go;
	}
	
	// < private
	private Vector3 _WorldToUIPoint(Vector3 vWorldPos, Vector3 vUIPosRevision)
	{
		Vector3 vScreenPos = _WorldToScreenPoint( vWorldPos);
		Vector3 vRes = _ScreenPointToUIRay( vScreenPos, vUIPosRevision);
		return vRes;
	}

	private Vector3 _WorldToScreenPoint(Vector3 vWorldPos)
	{
		return CameraMgr.Instance.WorldToScreenPoint( vWorldPos);
	}
	
	private Vector3 _ScreenPointToUIRay(Vector3 vScreenPos, Vector3 vUIPosRevision)
	{
		Vector3 vRes = CameraMgr.Instance.ScreenPointToUIRay( vScreenPos);
		vRes.x += vUIPosRevision.x;
		vRes.y += vUIPosRevision.y;
		vRes.z = vUIPosRevision.z;
		return vRes;
	}
	
	private void _Remove()
	{
		transform.parent = null;
		DestroyImmediate( gameObject);
	}
	
	private float _GetHeightRevision(float fCharacterHeight)
	{
		float fRes = 0.0f;
		
		if( fCharacterHeight < 2.4f)
			fRes = fCharacterHeight * 1.0f;
		else if( fCharacterHeight < 6.4f)
			fRes = fCharacterHeight * 0.7f;
		else if( fCharacterHeight < 8.66f)
			fRes = fCharacterHeight * 0.5f;
		else
			fRes = fCharacterHeight * 0.5f;
		
		return fRes;
	}
	
	private void _InteractivePanel_Reveal()
	{
		UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
		foreach( UIInteractivePanel panel in panels)
			panel.Reveal();
	}
	
	private void _InteractivePanel_End()
	{
		UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
		foreach( UIInteractivePanel panel in panels)
			panel.Hide();
	}
	// private >
}
