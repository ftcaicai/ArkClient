
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public delegate void delPurchaseConfirm( UIPStoreSearchSlot _slot, int _count);
public delegate void delListBtnClicked( UIPStoreSearchSlot _slot);

public class UIPStoreSearchDlg : MonoBehaviour
{
	readonly int MAXCOUNT_PER_PAGE = 8;
	enum eType1 {ALL, EquipItem, CosEquipItem, ActionItem, Enchant, Strengthen, Material}
	
	#region - member -
	
	#region - serialize member -
	// region
	[SerializeField] UIButton btnRegion_01_Btn_ALL;
	[SerializeField] UIButton btnRegion_02_Btn;
	[SerializeField] UIButton btnRegion_03_Btn;
	[SerializeField] UIButton btnRegion;
	[SerializeField] SpriteText txtRegion_title;
	
	// name
	[SerializeField] UITextField txtName_contents;
	[SerializeField] SpriteText txtName_title;
	
	// class
	[SerializeField] UIButton btnClass_01_Btn_ALL;
	[SerializeField] UIButton btnClass_02_Btn;
	[SerializeField] UIButton btnClass_03_Btn;
	[SerializeField] UIButton btnClass_04_Btn;
	[SerializeField] UIButton btnClass_05_Btn;
	[SerializeField] UIButton btnClass;
	[SerializeField] SpriteText txtClass_title;
	
	// type1
	[SerializeField] UIButton btnType1_01_Btn_ALL;
	[SerializeField] UIButton btnType1_02_Btn;
	[SerializeField] UIButton btnType1_03_Btn;
	[SerializeField] UIButton btnType1_04_Btn;
	[SerializeField] UIButton btnType1_05_Btn;
	[SerializeField] UIButton btnType1_06_Btn;
	[SerializeField] UIButton btnType1_07_Btn;
	[SerializeField] UIButton btnType1;
	[SerializeField] SpriteText txtType1_title;
	
	// type2
	[SerializeField] UIButton btnType2_01_Btn_ALL;
	[SerializeField] UIButton btnType2_02_Btn;
	[SerializeField] UIButton btnType2_03_Btn;
	[SerializeField] UIButton btnType2_04_Btn;
	[SerializeField] UIButton btnType2_05_Btn;
	[SerializeField] UIButton btnType2_06_Btn;
	[SerializeField] UIButton btnType2_07_Btn;
	[SerializeField] UIButton btnType2_08_Btn;
	[SerializeField] UIButton btnType2_09_Btn;
	[SerializeField] UIButton btnType2;
	[SerializeField] SpriteText txtType2_title;
	
	// level
	[SerializeField] UITextField txtLevel_contents_max;
	[SerializeField] UITextField txtLevel_contents_min;
	[SerializeField] SpriteText txtLevel_title;
	
	// grade
	[SerializeField] UIButton btnGrade_01_Btn_ALL;
	[SerializeField] UIButton btnGrade_02_Btn;
	[SerializeField] UIButton btnGrade_03_Btn;
	[SerializeField] UIButton btnGrade_04_Btn;
	[SerializeField] UIButton btnGrade_05_Btn;
	[SerializeField] UIButton btnGrade_06_Btn;
	[SerializeField] UIButton btnGrade;
	[SerializeField] SpriteText txtGrade_title;
	
	// dialog control
	[SerializeField] UIButton btnClosing;
	[SerializeField] UIButton btnSearch;
	[SerializeField] SpriteText txtName;
	[SerializeField] UIScrollList listResult;
	
	[SerializeField] UIButton btnLeft;
	[SerializeField] UIButton btnRight;
	[SerializeField] SpriteText txtPage;
	
	[SerializeField] SpriteText txtType1_Icon;
	[SerializeField] UIRadioBtn btnType2_Name;
	[SerializeField] UIRadioBtn btnType3_Grade;
	[SerializeField] UIRadioBtn btnType4_Level;
	[SerializeField] UIRadioBtn btnType5_Price;
	
	[SerializeField] UIPStoreQuantityDlg quantityDlg;
	#endregion
	
	Int32 m_PageIdx = 1;

	Int32 m_ItemTableIdx = 0;
	eCLASS m_Class = eCLASS.NONE;
	Int32 m_Category1 = 0;       //eITEMTYPE
	Int32 m_Category2 = 0;       //eEQUIPITEM_TYPE, eETCITEM_TYPE, eUSEITEM_TYPE, eACTIONITEM_TYPE
	Int32 m_LevelMin = 1;
	Int32 m_LevelMax = 30;
	eEQUIPITEM_GRADE m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_NOTHING;
	Int32 m_MapIdx = 1;
	
	ePRIVATESHOPSEARCHTYPE m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_GRADE_DESCENDING;
	
	// inner member
	eType1 m_CurType1 = eType1.ALL; // just use in this class
	int m_PageMax = 1;
	
	float m_SearchCool = 0f;
	
	int m_LevelLimit = 1;
	
