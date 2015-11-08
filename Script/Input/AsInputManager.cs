//#define DEBUG_INPUT
#define GAME_PLAYING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region - enum -
public enum eInputType{
	NONE,
	SINGLE_DOWN, SINGLE_UP, SINGLE_HOLD_BEGAN, SINGLE_HOLD_END, SINGLE_DRAG_BEGAN, SINGLE_DRAG_END, SINGLE_SHAKE,
	DOUBLE_DOWN, DOUBLE_UP, DOUBLE_HOLD_BEGAN, DOUBLE_HOLD_END, DOUBLE_DRAG_BEGAN, DOUBLE_DRAG_END, DOUBLE_SHAKE
}

public enum eTraceType{SINGLE, DOUBLE}

// < ilmeda 20120418
public enum eInputEventType
{
	NONE,
	ONE_FINGER_TOUCH_MOVE, ONE_FINGER_TOUCH_ATTACK, ONE_FINGER_DOUBLE_TOUCH, ONE_FINGER_DRAG_BEGIN, ONE_FINGER_DRAG_END,
	TWO_FINGER_TOUCH, TWO_FINGER_DRAG_BEGIN, TWO_FINGER_DRAG_END,
	//$yde
	ONE_FINGER_TOUCH_HOLD
}
// ilmeda 20120418 >
#endregion

public class AsInputManager : MonoBehaviour {
	
	#region - member -
	#region - singleton -
	static AsInputManager m_instance;
	public static AsInputManager Instance{get{return m_instance;}}
	#endregion	
	
	#region - outer functional -
	public bool showInputLog = false;
	public bool m_Activate = true;
	
	Camera m_UICamera;
	public Camera UICamera 
	{
		get {return m_UICamera;}
	}
	#endregion	
	#region - input judging -
	GameObject m_SelectedObject;
	
	eInputType m_CurInputState;
	
	Vector2 m_StoredFirstScreenPosition;
	float m_FirstClickedTime;
	
	public float m_DragInputLimitTime = 1.5f;
	public float m_DragDistanceUpperBound = 10f;
	
	float m_StoredLengthBetweenTwoTouchedPoint;//touch
	
	//EDITABLE POROPERTY
	public float m_CheckDragInterval = 0.01f;
	public int m_MaxTraceCount = 100;
	
	//SHAKE
	public float m_ShakeMovingThreshold = 200f;
	public float m_ShakeReverseThreshold = 250f;
	
	//DOUBLE CLICK
	public float m_DoubleClickTime = 0.5f;
	public float m_DoubleClickEnableGap = 10f;
	float m_LastClickedTime;
	GameObject m_LastClickedObject;
	
	//TRACING
	public float m_SmoothAngle = 50;
	public float m_DirectInputRatio = 0.9f;
	public float m_CircleInputRatioUpperBound = 0.3f;
	
	//HOLD & AUTO MOVE
	public float m_AutoMoveHoldTime = 1f;
//	Vector2 m_CurHeldPosition = Vector3.zero;
	float m_HeldPositionInterval = 0.3f;
	
	public float middleAndCenterRevision_ = 1;
	#endregion	
	#region - zoom in & out (kij) -
	// begin kij
	private Vector2 m_StoredFirstScreenPosition_1 = Vector2.zero;
	private Vector2 m_StoredFirstScreenPosition_2 = Vector2.zero;
	private bool m_IsZoomCameraState = false;
	public float ZoomDelta = 200.0f;
	private Vector2 m_StoredFirstDirection = Vector2.zero;	
	// end kij
	#endregion		
	#region - event & hud (ilmeda) -
	// < ilmeda 20120418
	private eInputEventType m_eInputEvent = eInputEventType.NONE;
	
	private Vector2 [] m_vTwoFingerTouchScreenPos = new Vector2[2];
	private Vector3 m_vAutoRunPickPos;
	// ilmeda 20120418 >
	
	private bool m_bFirstTouch_UI = false; // ilmeda, 20120821
	private float m_fMin_sqrMagnitude = 0.15f;
	#endregion
	#endregion
	
	#region - init -
	void Awake()
	{		
		if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.IPhonePlayer)
			Input.multiTouchEnabled = true;
		
		m_instance = this;
		
