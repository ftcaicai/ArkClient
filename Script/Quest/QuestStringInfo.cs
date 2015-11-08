using System;
using System.Collections.Generic;
using System.Text;

public class QuestStringInfo
{
    public int    ID          { get; set; }
    public string Title       { get; set; }
    public string Explain     { get; set; }
    public string Achievement { get; set; }

    public QuestStringInfo() { Title = Explain = Achievement = string.Empty; ID = -1; }

    public QuestStringInfo(int _id, string _title, string _expain, string _achievement)
    {
        ID          = _id;
        Title       = _title;
        Explain     = _expain;
        Achievement = _achievement;
    }
}