	bool m_SearchName_Up = false;
	body1_SC_PRIVATESHOP_SEARCH_RESULT m_result;
	List<UIPStoreSearchSlot> m_listSlot = new List<UIPStoreSearchSlot>();
	
//	static bool s_Activate = false; public static bool Activate{get{return s_Activate;}}
	#endregion
	#region - init & update & release -
	void Awake()
	{
		#region - set string -
		// region
//		btnRegion_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 1173);
//		btnRegion_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 65001);
//		btnRegion_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 65008);
		btnRegion_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 65001);
		btnRegion_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 65008);
		
		btnRegion.Text =  AsTableManager.Instance.GetTbl_String( 65001);
		txtRegion_title.Text = AsTableManager.Instance.GetTbl_String( 2161);
		
		// name
		txtName_contents.Text = "";
		txtName_title.Text = AsTableManager.Instance.GetTbl_String( 1168);
		
		// class
		btnClass_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 1173);
		btnClass_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 306);
		btnClass_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 307);
		btnClass_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 308);
		btnClass_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 309);
		btnClass.Text = AsTableManager.Instance.GetTbl_String( 1173);
		txtClass_title.Text = AsTableManager.Instance.GetTbl_String( 1250);
		
		// type1
		btnType1_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 1173);
		btnType1_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 2163);
		btnType1_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 1547);
		btnType1_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 2164);
		btnType1_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 1300);
		btnType1_06_Btn.Text = AsTableManager.Instance.GetTbl_String( 2165);
		btnType1_07_Btn.Text = AsTableManager.Instance.GetTbl_String( 2166);
		btnType1.Text = AsTableManager.Instance.GetTbl_String( 1173);
		txtType1_title.Text = AsTableManager.Instance.GetTbl_String( 2162);
			
		// type2. dynamic string init
		btnType2_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 1173);
//		btnType2_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 1032);
//		btnType2_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 1033);
//		btnType2_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 1034);
//		btnType2_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 1035);
//		btnType2_06_Btn.Text = AsTableManager.Instance.GetTbl_String( 1036);
//		btnType2_07_Btn.Text = AsTableManager.Instance.GetTbl_String( 1037);
//		btnType2_08_Btn.Text = AsTableManager.Instance.GetTbl_String( 1038);
//		btnType2_09_Btn.Text = AsTableManager.Instance.GetTbl_String( 1039);
		btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
		txtType2_title.Text = AsTableManager.Instance.GetTbl_String( 2167);
		
		// level
		txtLevel_contents_max.Text = "";
		txtLevel_contents_min.Text = "";
		txtLevel_title.Text = AsTableManager.Instance.GetTbl_String( 1724);
	
		// grade
		btnGrade_01_Btn_ALL.Text = AsTableManager.Instance.GetTbl_String( 1173);
		btnGrade_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 1028);
		btnGrade_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 1029);
		btnGrade_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 1030);
		btnGrade_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 1031);
		btnGrade_06_Btn.Text = AsTableManager.Instance.GetTbl_String( 1699);
		btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1173);
		txtGrade_title.Text = AsTableManager.Instance.GetTbl_String( 2168);
	
		// dialog control
//		btnClosing
		btnSearch.Text = AsTableManager.Instance.GetTbl_String( 1148);
		txtName.Text = AsTableManager.Instance.GetTbl_String( 2160);
		
		txtType1_Icon.Text = AsTableManager.Instance.GetTbl_String( 2169);
		btnType2_Name.Text = AsTableManager.Instance.GetTbl_String( 2170);
		btnType3_Grade.Text = AsTableManager.Instance.GetTbl_String( 2168);
		btnType4_Level.Text = AsTableManager.Instance.GetTbl_String( 1724);
		btnType5_Price.Text = AsTableManager.Instance.GetTbl_String( 2171);
		#endregion
	}
	
	void Start()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
		
		#region - set input delegate -		
		// region
		btnRegion_01_Btn_ALL.SetInputDelegate( Del_RegionBtns);
		btnRegion_02_Btn.SetInputDelegate( Del_RegionBtns);
		btnRegion_03_Btn.SetInputDelegate( Del_RegionBtns);
		btnRegion.SetInputDelegate( Del_RegionBtns);
		
		// name
		txtName_contents.SetValidationDelegate(KeyInputDelegate);

		// class
		btnClass_01_Btn_ALL.SetInputDelegate( Del_ClassBtns);
		btnClass_02_Btn.SetInputDelegate( Del_ClassBtns);
		btnClass_03_Btn.SetInputDelegate( Del_ClassBtns);
		btnClass_04_Btn.SetInputDelegate( Del_ClassBtns);
		btnClass_05_Btn.SetInputDelegate( Del_ClassBtns);
		btnClass.SetInputDelegate( Del_ClassBtns);
		
		// type1
		btnType1_01_Btn_ALL.SetInputDelegate( Del_Type1Btns);
		btnType1_02_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1_03_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1_04_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1_05_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1_06_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1_07_Btn.SetInputDelegate( Del_Type1Btns);
		btnType1.SetInputDelegate( Del_Type1Btns);
		
		// type2. dynamic string init
		btnType2_01_Btn_ALL.SetInputDelegate( Del_Type2Btns);
		btnType2_02_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_03_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_04_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_05_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_06_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_07_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_08_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2_09_Btn.SetInputDelegate( Del_Type2Btns);
		btnType2.SetInputDelegate( Del_Type2Btns);
		
		// level
		txtLevel_contents_max.SetValidationDelegate( Del_KeyInput);
		txtLevel_contents_min.SetValidationDelegate( Del_KeyInput);
