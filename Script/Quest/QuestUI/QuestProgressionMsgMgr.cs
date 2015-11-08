using UnityEngine;
using System.Collections;

public class QuestProgressMsgInfo
{
    public string szMsg;
    public Color color;

    public QuestProgressMsgInfo()
    {
        szMsg = "empty";
        color = Color.black;
    }

    public QuestProgressMsgInfo(string _msg, Color _color)
    {
        szMsg = _msg;
        color = _color;
    }
}

public class QuestProgressionMsgMgr : MonoBehaviour {

    public bool    bUseAppear;
    public SpriteText.Anchor_Pos textAnchor = SpriteText.Anchor_Pos.Middle_Left;
    public float   fAppearTime;
    public float   fShowTime;
    public float[] fResetDisplayTimes;
    public float   fDeQueueDelayTime;
    public float[] fLineHideTimes;
    public float[] fLinePosesY;
    public QuestProgressMsgPanel[] panels;
    public Color   colorMainQuest;
    public Color   colorDailyQuest;
    public Color   colorWantedQuest;
    public Color   colorProgresssionQuest;
    public Color   colorCompleteQuest;

    private Queue      msgQueue = new Queue();
    private GameObject questProgressMsgPanelPrefab = null;
//    private int        nowMesageCount = 0;
    private float      fTime;

    void Start()
    {
        questProgressMsgPanelPrefab = Resources.Load("UI/AsGUI/QuestProgressMsgPanel") as GameObject;
        panels = new QuestProgressMsgPanel[] { CreatePanel("panel_0"), CreatePanel("panel_1"), CreatePanel("panel_2") };

//        float spriteTextHeight = panels[0].spriteText.BaseHeight;

        fLinePosesY = new float[] { 0.0f, panels[0].bgGride.TotalHeight, panels[0].bgGride.TotalHeight * 2.0f };

        panels[0].gameObject.transform.localPosition = new Vector3(50.0f, fLinePosesY[0], 0.0f);
        panels[1].gameObject.transform.localPosition = new Vector3(50.0f, fLinePosesY[1], 0.0f);
        panels[2].gameObject.transform.localPosition = new Vector3(50.0f, fLinePosesY[2], 0.0f);
    }

    QuestProgressMsgPanel CreatePanel(string _szName)
    {
        GameObject panel = GameObject.Instantiate(questProgressMsgPanelPrefab) as GameObject;
        panel.name = _szName;
        panel.transform.parent = transform;
        panel.transform.position = Vector3.zero;
        return panel.GetComponent<QuestProgressMsgPanel>();
    }
	
	void Update ()
    {
        fTime += Time.deltaTime;

        if (fTime >= 0.5f)
        {
            if (DisplayMsg() == true)
                fTime = 0.0f;
        }

#if UNITY_EDITOR
        test();
#endif
	}

    public void ResetForChangeMap()
    {
        msgQueue.Clear();
        foreach (QuestProgressMsgPanel panel in panels)
            panel.ResetHiding();
    } 

    public void AddMessage(string _msg, QuestType _type)
    {
        QuestProgressMsgInfo msgInfo = null;

        if (_type == QuestType.QUEST_MAIN || _type == QuestType.QUEST_FIELD || _type == QuestType.QUEST_BOSS || _type == QuestType.QUEST_PVP)
            msgInfo = new QuestProgressMsgInfo(_msg, colorMainQuest);
        else if (_type == QuestType.QUEST_WANTED)
            msgInfo = new QuestProgressMsgInfo(_msg, colorWantedQuest);
        else if (_type == QuestType.QUEST_DAILY)
            msgInfo = new QuestProgressMsgInfo(_msg, colorDailyQuest);
        else
            msgInfo = new QuestProgressMsgInfo(_msg, Color.white);

        msgQueue.Enqueue(msgInfo);
    }
    
    public void AddMessage(string _msg, bool bComplete)
    {
        QuestProgressMsgInfo msgInfo = null;

        if (bComplete)
            msgInfo = new QuestProgressMsgInfo(_msg, colorCompleteQuest);
        else
            msgInfo = new QuestProgressMsgInfo(_msg, colorProgresssionQuest);

        msgQueue.Enqueue(msgInfo);
    }

    bool DisplayMsg()
    {
        bool canDequeue  = CanDequeue();
        int  msgCount    = GetMsgCount();

        if (canDequeue == true && msgCount != 3 && panels[0].nowState != QuestProgressionPanelState.APPEARING)
        {
            if (msgCount >= 1)
                PrepareDisplayMsg(msgCount);

            QuestProgressMsgInfo msgInfo = msgQueue.Dequeue() as QuestProgressMsgInfo;

            panels[0].Init(msgInfo, fAppearTime, fShowTime, fLineHideTimes[0]);
            

            if (bUseAppear == true)
                panels[0].Appear();
            else
                panels[0].Show();

            return true;
        }
        else
            return false;
    }

    bool CanDequeue()
    {
        if (msgQueue.Count == 0)
            return false;

        return true;
    }

    int GetMsgCount()
    {
        int count = 0;
        foreach(QuestProgressMsgPanel panel in panels)
        {
            if (panel.nowState == QuestProgressionPanelState.APPEARING || panel.nowState == QuestProgressionPanelState.SHOW || panel.nowState == QuestProgressionPanelState.HIDING)
                count++;
        }

        return count;
    }

    void PrepareDisplayMsg(int _msgCount)
    {
        if (_msgCount == 1)
        {
            panels[1].ResetShowTime(fResetDisplayTimes[1], new Vector3(panels[0].transform.localPosition.x, fLinePosesY[1], 0.0f), fLineHideTimes[1], panels[0].spriteText.Text, panels[0].spriteText.Color);
        }
        else if (_msgCount == 2)
        {
            panels[2].ResetShowTime(fResetDisplayTimes[2], new Vector3(panels[1].transform.localPosition.x, fLinePosesY[2], 0.0f), fLineHideTimes[2], panels[1].spriteText.Text, panels[1].spriteText.Color);
            panels[1].ResetShowTime(fResetDisplayTimes[1], new Vector3(panels[0].transform.localPosition.x, fLinePosesY[1], 0.0f), fLineHideTimes[1], panels[0].spriteText.Text, panels[0].spriteText.Color);
        }
    }

    void test()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddMessage(AsTableManager.Instance.GetTbl_String(879), QuestType.QUEST_NONE);

            string msg = string.Format(AsTableManager.Instance.GetTbl_String(879), "Park kab bong");
            AsEventNotifyMgr.Instance.CenterNotify.AddQuestMessageColorTag(msg);
        }
#endif
    }
}