		s_showInputLog = showInputLog;
	}
	
	void Start()
	{
		GameObject obj = GameObject.Find("UICamera");
		
		if(obj == null)
			m_UICamera = Camera.mainCamera;
		else
			m_UICamera = obj.GetComponent<Camera>();
		
		m_Activate = false;
	}
	
	void OnDisable()
	{
		StopHoldMove();
	}
	#endregion
	
	#region - update -
	void Update()
	{
		if(m_Activate == false)
			return;
		
		//temp
		#region - ui camera -
		if(m_UICamera == null)
		{
			GameObject cameraObj = GameObject.Find("UICamera");
			if(cameraObj != null)
			{
				Debug.Log( "ui camera is set");
				m_UICamera = cameraObj.GetComponent<Camera>();
			}
		}
		#endregion

		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
			TouchUpdate();
		else
			MouseUpdate();
	}
	#endregion
	
	#region - touch -
	void TouchUpdate()
	{
		if( GAME_STATE.STATE_LOADING == AsGameMain.s_gameState)
			return;
		
		if( null != AsHudDlgMgr.Instance )
			AsHudDlgMgr.Instance.FirstTouchInput( m_UICamera );
		
		#region - ui camera -
		if(m_UICamera != null)
		{
			if( 0 < Input.touchCount)
			{
				Ray ray = m_UICamera.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit/*, LayerMask.NameToLayer("GUI")*/) == true)
				{
					// < ilmeda 20120420
					if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GUI"))
					{
						if( null != AsHudDlgMgr.Instance )
							AsHudDlgMgr.Instance.GuiTouchInput( m_UICamera );
						
						if( true == _isInputAnyDonw())
						{							
							m_eInputEvent = eInputEventType.NONE;
							m_CurInputState = eInputType.NONE;							
							m_bFirstTouch_UI = true;
							
							m_InputOnUI = true;
//							InputOnUI();//$yde
							
							return;
						}
					}
					// ilmeda 20120420 >
				}
				else
					m_bFirstTouch_UI = false;
			}
			else
				m_bFirstTouch_UI = false;
		}
		#endregion
		
		if( null != AsHudDlgMgr.Instance )
			AsHudDlgMgr.Instance.TouchInput( m_UICamera );
		
		#region - single -
		if(Input.touchCount == 1)
		{
			// < ilmeda 20120427
			if( eInputEventType.TWO_FINGER_DRAG_BEGIN == m_eInputEvent)
			{
				m_eInputEvent = eInputEventType.TWO_FINGER_DRAG_END;
			}
			// ilmeda 20120427 >
			
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				m_StoredFirstScreenPosition = Input.mousePosition;
				m_FirstClickedTime = Time.time;
				m_CurInputState = eInputType.SINGLE_DOWN;
				
				m_SelectedObject = GetObjectOnScreenPosition(Input.GetTouch(0).position);
				if(m_SelectedObject != null)
				{
//					AsInputMessage msg = new AsInputMessage("rotate", m_SelectedObject, Input.GetTouch(0).position,
//				                                        Vector3.zero, , 0);
//					AsEntityManager.Instance.DispatchMessage(msg);
				}
				
				StopHoldMove();
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				if(m_CurInputState == eInputType.SINGLE_DOWN)
				{
					if(Vector3.Distance(m_StoredFirstScreenPosition, (Vector3)GetCurrentScreenPos()) > m_DragDistanceUpperBound)
					{
//						if(m_StoredFirstScreenPosition == Input.GetTouch(0).position)
//						{
//							//DOUBLE_HOLD
//	//						DispatchInputMessage(eInputType.DOUBLE_HOLD_BEGAN, m_SelectedObject, Input.mousePosition, Vector3.zero,
//	//							GetPositionOnBaseLand(Input.mousePosition), Time.deltaTime);
//	//						
//							m_CurInputState = eInputType.SINGLE_HOLD_BEGAN;
//						}
//						else
						{
							//DOUBLE_DRAG
							m_CurInputState = eInputType.SINGLE_DRAG_BEGAN;
							
							//m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_BEGIN; // ilmeda 20120418
						}
					}
					else if( Vector3.Distance(m_StoredFirstScreenPosition, (Vector3)GetCurrentScreenPos()) > 10f) // ilmeda, 20120821
					{
						m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_BEGIN; // ilmeda 20120821
					}
					else if(Time.time - m_FirstClickedTime > m_AutoMoveHoldTime)
					{
						m_CurInputState = eInputType.SINGLE_HOLD_BEGAN;
						BeginHoldMove();
						StopTrace(eTraceType.SINGLE);
					}
					
					StartTrace(eTraceType.SINGLE);
				}
				
				
//				if(m_CurInputState == eInputType.SINGLE_HOLD_BEGAN)
//				{
//					if(Time.time - m_HeldPositionRefreshTime > m_HeldPositionInterval)
//					{
//#if GAME_PLAYING
//						HoldMoveProcess();
//#endif
//					}
//				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{
				m_InputOnUI = false;
				
				switch(m_CurInputState)
				{
				case eInputType.SINGLE_DOWN:
//				case eInputType.SINGLE_HOLD_BEGAN:
					if(m_SelectedObject != null)
					{
						if(Time.time - m_LastClickedTime < m_DoubleClickTime &&
							Vector3.Distance(m_StoredMessage.screenPosition_, Input.GetTouch(0).position) < m_DoubleClickEnableGap)
						{
							//DOUBLE CLICK PROCESS
							if(m_LastClickedObject != null)
							{
#if DEBUG_INPUT
								Debug.Log( "Double click");
#endif
#if GAME_PLAYING
								// < ilmeda 20120418
								if( false == _isPlayCharSkillActive())
									m_eInputEvent = eInputEventType.ONE_FINGER_DOUBLE_TOUCH;
								// ilmeda 20120418 >
								
//								RaycastHit hit = GetHitInfoOnScreenPosition(GetCurrentScreenPos());
								DecideDoubleTouchInput(new Msg_Input(eInputType.SINGLE_DOWN, m_LastClickedObject,
									m_StoredMessage.screenPosition_, Vector3.zero, m_StoredMessage.worldPosition_, Time.deltaTime));
#endif
								m_LastClickedObject = null;
								m_LastClickedTime = 0;
							}
						}
						else
						{
							StoreInputMessage(eInputType.SINGLE_UP, m_SelectedObject, Input.GetTouch(0).position, Vector3.zero,
								GetPositionOnBaseLand(Input.GetTouch(0).position), Time.deltaTime);
							
							if(m_SingleClickDeciding == false)
								StartCoroutine("SingleClickDecision");
							
							m_LastClickedObject = m_SelectedObject;
							m_LastClickedTime = Time.time;
						}
					}
					break;
				case eInputType.SINGLE_HOLD_BEGAN:
//					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_MoveStopIndication());
					m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END;
					break;
				case eInputType.SINGLE_DRAG_BEGAN:
					if(Time.time - m_FirstClickedTime  < m_DragInputLimitTime)
					{
						CheckDragInput();
					}
					else
						m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120419
					break;
				}
	
				StopTrace(eTraceType.SINGLE);
				
				m_SelectedObject = null;
			}
		}
		#endregion
		#region - double -
		else if(Input.touchCount == 2)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began || true == m_bFirstTouch_UI)
			{
				// begin kij
				m_IsZoomCameraState = false;
				m_StoredFirstScreenPosition_1 = Input.GetTouch(0).position;
				m_StoredFirstScreenPosition_2 = Input.GetTouch(1).position;
				m_StoredFirstDirection = Vector2.zero;
				// end kij
				
				m_StoredFirstScreenPosition = GetCurrentScreenPos();
				m_FirstClickedTime = Time.time;
				m_CurInputState = eInputType.DOUBLE_DOWN;
				
				m_SelectedObject = GetObjectOnScreenPosition(GetCurrentScreenPos());
				
				StopHoldMove();
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Moved	 || Input.GetTouch(1).phase == TouchPhase.Moved &&
				Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(1).phase == TouchPhase.Stationary)
			{
				
				// being kij
				Vector2 vec2Touch_1 = Input.GetTouch(0).position;
				Vector2 vec2Touch_2 = Input.GetTouch(1).position;				
				
				if( eInputType.DOUBLE_DRAG_BEGAN != m_CurInputState) // ilmeda 20120427
				{					
						
					Vector2 vec2First = m_StoredFirstScreenPosition_1 - m_StoredFirstScreenPosition_2;
					Vector2 vec2Current = vec2Touch_1 - vec2Touch_2;
					float fDis = Mathf.Abs( vec2Current.magnitude - vec2First.magnitude );
					if( ZoomDelta <= fDis )
					{
						if( vec2First.magnitude < vec2Current.magnitude )
						{
							CameraMgr.Instance.SetZoomInState(fDis);
						}
						else
						{
							CameraMgr.Instance.SetZoomOutState(fDis);
						}
						
						m_StoredFirstScreenPosition_1 = vec2Touch_1;
						m_StoredFirstScreenPosition_2 = vec2Touch_2;
						
						m_IsZoomCameraState = true;
					}			
					
					// end kij
				}
						
				Vector2 vec2Direction = Input.GetTouch(0).position - m_StoredFirstScreenPosition_1;
				vec2Direction.Normalize();
						
				
				if( 0.0f > Vector2.Dot( vec2Direction, m_StoredFirstDirection ) || Vector2.zero == m_StoredFirstDirection )
				{					
					AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
					if( null != userEntity)
					{
						userEntity.HandleMessage(new Msg_Input_Rub());					
						m_StoredFirstDirection = Input.GetTouch(0).position - m_StoredFirstScreenPosition_1;
						m_StoredFirstDirection.Normalize();
					}
				}			
				
				
				if(m_CurInputState == eInputType.DOUBLE_DOWN)
				{	
					
					if(Vector2.Distance(m_StoredFirstScreenPosition, (Vector2)GetCurrentScreenPos()) > m_DragDistanceUpperBound)
					{
//						if(m_StoredFirstScreenPosition == GetCurrentScreenPos())
//						{
//							//DOUBLE_HOLD
//							DispatchInputMessage(eInputType.DOUBLE_HOLD_BEGAN, m_SelectedObject, GetCurrentScreenPos(), Vector3.zero,
//								GetPositionOnBaseLand(GetCurrentScreenPos()), Time.deltaTime);//							
//							m_CurInputState = eInputType.DOUBLE_HOLD_BEGAN;
//						}
//						else
						{
							
							if( false == m_IsZoomCameraState) // ilmeda 20120427
							{
								//DOUBLE_DRAG
								m_CurInputState = eInputType.DOUBLE_DRAG_BEGAN;
								m_eInputEvent = eInputEventType.TWO_FINGER_DRAG_BEGIN; // ilmeda 20120419								
							}
						}
					}
								
					if( false == m_IsZoomCameraState) // ilmeda 20120427
						StartTrace(eTraceType.DOUBLE);
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled && 
				Input.GetTouch(1).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Canceled)
				{					
					switch(m_CurInputState)
					{
					case eInputType.DOUBLE_DOWN:
						
					
						if(m_SelectedObject != null)
						{
//							DispatchInputMessage(eInputType.DOUBLE_UP, m_SelectedObject, GetCurrentScreenPos(), Vector3.zero,
//								GetPositionOnBaseLand(Input.mousePosition), Time.deltaTime);
#if GAME_PLAYING
							if( false == m_IsZoomCameraState )
							{
								AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
								if( null != userEntity)
								{
									Vector3 vNewPos = GetPositionOnBaseLand( GetCurrentScreenPos());
									Vector3 dir = vNewPos - userEntity.transform.position;
									if( dir.sqrMagnitude > m_fMin_sqrMagnitude)
									{
										if( vNewPos != Vector3.zero)
										{
											AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Input_Auto_Move( vNewPos));
	
											if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.USE_TWO_TOUCH_MOVE) != null)
												AsCommonSender.SendClearOpneUI(OpenUIType.USE_TWO_TOUCH_MOVE);
	
											
											_SetTwoFingerTouchEvent();
										}
									}
								}
							}
#endif
						}
						break;
					case eInputType.DOUBLE_HOLD_BEGAN:
//						DispatchInputMessage(eInputType.DOUBLE_HOLD_END, m_SelectedObject, GetCurrentScreenPos(), Vector3.zero,
//							m_StoredFirstScreenPosition, Time.deltaTime);
						break;
					case eInputType.DOUBLE_DRAG_BEGAN:
#if GAME_PLAYING
						Vector3 direction = (Vector3)(GetCurrentScreenPos() - m_StoredFirstScreenPosition);
						direction.z = direction.y;direction.y = 0;

						if( false == m_IsZoomCameraState )
						{
//							AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Input_Dash(direction.normalized));
							m_eInputEvent = eInputEventType.TWO_FINGER_DRAG_END; // ilmeda 20120419
						}
#endif
						break;
					}
				
				StopCoroutine("SetScreenPositionTrace");m_TraceBegan = false;
			}
		}
		#endregion
	}
	#endregion
	#region - mouse -
	void MouseUpdate()
	{
		
		if( null != AsHudDlgMgr.Instance )
			AsHudDlgMgr.Instance.FirstMouseInput( m_UICamera );
		
		
		//temp
		#region - ui camera -
		if(m_UICamera != null)
		{
			Ray ray = m_UICamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit/*, LayerMask.NameToLayer("GUI")*/) == true)
			{
				// < ilmeda 20120420
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GUI"))
				{
					if( null != AsHudDlgMgr.Instance )
						AsHudDlgMgr.Instance.GuiMouseInput( m_UICamera );
					
					if( true == _isInputAnyDonw())
					{
						m_eInputEvent = eInputEventType.NONE;
						m_CurInputState = eInputType.NONE;					
						
						return;
					}
				}
				// ilmeda 20120420 >
			}
		}
		#endregion
		
		if( null != AsHudDlgMgr.Instance )
			AsHudDlgMgr.Instance.MouseInput( m_UICamera );
		
		#region - single -
		if(Input.GetMouseButtonDown(0) == true)
		{
			m_StoredFirstScreenPosition = Input.mousePosition;
			m_FirstClickedTime = Time.time;
			m_CurInputState = eInputType.SINGLE_DOWN;
			
			m_SelectedObject = GetObjectOnScreenPosition(Input.mousePosition);
			if(m_SelectedObject != null)
			{
			}
			
			StopHoldMove();
		}
		else if(Input.GetMouseButton(0) == true)
		{
			if(m_CurInputState == eInputType.SINGLE_DOWN)
			{
				if(Vector3.Distance(m_StoredFirstScreenPosition, (Vector3)GetCurrentScreenPos()) > m_DragDistanceUpperBound)
				{
					//DOUBLE_DRAG
					m_CurInputState = eInputType.SINGLE_DRAG_BEGAN;					
					//m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_BEGIN; // ilmeda 20120418
				}
				else if( Vector3.Distance(m_StoredFirstScreenPosition, (Vector3)GetCurrentScreenPos()) > 0.0f) // ilmeda, 20120821
				{
					m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_BEGIN; // ilmeda 20120821
				}
				else if(Time.time - m_FirstClickedTime > m_AutoMoveHoldTime)
				{
					m_CurInputState = eInputType.SINGLE_HOLD_BEGAN;
					BeginHoldMove();
					
					StopTrace(eTraceType.SINGLE);
				}
				
				StartTrace(eTraceType.SINGLE);
			}
			
//			if(m_CurInputState == eInputType.SINGLE_HOLD_BEGAN)
//			{
//				if(Time.time - m_HeldPositionRefreshTime > m_HeldPositionInterval)
//				{
//#if GAME_PLAYING
//					HoldMoveProcess();
//#endif
//				}
//			}
		}
		else if(Input.GetMouseButtonUp(0) == true)
		{
			switch(m_CurInputState)
			{
			case eInputType.SINGLE_DOWN:
//			case eInputType.SINGLE_HOLD_BEGAN:
				if(m_SelectedObject != null)
				{
					if(Time.time - m_LastClickedTime < m_DoubleClickTime &&
						Vector3.Distance(m_StoredMessage.screenPosition_, Input.mousePosition) < m_DoubleClickEnableGap)
					{
						//DOUBLE CLICK PROCESS//
						if(m_LastClickedObject != null)
						{
#if DEBUG_INPUT
							Debug.Log( "Double click");
#endif
#if GAME_PLAYING
							// < ilmeda 20120418
							if( false == _isPlayCharSkillActive())
								m_eInputEvent = eInputEventType.ONE_FINGER_DOUBLE_TOUCH;
							// ilmeda 20120418 >
							
//							RaycastHit hit = GetHitInfoOnScreenPosition(GetCurrentScreenPos());
							DecideDoubleTouchInput(new Msg_Input(eInputType.SINGLE_DOWN, m_LastClickedObject,
								m_StoredMessage.screenPosition_, Vector3.zero, m_StoredMessage.worldPosition_, Time.deltaTime));
#endif
							m_LastClickedObject = null;
							m_LastClickedTime = 0;
						}
					}
					else
					{
						StoreInputMessage(eInputType.SINGLE_UP, m_SelectedObject, Input.mousePosition, Vector3.zero,
								GetPositionOnBaseLand(Input.mousePosition), Time.deltaTime);
						if(m_SingleClickDeciding == false)
							StartCoroutine("SingleClickDecision");
						
						m_LastClickedObject = m_SelectedObject;
						m_LastClickedTime = Time.time;
					}
				}
				break;
			case eInputType.SINGLE_HOLD_BEGAN:
				{
					AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
					if( null == userEntity)
						break;
					
//					userEntity.HandleMessage( new Msg_MoveStopIndication());
					m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END;
				}
				break;
			case eInputType.SINGLE_DRAG_BEGAN:
				if(Time.time - m_FirstClickedTime  < m_DragInputLimitTime)
				{
					CheckDragInput();
				}
				else
					m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120419
				break;
			}

			StopTrace(eTraceType.SINGLE);
			
			m_SelectedObject = null;
		}
		#endregion	
		#region - double -
		else if(Input.GetMouseButtonDown(1) == true)
		{	
			m_StoredFirstScreenPosition = Input.mousePosition;
			m_FirstClickedTime = Time.time;
			m_CurInputState = eInputType.DOUBLE_DOWN;
			
			m_SelectedObject = GetObjectOnScreenPosition(Input.mousePosition);
			
			StopHoldMove();
		}
		else if(Input.GetMouseButton(1) == true)
		{
			if(m_CurInputState == eInputType.DOUBLE_DOWN)
			{
				if(Vector3.Distance(m_StoredFirstScreenPosition, (Vector3)GetCurrentScreenPos()) > m_DragDistanceUpperBound)
				{
					//DOUBLE_DRAG
					m_CurInputState = eInputType.DOUBLE_DRAG_BEGAN;
					
					m_eInputEvent = eInputEventType.TWO_FINGER_DRAG_BEGIN; // ilmeda 20120419
				}
				
				StartTrace(eTraceType.DOUBLE);
			}
		}
		else if(Input.GetMouseButtonUp(1) == true)
		{
			switch(m_CurInputState)
			{
			case eInputType.DOUBLE_DOWN:
				if(m_SelectedObject != null)
				{
#if GAME_PLAYING
					AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
					if( null != userEntity)
					{
						Vector3 vNewPos = GetPositionOnBaseLand(Input.mousePosition);
						Vector3 dir = vNewPos - userEntity.transform.position;
						if( dir.sqrMagnitude > m_fMin_sqrMagnitude)
						{
							if( vNewPos != Vector3.zero)
							{
								AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Input_Auto_Move( vNewPos));
								_SetTwoFingerTouchEvent();
	
								if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.USE_TWO_TOUCH_MOVE) != null)
									AsCommonSender.SendClearOpneUI(OpenUIType.USE_TWO_TOUCH_MOVE);
							}
						}
					}