//		txtLevel_contents_max.SetCommitDelegate( Del_KeyCommit_Max);
//		txtLevel_contents_min.SetCommitDelegate( Del_KeyCommit_Min);
	
		// grade
		btnGrade_01_Btn_ALL.SetInputDelegate( Del_GradeBtns);
		btnGrade_02_Btn.SetInputDelegate( Del_GradeBtns);
		btnGrade_03_Btn.SetInputDelegate( Del_GradeBtns);
		btnGrade_04_Btn.SetInputDelegate( Del_GradeBtns);
		btnGrade_05_Btn.SetInputDelegate( Del_GradeBtns);
		btnGrade_06_Btn.SetInputDelegate( Del_GradeBtns);
		btnGrade.SetInputDelegate( Del_GradeBtns);
	
		// dialog control
		btnClosing.SetInputDelegate( Del_ClosingBtn);
		btnSearch.SetInputDelegate( Del_SearchBtn);
		
		btnLeft.SetInputDelegate( Del_PageLeftBtn);
		btnRight.SetInputDelegate( Del_PageRightBtn);
		
		btnType2_Name.SetInputDelegate( Del_SortNameBtn);
		btnType3_Grade.SetInputDelegate( Del_SortGradeBtn);
		btnType4_Level.SetInputDelegate( Del_SortLevelBtn);
		btnType5_Price.SetInputDelegate( Del_SortPriceBtn);
		#endregion		
		#region - language -
		// region
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnRegion_01_Btn_ALL.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnRegion_02_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnRegion_03_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnRegion.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtRegion_title);
		
		// name
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtName_contents);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtName_title);
		
		// class
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass_01_Btn_ALL.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass_02_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass_03_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass_04_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass_05_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnClass.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtClass_title);
		
		// type1
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_01_Btn_ALL.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_02_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_03_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_04_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_05_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_06_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1_07_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType1.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtType1_title);
		
		
		// type2. dynamic string init
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_01_Btn_ALL.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_02_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_03_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_04_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_05_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_06_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_07_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_08_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_09_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtType2_title);
		
		// level
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtLevel_contents_max);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtLevel_contents_min);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtLevel_title);
	
		// grade
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_01_Btn_ALL.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_02_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_03_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_04_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_05_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade_06_Btn.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnGrade.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtGrade_title);
	
		// dialog control
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnSearch.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtName);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtPage);
		
		AsLanguageManager.Instance.SetFontFromSystemLanguage( txtType1_Icon);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType2_Name.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType3_Grade.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType4_Level.spriteText);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( btnType5_Price.spriteText);
		#endregion
		
		txtLevel_contents_min.Text = "1";
		m_LevelMax = (int)AsTableManager.Instance.GetTbl_GlobalWeight_Record("MaxLevelLimit").Value; m_LevelLimit = m_LevelMax;
		txtLevel_contents_max.Text = m_LevelMax.ToString();
		
		txtPage.Text = "0 / 1";
		
		CloseAllSubBtns();
		
		quantityDlg.Close();
		
		btnType2_Name.SetGroup( 0);
		btnType3_Grade.SetGroup( 0);
		btnType4_Level.SetGroup( 0);
		btnType5_Price.SetGroup( 0);
		
		m_SearchCool = AsTableManager.Instance.GetTbl_GlobalWeight_Record( "StoreSearch_Cool").Value;
	}
	
	void CloseAllSubBtns()
	{
		CloseRegionDropDown();
		CloseClassDropDown();
		CloseType1DropDown();
		CloseType2DropDown();
		CloseGradeDropDown();
	}
	
	void OnEnable()
	{
		Input.imeCompositionMode = IMECompositionMode.On;
	}
	
	void OnDisable()
	{
		Input.imeCompositionMode = IMECompositionMode.Auto;
	}
	
	void OnDestroy()
	{
		if( transform.parent != null)
			Destroy( transform.parent.gameObject);
	}
	#endregion
	
	#region - delegate -
	
	#region - search option -
	void Del_RegionBtns(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int idx = _GetIndexFromName( ptr);
			RegionProc( idx);
		}
	}
	
	void Del_ClassBtns(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int idx = _GetIndexFromName( ptr);
			ClassProc( idx);
		}
	}
	
	void Del_Type1Btns(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int idx = _GetIndexFromName( ptr);
			Type1Proc( idx);
		}
	}
	
	void Del_Type2Btns(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int idx = _GetIndexFromName( ptr);
			Type2Proc( idx);
		}
	}
	
	string Del_KeyInput( UITextField _field, string _text, ref int _insertionPoint)
	{
		string input = "";
		Int32 num = 0;
		for( int i = 0; i < _text.Length; ++i)
		{
			if( Int32.TryParse( _text[i].ToString(), out num) == true)
			{
				input += num.ToString();
			}
		}
		
		return input;
	}
	
//	void Del_KeyCommit_Min( IKeyFocusable _focus)
//	{
//		int lv = _GetValidLevelNumber( _focus.Content);
//		
//		if( lv <= m_LevelMax)
//		{
//			m_LevelMin = lv;
//			txtLevel_contents_min.Text = lv.ToString();
//		}
//		else
//		{
//			txtLevel_contents_min.Text = m_LevelMin.ToString();
//		}
//	}
	
//	void Del_KeyCommit_Max( IKeyFocusable _focus)
//	{
//		int lv = _GetValidLevelNumber( _focus.Content);
//		
//		if( lv >= m_LevelMin)
//		{
//			m_LevelMax = lv;
//			txtLevel_contents_max.Text = lv.ToString();
//		}
//		else
//		{
//			txtLevel_contents_max.Text = m_LevelMax.ToString();
//		}
//	}
	
