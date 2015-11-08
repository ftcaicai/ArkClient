using UnityEngine;
using System.Collections;

public class AsMenuBtnAnimController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
        foreach (UIInteractivePanel panel in panels)
        {
            if (panel.gameObject.name.Contains("4_social") || panel.gameObject.name.Contains("1_product"))
            {
                EZTransition ezTransExpand   = panel.GetTransition(UIPanelManager.SHOW_MODE.BringInForward);
                EZTransition ezTransCollapse = panel.GetTransition(UIPanelManager.SHOW_MODE.DismissForward);

                ezTransExpand.AddTransitionEndDelegate(ExpandComplete);
                ezTransCollapse.AddTransitionEndDelegate(CollapseComplete);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
    }

    public void Expand()
    {
        UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
        foreach (UIInteractivePanel panel in panels)
            panel.Reveal();
    }

    public void Collapse()
    {
		if (CanProcessCollapseQuestMsg() == true)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.MENU_COLLAPSE));
        
		UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
        foreach (UIInteractivePanel panel in panels)
            panel.Hide();

    }

    public void ForcedCollapse()
    {
		if (CanProcessCollapseQuestMsg() == true)
			QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.MENU_COLLAPSE));

        UIInteractivePanel[] panels = gameObject.GetComponentsInChildren<UIInteractivePanel>();
        foreach (UIInteractivePanel panel in panels)
        {
            panel.Hide();
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.zero;
            EZTransition trans = panel.GetTransition(UIPanelManager.SHOW_MODE.DismissForward);
            trans.StopSafe();
        }
    }

    public void ExpandComplete(EZTransition _transition)
    {
        QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.MENU_EXPAND));
    }

    public void CollapseComplete(EZTransition _transition)
    {
       // QuestTutorialMgr.Instance.ProcessQuestTutorialMsg(new QuestTutorialMsgInfo(QuestTutorialMsg.MENU_COLLAPSE));
    }

	public bool CanProcessCollapseQuestMsg()
	{
		if (AsSocialManager.Instance.SocialUI.IsOpenFindFriendDlg != true && AsHudDlgMgr.Instance.IsOpenSystemDlg != true &&
			AsSocialManager.Instance.IsOpenSocialDlg() != true && AsPartyManager.Instance.PartyUI.IsOpenPartyList != true && AsPartyManager.Instance.PartyUI.IsOpenPartyMatching != true)
			return true;
		else
			return false;
	}
}
