using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsShaderManager
{
	private Dictionary<string,Material> dicMaterial = new Dictionary<string,Material>();
	
	static private AsShaderManager instance = null;
	static public AsShaderManager Instance
	{
		get
		{
			if( null == instance)
				instance = new AsShaderManager();
			
			return instance;
		}
	}
	
	private AsShaderManager()
	{
	}
	
	public void Init()
	{
		Insert( "newark_shader");
		Insert( "Unlit/Texture");
		Insert( "Unlit/Transparent");
		Insert( "Diffuse");
		Insert( "Mobile/Particles/Additive");
		Insert( "Mobile/Particles/Multiply");
		Insert( "Mobile/Particles/Alpha Blended");
		Insert( "Mobile/VertexLit");
		Insert( "Particles/Additive");
		Insert( "Custom/AsGUIWithAlpha");

		// < fx shader
		Insert( "TexurePan_02");
		Insert( "PanY");
		Insert( "colorInvert");
		Insert( "sortPanY");
		Insert( "unlitPanY");
		// fx shader >
		
//		Shader.WarmupAllShaders();
	}
	
	public void Insert( string name)
	{
		if( true == dicMaterial.ContainsKey( name.ToLower()))
			return;
		
		Material mtrl = new Material( Shader.Find( name));
		dicMaterial.Add( name.ToLower(), mtrl);
	}
	
	public Shader Get( string name)
	{
		if( false == dicMaterial.ContainsKey( name))
		{
			Debug.LogWarning( "Shader not exist!!! - " + name);
			return dicMaterial[ "mobile/vertexlit"].shader;
		}
		
		return dicMaterial[ name].shader;
	}
	
	public Material CreateMaterial( string strShaderName, Texture tex)
	{
		Shader shader = Get( strShaderName);
		Material mtrl = new Material( shader);
		
		if( strShaderName.Equals( "newark_shader"))
			mtrl.SetTexture( "_maintex", tex);
		else
			mtrl.SetTexture( "_MainTex", tex);
		
		return mtrl;
	}
}