#endif
				}
				break;
			case eInputType.DOUBLE_HOLD_BEGAN:
//				DispatchInputMessage(eInputType.DOUBLE_HOLD_END, m_SelectedObject, Input.mousePosition, Vector3.zero,
//					m_StoredFirstScreenPosition, Time.deltaTime);
				break;
			case eInputType.DOUBLE_DRAG_BEGAN:
#if GAME_PLAYING
//				Vector3 direction = Input.mousePosition - (Vector3)m_StoredFirstScreenPosition;
//				direction.z = direction.y;direction.y = 0;
//				
//				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_Input_Dash(direction.normalized));
				
				m_eInputEvent = eInputEventType.TWO_FINGER_DRAG_END; // ilmeda 20120419
#endif
				
//				DispatchInputMessage(eInputType.DOUBLE_DRAG_END, m_SelectedObject, Input.mousePosition, projected,
//					curWorldPositionOnScreen, Time.deltaTime);	
				break;
			}
			
			StopTrace(eTraceType.DOUBLE);
			
//			if(m_SelectedObject != null)
//			{
//				//test
//				DispatchInputMessage(eInputType.DOUBLE_UP, m_SelectedObject, Input.mousePosition, Vector3.zero,
//					GetPositionOnBaseLand(Input.mousePosition), Time.deltaTime);
//			}
			
			m_SelectedObject = null;
		}
		#endregion
		#region - zoom in & out
		float fValue = Input.GetAxis("Mouse ScrollWheel");
		if(fValue < 0) 
		{ 			
			CameraMgr.Instance.SetZoomOutState(50.0f);
		  	
		} 
		else if(fValue > 0) 
		{ 		
			CameraMgr.Instance.SetZoomInState(50.0f);		
		}
		#endregion
	}
	#endregion
	
	#region - single click -
	bool m_SingleClicked = true;
	bool m_SingleClickDeciding = false;
	IEnumerator SingleClickDecision()
	{
		m_SingleClickDeciding = true;
		
//		yield return new WaitForSeconds(m_DoubleClickTime);
//		yield return new WaitForSeconds(m_AutoMoveHoldTime);
//		yield return new WaitForSeconds(0);
		yield return null;
		
		if(m_SingleClicked == true)
		{
#if DEBUG_INPUT
			Debug.Log( "Single click");
#endif
			
#if GAME_PLAYING
			//AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(m_StoredMessage);
			DecideSingleTouchInput(m_StoredMessage);
#endif
		}
		
		m_SingleClickDeciding = false;
		m_SingleClicked = true;
	}
	#endregion
	#region - tracing -
	bool m_TraceBegan = false;
	void StartTrace(eTraceType _type)
	{
		if(m_TraceBegan == false)
		{
			switch(_type)
			{
			case eTraceType.SINGLE:
				StartCoroutine("StartDragTrace");
				break;
			case eTraceType.DOUBLE:
				StartCoroutine("StartShakeTrace");
				break;
			}
			
			m_TraceBegan = true;
		}
	}
	void StopTrace(eTraceType _type)
	{
		switch(_type)
		{
		case eTraceType.SINGLE:
			StopCoroutine("StartDragTrace");
			break;
		case eTraceType.DOUBLE:
			StopCoroutine("StartShakeTrace");
			break;
		}

		m_TraceBegan = false;
	}
	
	List<DirectionUnit> m_listTrace = new List<DirectionUnit>();
	List<DirectionUnit> m_listArcTrace = new List<DirectionUnit>();
	
	IEnumerator StartDragTrace()
	{
		m_listArcTraceBase.Clear();
		m_listArcTraceBase.Add(GetCurrentScreenPos());
		_AddTraceBuf( true, false); // ilmeda 20120430
		
		while(true)
		{
			yield return new WaitForSeconds(m_CheckDragInterval);
			
			DragProcess();
		}
	}
	
	IEnumerator StartShakeTrace()
	{
		m_listTrace.Clear();
		m_listTrace.Add(new DirectionUnit(GetCurrentScreenPos(), GetCurrentScreenPos()));
		_AddTraceBuf( true, true); // ilmeda 20120420
		
		while(true)
		{
			yield return new WaitForSeconds(m_CheckDragInterval);

			ShakeProcess();
			
//			Debug.LogWarning("AsInputManager::CheckDragInput: Thred is WORKING ?!");
		}
	}
	#endregion
	#region - shake -
	void ShakeProcess()
	{
		//save trace
		m_listTrace.Add(new DirectionUnit(GetCurrentScreenPos(), m_listTrace[m_listTrace.Count-1].m_Pos));
		_AddTraceBuf( false, true); // ilmeda 20120420
		
		if(m_listTrace.Count >5)
		{
			Vector3 preDir1 = m_listTrace[m_listTrace.Count-3].m_Direction;
			Vector3 preDir2 = m_listTrace[m_listTrace.Count-2].m_Direction;
			Vector3 curDir = m_listTrace[m_listTrace.Count-1].m_Direction;
			
			float shakeMoving = preDir1.magnitude + preDir2.magnitude + curDir.magnitude;
			//Debug.Log("AsInputmanager::SetScreenPositionTrace: shakeMoving: " + shakeMoving);
			//preDir1.magnitude + "," + preDir2.magnitude + ","  + curDir.magnitude);
			if(shakeMoving > m_ShakeMovingThreshold)
			{
				Vector3 shakeReverse = preDir1 + preDir2 + curDir;
				//Debug.Log("AsInputmanager::SetScreenPositionTrace: shakeReverse: " + shakeReverse.magnitude);
				if(shakeReverse.magnitude < m_ShakeReverseThreshold)
				{
					Debug.Log( "AsInputmanager::SetScreenPositionTrace: it's shaked");
				}
			}	
		}
		
		//release exceeded trace
		if(m_listTrace.Count > m_MaxTraceCount)
		{
			m_listTrace.Clear();
			m_listTrace.Add(new DirectionUnit(GetCurrentScreenPos(), GetCurrentScreenPos()));
			_AddTraceBuf( false, true); // ilmeda 20120420
		}
	}
	#endregion
	#region - drag dash & arc & circle -
	// TRACING
	List<Vector3> m_listArcTraceBase = new List<Vector3>();
	void DragProcess()
	{
		Vector3 curPos = GetCurrentScreenPos();
		if(m_listArcTraceBase[m_listArcTraceBase.Count - 1] != curPos)
			m_listArcTraceBase.Add(curPos);
		
		_AddTraceBuf( false, false); // ilmeda 20120430
	}
	
	bool CheckDragInput()
	{
		#region - concern frame rate -
		int trashCount = 0;
		int ignoreCount = 0;
		
//		int trashCount = 3;
//		int ignoreCount = 1;
//		
//		if(0.05f < Time.deltaTime || 0.05f < m_CheckDragInterval)
//		{
//			trashCount = 0;
//			ignoreCount = 1;
//		}
//		else if(0.04f < Time.deltaTime || 0.04f < m_CheckDragInterval)
//		{
//			trashCount = 1;
//			ignoreCount = 1;
//		}
		#endregion
		
		m_listArcTrace.Clear();		
		for(int i=1; i<m_listArcTraceBase.Count; ++i)
		{
			m_listArcTrace.Add(new DirectionUnit(m_listArcTraceBase[i - 1], m_listArcTraceBase[i]));
		}
		
#if DEBUG_INPUT
		Debug.Log("AsInputManager::CheckDragInput: m_listArcTrace:" + m_listArcTrace.Count);
#endif
		
		if(m_listArcTrace.Count < ignoreCount)
		{
			m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120427
			m_CurInputState = eInputType.SINGLE_DRAG_END; // ilmeda 20120427
			return false;
		}
		
		Vector3 totDirection = Vector3.zero;
		float totMagnitude = 0;
		float totAngle = 0;
		int validAngleQuantity = 0;
		bool smoothDragging = true;
		Vector3 totPos = Vector3.zero;
		
		List<float> listMagnitude = new List<float>();
		
		for(int i=trashCount; i<m_listArcTrace.Count - (trashCount + 1); ++i)
		{
			DirectionUnit unit1 = m_listArcTrace[i];
			DirectionUnit unit2 = m_listArcTrace[i + 1];
			
			//CALCULATE DIRECTION
			totDirection += unit1.m_Direction;
			
			//CALCULATE MAGNITUDE
			totMagnitude += unit1.m_Direction.magnitude;
#if DEBUG_INPUT
			listMagnitude.Add(unit1.m_Direction.magnitude);
#endif
			
			//CALCULATE AVERAGE POSITION
			totPos += unit1.m_Pos;
			
#if DEBUG_INPUT
//			Debug.Log("AsInputManager::CheckDragInput: unit1.m_Direction.magnitude:" + unit1.m_Direction.magnitude);
#endif
			//CALCULATE ANGLE
			float angle = Vector3.Angle(unit1.m_Direction, unit2.m_Direction);
#if DEBUG_INPUT
//			Debug.Log("AsInputManager::CheckDragInput: angle:" + angle + "(" + unit1.m_Direction + ", " + unit2.m_Direction + ")");
#endif
			totAngle += angle;
			++validAngleQuantity;
			
			if(angle > m_SmoothAngle && trashCount < i && i < m_listArcTrace.Count - trashCount)
				smoothDragging = false;
		}
		
		if(m_listArcTrace.Count == 0)
			return false;
		
		DirectionUnit head = m_listArcTrace[0];
		DirectionUnit tail = m_listArcTrace[m_listArcTrace.Count - 1];

		float directMagnitude = (tail.m_Pos - head.m_Pos).magnitude;
		
		Vector3 avgPos = totPos / (m_listArcTrace.Count - 1);
		
#if DEBUG_INPUT	
//		Debug.Log("AsInputManager::CheckDragInput: totDirection:" + totDirection);
//		Debug.Log("AsInputManager::totMagnitude: totMagnitude:" + totMagnitude);
//		Debug.Log("AsInputManager::CheckDragInput: totAngle:" + totAngle);
//	
//		Debug.Log("AsInputManager::CheckDragInput: tail & head = " + tail.m_Pos + "," + head.m_Pos);
//		Debug.Log("AsInputManager::CheckDragInput: tail - head = " + (tail.m_Pos - head.m_Pos));
//		
//		Debug.Log("AsInputManager::CheckDragInput: directMagnitude -- " + directMagnitude);
//		Debug.Log("AsInputManager::CheckDragInput: Magnitude Difference -- " + (totMagnitude - directMagnitude));
#endif		
		if(smoothDragging == true)
		{
#if DEBUG_INPUT
//			Debug.Log(string.Format("[Drag] totMagnitude({0}) - directMagnitude({1}) < " +
//				"m_StraightUpperBound + Mathf.Sqrt(totMagnitude) * m_StraightUpperBoundRevision({2})",
//				totMagnitude, directMagnitude, Mathf.Sqrt(totMagnitude) * m_StraightUpperBoundRevision));
#endif
			float ratio =  totMagnitude / directMagnitude;
			if(ratio < m_DirectInputRatio)//drag
			{
//				Debug.Log("AsInputManager::CheckDragInput: totAngle -- " + totAngle);
				
				#region - straight -
				Vector3 direction = tail.m_Pos - head.m_Pos;
				direction.z = direction.y;direction.y = 0;
#if DEBUG_INPUT				
				Debug.Log("AsInputManager::CheckDragInput: Straight [totMagnitude / directMagnitude(" + ratio + ")] < [m_DirectInputRatio(" + m_DirectInputRatio + ")]");
#endif
#if GAME_PLAYING
				m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120418
				
				AsEntityManager.Instance.UserEntity.HandleMessage(
				//AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(
					new Msg_Input_DragStraight(head.m_Pos, (head.m_Pos + tail.m_Pos)/2, tail.m_Pos, direction));
				
				
				m_CurInputState = eInputType.SINGLE_DRAG_END; // ilmeda 20120427
#endif
				return true;
				#endregion
			}
			else
			{
				if(m_listArcTraceBase.Count < 3)
					return false;
				
				#region - calculate trace -
				float curTotMagnitude = 0;
				Vector3 centerPos = Vector3.zero;
				
				for(int i=1; i<m_listArcTrace.Count - 1; ++i)
				{
					DirectionUnit unit = m_listArcTrace[i];						
					curTotMagnitude += unit.m_Direction.magnitude;
					if(curTotMagnitude > totMagnitude * 0.5f)
					{
						if(centerPos == Vector3.zero)
						{
							centerPos = (unit.m_Pos + m_listArcTrace[i - 1].m_Pos) * 0.5f;
						}
					}
				}
				
//				if(centerPos == Vector3.zero)
//					centerPos = avgPos;
				
//				Vector3 lineMiddlePos = m_listArcTrace[m_listArcTrace.Count/2].m_Pos;
				Vector3 middlePos = GetMiddlePoint(m_listArcTraceBase);
				Vector3 vFirst = m_listArcTraceBase[0] - middlePos;
				Vector3 vLast = m_listArcTraceBase[2] - middlePos;
				
				Vector3 cross = Vector3.Cross(vFirst, vLast);
				
				eClockWise cw;
				
				if(cross.z < 0)
				{
					if(showInputLog == true) Debug.Log("clockwise rotation");
					cw = eClockWise.CW;
				}
				else
				{
					if(showInputLog == true) Debug.Log("counterclockwise rotation");
					cw = eClockWise.CCW;
				}
				#endregion
			
#if DEBUG_INPUT
//				Debug.Log(string.Format("[Circle] directMagnitude({0}) < " +
//					"m_CircleUpperBound({1}) + Mathf.Sqrt(totMagnitude) * m_CircleUpperBoundRevision({2})",
//					directMagnitude, m_CircleUpperBound, Mathf.Sqrt(totMagnitude) * m_CircleUpperBoundRevision));
#endif
				if(ratio > m_CircleInputRatioUpperBound)//circle
				{
					#region - circle -
#if DEBUG_INPUT
					Debug.Log("AsInputManager::CheckDragInput: Circle [totMagnitude / directMagnitude(" + ratio + ")] > [m_CircleInputRatioUpperBound(" + m_CircleInputRatioUpperBound + ")]");
#endif
#if GAME_PLAYING
					m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120419
					
					//Vector3 center = (head.m_Pos + tail.m_Pos)/2;
					Vector3 center = centerPos;
					Vector3 std = new Vector3(Screen.width/2, Screen.height/2, 0);
					Vector3 direction = center - std;
					direction.z = direction.y;direction.y = 0;
					
					
					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(
						new Msg_Input_Circle(head.m_Pos, avgPos, tail.m_Pos, direction, cw));
				
					
					m_CurInputState = eInputType.SINGLE_DRAG_END; // ilmeda 20120427
#endif
					return true;
					#endregion
				}
				else
				{
#if DEBUG_INPUT					
//					Debug.Log(string.Format("[Arc] (totMagnitude - directMagnitude)({0}) < " +
//						"m_ArcUpperBound({1}) + Mathf.Sqrt(totMagnitude) * m_ArcUpperBoundRevision({2})",
//						totMagnitude - directMagnitude, m_ArcUpperBound, Mathf.Sqrt(totMagnitude) * m_ArcUpperBoundRevision));
#endif

					if(showInputLog == true)
					{
						string strMag = "";
						foreach(float mag in listMagnitude)
						{
							strMag += mag + ", ";
						}
						Debug.Log("AsInputManager::CheckDragInput: total magnitude = " + strMag);
						strMag = "";
						for(int i=0; i<m_listArcTraceBase.Count - 1; ++i)
						{
							strMag += m_listArcTraceBase[i] + ", ";
						}
						Debug.Log("AsInputManager::CheckDragInput: total position = " + strMag);
						Debug.Log("AsInputManager::CheckDragInput: [ratio(" + ratio + ")] ? [directMagnitude(" + directMagnitude + ")]");
						Debug.Log("AsInputManager::CheckDragInput: deciding arc or straight. distance between middle and center is " + Vector3.Distance(middlePos, centerPos));
					}
					if(Vector3.Distance(middlePos, centerPos) > directMagnitude * middleAndCenterRevision_)
					{
						#region - arc -
#if DEBUG_INPUT						
						Debug.Log("AsInputManager::CheckDragInput: Arc [Distance(middlePos, centerPos)(" + Vector3.Distance(middlePos, centerPos) + ")] > [directMagnitude * middleAndCenterRevision_(" + directMagnitude * middleAndCenterRevision_ + ")]");
#endif						
#if GAME_PLAYING
						m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120419
						
						//Vector3 center = (head.m_Pos + tail.m_Pos)/2;
						Vector3 center = centerPos;
						Vector3 std = new Vector3(Screen.width/2, Screen.height/2, 0);
//						Vector3 direction = center - std;
//						direction.z = direction.y;direction.y = 0;
						
						Vector3 direction = tail.m_Pos - head.m_Pos;
						switch(cw)
						{
						case eClockWise.CW:
							float temp = direction.x; direction.x = -direction.y; direction.y = temp;
							break;
						case eClockWise.CCW:
							temp = direction.x; direction.x = direction.y; direction.y = -temp;
							break;
						}
						
						direction += std;
						
						
//						Debug.Log("AsInputManager::CheckDragInput: input arc [center(" + center + ")][avgPos(" + avgPos + ")]");
						if(centerPos == Vector3.zero)
							center = avgPos;
						
						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(
							new Msg_Input_Arc(head.m_Pos, center, tail.m_Pos, direction, cw));
					
						m_CurInputState = eInputType.SINGLE_DRAG_END; // ilmeda 20120427
#endif
						return true;
						#endregion
					}
					else
					{
						#region - straight -
#if DEBUG_INPUT							
						Debug.Log("AsInputManager::CheckDragInput: Straight [Distance(middlePos, centerPos)(" + Vector3.Distance(middlePos, centerPos) + ")] < [directMagnitude * middleAndCenterRevision_(" + directMagnitude * middleAndCenterRevision_ + ")]");
#endif						
						Vector3 direction = tail.m_Pos - head.m_Pos;
						direction.z = direction.y;direction.y = 0;
		#if DEBUG_INPUT				
						Debug.Log("AsInputManager::CheckDragInput: Drag");
		#endif
		#if GAME_PLAYING
						m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120418
						
						
						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(
							new Msg_Input_DragStraight(head.m_Pos, (head.m_Pos + tail.m_Pos)/2, tail.m_Pos, direction));
						
						
						m_CurInputState = eInputType.SINGLE_DRAG_END; // ilmeda 20120427
		#endif
						#endregion
						return true;
					}
				}

			}
#if DEBUG_INPUT
			Debug.Log("AsInputManager::CheckDragInput: Ambiguous input. ignore this.");
#endif
		}
		else
#if DEBUG_INPUT			
			Debug.Log("It is not smooth moving. No input occured");
#endif
		
		
		m_eInputEvent = eInputEventType.ONE_FINGER_DRAG_END; // ilmeda 20120419

		return false;
	}
	
	Vector3 GetMiddlePoint(List<Vector3> _listTrace)
	{
		Vector3 totPos = Vector3.zero;
		
		foreach(Vector3 pos in _listTrace)
		{
			totPos += pos;
		}
		
		return totPos / _listTrace.Count; 
	}
	#endregion
	
	#region - function -
	void DecideSingleTouchInput( Msg_Input _single)
	{
//		Debug.Log( "layer : " + LayerMask.LayerToName( _single.inputObject_.layer) + " Name : " + _single.inputObject_.name);
		
		if( null == _single.inputObject_)
			return;
		
		string layerName = LayerMask.LayerToName( _single.inputObject_.layer);
		switch( layerName)
		{
		case "Terrain":
		case "Item":
		case "Potal":
			{
				AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
				if( null != user)
				{
					if( _single.worldPosition_ != Vector3.zero)
					{
						user.HandleMessage( new Msg_Input_Move( _single.worldPosition_));
						m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_MOVE; // ilmeda 20120418
					}
					else
					{
						Debug.LogWarning("AsInputManager:: DecideSingleTouchInput: input process is ignored. _single.worldPosition_ = " + _single.worldPosition_);
					}
				}
			}
			break;
		case "Monster":
			{
				AsNpcEntity enemy = AsEntityManager.Instance.GetEntityByInstanceId( _single.inputObject_.GetInstanceID()) as AsNpcEntity;
				if( null != enemy)
				{
					if( enemy.isSegregate == false)
					{
						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_Input_Attack( enemy));
						m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK; // ilmeda 20120418
					}
				}
				else
				{
					Debug.LogError( "AsInputManager::DecideSingleTouchInput: invalid enemy selection.");
				}
			}
			break;
		case "Object":
			{
				
				AsNpcEntity npcObject = AsEntityManager.Instance.GetEntityByInstanceId( _single.inputObject_.GetInstanceID()) as AsNpcEntity;
				if( null != npcObject)
				{	
				
					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_NpcClick(npcObject.SessionId));
				
					int iGroupIndex = npcObject.GetProperty<int>( eComponentProperty.GROUP_INDEX);
					if( 0 >= iGroupIndex)
					{
						/*AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_Input_Move( _single.worldPosition_));
						m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_MOVE; // ilmeda 20120418*/				
						
						//AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage(new Msg_NpcClick(npcObject.SessionId) );
						//AsEntityManager.Instance.BroadcastMessageToAllEntities( new Msg_NpcClick(npcObject.SessionId) );
						AsEntityManager.Instance.DispatchMessageByNpcSessionId(npcObject.SessionId, new Msg_NpcClick(npcObject.SessionId) );
					}
					else
					{
		                int iLinkIndex = npcObject.GetProperty<int>( eComponentProperty.LINK_INDEX);
		                int iDeiff = Mathf.Abs( iLinkIndex - AsEntityManager.Instance.UserEntity.linkIndex_);
		                if( AsEntityManager.Instance.UserEntity.linkIndex_ != 0 && ( iLinkIndex == AsEntityManager.Instance.UserEntity.linkIndex_ || iDeiff > 1) )
		                {
		                    Debug.Log( "LinkIndex return");
		                }
		                else
						{
		                    ObjSteppingMgr.Instance.ClickObject( iGroupIndex, npcObject, _single.worldPosition_);
		                }
					}
				}
				else
				{
					Debug.LogError( "AsInputManager::DecideSingleTouchInput: invalid Object selection.");
				}
			}
			break;
		case "OtherUser":
			AsUserEntity otherUser = AsEntityManager.Instance.GetEntityByInstanceId( _single.inputObject_.GetInstanceID()) as AsUserEntity;
			if( null != otherUser)// && otherUser.Hide == false)
			{
				AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_OtherUserClick( otherUser.UniqueId));
				m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK;
			}
			break;
		case "Npc":
			AsNpcEntity enemy = AsEntityManager.Instance.GetEntityByInstanceId( _single.inputObject_.GetInstanceID()) as AsNpcEntity;
			if( null != enemy)
			{
				if( enemy.isNoWarpIndex )
				{
					if(enemy.GetClickable() == true)
					{
						AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_NpcClick(enemy.SessionId));
						m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK; // ilmeda 20120418
					}
				}
				else
				{
					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_NpcClick(enemy.SessionId));
					m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK; // ilmeda 20120418
				}				
			}
			break;
		case "Collection":
			AsNpcEntity _collection = AsEntityManager.Instance.GetEntityByInstanceId( _single.inputObject_.GetInstanceID()) as AsNpcEntity;
			if( null != _collection )
			{
				eITEM_PRODUCT_TECHNIQUE_TYPE _type = (eITEM_PRODUCT_TECHNIQUE_TYPE)_collection.GetProperty<int>(eComponentProperty.COLLECTOR_TECHNIC_TYPE);
				
				bool bCanCollect = false;
				if (_collection.collectionMark != null)
					 bCanCollect = _collection.collectionMark.Visible;
				
				if( _type != eITEM_PRODUCT_TECHNIQUE_TYPE.QUEST || true == bCanCollect )
				{
					AsUserInfo.Instance.GetCurrentUserEntity().HandleMessage( new Msg_CollectClick(_collection.SessionId));
					AsEntityManager.Instance.DispatchMessageByNpcSessionId(_collection.SessionId, new Msg_NpcClick(_collection.SessionId) );
					m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK; // ilmeda 20120418
				}
			}
			break;
		case "Player":
			AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
			if(user != null)
				user.HandleMessage(new Msg_PlayerClick());
			break;
		}
	}
	
	void DecideDoubleTouchInput(Msg_Input _double)
	{
		if(_double.inputObject_ != null)
		{
			AsUserEntity user = AsUserInfo.Instance.GetCurrentUserEntity();
			
			if(_double.inputObject_.layer == LayerMask.NameToLayer("Terrain"))
			{		
//				if(user != null)
//				{
////					Vector3 direction = _double.screenPosition_ - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
//					Vector3 direction = _double.worldPosition_ - user.transform.position;
////					direction.z = direction.y;direction.y = 0;
//					
//					user.HandleMessage(new Msg_Input_Dash(direction.normalized));
//				} 
				
				if( null != user )
				{
					Msg_Input_DoubleTab input = new Msg_Input_DoubleTab(_double, Msg_Input_DoubleTab.eDoubleTabType.Terrain);				
					user.HandleMessage(input);
				}
				
				
			}
			else if(_double.inputObject_.layer == LayerMask.NameToLayer("Player"))
			{
				Msg_Input_DoubleTab input = new Msg_Input_DoubleTab(_double, Msg_Input_DoubleTab.eDoubleTabType.Player);				
					user.HandleMessage(input);
			}
			else if(_double.inputObject_.layer == LayerMask.NameToLayer("OtherUser"))
			{
			}
			else if(_double.inputObject_.layer == LayerMask.NameToLayer("Monster"))
			{
				AsNpcEntity enemy = AsEntityManager.Instance.GetEntityByInstanceId( _double.inputObject_.GetInstanceID()) as AsNpcEntity;
				if( null != enemy && enemy.isSegregate == false)
				{
					Msg_Input_DoubleTab input = new Msg_Input_DoubleTab(_double, Msg_Input_DoubleTab.eDoubleTabType.Monster);				
					user.HandleMessage(input);
				}
				
			}
			
			m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_ATTACK; // ilmeda 20120716
		}
	}
	
	bool m_HoldMoveBegan = false;
	void BeginHoldMove()
	{
		if(m_HoldMoveBegan == false)
		{
			m_HoldMoveBegan = true;

			if (ArkQuestmanager.instance.CheckHaveOpenUIType(OpenUIType.USE_TWO_TOUCH_MOVE) != null)
				AsCommonSender.SendClearOpneUI(OpenUIType.USE_TWO_TOUCH_MOVE);

			StartCoroutine("HoldMoveProcess");
		}
	}
	
	IEnumerator HoldMoveProcess()
	{
		while(true)
		{
			yield return new WaitForSeconds(m_HeldPositionInterval);
			
			if(m_HoldMoveBegan == false)
				yield break;
			
			AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
			if( null != userEntity)
			{
				Vector3 screenPos = GetCurrentScreenPos();
				if(screenPos == Vector3.zero || m_InputOnUI == true)
					screenPos = m_LastStayedPos;
				else
					m_LastStayedPos = screenPos;
					
				Vector3 dir = GetPositionOnBaseLand(screenPos) - userEntity.transform.position;
				if(dir.sqrMagnitude > m_fMin_sqrMagnitude)
				{
					dir = dir.normalized * 7;
					
					Msg_Input_Move move = new Msg_Input_Move(userEntity.transform.position + dir);
					userEntity.HandleMessage(move);
					
					_SetOneFingerHoldEvent(screenPos);
				}
			}
			
//			_SetOneFingerHoldEvent();
		}
	}
	
	void StopHoldMove()
	{
		m_HoldMoveBegan = false;
		StopCoroutine("HoldMoveProcess");
	}
	
	public void BlockHoldMove()
	{
		m_HoldMoveBegan = false;
	}
	
	void InputOnUI()
	{
		StartCoroutine(InputOnUI_CR());
	}
	
	bool m_InputOnUI = false; public bool inputOnUI{get{return m_InputOnUI;}}
	IEnumerator InputOnUI_CR()
	{
		m_InputOnUI = true;
		
		yield return new WaitForSeconds(m_HeldPositionInterval + 0.1f);
		
		m_InputOnUI = false;
	}
	
	Msg_Input m_StoredMessage;
	void StoreInputMessage(eInputType _type, GameObject _inputObject, Vector3 _screenPosition, 
		Vector3 _deltaPosition, Vector3 _worldPosition, float _deltaTime)
	{
		m_StoredMessage = new Msg_Input(_type, _inputObject, _screenPosition,
				                                _deltaPosition, _worldPosition, _deltaTime);
	}
	
	Vector3 m_LastStayedPos;
	Vector2 GetCurrentScreenPos()
	{
		Vector3 pos = Vector3.zero;
		
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
		{
			if(Input.touchCount == 1)
				pos = Input.GetTouch(0).position;
			else if(Input.touchCount == 2)
				pos = (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f;
		}
		else
		{
			pos = Input.mousePosition;
		}
		
		return pos;
	}
	
	public static GameObject GetObjectOnScreenPosition(Vector3 _pos)
	{
		if( null == Camera.mainCamera)
			return null;
		
		Ray ray = Camera.mainCamera.ScreenPointToRay( _pos);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, 10000f, (-1) - (1 << LayerMask.NameToLayer("Hide"))) == true)
			return hit.collider.gameObject;
		else
			return null;
	}

	public static RaycastHit GetHitInfoOnScreenPosition(Vector3 _pos)
	{
		if( null == Camera.mainCamera)
			return new RaycastHit();
		
		Ray ray = Camera.mainCamera.ScreenPointToRay(_pos);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, 10000f) == true)
			return hit;
		else
			return new RaycastHit();;
	}
	
	static bool s_showInputLog = false;
	public static Vector3 GetPositionOnBaseLand(Vector3 _pos)
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay(_pos);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, 10000f, 1<<LayerMask.NameToLayer("Terrain")) == true)
			return hit.point;
		else
			return Vector3.zero;
	}
	#endregion
	#region - struct -
	public class DirectionUnit
	{
		public Vector3 m_Pos;
		public Vector3 m_Direction;
		
		public DirectionUnit(Vector3 _curPos, Vector3 _prePos)
		{
			m_Pos = _curPos;
			//m_Direction = (_curPos - _prePos).normalized;
			m_Direction = _curPos - _prePos;
		}
	}
	#endregion
	
	#region - ilmeda -
	// < ilmeda 20120418
	public eInputEventType eInputEvent
	{
		get{ return m_eInputEvent;}
		set{ m_eInputEvent = value;}
	}
	
	private void _SetTwoFingerTouchEvent()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.touchCount == 2)
			{
				m_vTwoFingerTouchScreenPos[0] = Input.GetTouch(0).position;
				m_vTwoFingerTouchScreenPos[1] = Input.GetTouch(1).position;
			}
		}
		else
		{
			m_vTwoFingerTouchScreenPos[0] = Input.mousePosition;
		}
		
		m_eInputEvent = eInputEventType.TWO_FINGER_TOUCH;
		m_vAutoRunPickPos = GetPositionOnBaseLand( GetCurrentScreenPos());
	}
	
	//$yde
	private void _SetOneFingerHoldEvent(Vector3 _pos)
	{
		m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_HOLD;
		m_vAutoRunPickPos = GetPositionOnBaseLand( _pos);
	}
	
	private void _SetOneFingerHoldEvent()
	{
		m_eInputEvent = eInputEventType.ONE_FINGER_TOUCH_HOLD;
		m_vAutoRunPickPos = GetPositionOnBaseLand( GetCurrentScreenPos());
	}
	
	private bool _isPlayCharSkillActive()
	{
		if( null == AsEntityManager.Instance)
			return false;
		
		AsPlayerFsm playerFsm = AsEntityManager.Instance.GetPlayerCharFsm();
		if( playerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_HIT
			|| playerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_READY
			|| playerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.SKILL_FINISH
			|| playerFsm.CurrnetFsmStateType == AsPlayerFsm.ePlayerFsmStateType.DASH)
			return true;
		
		return false;
	}

	public Vector2 GetTwoFingerTouchScreenPos(int nIndex) { return m_vTwoFingerTouchScreenPos[nIndex];}
	public Vector3 vStoredWorldPos { get{ return m_StoredMessage.worldPosition_;}}
	public Vector3 vStoredScreenPos { get{ return m_StoredMessage.screenPosition_;}}
	public int GetTraceBufCount_1() { return m_listTraceBuf1.Count;}
	public Vector3 GetTraceBufPos_1(int i) { return m_listTraceBuf1[i].m_Pos;}
	public int GetTraceBufCount_2() { return m_listTraceBuf2.Count;}
	public Vector3 GetTraceBufPos_2(int i) { return m_listTraceBuf2[i].m_Pos;}
	public Vector3 GetAutoRunPickPos() { return m_vAutoRunPickPos;}
