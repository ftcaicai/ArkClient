using UnityEngine;
using System.Collections;

public class AsInputMarkController : MonoBehaviour
{
	public AsInputMarkSingle singleMark1 = null;
	public AsInputMarkSingle singleMark2 = null;
	public AsInputMarkTrace traceMark1 = null;
	public AsInputMarkTrace traceMark2 = null;
	public TrailRenderer trailRenderer = null;
	
	private int m_nMovePointEffIndexBuf = 0;
	private int m_nAutoMoveEffIndexBuf = 0;
	private float m_fTimeBuf = 0.0f;
	private AsEffectEntity m_AutoMoveEff;
	private AsPlayerFsm m_PlayerFsmBuf;
	private bool m_bOneTouchHold = false;

	// < test
	private string m_strMovePointEffPath = "FX/Effect/Decal/Fx_MovePoint_Dummy";
	private string m_strAutoMoveEffPath = "FX/Effect/Decal/Fx_AutoMove_Dummy";
	// test >
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
		
		if( null != userEntity && userEntity.GetProperty<bool>( eComponentProperty.SHOP_OPENING))
		{
			_StopEff_MovePoint();
			_StopEff_AutoMove();
			return;
		}
		
		if( eInputEventType.NONE !=  AsInputManager.Instance.eInputEvent
			&& false == _isDragEvent())
		{
			_StopEff_MovePoint();
			_StopEff_AutoMove();
			
			_ShowInputEvent( AsInputManager.Instance.eInputEvent);
			
			if( eInputEventType.ONE_FINGER_TOUCH_HOLD != AsInputManager.Instance.eInputEvent)
				m_bOneTouchHold = false;

			AsInputManager.Instance.eInputEvent = eInputEventType.NONE;
		}
		
//		if( true == _isDragEvent())
//			_Update_DragTrace();
		
		// TrailRenderer
		_Update_TrailRenderer();
		
		_Update_MovePointEffect();
		_Update_AutoMoveEffect();
		