//	int _GetValidLevelNumber( string _text)
//	{
//		Int32 num = 0;
//		for( int i = 0; i < _text.Length; ++i)
//		{
//			if( Int32.TryParse( _text[i].ToString(), out num) == true)
//			{
//				input += num.ToString();
//			}
//		}
//		
//		return input;
//	}
	
	void Del_GradeBtns(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			int idx = _GetIndexFromName( ptr);
			GradeProc( idx);
		}
	}
	#endregion
	#region - dialog control -
	void Del_ClosingBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			AsPStoreManager.Instance.ClosePStoreSearch();
		}
	}	
	void Del_SearchBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			SendSearch();
		}
	}	
	void Del_PageLeftBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_PageIdx > 1)
			{
				m_PageIdx--;
				txtPage.Text = m_PageIdx + " / " + m_PageMax;
				SendSearch();
			}
		}
	}
	void Del_PageRightBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_PageIdx < m_PageMax)
			{
				m_PageIdx++;
				txtPage.Text = m_PageIdx + " / " + m_PageMax;
				SendSearch();
			}
		}
	}
	
	// sort
	void Del_SortNameBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			_SortByName();
		}
	}
	void _SortByName()
	{
		m_SearchName_Up = !m_SearchName_Up;

//		m_listSlot.Sort( delegate ( UIPStoreSearchSlot _left, UIPStoreSearchSlot _right){
//			int ret = String.Compare( _left.SimpleName, _right.SimpleName);
		m_listSlot.Sort( delegate ( UIPStoreSearchSlot _left, UIPStoreSearchSlot _right){
			int ret = String.Compare( _left.TextName, _right.TextName);

			if( m_SearchName_Up == true)
				return ret;
			else
				return -ret;
		});

		List<body2_SC_PRIVATESHOP_SEARCH_RESULT> listInfo = new List<body2_SC_PRIVATESHOP_SEARCH_RESULT>();
		foreach( UIPStoreSearchSlot node in m_listSlot)
		{
			listInfo.Add( node.SearchInfo);
		}

		listResult.ClearList( true);

		foreach( body2_SC_PRIVATESHOP_SEARCH_RESULT node in listInfo)
		{
			GameObject objItemBg = Instantiate( Resources.Load( "UI/Optimization/ListItem/SearchStoreDlgListItem")) as GameObject;
			UIPStoreSearchSlot slot = objItemBg.GetComponent<UIPStoreSearchSlot>();
			slot.Init( listResult, node, Del_ListBtnClicked);
		}
	}
	void Del_SortGradeBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_SearchType == ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_GRADE_DESCENDING)
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_GRADE_ASCENDING;
			else
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_GRADE_DESCENDING;
			
			SendSearch();
		}
	}
	void Del_SortLevelBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_SearchType == ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_LEVEL_DESCENDING)
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_LEVEL_ASCENDING;
			else
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_LEVEL_DESCENDING;
			
			SendSearch();
		}
	}
	void Del_SortPriceBtn(ref POINTER_INFO ptr)
	{
		if( ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if( m_SearchType == ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_PRICE_DESCENDING)
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_PRICE_ASCENDING;
			else
				m_SearchType = ePRIVATESHOPSEARCHTYPE.ePRIVATESHOPSEARCHTYPE_PRICE_DESCENDING;
			
			SendSearch();
		}
	}
	#endregion
	#region - purchase -
	void Del_ListBtnClicked( UIPStoreSearchSlot _slot)
	{
		CloseAllSubBtns();
		
		m_SavedSlot = _slot;
		
		TooltipMgr.Instance.OpenTooltip( TooltipMgr.eOPEN_DLG.right, _slot.SearchInfo.sItem, this, "Del_PurchaseBtnClicked", -10.0f);
		UIPStoreSearchSlot.TooltipActivate( true);
	}
	
	void Del_PurchaseBtnClicked()
	{
		OpenQuantityDlg( m_SavedSlot);
	}
	
	void Del_PurchaseConfirm( UIPStoreSearchSlot _slot, int _count)
	{
		quantityDlg.Close();
		
//		m_SavedSlot = _slot;
		m_SavedCount = _count;
		
		string title = AsTableManager.Instance.GetTbl_String(126);
		string content = AsTableManager.Instance.GetTbl_String(407);
		content = string.Format( content, _slot.TextName + Color.white, _count.ToString(), ( _slot.SearchInfo.nItemGold * (long)_count).ToString( "#,#0", CultureInfo.InvariantCulture));
		string left = AsTableManager.Instance.GetTbl_String(1151);
		string right = AsTableManager.Instance.GetTbl_String(1152);
		
		AsNotify.Instance.MessageBox(title, content, right, left, this, "SendPurchaseInfo", "", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL);
	}
	
	UIPStoreSearchSlot m_SavedSlot;
	int m_SavedCount;
	void SendPurchaseInfo()
	{
		body_CS_PRIVATESHOP_ENTER enter = new body_CS_PRIVATESHOP_ENTER( m_SavedSlot.SearchInfo.nPrivateShopUID);
		AsCommonSender.Send(enter.ClassToPacketBytes());
		
		body_CS_PRIVATESHOP_ITEMBUY buy = new body_CS_PRIVATESHOP_ITEMBUY( m_SavedSlot.SearchInfo.nPrivateShopUID,
			m_SavedSlot.SearchInfo.nPrivateShopSlot, (byte)m_SavedCount);
		AsPStoreManager.Instance.Request_ItemBuy(buy);
		
		body_CS_PRIVATESHOP_LEAVE leave = new body_CS_PRIVATESHOP_LEAVE( m_SavedSlot.SearchInfo.nPrivateShopUID);
		AsCommonSender.Send(leave.ClassToPacketBytes());
	}
	#endregion
	
	#endregion
	
	#region - region -
	void RegionProc( int _idx)
	{
		switch( _idx)
		{
//		case 1:
//			btnRegion.Text = AsTableManager.Instance.GetTbl_String( 1173);
//			m_MapIdx = 0;
//			CloseRegionDropDown();
//			break;
		case 1:
			btnRegion.Text = AsTableManager.Instance.GetTbl_String( 65001);
			m_MapIdx = 1;
			CloseRegionDropDown();
			break;
		case 2:
			btnRegion.Text = AsTableManager.Instance.GetTbl_String( 65008);
			m_MapIdx = 8;
			CloseRegionDropDown();
			break;
		case -1:
			CloseAllSubBtns();
			OpenRegionDropDown();
			break;
		default:
			Debug.LogError("UIPStoreSearchDlg::RegionProc: unknown index is named. check region button name");
			break;
		}
	}
	
	void OpenRegionDropDown()
	{
		btnRegion_01_Btn_ALL.gameObject.SetActive( true);
		btnRegion_02_Btn.gameObject.SetActive( true);
//		btnRegion_03_Btn.gameObject.SetActive( true);
	}
	
	void CloseRegionDropDown()
	{
		btnRegion_01_Btn_ALL.gameObject.SetActive( false);
		btnRegion_02_Btn.gameObject.SetActive( false);
		btnRegion_03_Btn.gameObject.SetActive( false);
	}
	#endregion
	#region - class -
	void ClassProc( int _idx)
	{
		switch( _idx)
		{
		case 1:
			btnClass.Text = AsTableManager.Instance.GetTbl_String( 1173);
			m_Class = eCLASS.NONE;
			CloseClassDropDown();
			break;
		case 2:
			btnClass.Text = AsTableManager.Instance.GetTbl_String( 306);
			m_Class = eCLASS.DIVINEKNIGHT;
			CloseClassDropDown();
			break;
		case 3:
			btnClass.Text = AsTableManager.Instance.GetTbl_String( 307);
			m_Class = eCLASS.MAGICIAN;
			CloseClassDropDown();
			break;
		case 4:
			btnClass.Text = AsTableManager.Instance.GetTbl_String( 308);
			m_Class = eCLASS.CLERIC;
			CloseClassDropDown();
			break;
		case 5:
			btnClass.Text = AsTableManager.Instance.GetTbl_String( 309);
			m_Class = eCLASS.HUNTER;
			CloseClassDropDown();
			break;
		case -1:
			CloseAllSubBtns();
			OpenClassDropDown();
			break;
		default:
			Debug.LogError("UIPStoreSearchDlg::ClassProc: unknown index is named. check class button name");
			break;
		}
	}
	
	void OpenClassDropDown()
	{
		btnClass_01_Btn_ALL.gameObject.SetActive( true);
		btnClass_02_Btn.gameObject.SetActive( true);
		btnClass_03_Btn.gameObject.SetActive( true);
		btnClass_04_Btn.gameObject.SetActive( true);
		btnClass_05_Btn.gameObject.SetActive( true);
	}
	
	void CloseClassDropDown()
	{
		btnClass_01_Btn_ALL.gameObject.SetActive( false);
		btnClass_02_Btn.gameObject.SetActive( false);
		btnClass_03_Btn.gameObject.SetActive( false);
		btnClass_04_Btn.gameObject.SetActive( false);
		btnClass_05_Btn.gameObject.SetActive( false);
	}
	#endregion
	#region - type1 -
	void Type1Proc( int _idx)
	{
		switch( _idx)
		{
		case 1:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 1173);
			m_Category1 = (int)Item.eITEM_TYPE.None;
			m_CurType1 = eType1.ALL;
			CloseType1DropDown();
			ResetType2Selection();
			break;
		case 2:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 2163);
			m_Category1 = (int)Item.eITEM_TYPE.EquipItem;
			m_CurType1 = eType1.EquipItem;
			CloseType1DropDown();
			ResetType2Selection();
			break;
		case 3:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 1547);
			m_Category1 = (int)Item.eITEM_TYPE.CosEquipItem;
			m_CurType1 = eType1.CosEquipItem;
			CloseType1DropDown();
			ResetType2Selection();
			break;
		case 4:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 2164);
			m_Category1 = (int)Item.eITEM_TYPE.ActionItem;
			m_CurType1 = eType1.ActionItem;
			CloseType1DropDown();
			ResetType2Selection();
			break;
		case 5:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 1300);
			m_Category1 = (int)Item.eITEM_TYPE.EtcItem;
			m_CurType1 = eType1.Enchant;
			m_Category2 = (int)Item.eEtcItem.Enchant;
			CloseType1DropDown();
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
			break;
		case 6:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 2165);
			m_Category1 = (int)Item.eITEM_TYPE.EtcItem;
			m_CurType1 = eType1.Strengthen;
			m_Category2 = (int)Item.eEtcItem.Strengthen;
			CloseType1DropDown();
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
			break;
		case 7:
			btnType1.Text = AsTableManager.Instance.GetTbl_String( 2166);
			m_Category1 = (int)Item.eITEM_TYPE.EtcItem;
			m_CurType1 = eType1.Material;
			m_Category2 = (int)Item.eEtcItem.Material;
			CloseType1DropDown();
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
			break;
		case -1:
			CloseAllSubBtns();
			OpenType1DropDown();
			break;
		default:
			Debug.LogError("UIPStoreSearchDlg::Type1Proc: unknown index is named. check type1 button name");
			break;
		}
	}
	
	void OpenType1DropDown()
	{
		btnType1_01_Btn_ALL.gameObject.SetActive( true);
		btnType1_02_Btn.gameObject.SetActive( true);
		btnType1_03_Btn.gameObject.SetActive( true);
		btnType1_04_Btn.gameObject.SetActive( true);
		btnType1_05_Btn.gameObject.SetActive( true);
		btnType1_06_Btn.gameObject.SetActive( true);
		btnType1_07_Btn.gameObject.SetActive( true);
	}
	
	void CloseType1DropDown()
	{
		btnType1_01_Btn_ALL.gameObject.SetActive( false);
		btnType1_02_Btn.gameObject.SetActive( false);
		btnType1_03_Btn.gameObject.SetActive( false);
		btnType1_04_Btn.gameObject.SetActive( false);
		btnType1_05_Btn.gameObject.SetActive( false);
		btnType1_06_Btn.gameObject.SetActive( false);
		btnType1_07_Btn.gameObject.SetActive( false);
	}
	#endregion
	#region - type2 -
	void Type2Proc( int _idx)
	{
		switch( _idx)
		{
		case 1:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
			m_Category2 = 0;
			CloseType2DropDown();
			break;
		case 2:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1032);
			m_Category2 = (int)Item.eEQUIP.Weapon;
			CloseType2DropDown();
			break;
		case 3:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1033);
			m_Category2 = (int)Item.eEQUIP.Head;
			CloseType2DropDown();
			break;
		case 4:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1034);
			m_Category2 = (int)Item.eEQUIP.Armor;
			CloseType2DropDown();
			break;
		case 5:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1035);
			m_Category2 = (int)Item.eEQUIP.Gloves;
			CloseType2DropDown();
			break;
		case 6:
			switch( m_CurType1)
			{
			case eType1.EquipItem:
			case eType1.CosEquipItem:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1036);
				m_Category2 = (int)Item.eEQUIP.Point;
				break;
