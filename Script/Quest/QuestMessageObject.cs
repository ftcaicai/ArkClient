using System;
using System.Collections.Generic;
using System.Text;


public class QuestMessageObject
{
    public QuestMessages MessageType { get; set; }
    public Object Data { get; set; }


    public QuestMessageObject()
    {
        MessageType = QuestMessages.QM_GET_ITEM;
        Data = null;
    }
    public QuestMessageObject(QuestMessages _messageType, Object _data)
    {
        MessageType = _messageType;
        Data = _data;
    }
}