		// etc
		if( m_fTimeBuf > 0.0f)
		{
			if( Time.time - m_fTimeBuf > 0.2f)
			{
				singleMark2.Show( AsInputManager.Instance.vStoredScreenPos);
				m_fTimeBuf = 0.0f;
			}
		}
	}
	
	// < private
	private void _ShowInputEvent(eInputEventType eCurInputEventType)
	{
		if( eInputEventType.ONE_FINGER_DOUBLE_TOUCH != eCurInputEventType
			&& true == _isPlayerChar_SkillActive())
			return;
		
		switch( eCurInputEventType)
		{
		case eInputEventType.ONE_FINGER_TOUCH_MOVE:
			_OneFingerTouch_Move();
			break;
			
		case eInputEventType.ONE_FINGER_TOUCH_ATTACK:
			_OneFingerTouch_Attack();
			break;
			
		case eInputEventType.ONE_FINGER_DOUBLE_TOUCH:
			_OneFingerDoubleTouch();
			break;

		case eInputEventType.TWO_FINGER_TOUCH:
			if( null != traceMark1 && true == traceMark1.isActive())
				traceMark1.EndTrace();
			_TwoFingerTouch();
			break;
			//$yde
		case eInputEventType.ONE_FINGER_TOUCH_HOLD:
			if( null != traceMark1 && true == traceMark1.isActive())
				traceMark1.EndTrace();
			_OneFingerHold();
			m_bOneTouchHold = true;
			break;
		}
	}
	
	private void _OneFingerTouch_Move()
	{
		// Show One Finger Touch Mark
		singleMark1.Show( AsInputManager.Instance.vStoredScreenPos);

		// Player Character Move Position Effect
		Vector3 vWorldPos = AsInputManager.Instance.vStoredWorldPos;
		
		AsEffectEntity effData = AsEffectManager.Instance.GetEffectEntity( m_nMovePointEffIndexBuf);
		
		if( null == effData)
		{
			m_nMovePointEffIndexBuf = AsEffectManager.Instance.PlayEffect( m_strMovePointEffPath, vWorldPos);
			effData = AsEffectManager.Instance.GetEffectEntity( m_nMovePointEffIndexBuf);
			if( null != effData)
				effData.EffectPrefab.AutoDel = false;
		}
		else
		{
			effData.PlayOnce();
			effData.SetPosition( vWorldPos);
		}
	}

	private void _OneFingerTouch_Attack()
	{
		// Show One Finger Touch Mark
		singleMark1.Show( AsInputManager.Instance.vStoredScreenPos);
	}
	
	private void _OneFingerDoubleTouch()
	{
		singleMark1.Show( AsInputManager.Instance.vStoredScreenPos);
		m_fTimeBuf = Time.time;
	}
	
	private void _TwoFingerTouch()
	{
		// Show Two Finger Touch Mark
		Vector3 vBuf1 = AsInputManager.Instance.GetTwoFingerTouchScreenPos(0);
		Vector3 vBuf2 = AsInputManager.Instance.GetTwoFingerTouchScreenPos(1);
		singleMark1.Show( vBuf1);
		if(Application.platform == RuntimePlatform.IPhonePlayer)
			singleMark2.Show( vBuf2);
		
		// Player Character Direction Effect
		Transform CharTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		m_AutoMoveEff = AsEffectManager.Instance.GetEffectEntity( m_nAutoMoveEffIndexBuf);
		if( null == m_AutoMoveEff)
		{
			m_nAutoMoveEffIndexBuf = AsEffectManager.Instance.PlayEffect( m_strAutoMoveEffPath, CharTransform.position, true, 0.0f);
			m_AutoMoveEff = AsEffectManager.Instance.GetEffectEntity( m_nAutoMoveEffIndexBuf);
			if( null != m_AutoMoveEff)
				m_AutoMoveEff.EffectPrefab.AutoDel = false;
		}
		else
		{
			m_AutoMoveEff.PlayOnce();
			m_AutoMoveEff.SetAngleY( 0.0f, true);
		}
		
		//$yde
		Quaternion lookAt = Quaternion.LookRotation(AsInputManager.Instance.GetAutoRunPickPos() - CharTransform.position);
		float fAngle = lookAt.eulerAngles.y - 90.0f;
		AsEffectManager.Instance.SetAngleY( m_nAutoMoveEffIndexBuf, fAngle);	
	}
	
	//$yde
	private void _OneFingerHold()
	{
		Transform CharTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
		if( null == CharTransform)
			return;
		
		m_AutoMoveEff = AsEffectManager.Instance.GetEffectEntity( m_nAutoMoveEffIndexBuf);
		if( null == m_AutoMoveEff)
		{
			m_nAutoMoveEffIndexBuf = AsEffectManager.Instance.PlayEffect( m_strAutoMoveEffPath, CharTransform.position, true, 0.0f);
			m_AutoMoveEff = AsEffectManager.Instance.GetEffectEntity( m_nAutoMoveEffIndexBuf);
			if( null != m_AutoMoveEff)
				m_AutoMoveEff.EffectPrefab.AutoDel = false;
		}
		else
		{
			m_AutoMoveEff.PlayOnce();
		}

		Quaternion lookAt = Quaternion.LookRotation(AsInputManager.Instance.GetAutoRunPickPos() - CharTransform.position);
		float fAngle = lookAt.eulerAngles.y - 90.0f;
		AsEffectManager.Instance.SetAngleY( m_nAutoMoveEffIndexBuf, fAngle);	
	}
	
	private void _StopEff_MovePoint()
	{
		if( _isMovePointEff())
		{
			AsEffectManager.Instance.StopEffect( m_nMovePointEffIndexBuf);
		}
	}
	
	private bool _isMovePointEff()
	{
		AsEffectEntity effData = AsEffectManager.Instance.GetEffectEntity( m_nMovePointEffIndexBuf);
		if( null == effData)
			return false;
		if( false == effData.enabled)
			return false;
		return true;
	}
	
	private void _StopEff_AutoMove()
	{
		if( eInputEventType.ONE_FINGER_TOUCH_HOLD ==  AsInputManager.Instance.eInputEvent)
			return;
		
		if( _isAutoMoveEff())
		{
			AsEffectManager.Instance.StopEffect( m_nAutoMoveEffIndexBuf);
		}
	}
	
	private bool _isAutoMoveEff()
	{
		if( null == AsEffectManager.Instance.GetEffectEntity( m_nAutoMoveEffIndexBuf))
			return false;
		return true;
	}

	private bool _isPlayerChar_SkillActive()
	{
		if( null == m_PlayerFsmBuf)
		{
			m_PlayerFsmBuf = AsEntityManager.Instance.GetPlayerCharFsm();
			if( null == m_PlayerFsmBuf)
				return false;
		}
		
		if( m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_HIT
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_READY
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_FINISH
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.DASH
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.DEATH
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.IDLE
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.BATTLE_RUN
			|| m_PlayerFsmBuf.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.PRODUCT)
			return true;
		
		return false;
	}
	
	private bool _isDragEvent()
	{
		if( eInputEventType.ONE_FINGER_DRAG_BEGIN == AsInputManager.Instance.eInputEvent
			|| eInputEventType.ONE_FINGER_DRAG_END == AsInputManager.Instance.eInputEvent
			|| eInputEventType.TWO_FINGER_DRAG_BEGIN == AsInputManager.Instance.eInputEvent
			|| eInputEventType.TWO_FINGER_DRAG_END == AsInputManager.Instance.eInputEvent)
			return true;
		
		return false;
	}

	private void _Update_MovePointEffect()
	{
		if( true ==_isMovePointEff())
		{
			Transform CharTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
			
			if( null != CharTransform)
			{
				Vector3 vPlayerPos = CharTransform.position;
				Vector3 vWorldPos = AsInputManager.Instance.vStoredWorldPos;
				vPlayerPos.y = vWorldPos.y = 0.0f;
				
				if( vPlayerPos == vWorldPos)
					_StopEff_MovePoint();
			}
			else
				_StopEff_MovePoint();
			
			if( true == _isPlayerChar_SkillActive())
				_StopEff_MovePoint();
		}
	}
	
	float m_SavedAngle = 0f;
	private void _Update_AutoMoveEffect()
	{
		if( null != m_AutoMoveEff)
		{	
			Transform CharTransform = CameraMgr.Instance.GetPlayerCharacterTransform();
			if( null != CharTransform)
			{
				m_AutoMoveEff.SetPosition( CharTransform.position);
				
				if( true == m_bOneTouchHold)
				{
					if(AsInputManager.Instance.inputOnUI == true)//$yde
					{
						m_AutoMoveEff.SetAngleY( m_SavedAngle, true);
						return;
					}
					
//					Vector3 vNewPos = AsInputManager.Instance.GetCurAutoRunPickPos();
//					
//					if( Vector3.zero != vNewPos)
//					{
//						Quaternion lookAt = Quaternion.LookRotation( - CharTransform.position);
//						float fAngle = lookAt.eulerAngles.y - 90.0f;
//						m_AutoMoveEff.SetAngleY( fAngle, true);
//						m_SavedAngle = fAngle;
//					}
					
					float fAngle = CharTransform.eulerAngles.y - 90.0f;
					m_AutoMoveEff.SetAngleY( fAngle, true);
					m_SavedAngle = fAngle;
				}
			}
			else
				_StopEff_AutoMove();
			
			if( true == _isPlayerChar_SkillActive())
				_StopEff_AutoMove();
		}
		else
		{
			m_nAutoMoveEffIndexBuf = 0;
		}
	}
	
	private void _Update_DragTrace()
	{
		// One Finger Drag
		if( eInputEventType.ONE_FINGER_DRAG_BEGIN == AsInputManager.Instance.eInputEvent)
		{
			int nCnt = AsInputManager.Instance.GetTraceBufCount_1();
			if( nCnt > 0)
			{
				Vector3 vPos = AsInputManager.Instance.GetTraceBufPos_1( nCnt - 1);
				traceMark1.SetTracePos( vPos);
			}
		}
		else if( eInputEventType.ONE_FINGER_DRAG_END == AsInputManager.Instance.eInputEvent)
		{
			traceMark1.EndTrace();
			AsInputManager.Instance.eInputEvent = eInputEventType.NONE;

			if( true == _isPlayerChar_SkillActive())
			{
				_StopEff_MovePoint();
				_StopEff_AutoMove();
			}
		}
		
		// Two Finger Drag
		if( eInputEventType.TWO_FINGER_DRAG_BEGIN == AsInputManager.Instance.eInputEvent)
		{
			int nCnt = AsInputManager.Instance.GetTraceBufCount_1();
			if( nCnt > 0)
			{
				Vector3 vPos = AsInputManager.Instance.GetTraceBufPos_1( nCnt - 1);
				traceMark1.SetTracePos( vPos);
			}
			
			nCnt = AsInputManager.Instance.GetTraceBufCount_2();
			if( nCnt > 0)
			{
				Vector3 vPos = AsInputManager.Instance.GetTraceBufPos_2( nCnt - 1);
				traceMark2.SetTracePos( vPos);
			}
		}
		else if( eInputEventType.TWO_FINGER_DRAG_END == AsInputManager.Instance.eInputEvent)
		{
			traceMark1.EndTrace();
			traceMark2.EndTrace();
			AsInputManager.Instance.eInputEvent = eInputEventType.NONE;

			_StopEff_MovePoint();
			_StopEff_AutoMove();
		}
	}

	void _Update_TrailRenderer()
	{
		if( eInputEventType.ONE_FINGER_DRAG_BEGIN == AsInputManager.Instance.eInputEvent
			|| eInputType.SINGLE_DRAG_BEGAN == AsInputManager.Instance.GetCurInputState())
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if( 1 == Input.touchCount)
				{
					Vector3 vPos = CameraMgr.Instance.ScreenPointToUIRay( Input.GetTouch(0).position);
					vPos.z = 2.0f;
					trailRenderer.transform.position = vPos;
				}
			}
			else
			{
				if( true == Input.GetMouseButton( 0))
				{
					Vector3 vPos = CameraMgr.Instance.ScreenPointToUIRay( Input.mousePosition);
					vPos.z = 2.0f;
					trailRenderer.transform.position = vPos;
				}
			}
		}
	}
	// private >
}
