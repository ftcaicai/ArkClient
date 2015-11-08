using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class ApRewardInfo
{
    public int itemID;
    public int itemCount;

    public ApRewardInfo()
    {
        itemID = -1;
        itemCount = -1;
    }

    public ApRewardInfo(int _itemID, int _itemCount)
    {
        itemID = _itemID;
        itemCount = _itemCount;
    }
}

public class ApRewardGroup
{
    public List<ApRewardInfo> listGroup1 = new List<ApRewardInfo>();
    public List<ApRewardInfo> listGroup2 = new List<ApRewardInfo>();

    public List<ApRewardInfo> GetApRewardList(int _group)
    {
        return _group == 0 ? listGroup1 : listGroup2;
    }
}

public class ApRewardInfoLineup
{
    public uint min = 0;
    public uint max = 0;
    public List<ApRewardInfo> listRewardInfo = new List<ApRewardInfo>();

    public ApRewardInfoLineup(uint _min, uint _max, List<ApRewardInfo> _listRewardInfo)
    {
        min = _min;
        max = _max;
        listRewardInfo = _listRewardInfo;
    }
}

public class ApRewardRecord
{
    public uint min = 0;
    public uint max = 0;
    public Dictionary<eCLASS, ApRewardGroup> dicRewardInfo = new Dictionary<eCLASS, ApRewardGroup>();

    public ApRewardRecord(XmlNode _node)
    {
        // add dictionary
        dicRewardInfo.Add(eCLASS.DIVINEKNIGHT, new ApRewardGroup());
        dicRewardInfo.Add(eCLASS.MAGICIAN, new ApRewardGroup());
        dicRewardInfo.Add(eCLASS.CLERIC, new ApRewardGroup());
        dicRewardInfo.Add(eCLASS.HUNTER, new ApRewardGroup());

        min = uint.Parse(_node["Rank_Min"].InnerText);
        max = uint.Parse(_node["Rank_Max"].InnerText);

        string szReward = "_Reward_";

        int maxClassCount = (int)eCLASS.HUNTER;
        eCLASS nowClass = eCLASS.NONE;
        string szClassKey = string.Empty;

        StringBuilder sbItem = new StringBuilder();
        StringBuilder sbCount = new StringBuilder();

        for (int classCount = 1; classCount <= maxClassCount; classCount++)
        {
            nowClass = (eCLASS)classCount;

            szClassKey = (nowClass.ToString())[0].ToString();

            for (int groupCount = 1; groupCount <= 2; groupCount++)
            {
                for (int itemCount = 1; itemCount <= 3; itemCount++)
                {
                    // reset
                    sbItem.Remove(0, sbItem.Length);
                    sbCount.Remove(0, sbCount.Length);

                    // sb Item
                    sbItem.Append(szClassKey);
                    sbItem.Append(szReward);
                    sbItem.Append("G");
                    sbItem.Append(groupCount.ToString());
                    sbItem.Append("_Item");
                    sbItem.Append(itemCount.ToString());

                    // sb Count
                    sbCount.Append(szClassKey);
                    sbCount.Append(szReward);
                    sbCount.Append("G");
                    sbCount.Append(groupCount.ToString());
                    sbCount.Append("_Count");
                    sbCount.Append(itemCount.ToString());

                    string szItemResultKey = sbItem.ToString();
                    string szCountResultKey = sbCount.ToString();

                    if (_node[szItemResultKey].InnerText == "NONE")
                        continue;

                    int id = Int32.Parse(_node[szItemResultKey].InnerText);
                    int count = Int32.Parse(_node[szCountResultKey].InnerText);

                    string debug = "[" + nowClass + "][" + groupCount + "] = ID(" + id + "[" + count + "])";

					Debug.LogWarning(debug);

                    if (groupCount == 1)
                        dicRewardInfo[nowClass].listGroup1.Add(new ApRewardInfo(id, count));
                    else if (groupCount == 2)
                        dicRewardInfo[nowClass].listGroup2.Add(new ApRewardInfo(id, count));

                }
            }
        }
    }

    public bool CanGetApReward(uint _nowRank)
    {
        if (max >= _nowRank && _nowRank >= min)
            return true;
        else
            return false;
    }

    public List<ApRewardInfo> GetApRewardList(eCLASS _class, uint _nowRank, int _group)
    {
        if (CanGetApReward(_nowRank))
        {
            if (dicRewardInfo.ContainsKey(_class))
                return dicRewardInfo[_class].GetApRewardList(_group);
        }

        return new List<ApRewardInfo>();
    }

    public ApRewardInfoLineup GetApRewardInfoLineup(eCLASS _class, int _group)
    {
        if (dicRewardInfo.ContainsKey(_class))
        {
            ApRewardInfoLineup info = new ApRewardInfoLineup(min, max, dicRewardInfo[_class].GetApRewardList(_group));

            return info;
        }
        else
            return null;
    }
}


public class Tbl_ApRewardTable : AsTableBase
{
    List<ApRewardRecord> listRecord = new List<ApRewardRecord>();

    public Tbl_ApRewardTable(string _path)
	{
		LoadTable(_path);
	}

	public override void LoadTable(string _path)
	{
		try
		{
			XmlElement root = null;

            root = GetXmlRootElement(_path);

			// 파일이 없다
			if (root == null)
				throw new Exception("File is not exist = " + _path);

            foreach (XmlNode node in root.ChildNodes)
			{
                listRecord.Add(new ApRewardRecord(node));
			}

		}
		catch (System.Exception e)
		{
			System.Diagnostics.Trace.WriteLine(e);
		}
	}

    public List<ApRewardInfoLineup> GetAllRewardInfoLineup(eCLASS _class, int _group)
    {
        List<ApRewardInfoLineup> returnList = new List<ApRewardInfoLineup>();

        foreach (ApRewardRecord record in listRecord)
        {
            ApRewardInfoLineup info = record.GetApRewardInfoLineup(_class, _group);

            if (info != null)
                returnList.Add(info);
        }

        return returnList;
    }

    public List<ApRewardInfo> GetRewardInfoList(eCLASS _class, uint _rank, int _group)
    {
        foreach (ApRewardRecord record in listRecord)
        {
            if (record.CanGetApReward(_rank))
                return record.GetApRewardList(_class, _rank, _group);
        }

        return new List<ApRewardInfo>();
    }
}




