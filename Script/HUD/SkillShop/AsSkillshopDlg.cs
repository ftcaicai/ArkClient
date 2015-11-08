using UnityEngine;
using System.Collections;
using System.Text;

public enum eSkillSelectType
{
	Type_Choice,
	Type_Base
};

public class AsSkillshopDlg : MonoBehaviour
{
	public delegate void CloseDelegate();
	public CloseDelegate closeDel;

	public AsSkillshopTab tab = null;
	public SpriteText m_TextTitle;
	public SpriteText m_TextBase;
	public SpriteText m_TextChoice;
	public SpriteText m_TextRadioBtn;

	// Use this for initialization
	void Start()
	{
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextTitle);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextBase);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextChoice);
		AsLanguageManager.Instance.SetFontFromSystemLanguage( m_TextRadioBtn);

		m_TextTitle.Text = AsTableManager.Instance.GetTbl_String( 349);
		m_TextBase.Text = AsTableManager.Instance.GetTbl_String( 350);
		m_TextChoice.Text = AsTableManager.Instance.GetTbl_String( 351);
		m_TextRadioBtn.Text = AsTableManager.Instance.GetTbl_String( 352);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Open( int npcID)
	{
		gameObject.SetActiveRecursively( true);
		tab.Init( npcID, eSkillSelectType.Type_Choice);
	}

	public void Close()
	{
		AsSoundManager.Instance.PlaySound( "Sound/Interface/S6002_EFF_Button", Vector3.zero, false);
		QuestTutorialMgr.Instance.ProcessQuestTutorialMsg( new QuestTutorialMsgInfo( QuestTutorialMsg.CLOSE_SKILLSHOP, 0));
		AsNotify.Instance.CloseAllMessageBox();
		closeDel();
	}

	public void Restructure()
	{
		tab.Restructure();
	}

	//$yde
	private int id = -1;
	private int level = -1;
	public void SkillSelection( Tbl_Skill_Record skillRecord, int _id, int _level)
	{
		id = _id;
		level = _level;

		StringBuilder sb = new StringBuilder();
		sb.AppendFormat( AsTableManager.Instance.GetTbl_String(72), AsTableManager.Instance.GetTbl_String( skillRecord.SkillName_Index), _level);

		AsMessageBox msgBox = AsNotify.Instance.MessageBox( AsTableManager.Instance.GetTbl_String(353), sb.ToString(),
			this, "SkillLearn", AsNotify.MSG_BOX_TYPE.MBT_OKCANCEL, AsNotify.MSG_BOX_ICON.MBI_QUESTION);
		msgBox.SetOkText( AsTableManager.Instance.GetTbl_String(1152));
		msgBox.SetCancelText( AsTableManager.Instance.GetTbl_String(1151));
	}

	public void SkillLearn()
	{
		SkillShop.Instance.PurchaseSkill( id, level);
	}
}
