//#define LocalTest

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CashStoreAdInfoWeb
{
	public Texture2D texture;
	public string imageName;
	public eCashStoreMenuMode menuMode;
	public eCashStoreMenuMode gotoMenuMode;
	public eCashStoreSubCategory subCategory;
	public int selectMenuIdx;
	public string linkUrl;

	public CashStoreAdInfoWeb(Texture2D _texture, string _imageName, eCashStoreMenuMode _menuMode, eCashStoreMenuMode _gotoMenu, eCashStoreSubCategory _subCategory, string _linkUrl)
	{
		texture = _texture;
		imageName = _imageName;
		menuMode = _menuMode;
		gotoMenuMode = _gotoMenu;
		subCategory = _subCategory;
		linkUrl = _linkUrl;
	}

	public override string ToString()
	{
		string loadedImg = texture == null ? "[NotLoaded]" : "[Loaded]";

		return loadedImg + " mode = " + menuMode + " goto = " + gotoMenuMode + " " + selectMenuIdx + " link = " + linkUrl;
		
	}
}


public class CashStoreMenuAd : CashStoreMenu
{
	public GameObject loadingIndigator;
	public GameObject adPrefabObject;
	public bool IsLoaded { get; set; }
	bool IsInitilized = false;

	private List<CashStoreAdInfoWeb> listCashStoreAdInfo = new List<CashStoreAdInfoWeb>();
	private Dictionary<GameObject, CashStoreAdInfoWeb> dicListItem = new Dictionary<GameObject, CashStoreAdInfoWeb>();

	public override void InitMenu(eCLASS _userClass)
	{
		userClass = _userClass;

		if (IsInitilized == false)
			StartCoroutine("LoadAllTextureFromWeb");

		if (loadingIndigator != null && IsInitilized == false)
			loadingIndigator.SetActive(true);

		mainScrollList.SetValueChangedDelegate(ClickAd);
	}

	void Update()
	{
		if (IsLoaded == true)
		{
			if (IsInitilized == false)
			{
				IsInitilized = true;
				AddAdvertiseListItem();
			}
		}
	}

	void AddAdvertiseListItem()
	{
		foreach (CashStoreAdInfoWeb info in listCashStoreAdInfo)
		{
			UIListItem item = mainScrollList.CreateItem(adPrefabObject) as UIListItem;

			SimpleSprite sprite = item.gameObject.transform.GetChild(0).GetComponent<SimpleSprite>();

			sprite.renderer.material.mainTexture = info.texture;

			// dic
			dicListItem.Add(item.gameObject, info);
		}

		if (loadingIndigator != null)
			loadingIndigator.SetActive(false);
	}

	public override void Reset()
	{
		//if (IsLoaded == false)
		//    StopCoroutine("LoadAllTextureFromWeb");
	}

	public void ClickAd(IUIObject obj)
	{
		if (AsCashStore.Instance.IsLockInput == true)
			return;

		if (IsInitilized == false)
			return;

		if (dicListItem.ContainsKey(mainScrollList.SelectedItem.gameObject))
		{
			CashStoreAdInfoWeb info = dicListItem[mainScrollList.SelectedItem.gameObject];

			if (info.linkUrl != string.Empty)
				Application.OpenURL(info.linkUrl);
			else if (info.gotoMenuMode != eCashStoreMenuMode.NONE)
				AsCashStore.Instance.GoToMenu(eCashStoreMenuMode.CONVENIENCE, eCashStoreSubCategory.NONE);
		}
		else
		{
			Debug.LogWarning("Not contain");
		}
	}

	IEnumerator LoadAllTextureFromWeb()
	{
		StringBuilder sb = new StringBuilder();

		string downPath = AsNetworkDefine.ImageServerAddress;

#if LocalTest
		downPath = "file://" + Application.dataPath;
		sb.Append(downPath);
		sb.Append("/CashShopAd.json");
#else
		sb.Append(downPath);
		sb.Append("CashShopAd.json");
#endif

		Debug.LogWarning(sb.ToString());

		WWW www = new WWW(sb.ToString());
		yield return www;

		if (www.isDone == true && www.error == null)
		{
			WmWemeSDK.JSON.JSONObject json = null;

			try
			{
				json = WmWemeSDK.JSON.JSONObject.Parse(www.text);
				Debug.LogWarning(www.text);
				www.Dispose();
				www = null;
			}
			catch (Exception e) { Debug.Log(e.Message); }


			WmWemeSDK.JSON.JSONObject infoObj = null;
			WmWemeSDK.JSON.JSONArray infos = null;
			IEnumerator<WmWemeSDK.JSON.JSONValue> enumerator = null;

			if (json != null)
				infoObj = json.GetObject("ad");

			if (infoObj != null)
				infos = infoObj.GetArray("info");

			if (infos != null)
				enumerator = infos.GetEnumerator();

			if (enumerator != null)
			{
				while (enumerator.MoveNext())
				{
					WmWemeSDK.JSON.JSONValue info = enumerator.Current;

					string imgName = info.Obj.GetString("img");
					string menu = info.Obj.GetString("menu");
					string subCategory = info.Obj.GetString("subcategory");
					string gotoMenu = info.Obj.GetString("goto");
					string linkUrl = info.Obj.GetString("url");

					// only same menu
					if (menu != menuMode.ToString())
						continue;

					sb.Remove(0, sb.Length);
					sb.Append(AsNetworkDefine.ImageServerAddress);
					sb.Append(imgName);

					WWW wwwImg = new WWW(sb.ToString());
					yield return wwwImg;

					if (wwwImg.isDone == true && wwwImg.error == null)
					{
						try
						{
							// menu
							eCashStoreMenuMode eMenu = eCashStoreMenuMode.MAIN;
							int index = 0;

							if (Enum.IsDefined(typeof(eCashStoreMenuMode), menu))
								eMenu = (eCashStoreMenuMode)Enum.Parse(typeof(eCashStoreMenuMode), menu, true);

							// goto menu
							eCashStoreMenuMode eGotoMenu = gotoMenu == string.Empty ? eCashStoreMenuMode.NONE : (eCashStoreMenuMode)Enum.Parse(typeof(eCashStoreMenuMode), gotoMenu, true);

							eCashStoreSubCategory eSubCategory = subCategory == string.Empty ? eCashStoreSubCategory.NONE : (eCashStoreSubCategory)Enum.Parse(typeof(eCashStoreSubCategory), subCategory, true);

							CashStoreAdInfoWeb adInfo = new CashStoreAdInfoWeb(wwwImg.texture,
																			   imgName,
																			   eMenu,
																			   eGotoMenu,
																			   eSubCategory,
																			   linkUrl);

							listCashStoreAdInfo.Add(adInfo);
						}
						catch (Exception e)
						{
							Debug.Log(e.Message);
						}
					}

					wwwImg.Dispose();
					wwwImg = null;
				}
			}
		}
		else
		{
			www.Dispose();
			www = null;
		}

		foreach (CashStoreAdInfoWeb info in listCashStoreAdInfo)
			Debug.LogWarning(info.ToString());

		IsLoaded = true;
	}
}
