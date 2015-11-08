using UnityEngine;
using System.Collections;

public class AsPartyTrackManager : MonoBehaviour {

	public enum ePartyTrackEvent
	{
		eCompletion_Patch = 0,
		eCompletion_Loading,
		eFirst_Payment,
		eGame_Withdrawal,
		eLevel,
		eJoin_Party,
		eStart_Lobi,
		eStart_LobiRec,
		ePayment_Item_900,
		MAX
	}
	
	private static AsPartyTrackManager m_instance = null;
	public static AsPartyTrackManager Instance
	{
		get
		{
			if( null == m_instance)
			{
				m_instance = FindObjectOfType( typeof(AsPartyTrackManager)) as AsPartyTrackManager;
				if( null == m_instance)
					Debug.Log( "Fail to get AsPartyTrackManager Instance");
			}
			
			return m_instance;
		}
	}
	
	void Awake()
	{
		if( m_instance == null)
		{
			m_instance = this;
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	void SetLevelEvent(int lv)
	{
		Debug.Log("SetLevelEvent"+lv.ToString());
		switch(lv)
		{
		case 2:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25536);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25558);
			#endif
			break;
		case 3:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25537);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25564);
			#endif
			break;
		case 5:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25538);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25565);
			#endif
			break;
		case 10:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25539);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25566);
			#endif
			break;
		case 15:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25540);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25567);
			#endif
			break;
		case 20:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25541);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25568);
			#endif
			break;
		case 25:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25542);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25569);
			#endif
			break;
		case 30:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25543);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25570);
			#endif
			break;
		}
	}
	
	public void SetEvent(ePartyTrackEvent eEventType, int value = 0)
	{
		#if ( !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN)
		int nState = 0;
		switch(eEventType)
		{
		case ePartyTrackEvent.eCompletion_Patch:
			nState = PlayerPrefs.GetInt( "PTE_Completion_Patch"); 
			if(nState == 0 )
			{
				PlayerPrefs.SetInt( "PTE_Completion_Patch", 1);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25531);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25555);
				#endif
			}
			break;
		case ePartyTrackEvent.eCompletion_Loading:
			nState = PlayerPrefs.GetInt( "PTE_Completion_Loading"); 
			if(nState == 0 )
			{
				PlayerPrefs.SetInt( "PTE_Completion_Loading", 1);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25532);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25556);
				#endif

			}
			break;
		case ePartyTrackEvent.eFirst_Payment:
			nState = PlayerPrefs.GetInt( "PTE_First_Payment"); 
			if(nState == 0 )
			{
				PlayerPrefs.SetInt( "PTE_First_Payment", 1);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25534);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25533);
				#endif
			}
			break;
		case ePartyTrackEvent.eGame_Withdrawal:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(25535);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(25557);
			#endif
			break;
		case ePartyTrackEvent.eLevel:
			SetLevelEvent(value);
			break;
		case ePartyTrackEvent.eJoin_Party:
			nState = PlayerPrefs.GetInt( "PTE_Join_Party"); 
			if(nState != System.DateTime.Now.Day )
			{
				PlayerPrefs.SetInt( "PTE_Join_Party", System.DateTime.Now.Day);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25544);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25571);
				#endif
			}
			break;
		case ePartyTrackEvent.eStart_Lobi:
			nState = PlayerPrefs.GetInt( "PTE_Start_Lobi"); 
			if(nState != System.DateTime.Now.Day )
			{
				PlayerPrefs.SetInt( "PTE_Start_Lobi", System.DateTime.Now.Day);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25545);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25572);
				#endif
			}
			break;
		case ePartyTrackEvent.eStart_LobiRec:
			nState = PlayerPrefs.GetInt( "PTE_Start_LobiRec"); 
			if(nState != System.DateTime.Now.Day )
			{
				PlayerPrefs.SetInt( "PTE_Start_LobiRec", System.DateTime.Now.Day);
				#if UNITY_IPHONE 
				Partytrack.sendEvent(25546);
				#endif
				#if UNITY_ANDROID
				Partytrack.sendEvent(25573);
				#endif
			}
			break;
		case ePartyTrackEvent.ePayment_Item_900:
			#if UNITY_IPHONE 
			Partytrack.sendEvent(31149);
			#endif
			#if UNITY_ANDROID
			Partytrack.sendEvent(31147);
			#endif
			break;
		}
		#endif
	}

	public void SendPayment(string item_name, int item_num, string item_price_currency, double item_price)
	{
		Partytrack.sendPayment( item_name,  item_num,  item_price_currency,  item_price);
	}
}
/*
	◆　iOS PartyTrack Event 설정코드						
							
	No.	우선도	이벤트명	비고	성과지점	설정의도	eventId
	1	최우선	Completion_Patch	패치 완료(5～16분 정도의 패치)	인스톨 후 패치 업데이트 완료 시	인스톨 후 패치 업데이트 돌파율 확인	25531
	2	최우선	Completion_Loading	"로딩 완료 후, 기기 인증
(또는 Twitter 첫 로그인 시)"	"위의 패치 업데이트 완료 후, 일반 로딩까지 끝내고 
TOP화면에서 인게임으로 들어간 시점"	로딩 화면에서 메모리 오버로 강제 종료될 가능성이 있기 때문에 로딩 화면의 돌파율을 확인	25532
	3	최우선	First_Payment	첫 과금 시	금액에 관계없이 해당 유저가 처음 과금한 시점	게임 플레이 시작부터 과금에 도달하기까지의 기간 확인(일수)	25534
	4	최우선	Game_Withdrawal	게임 탈퇴 시	TOP화면의 [계정관리(アカウント管理)] 버튼을 통해 탈퇴한 경우	탈퇴 유저 확인	25535
	5	최우선	Lv2	Lv2돌파	Lv2달성 시	부스트(튜토리얼 돌파) 성과지점	25536
	6	최우선	Lv3	Lv3돌파	Lv3달성 시		25537
	7	우선	Lv5	Lv5돌파	Lv5달성 시		25538
	8	우선	Lv10	Lv10돌파	Lv10달성 시	레벨에 따른 이탈율 측정	25539
	9	우선	Lv15	Lv15돌파	Lv15달성 시		25540
	10	우선	Lv20	Lv20돌파	Lv20달성 시		25541
	11	우선	Lv25	Lv25돌파	Lv25달성 시		25542
	12	우선	Lv30	Lv30돌파	Lv30달성 시		25543
	13	보통	Join_Party	파티 참가	2명 이상의 파티를 작성 또는 참가 시	MMORPG의 지속율에 영향을 미치는 게임 내 파티 참가율을 확인	25544
	14	보통	Start_Lobi	Lobi커뮤니티 표시 시	게임 내의 [Lobi] 버튼을 눌러 커뮤니티를 표시했을 시	Lobi 이용율을 확인	25545
	15	보통	Start_LobiRec	LobiRec 녹화 시작 시	LobiRec 녹화 시작 버튼 터치 시	LobiRec 이용율을 확인	25546
	16	최우선	item_900	데일리 미라클 900엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시 과금 상품 구입수를 축정하기 위함	31149
							
		추후 업데이트 시 설정					
	삭제 16	최우선	item_1000	정기과금 1000엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시	과금 상품 구입수를 측정하기 위함	25547
	삭제 17	최우선	item_3000	정기과금 3000엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시		25548
	18	우선	Lv35	Lv35돌파	Lv35달성 시	레벨에 따른 이탈율 측정	25549
	19	우선	Lv40	Lv40돌파	Lv40달성 시		25550
	20	우선	Lv45	Lv45돌파	Lv45달성 시		25551
	21	우선	Lv50	Lv50돌파	Lv50달성 시		25552
	22	우선	Lv55	Lv55돌파	Lv55달성 시		25553
	23	우선	Lv60	Lv60돌파	Lv60달성 시		25554

◆　aOS PartyTrack Event 설정코드												
No.	우선도	이벤트명	비고	설치장소	설정의도	eventId
1	최우선	Completion_Patch	패치 완료(5～16분 정도의 패치)	인스톨 후 패치 업데이트 완료 시	인스톨 후 패치 업데이트 돌파율 확인	25555
2	최우선	Completion_Loading	"로딩 완료 후, 기기 인증
(또는 Twitter 첫 로그인 시)"	"위의 패치 업데이트 완료 후, 일반 로딩까지 끝내고 
TOP화면에서 인게임으로 들어간 시점"	로딩 화면에서 메모리 오버로 강제 종료될 가능성이 있기 때문에 로딩 화면의 돌파율을 확인	25556
3	최우선	First_Payment	첫 과금 시	금액에 관계없이 해당 유저가 처음 과금한 시점	게임 플레이 시작부터 과금에 도달하기까지의 기간 확인(일수)	25533
4	최우선	Game_Withdrawal	게임 탈퇴 시	TOP화면의 [계정관리(アカウント管理)] 버튼을 통해 탈퇴한 경우	탈퇴 유저 확인	25557
5	최우선	Lv2	Lv2돌파	Lv2달성 시	부스트(튜토리얼 돌파) 성과지점	25558
6	최우선	Lv3	Lv3돌파	Lv3달성 시		25564
7	우선	Lv5	Lv5돌파	Lv5달성 시		25565
8	우선	Lv10	Lv10돌파	Lv10달성 시	레벨에 따른 이탈율 측정	25566
9	우선	Lv15	Lv15돌파	Lv15달성 시		25567
10	우선	Lv20	Lv20돌파	Lv20달성 시		25568
11	우선	Lv25	Lv25돌파	Lv25달성 시		25569
12	우선	Lv30	Lv30돌파	Lv30달성 시		25570
13	보통	Join_Party	파티 참가	2명 이상의 파티를 작성 또는 참가 시	MMORPG의 지속율에 영향을 미치는 게임 내 파티 참가율을 확인	25571
14	보통	Start_Lobi	Lobi커뮤니티 표시 시	게임 내의 [Lobi] 버튼을 눌러 커뮤니티를 표시했을 시	Lobi 이용율을 확인	25572
15	보통	Start_LobiRec	LobiRec 녹화 시작 시	LobiRec 녹화 시작 버튼 터치 시	LobiRec 이용율을 확인	25573
16	최우선	item_900	데일리 미라클 900엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시 과금 상품 구입수를 축정하기 위함	31147
						
	추후 업데이트 시 설정					
삭제 16	최우선	item_1000	정기과금 1000엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시	과금 상품 구입수를 측정하기 위함	25574
삭제 17	최우선	item_3000	정기과금 3000엔 아이템 구입	30일간 미라클을 매일 받을 수 있는 정액상품을 구입했을 시		25575
18	우선	Lv35	Lv35돌파	Lv35달성 시	레벨에 따른 이탈율 측정	25576
19	우선	Lv40	Lv40돌파	Lv40달성 시		25577
20	우선	Lv45	Lv45돌파	Lv45달성 시		25578
21	우선	Lv50	Lv50돌파	Lv50달성 시		25579
22	우선	Lv55	Lv55돌파	Lv55달성 시		25580
23	우선	Lv60	Lv60돌파	Lv60달성 시		25581
						
* 16~17번의 경우 지난 10월 22일(수)에 제안 드렸던 VIP시스템에 대한 내용으로 상기와 같이 2가지 상품을 생각하고 있습니다.						
		*/
