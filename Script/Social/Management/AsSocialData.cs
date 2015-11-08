
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class AsFaceBookFriend
{
	public UInt64 id = 0;
	public string name = "";
	public string str_id = "";
	public bool installed = false;
	
}

public class AsSMSFriend
{
	public double id = 0; //Phone.CONTACT_ID
	public string name = "";
	public string phonenumber = "";
}

public class AsTwitterFollower
{
	public double id = 0; //id
	public string name = "";
	public string screen_name = "";
	public string profile_image = "";
	
}


public class AsSocialData
{
	#region --FaceBookFriend
	private bool m_bFacebookFriendLoad;
	public bool FacebookFriendLoad
	{
		get { return m_bFacebookFriendLoad; }
		set { m_bFacebookFriendLoad = value;}
	}
	const int FACEBOOKFRIEND_LIST_PER_PAGE = 5;
	private int m_nFacebookFriendListMaxPage = 0;
	Dictionary<UInt64, AsFaceBookFriend> m_FacebookFrinedList = new Dictionary<UInt64, AsFaceBookFriend>();
	
	public Dictionary<UInt64, AsFaceBookFriend> GetFacebookFriendList()
	{
		return m_FacebookFrinedList;
	}
	
	public AsFaceBookFriend GetFacebookFriend( UInt64 _id)
	{
		if( m_FacebookFrinedList.ContainsKey( _id) == true)
			return m_FacebookFrinedList[_id];
		else 
			return null;
	}
	
	public int GetFaceBookFriendMaxPage()
	{
		if( m_FacebookFrinedList.Count < FACEBOOKFRIEND_LIST_PER_PAGE)
		{
			m_nFacebookFriendListMaxPage = 1;
		}
		else
		{
			if( ( m_FacebookFrinedList.Count % FACEBOOKFRIEND_LIST_PER_PAGE) == 0)
				m_nFacebookFriendListMaxPage = m_FacebookFrinedList.Count / FACEBOOKFRIEND_LIST_PER_PAGE;
			else
				m_nFacebookFriendListMaxPage =( m_FacebookFrinedList.Count / FACEBOOKFRIEND_LIST_PER_PAGE) + 1;
		}

		return m_nFacebookFriendListMaxPage;
	}

	public AsFaceBookFriend[] GetFaceBookFriendByPage( int page)
	{
		int index = 0;
		int pageIndex = page * FACEBOOKFRIEND_LIST_PER_PAGE;
		int id = 0;
	
		AsFaceBookFriend[] arrayFaceBook = new AsFaceBookFriend[FACEBOOKFRIEND_LIST_PER_PAGE];
		foreach( KeyValuePair<UInt64, AsFaceBookFriend> pair in m_FacebookFrinedList)
		{
			//Start
			if( index >= pageIndex && index < pageIndex + FACEBOOKFRIEND_LIST_PER_PAGE)
			{
				arrayFaceBook[id] = pair.Value;
				++id;
			}
			
			++index;
			
			if( index > pageIndex + FACEBOOKFRIEND_LIST_PER_PAGE)
				break;
		}
		
		return arrayFaceBook;
	}
	
	public void FaceBookFriendInsert( string name, string id, string installed = null)
	{
		Debug.Log( "data name: " + name + "id: " + id +"installed: "+ installed);
		UInt64 convert_id = 0;
		if( UInt64.TryParse( id , out convert_id) != true)
			return;
		
		if( m_FacebookFrinedList.ContainsKey( convert_id) != true)
		{
			AsFaceBookFriend facebookfriend = new AsFaceBookFriend();

			facebookfriend.id = convert_id;
			facebookfriend.name = name;
			facebookfriend.str_id = id;

			if( null != installed)
				facebookfriend.installed = true;

			m_FacebookFrinedList.Add( convert_id, facebookfriend);
		}
	}
	#endregion
	
	#region --SMSFriend
	private bool m_bSMSFriendLoad;
	public bool SMSFriendLoad
	{
		get { return m_bSMSFriendLoad; }
		set { m_bSMSFriendLoad = value;}
	}
	const int SMSFRIEND_LIST_PER_PAGE = 5;
	private int m_nSMSFriendListMaxPage = 0;
	Dictionary<double, AsSMSFriend> m_SMSFrinedList = new Dictionary<double, AsSMSFriend>();
	
	public Dictionary<double, AsSMSFriend> GetSMSFriendList()
	{
		return m_SMSFrinedList;
	}
	
	public AsSMSFriend GetSMSFriend( double _id)
	{
		if( m_SMSFrinedList.ContainsKey( _id) == true)
			return m_SMSFrinedList[_id];
		else
			return null;
	}
	
	public int GetSMSFriendMaxPage()
	{
		if( m_SMSFrinedList.Count < SMSFRIEND_LIST_PER_PAGE)
		{
			m_nSMSFriendListMaxPage = 1;
		}
		else
		{
			if( ( m_SMSFrinedList.Count % SMSFRIEND_LIST_PER_PAGE) == 0)
				m_nSMSFriendListMaxPage = m_SMSFrinedList.Count / SMSFRIEND_LIST_PER_PAGE;
			else
				m_nSMSFriendListMaxPage =( m_SMSFrinedList.Count / SMSFRIEND_LIST_PER_PAGE) + 1;
		}

		return m_nSMSFriendListMaxPage;
	}

