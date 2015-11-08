using UnityEngine;
using System.Collections;

public class QuestTalkInfo 
{
    public int npcTexIdx   { get; set; }
    public string contents { get; set; }

    public QuestTalkInfo()
    {
        npcTexIdx = -1;
        contents  = string.Empty;
    }

    public QuestTalkInfo(int _npcTexIdx, string _contents)
    {
        npcTexIdx = _npcTexIdx;
        contents  = _contents;
    }
}
