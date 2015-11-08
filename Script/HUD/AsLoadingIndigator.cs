using UnityEngine;
using System.Collections;

public class AsLoadingIndigator : MonoBehaviour
{
	[SerializeField]private SpriteText msg = null;
	[SerializeField]private PackedSprite gear = null;

    private Vector3 vOrgPos = Vector3.zero;
	private static AsLoadingIndigator instance = null;
	public static AsLoadingIndigator Instance
	{
		get
		{
			if( null == instance)
				instance = FindObjectOfType( typeof( AsLoadingIndigator)) as AsLoadingIndigator;
			
			if( null == instance)
			{
				GameObject obj = new GameObject( "LoadingIndigator");
				instance = obj.AddComponent( typeof( AsLoadingIndigator)) as AsLoadingIndigator;
			}
			
			return instance;
		}
	}
	
	void Awake()
	{
		DontDestroyOnLoad( gameObject);
	}
	
	// Use this for initialization
	void Start()
	{
        vOrgPos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	public void ShowIndigator( string msg)
	{
		this.msg.gameObject.SetActiveRecursively( true);
		this.gear.gameObject.SetActiveRecursively( true);
		this.msg.Text = msg;
	}

    public void ShowIndigator( Vector3 vPos, string msg)
    {
		gameObject.transform.position = vPos;
        ShowIndigator( msg);
    }
	
	public void HideIndigator()
	{
		if( GAME_STATE.STATE_LOADING == AsGameMain.s_gameState)
			return;
		
		msg.gameObject.SetActiveRecursively( false);
		gear.gameObject.SetActiveRecursively( false);
        gameObject.transform.position = vOrgPos;
	}
}