//			case eType1.Enchant:
//				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
//				m_Category2 = (int)Item.eEtcItem.Enchant;
//				break;
//			case eType1.Strengthen:
//				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
//				m_Category2 = (int)Item.eEtcItem.Strengthen;
//				break;
//			case eType1.Material:
//				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
//				m_Category2 = (int)Item.eEtcItem.Material;
//				break;
//			default:
//				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
//				m_Category2 = 0;
//				break;
			}
			CloseType2DropDown();
			break;
		case 7: // special case
			switch( m_CurType1)
			{
			case eType1.EquipItem:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1037);
				m_Category2 = (int)Item.eEQUIP.Earring;
				break;
			case eType1.CosEquipItem:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1570);
				if((int)Item.eEQUIP.Fairy != 10) //  must check this later
					m_Category2 = 10;
				else
					m_Category2 = (int)Item.eEQUIP.Fairy;
				break;
			}
			CloseType2DropDown();
			break;
		case 8: // special case
			switch( m_CurType1)
			{
			case eType1.EquipItem:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1038);
				m_Category2 = (int)Item.eEQUIP.Necklace;
				break;
			case eType1.CosEquipItem:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1571);
				if((int)Item.eEQUIP.Wing != 9)
					m_Category2 = 9;
				else
					m_Category2 = (int)Item.eEQUIP.Wing;
				break;
			default:
				btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
				m_Category2 = 0;
				break;
			}
			CloseType2DropDown();
			break;
		case 9:
			btnType2.Text = AsTableManager.Instance.GetTbl_String( 1039);
			m_Category2 = (int)Item.eEQUIP.Ring;
			CloseType2DropDown();
			break;
		case -1:
			CloseAllSubBtns();
			OpenType2DropDown();
			break;
		default:
			Debug.LogError("UIPStoreSearchDlg::Type2Proc: unknown index is named. check type2 button name");
			break;
		}
	}
	
	void OpenType2DropDown()
	{
		switch( m_CurType1)
		{
//		case eType1.ALL:
//			break;
		case eType1.EquipItem:
			btnType2_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 1032);
			btnType2_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 1033);
			btnType2_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 1034);
			btnType2_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 1035);
			btnType2_06_Btn.Text = AsTableManager.Instance.GetTbl_String( 1036);
			btnType2_07_Btn.Text = AsTableManager.Instance.GetTbl_String( 1037);
			btnType2_08_Btn.Text = AsTableManager.Instance.GetTbl_String( 1038);
			btnType2_09_Btn.Text = AsTableManager.Instance.GetTbl_String( 1039);

			btnType2_02_Btn.gameObject.SetActive( true);
			btnType2_03_Btn.gameObject.SetActive( true);
			btnType2_04_Btn.gameObject.SetActive( true);
			btnType2_05_Btn.gameObject.SetActive( true);
			btnType2_06_Btn.gameObject.SetActive( true);
			btnType2_07_Btn.gameObject.SetActive( true);
			btnType2_08_Btn.gameObject.SetActive( true);
			btnType2_09_Btn.gameObject.SetActive( true);
			break;
		case eType1.CosEquipItem:
			btnType2_02_Btn.Text = AsTableManager.Instance.GetTbl_String( 1032);
			btnType2_03_Btn.Text = AsTableManager.Instance.GetTbl_String( 1033);
			btnType2_04_Btn.Text = AsTableManager.Instance.GetTbl_String( 1034);
			btnType2_05_Btn.Text = AsTableManager.Instance.GetTbl_String( 1035);
			btnType2_06_Btn.Text = AsTableManager.Instance.GetTbl_String( 1036);
			btnType2_07_Btn.Text = AsTableManager.Instance.GetTbl_String( 1570);
			btnType2_08_Btn.Text = AsTableManager.Instance.GetTbl_String( 1571);

			btnType2_02_Btn.gameObject.SetActive( true);
			btnType2_03_Btn.gameObject.SetActive( true);
			btnType2_04_Btn.gameObject.SetActive( true);
			btnType2_05_Btn.gameObject.SetActive( true);
			btnType2_06_Btn.gameObject.SetActive( true);
			btnType2_07_Btn.gameObject.SetActive( true);
			btnType2_08_Btn.gameObject.SetActive( true);
			break;