	public AsSMSFriend[] GetSMSFriendByPage( int page)
	{
		int index = 0;
		int pageIndex = page * SMSFRIEND_LIST_PER_PAGE;
		int id = 0;
	
		AsSMSFriend[] arraySMS = new AsSMSFriend[SMSFRIEND_LIST_PER_PAGE];
		foreach( KeyValuePair<double, AsSMSFriend> pair in m_SMSFrinedList)
		{
			//Start
			if( index >= pageIndex && index < pageIndex + SMSFRIEND_LIST_PER_PAGE)
			{
				arraySMS[id] = pair.Value;
				++id;
			}
			
			++index;
			
			if( index > pageIndex + SMSFRIEND_LIST_PER_PAGE)
				break;
		}
		
		return arraySMS;
	}

	public void SMSFriendInsert( string name, double id, string phonenumber)
	{
		Debug.Log( "data name: " + name + "id: " + id +"phonenumber: "+ phonenumber);

		if( m_SMSFrinedList.ContainsKey( id) != true)
		{
			AsSMSFriend smsfriend = new AsSMSFriend();

			smsfriend.id = id;
			smsfriend.name = name;
			smsfriend.phonenumber = phonenumber;

			m_SMSFrinedList.Add( id, smsfriend);
		}
	}
	#endregion
	
	
	#region --TwitterFollowers
	private bool m_bTwitterFollowersLoad;
	public bool TwitterFollowersLoad
	{
		get { return m_bTwitterFollowersLoad; }
		set { m_bTwitterFollowersLoad = value;}
	}
	const int TWITTERFOLLOWERS_LIST_PER_PAGE = 5;
	private int m_nTwitterFollowersListMaxPage = 0;
	Dictionary<double, AsTwitterFollower> m_TwitterFollowerList = new Dictionary<double, AsTwitterFollower>();
	
	public Dictionary<double, AsTwitterFollower> GetTwitterFollowerList()
	{
		return m_TwitterFollowerList;
	}
	
	public AsTwitterFollower GetTwitterFollower( double _id)
	{
		if( m_TwitterFollowerList.ContainsKey( _id) == true)
			return m_TwitterFollowerList[_id];
		else
			return null;
	}
	
	public int GetTwitterFollowerMaxPage()
	{
		if( m_TwitterFollowerList.Count < TWITTERFOLLOWERS_LIST_PER_PAGE)
		{
			m_nTwitterFollowersListMaxPage = 1;
		}
		else
		{
			if( ( m_TwitterFollowerList.Count % TWITTERFOLLOWERS_LIST_PER_PAGE) == 0)
				m_nTwitterFollowersListMaxPage = m_TwitterFollowerList.Count / TWITTERFOLLOWERS_LIST_PER_PAGE;
			else
				m_nTwitterFollowersListMaxPage =( m_TwitterFollowerList.Count / TWITTERFOLLOWERS_LIST_PER_PAGE) + 1;
		}

		return m_nTwitterFollowersListMaxPage;
	}

	public AsTwitterFollower[] GetTwitterFollowerByPage( int page)
	{
		int index = 0;
		int pageIndex = page * TWITTERFOLLOWERS_LIST_PER_PAGE;
		int id = 0;
	
		AsTwitterFollower[] arrayTW = new AsTwitterFollower[TWITTERFOLLOWERS_LIST_PER_PAGE];
		foreach( KeyValuePair<double, AsTwitterFollower> pair in m_TwitterFollowerList)
		{
			//Start
			if( index >= pageIndex && index < pageIndex + TWITTERFOLLOWERS_LIST_PER_PAGE)
			{
				arrayTW[id] = pair.Value;
				++id;
			}
			
			++index;
			
			if( index > pageIndex + TWITTERFOLLOWERS_LIST_PER_PAGE)
				break;
		}
		
		return arrayTW;
	}

	public void TwitterFollowerInsert( string name, double id, string screen_name, string profile_image)
	{
		Debug.Log( "data name: " + name + "id: " + id +"screen_name: "+ screen_name+"profile_image: "+ profile_image);

		if( m_TwitterFollowerList.ContainsKey( id) != true)
		{
			AsTwitterFollower twitterFollower = new AsTwitterFollower();

			twitterFollower.id = id;
			twitterFollower.name = name;
			twitterFollower.screen_name = screen_name;
			twitterFollower.profile_image = profile_image;
			
			m_TwitterFollowerList.Add( id, twitterFollower);
		}
	}
	#endregion
	

	#region --BlockOutList
	const int BLOCK_LIST_PER_PAGE = 5;
	private int m_nBlockListMaxPage = 0;
	Dictionary<uint, body2_SC_BLOCKOUT_LIST> m_BlockOutList = new Dictionary<uint, body2_SC_BLOCKOUT_LIST>();

	public Dictionary<uint, body2_SC_BLOCKOUT_LIST> GetBlockOutList()
	{
		return m_BlockOutList;
	}
	
