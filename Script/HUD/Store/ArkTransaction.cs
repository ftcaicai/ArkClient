using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class ArkTransaction
{
    public static string path           = Application.temporaryCachePath;
    public static string fileExtention  = ".ast";

    public string userID		= string.Empty;
    public string characterName	= string.Empty;
    public string productID		= string.Empty;
    public string transactionIdentifier = string.Empty;
    public string json		= string.Empty;
    public int	slot		= 0;
    public int	quantity	= 0;
    public uint	characterUniqueKey	= 0;
    public uint	itemUniqueID	 = 0;
    public int	serverID		 = 0;
    public uint	friendUniqueKey	 = 0;
	public int	registerKey		 = 0;
	public bool isUnHandled      = false;
	public bool isRetryConnectIAPAServer = false;
	int  remainConnectCount		 = 0;

	public ArkTransaction(string _usrID, string _characterName, string _productID, string _transactionID, string _json, int _slot, int _quantity, uint _characterUniqueKey, int _serverID, uint _friendUnique, int _registerKey, bool _isUnhandled = false, bool _isRetryConnectIAPAServer = false)
    {
        userID			= _usrID;
        characterName	= _characterName;
        productID		= _productID;
        json			= _json;
        transactionIdentifier	 = _transactionID;
        slot					 =  _slot;
        quantity				 = _quantity;
        characterUniqueKey		 = _characterUniqueKey;
        serverID				 = _serverID;
		friendUniqueKey			 = _friendUnique;
		registerKey				 = _registerKey;
		isUnHandled              = _isUnhandled;
		isRetryConnectIAPAServer = _isRetryConnectIAPAServer;
    }

	public bool CanRetryConnectIAPAServer()
	{
		return remainConnectCount > 0;
	}

	public void DiscountRetryConnectCount()
	{
		remainConnectCount--;

		if (remainConnectCount <= 0)
			isRetryConnectIAPAServer = false;
	}

	public void SetRetryConnectServerCount()
	{
		remainConnectCount = 2;
	}

    public ArkTransaction()
    {

    }
	
	public override string ToString ()
	{
		string szTemp =  "UsrID = " + userID +
                         ", Name = " + characterName +
                         ", product ID = " + productID +
                         ", Transaction Id = " + transactionIdentifier +
                         ", slot = " + slot +
                         ", character unique = " + characterUniqueKey +
                         ", server ID = " + serverID +
						 ", user UniqueKey = " + friendUniqueKey +
						 ", registerKey = " + registerKey;
		return szTemp;
	}

    public int GetSize()
    {
        int size = System.Text.Encoding.Default.GetByteCount(json);
        return size;
    }

    public bool WriteToFile()
    {
        string filePath = Application.temporaryCachePath + "/" + transactionIdentifier + fileExtention;


		if (File.Exists(filePath))
		{
			File.Delete(filePath);
			Debug.Log("Delete exists File");
		}

		FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);

        bw.Write(userID);
        bw.Write(characterName);
        bw.Write(productID);
        bw.Write(transactionIdentifier);
        bw.Write(json);
        bw.Write(slot);
        bw.Write(quantity);
        bw.Write(characterUniqueKey);
        bw.Write(serverID);
		bw.Write(friendUniqueKey);
		bw.Write(registerKey);
        bw.Close();

        byte[] buffer = ms.GetBuffer();
        ms.Close();

        string transBuffer = System.Convert.ToBase64String(buffer);
        bw                 = new BinaryWriter(fs);
        bw.Write(transBuffer);
        bw.Close();
        fs.Close();

        Debug.LogWarning("Make file = " + transactionIdentifier);

        return true;
	}

    public static ArkTransaction ReadFromFile(string _fileName)
    {
        string filePath = path + "/" + _fileName + fileExtention;

        if (File.Exists(filePath))
        {
            FileStream fs   = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);

            string transBuffer = br.ReadString();

            byte[] buffer = System.Convert.FromBase64String(transBuffer);

            br.Close();

            fs.Close();

            MemoryStream ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);

            ArkTransaction transa = new ArkTransaction();
            transa.userID = br.ReadString();
            transa.characterName = br.ReadString();
            transa.productID = br.ReadString();
            transa.transactionIdentifier = br.ReadString();
            transa.json = br.ReadString();
            transa.slot = br.ReadInt32();
            transa.quantity = br.ReadInt32();
            transa.characterUniqueKey = br.ReadUInt32();
            transa.serverID = br.ReadInt32();
			transa.friendUniqueKey = br.ReadUInt32();
			transa.registerKey = br.ReadInt32();

            br.Close();
            ms.Close();

            return transa;
        }
        else
        {
            Debug.Log("File is not exist = " + filePath);
            return null;
        }
    }

    public static bool DeleteArkTransactionFile(string _fileName)
    {
        try
        {
            string szFilePath = path + "/" + _fileName + fileExtention;

            File.Delete(szFilePath);

            Debug.Log("Delete = " + szFilePath);

            return true;
        }
        catch (IOException exception)
        {
            UnityEngine.Debug.LogWarning(exception.Message);
            return false;
        }
    }

    static public List<ArkTransaction> GetAllTransaction(uint _characterUniquekey)
    {
        List<ArkTransaction> resultList = new List<ArkTransaction>();

        try
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            FileInfo[] files = directory.GetFiles("*" + fileExtention);

            foreach (FileInfo file in files)
            {
                ArkTransaction trans = ReadFromFile(file.Name.Replace(fileExtention, ""));

                if (trans.productID.Contains("gold"))
                {
                    if (trans.characterUniqueKey == _characterUniquekey)
                        resultList.Add(trans);
                }
                else
                {
                    resultList.Add(trans);
                }
            }

            return resultList;
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e.Message);
        }

        return resultList;
    }

    static public Dictionary<string, ArkTransaction> GetAllTransactionDic()
    {
        Dictionary<string, ArkTransaction> resultDic = new Dictionary<string, ArkTransaction>();

        try
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            FileInfo[] files = directory.GetFiles("*" + fileExtention);

            foreach (FileInfo file in files)
            {
                string fileName = file.Name.Replace(fileExtention, "");
                resultDic.Add(fileName, ReadFromFile(file.Name.Replace(fileExtention, "")));
            }

            return resultDic;
        }
        catch (System.Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e.Message);
        }

        return resultDic;
    }
}