//	public Vector3 GetCurAutoRunPickPos()
//	{
//		Vector3 screenPos = GetCurrentScreenPos();
//		if(screenPos == Vector3.zero)
//			screenPos = m_LastStayedPos;
//		
//		//return GetPositionOnBaseLand( screenPos);
//		
//		AsUserEntity userEntity = AsUserInfo.Instance.GetCurrentUserEntity();
//		if( null != userEntity)
//		{
//			Vector3 vNewPos = GetPositionOnBaseLand( screenPos);
//			Vector3 dir = vNewPos - userEntity.transform.position;
//			if( dir.sqrMagnitude > m_fMin_sqrMagnitude)
//				return vNewPos;
//		}
//		
//		return Vector3.zero;
//	}
	public eInputType GetCurInputState() { return m_CurInputState;}
	// ilmeda 20120418 >

	// < ilmeda 20120420
	// use trace only
	List<DirectionUnit> m_listTraceBuf1 = new List<DirectionUnit>();
	List<DirectionUnit> m_listTraceBuf2 = new List<DirectionUnit>();
	
	private void _AddTraceBuf(bool isClearBuf, bool isUsedTwoBuf)
	{
		if( true == isClearBuf)
		{
			m_listTraceBuf1.Clear();
			if( true == isUsedTwoBuf)
				m_listTraceBuf2.Clear();
		}
		
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if( Input.touchCount > 0)
			{
				Vector3 vTouch0Cur;
				Vector3 vTouch0Pre;
				
				vTouch0Cur = vTouch0Pre = Input.GetTouch(0).position;
				
				if( m_listTraceBuf1.Count > 0)
					vTouch0Pre = m_listTraceBuf1[m_listTraceBuf1.Count-1].m_Pos;
				
				m_listTraceBuf1.Add( new DirectionUnit( vTouch0Cur, vTouch0Pre));
			}
			
			if( Input.touchCount > 1)
			{
				Vector3 vTouch1Cur;
				Vector3 vTouch1Pre;
				
				vTouch1Cur = vTouch1Pre = Input.GetTouch(1).position;

				if( m_listTraceBuf2.Count > 0)
					vTouch1Pre = m_listTraceBuf2[m_listTraceBuf2.Count-1].m_Pos;
				
				m_listTraceBuf2.Add( new DirectionUnit( vTouch1Cur, vTouch1Pre));
			}
		}
		else
		{
			Vector3 vMouseCur = Input.mousePosition;
			Vector3 vMousePre = Input.mousePosition;
			
			if( m_listTraceBuf1.Count > 0)
				vMousePre = m_listTraceBuf1[m_listTraceBuf1.Count-1].m_Pos;
			
			m_listTraceBuf1.Add( new DirectionUnit( vMouseCur, vMousePre));
		}
	}
	
	private bool _isInputAnyDonw()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if( 1 == Input.touchCount)
			{
				if( Input.GetTouch(0).phase == TouchPhase.Began)
					return true;
			}
			else if( 2 == Input.touchCount)
			{
				if( Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
					return true;
			}
		}
		else
		{
			if( true == Input.GetMouseButtonDown(0) || true == Input.GetMouseButtonDown(1))
				return true;
		}
		
		return false;
	}
	// ilmeda 20120420 >
	#endregion
	
	#region - public -
	public void Warped()
	{
		StopHoldMove();
	}
	
	public void AutoMoveStopped()
	{
		StopHoldMove();
	}
	#endregion
}