	public body2_SC_BLOCKOUT_LIST GetBlockOut( uint _id)
	{
		if( m_BlockOutList.ContainsKey( _id) == true)
			return m_BlockOutList[_id];
		else
			return null;
	}
	
	public int GetBlockListMaxPage()
	{
		if( m_BlockOutList.Count < BLOCK_LIST_PER_PAGE)
		{
			m_nBlockListMaxPage = 1;
		}
		else
		{
			if( ( m_BlockOutList.Count % BLOCK_LIST_PER_PAGE) == 0)
				m_nBlockListMaxPage = m_BlockOutList.Count / BLOCK_LIST_PER_PAGE;
			else
				m_nBlockListMaxPage =( m_BlockOutList.Count / BLOCK_LIST_PER_PAGE) + 1;
		}

		return m_nBlockListMaxPage;
	}

	public body2_SC_BLOCKOUT_LIST[] GetBlockByPage( int page)
	{
		int index = 0;
		int pageIndex = page * BLOCK_LIST_PER_PAGE;
		int id = 0;
	
		body2_SC_BLOCKOUT_LIST[] arrayBlock = new body2_SC_BLOCKOUT_LIST[BLOCK_LIST_PER_PAGE];
		foreach( KeyValuePair<uint, body2_SC_BLOCKOUT_LIST> pair in m_BlockOutList)
		{
			//Start
			if( index >= pageIndex && index < pageIndex + BLOCK_LIST_PER_PAGE)
			{
				arrayBlock[id] = pair.Value;
				++id;
			}
			
			++index;
			
			if( index > pageIndex + BLOCK_LIST_PER_PAGE)
				break;
			
		}
		
		return arrayBlock;
	}
	#endregion --BlockOutList

	private int m_nSocialPoint;
	public int SocialPoint
	{
		get { return m_nSocialPoint; }
		set { m_nSocialPoint = value;}
	}
	private int m_nMaxSocialPoint;
	public int MaxSocialPoint
	{
		get { return m_nMaxSocialPoint; }
		set { m_nMaxSocialPoint = value; }
	}
	private short m_nHelloCount;
	public short HelloCount
	{
		get { return m_nHelloCount; }
		set { m_nHelloCount = value; }
	}
	
	private short m_nMaxHelloCount;
	public short MaxHelloCount
	{
		get { return m_nMaxHelloCount; }
		set { m_nMaxHelloCount = value; }
	}

	public body_SC_SOCIAL_INFO m_SocialInfo;
	public body_SC_SOCIAL_INFO SocialInfo
	{
		get { return m_SocialInfo; }
		set { m_SocialInfo = value; }
	}

	private body2_SC_FRIEND_LIST m_FriendItem = null;
	public body2_SC_FRIEND_LIST FriendItem
	{
		get { return m_FriendItem; }
		set { m_FriendItem = value; }
	}
	
	private string m_ConnectReuqestUserName;
	public string ConnectReuqestUserName
	{
		get { return m_ConnectReuqestUserName; }
		set { m_ConnectReuqestUserName = value; }
	}
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void ClearList()
	{
		m_BlockOutList.Clear();
		m_FacebookFrinedList.Clear();
		FacebookFriendLoad = false;
		
		m_SMSFrinedList.Clear();
		SMSFriendLoad = false;
	}

	public body2_SC_BLOCKOUT_LIST GetBlockOutUser( uint _id)
	{
		if( m_BlockOutList.ContainsKey( _id) == true)
			return m_BlockOutList[_id];
		else
			return null;
	}
	
	public body2_SC_BLOCKOUT_LIST GetBlockOutUserByName( string name)
	{
		foreach( KeyValuePair<uint, body2_SC_BLOCKOUT_LIST> pair in m_BlockOutList)
		{
			if( pair.Value.szUserId.CompareTo( name) == 0)
				return pair.Value;
		}
	
		return null;
	}
	
	public void ReceiveBlockOutList( body2_SC_BLOCKOUT_LIST[] list)
	{
		m_BlockOutList.Clear();
		
		if( null == list)
			return;
		
		foreach( body2_SC_BLOCKOUT_LIST data in list)
			m_BlockOutList.Add( data.nUserUniqKey, data);
	}
	
	public void ReceiveBlockOutInsert( body_SC_BLOCKOUT_INSERT blockOutInsert)
	{
		if( m_BlockOutList.ContainsKey( blockOutInsert.nUserUniqKey) != true)
		{
			body2_SC_BLOCKOUT_LIST data = new body2_SC_BLOCKOUT_LIST();
			data.nUserUniqKey = blockOutInsert.nUserUniqKey;
			data.szUserId = AsUtil.GetRealString( System.Text.UTF8Encoding.UTF8.GetString( blockOutInsert.szUserId));
			m_BlockOutList.Add( blockOutInsert.nUserUniqKey, data);
		}
	}
	
	public void ReceiveBlockOutDelete( body_SC_BLOCKOUT_DELETE blockOutDelete)
	{
		m_BlockOutList.Remove( blockOutDelete.nUserUniqKey);
	}
}