//		case eType1.ActionItem:
//		case eType1.Enchant:
//		case eType1.Strengthen:
//		case eType1.Material:
		default:
			btnType2_08_Btn.Text = AsTableManager.Instance.GetTbl_String( 1173);
			btnType2_08_Btn.gameObject.SetActive( true);
			break;
		}

//		btnType2_01_Btn_ALL.gameObject.SetActive( true);
	}
	
	void CloseType2DropDown()
	{
		btnType2_01_Btn_ALL.gameObject.SetActive( false);
		btnType2_02_Btn.gameObject.SetActive( false);
		btnType2_03_Btn.gameObject.SetActive( false);
		btnType2_04_Btn.gameObject.SetActive( false);
		btnType2_05_Btn.gameObject.SetActive( false);
		btnType2_06_Btn.gameObject.SetActive( false);
		btnType2_07_Btn.gameObject.SetActive( false);
		btnType2_08_Btn.gameObject.SetActive( false);
		btnType2_09_Btn.gameObject.SetActive( false);
	}
	
	void ResetType2Selection()
	{
		btnType2.Text = AsTableManager.Instance.GetTbl_String( 1173);
		m_Category2 = 0;
	}
	#endregion
	#region - grade -
	void GradeProc( int _idx)
	{
		switch( _idx)
		{
		case 1:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1173);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_NOTHING;
			CloseGradeDropDown();
			break;
		case 2:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1028);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_NORMAL;
			CloseGradeDropDown();
			break;
		case 3:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1029);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_MAGIC;
			CloseGradeDropDown();
			break;
		case 4:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1030);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_RARE;
			CloseGradeDropDown();
			break;
		case 5:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1031);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_EPIC;
			CloseGradeDropDown();
			break;
		case 6:
			btnGrade.Text = AsTableManager.Instance.GetTbl_String( 1699);
			m_Grade = eEQUIPITEM_GRADE.eEQUIPITEM_GRADE_ARK;
			CloseGradeDropDown();
			break;
		case -1:
			CloseAllSubBtns();
			OpenGradeDropDown();
			break;
		default:
			Debug.LogError("UIPStoreSearchDlg::GradeProc: unknown index is named. check Grade button name");
			break;
		}
	}
	
	void OpenGradeDropDown()
	{
		btnGrade_01_Btn_ALL.gameObject.SetActive( true);
		btnGrade_02_Btn.gameObject.SetActive( true);
		btnGrade_03_Btn.gameObject.SetActive( true);
		btnGrade_04_Btn.gameObject.SetActive( true);
		btnGrade_05_Btn.gameObject.SetActive( true);
		btnGrade_06_Btn.gameObject.SetActive( true);
	}
	
	void CloseGradeDropDown()
	{
		btnGrade_01_Btn_ALL.gameObject.SetActive( false);
		btnGrade_02_Btn.gameObject.SetActive( false);
		btnGrade_03_Btn.gameObject.SetActive( false);
		btnGrade_04_Btn.gameObject.SetActive( false);
		btnGrade_05_Btn.gameObject.SetActive( false);
		btnGrade_06_Btn.gameObject.SetActive( false);
	}
	#endregion
	
	#region - method -
	int _GetIndexFromName( POINTER_INFO ptr)
	{
		string[] asStr = ptr.hitInfo.transform.name.Split( new string[]{"_"}, StringSplitOptions.None);
		int idx = -1;
		if( asStr != null && asStr.Length > 0 && int.TryParse( asStr[0], out idx) == true)
			return idx;
		else
			return -1;
	}
	
	bool m_ServerSearching = false;
	Color m_PrevColor = Color.white;
	void SendSearch()	
	{
		CloseAllSubBtns();
		
		if( m_ServerSearching == false)
		{
			SetCoolTime();
		}
		else
		{
			Debug.Log("UIPStoreSearchDlg::SendSearch: server is searching. ignore send order");
			return;
		}
		
		int levelMax = 0;
		if( int.TryParse( txtLevel_contents_max.Text, out levelMax) == false)
		{
			Debug.LogWarning("UIPStoreSearchDlg::SendSearch: txtLevel_contents_max.Text = " + txtLevel_contents_max.Text + ". parsing failed");
			CoolCompletion();
			return;
		}
		else
		{
			int levelMin = 0;
			if( int.TryParse( txtLevel_contents_min.Text, out levelMin) == true)
				m_LevelMin = levelMin;
			else
			{
				Debug.LogWarning("UIPStoreSearchDlg::SendSearch: txtLevel_contents_min.Text = " + txtLevel_contents_min.Text + ". parsing failed");
				CoolCompletion();
				return;
			}
			
			if( m_LevelLimit < levelMax)
			{
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2184));
				CoolCompletion();
				return;
			}
			
			m_LevelMax = levelMax;
		}
		
		StartCoroutine( "FindItem_CR");
	}
	
	void SetCoolTime()
	{
		m_ServerSearching = true;
		m_PrevColor = btnSearch.renderer.material.color;
		btnSearch.renderer.material.color = Color.gray;
	}
	
	IEnumerator Cool_CR()
	{
		yield return new WaitForSeconds( m_SearchCool);
		
		CoolCompletion();
	}
	
	void CoolCompletion()
	{
		m_ServerSearching = false;
		
		btnSearch.renderer.material.color = m_PrevColor;
	}
	
	IEnumerator FindItem_CR()
	{
		List<Item> listItem = new List<Item>();
		MultiDictionary<string, Item> mdic = ItemMgr.ItemManagement.mdicItemByName;
		string name = txtName_contents.Text;
		
		if( name != "")
		{
			int count = 0;
			foreach(string key in mdic.Keys)
			{
	//			if( key.Contains( name) == true)
	//			{
	//				Debug.Log(key);
	//				listItem.AddRange( mdic[key]);
	//			}
				
				if( key == name)
				{
					foreach( Item node in mdic[key])
					{
						Debug.Log(key + " : " + node.ItemID);
					}
					
					listItem.AddRange( mdic[key]);
				}
				
				count++;
				
				if( count > 512)
				{
					count = 0;
					yield return null;
				}
			}
			
			if( listItem == null || listItem.Count == 0)
			{
				CoolCompletion();
				
				AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2183));
				
				Debug.Log("UIPStoreSearchDlg:: SendSearch: [ txtName_contents.Text = " + txtName_contents.Text + " ] is not found");
				yield break;
			}
			
			if( listItem.Count > 1)
			{
				Debug.LogWarning("UIPStoreSearchDlg:: SendSearch: same named item : " + name + ". searching will not be processed normally");
				
				listItem.Sort( delegate( Item _a, Item _b){
					if( _a.ItemID < _b.ItemID)
						return -1;
					else if( _a.ItemID == _b.ItemID)
						return 0;
					else
						return 1;
				});
			}
			
			m_ItemTableIdx = listItem[0].ItemID;
		}
		else
			m_ItemTableIdx = 0;
		
		StartCoroutine( "Cool_CR");
		
		if( m_PageIdx == 0) m_PageIdx = 1;
		
		body_CS_PRIVATESHOP_SEARCH search = new body_CS_PRIVATESHOP_SEARCH(
			m_PageIdx, m_ItemTableIdx, m_Class,
			m_Category1, m_Category2, m_LevelMin, m_LevelMax,
			m_Grade, m_MapIdx, m_SearchType);
		
		AsCommonSender.Send(search.ClassToPacketBytes());
	}
	
	void OpenQuantityDlg( UIPStoreSearchSlot _slot)
	{
		CloseAllSubBtns();
		
		quantityDlg.Open( _slot, Del_PurchaseConfirm);
	}
	
	void CloseQuantityDlg()
	{
		quantityDlg.Close();
	}

	string KeyInputDelegate( UITextField _field, string _text, ref int _insertionPoint)
	{
		if(_field.spriteText != null)
			_field.spriteText.maxWidth = _field.width;

		return _text;
	}
	#endregion
	
	#region - public -
	public void Recv_Search( body1_SC_PRIVATESHOP_SEARCH_RESULT _result)
	{
		StopCoroutine( "Cool_CR");
		CoolCompletion();
		
		m_listSlot.Clear();
		listResult.ClearList( true);
		
		if( _result.body.Length == 0)
			AsNotify.Instance.MessageBox(AsTableManager.Instance.GetTbl_String(126), AsTableManager.Instance.GetTbl_String(2183));
		
		foreach( body2_SC_PRIVATESHOP_SEARCH_RESULT node in _result.body)
		{
			GameObject objItemBg = Instantiate( Resources.Load( "UI/Optimization/ListItem/SearchStoreDlgListItem")) as GameObject;
			UIPStoreSearchSlot slot = objItemBg.GetComponent<UIPStoreSearchSlot>();
			m_listSlot.Add( slot);
			slot.Init( listResult, node, Del_ListBtnClicked);
		}
		
//		_SortByName();
		
		#region - page -
		m_PageMax = _result.nMaxPageCount;
		if( m_PageIdx > m_PageMax)
			m_PageIdx = m_PageMax;
		txtPage.Text = m_PageIdx + " / " + m_PageMax;
		#endregion
	}
	
	public void PurchaseComplete( body_SC_PRIVATESHOP_ITEMBUY _buy)
	{
		m_SavedSlot.BuyProc( _buy);
		
		if( m_SavedSlot.SearchInfo.sItem.nOverlapped < 1)
		{
			SetCoolTime();
			
			StartCoroutine( "Cool_CR");
			
			if( m_PageIdx == 0) m_PageIdx = 1;
			
			body_CS_PRIVATESHOP_SEARCH search = new body_CS_PRIVATESHOP_SEARCH(
				m_PageIdx, m_ItemTableIdx, m_Class,
				m_Category1, m_Category2, m_LevelMin, m_LevelMax,
				m_Grade, m_MapIdx, m_SearchType);
			
			AsCommonSender.Send(search.ClassToPacketBytes());
		}
	}
	
	public void RequestSearch()
	{
		SendSearch();
	}
	#endregion
}
