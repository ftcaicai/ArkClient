using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsCharacterTextureManager
{
	private Dictionary<string,Texture> dicTexture = new Dictionary<string,Texture>();
	
	static private AsCharacterTextureManager instance = null;
	static public AsCharacterTextureManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsCharacterTextureManager();
			
			return instance;
		}
	}
	
	public Texture Get( string name)
	{
		if( true == dicTexture.ContainsKey( name))
			return dicTexture[ name];
		
		return null;
	}
	
	public void Insert( string name, Texture texture)
	{
		if( true == dicTexture.ContainsKey( name))
			return;
		
		dicTexture.Add( name, texture);
	}
	
	public void Clear()
	{
		dicTexture.Clear();
	}
}
