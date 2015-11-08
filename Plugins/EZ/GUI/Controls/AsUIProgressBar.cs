using UnityEngine;
using System.Collections;
using System;

[AddComponentMenu( "ArkSphere/Controls/AsProgress Bar" )]
[ExecuteInEditMode]
public class AsUIProgressBar : AutoSpriteControlBase
{
	enum PROGRESS_STATE
	{
		PS_INVALID = -1,
		PS_INCREASE,
		PS_DECREASE,
		PS_EQUAL,

		MAX_STATE
	};

	enum PROGRESS_STEP
	{
		STP_INVALID = -1,
		STP_STEP1,
		STP_STEP2,

		MAX_STEP
	};

	public float speed = 0.1f;
//	private float step_1_value = Single.MinValue;
//	private float step_2_value = Single.MinValue;
	private float step_1_value = 1.0f;
	private float step_2_value = 1.0f;
	private PROGRESS_STATE state = PROGRESS_STATE.PS_INVALID;
	private PROGRESS_STEP step = PROGRESS_STEP.STP_INVALID;
	protected float m_value = 1.0f;

	public float Value
	{
		get	{ return m_value; }
		set
		{
			m_value = Mathf.Clamp01( value );

			if( Single.MinValue == step_2_value)
			{
				step_2_value = m_value;
				step_1_value = m_value;
			}

			if( step_2_value > m_value)
				state = PROGRESS_STATE.PS_DECREASE;
			else if( step_2_value < m_value)
				state = PROGRESS_STATE.PS_INCREASE;
			else
				state = PROGRESS_STATE.PS_EQUAL;

			step = PROGRESS_STEP.STP_STEP1;

			UpdateProgress();
		}
	}
	
	public float GuideValue
	{
		set { step_1_value = value; }
	}

	protected AutoSprite emptySprite;
	protected AutoSprite afterImageSprite;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim( "Filled"),
			new TextureAnim( "AfterImage"),
			new TextureAnim( "Empty")
		};

	public override TextureAnim[] States
	{
		get	{ return states; }
		set	{ states = value; }
	}

	public override EZTransitionList GetTransitions( int index )
	{
		return null;
	}

	public override EZTransitionList[] Transitions
	{
		get	{ return null; }
		set	{}
	}

	public SpriteRoot[] filledLayers = new SpriteRoot[0];
	public SpriteRoot[] afterImageLayers = new SpriteRoot[0];
	public SpriteRoot[] emptyLayers = new SpriteRoot[0];

	protected int[] filledIndices;
	protected int[] afterImageIndices;
	protected int[] emptyIndices;

	public override void OnInput( ref POINTER_INFO ptr )
	{
	}

	// Use this for initialization
	public override void Start()
	{
		if( m_started )
			return;

		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[3][];
		aggregateLayers[0] = filledLayers;
		aggregateLayers[1] = afterImageLayers;
		aggregateLayers[2] = emptyLayers;

		// Runtime init stuff:
		if( Application.isPlaying )
		{
			filledIndices = new int[filledLayers.Length];
			afterImageIndices = new int[afterImageLayers.Length];
			emptyIndices = new int[emptyLayers.Length];

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for( int i = 0; i < filledLayers.Length; ++i )
			{
				if( filledLayers[i] == null )
				{
					Debug.LogError( "A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element." );
					continue;
				}

				filledIndices[i] = filledLayers[i].GetStateIndex( "filled" );
				if( filledIndices[i] != -1 )
					filledLayers[i].SetState( filledIndices[i] );
			}

			for( int i = 0; i < afterImageLayers.Length; ++i)
			{
				if( null == afterImageLayers[i])
				{
					Debug.LogError( "A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element." );
					continue;
				}

				afterImageIndices[i] = afterImageLayers[i].GetStateIndex( "afterimage");
				if( afterImageIndices[i] != -1)
					afterImageLayers[i].SetState( afterImageIndices[i]);
			}

			for( int i = 0; i < emptyLayers.Length; ++i )
			{
				if( emptyLayers[i] == null )
				{
					Debug.LogError( "A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element." );
					continue;
				}

				emptyIndices[i] = emptyLayers[i].GetStateIndex( "empty" );
				if( emptyIndices[i] != -1 )
					emptyLayers[i].SetState( emptyIndices[i] );
			}

			GameObject afterImageObj = new GameObject();
			afterImageObj.name = name + " - AfterImage";
			afterImageObj.transform.parent = transform;
			//afterImageObj.transform.localPosition = Vector3.zero;
			afterImageObj.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.1f );
			afterImageObj.transform.localScale = Vector3.one;
			afterImageObj.transform.localRotation = Quaternion.identity;
			afterImageObj.layer = gameObject.layer;

			afterImageSprite = (AutoSprite)afterImageObj.AddComponent( typeof( AutoSprite ) );
			afterImageSprite.plane = plane;
			afterImageSprite.autoResize = autoResize;
			afterImageSprite.pixelPerfect = pixelPerfect;
			afterImageSprite.persistent = persistent;
			//afterImageSprite.ignoreClipping = ignoreClipping;
			afterImageSprite.bleedCompensation = bleedCompensation;

			if( !managed )
			{
				afterImageSprite.renderer.sharedMaterial = renderer.sharedMaterial;
			}
			else
			{
				if( manager != null )
				{
					afterImageSprite.Managed = managed;
					manager.AddSprite( afterImageSprite );
					afterImageSprite.SetDrawLayer( drawLayer );	// Knob should be drawn in front of the bar
				}
				else
					Debug.LogError( "Sprite on object \"" + name + "\" not assigned to a SpriteManager!" );
			}
			afterImageSprite.color = color;
			afterImageSprite.SetAnchor( anchor );
			afterImageSprite.Setup( width, height, m_spriteMesh.material );
			if( states[1].spriteFrames.Length != 0 )
			{
				afterImageSprite.animations = new UVAnimation[1];
				afterImageSprite.animations[0] = new UVAnimation();
				afterImageSprite.animations[0].SetAnim( states[1], 0 );
				afterImageSprite.PlayAnim( 0, 0 );
			}
			afterImageSprite.renderCamera = renderCamera;
			afterImageSprite.Hide( IsHidden() );

			// Create our other sprite for the 
			// empty/background portion:
			GameObject emptyObj = new GameObject();
			emptyObj.name = name + " - Empty Bar";
			emptyObj.transform.parent = transform;
			//emptyObj.transform.localPosition = Vector3.zero;
			emptyObj.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.2f );
			emptyObj.transform.localScale = Vector3.one;
			emptyObj.transform.localRotation = Quaternion.identity;
			emptyObj.layer = gameObject.layer;

			emptySprite = (AutoSprite)emptyObj.AddComponent( typeof( AutoSprite ) );
			emptySprite.plane = plane;
			emptySprite.autoResize = autoResize;
			emptySprite.pixelPerfect = pixelPerfect;
			emptySprite.persistent = persistent;
			//emptySprite.ignoreClipping = ignoreClipping;
			emptySprite.bleedCompensation = bleedCompensation;

			if( !managed )
			{
				emptySprite.renderer.sharedMaterial = renderer.sharedMaterial;
			}
			else
			{
				if( manager != null )
				{
					emptySprite.Managed = managed;
					manager.AddSprite( emptySprite );
					emptySprite.SetDrawLayer( drawLayer );	// Knob should be drawn in front of the bar
				}
				else
					Debug.LogError( "Sprite on object \"" + name + "\" not assigned to a SpriteManager!" );
			}
			emptySprite.color = color;
			emptySprite.SetAnchor( anchor );
			emptySprite.Setup( width, height, m_spriteMesh.material );
			if( states[2].spriteFrames.Length != 0 )
			{
				emptySprite.animations = new UVAnimation[1];
				emptySprite.animations[0] = new UVAnimation();
				emptySprite.animations[0].SetAnim( states[2], 0 );
				emptySprite.PlayAnim( 0, 0 );
			}
			emptySprite.renderCamera = renderCamera;
			emptySprite.Hide( IsHidden() );

			Value = m_value;

			if( container != null )
				container.AddChild( emptyObj );

			SetState( 0 );
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if( managed && m_hidden )
			Hide( true );
	}

	public override void SetSize( float width, float height )
	{
		base.SetSize( width, height );

		if( ( null == emptySprite ) || ( null == afterImageSprite ) )
			return;

		emptySprite.SetSize( width, height );
		afterImageSprite.SetSize( width, height);
	}

	public override void Copy( SpriteRoot s )
	{
		Copy( s, ControlCopyFlags.All );
	}

	public override void Copy( SpriteRoot s, ControlCopyFlags flags )
	{
		base.Copy( s, flags );

		if( !( s is AsUIProgressBar ) )
			return;

		if( Application.isPlaying )
		{
			AsUIProgressBar b = (AsUIProgressBar)s;

			if( ( flags & ControlCopyFlags.Appearance ) == ControlCopyFlags.Appearance )
			{
				if( emptySprite != null )
					emptySprite.Copy( b.emptySprite );

				if( null != afterImageSprite)
					afterImageSprite.Copy( b.afterImageSprite);
			}
		}
	}

	public override void InitUVs()
	{
		if( states[0].spriteFrames.Length != 0 )
			frameInfo.Copy( states[0].spriteFrames[0] );

		base.InitUVs();
	}

	void Update()
	{
		if( PROGRESS_STEP.STP_INVALID != step)
			UpdateProgress();
	}

	private void UpdateIncreaseProgress()
	{
		switch( step)
		{
		case PROGRESS_STEP.STP_STEP1:
			{
				if( null == afterImageSprite )
					break;

				step_1_value += ( Time.deltaTime * speed );

				if( m_value <= step_1_value )
				{
					step_1_value = m_value;
					step = PROGRESS_STEP.STP_STEP2;
				}

				afterImageSprite.TruncateRight( step_1_value );

				for( int i = 0; i < afterImageLayers.Length; ++i )
					afterImageLayers[i].TruncateRight( step_1_value );
			}
			break;
		case PROGRESS_STEP.STP_STEP2:
			{
				if( null == emptySprite )
					break;

				step_2_value += ( Time.deltaTime * speed );

				if( m_value <= step_2_value )
				{
					step_2_value = m_value;
					step = PROGRESS_STEP.STP_INVALID;
				}

				this.TruncateRight( step_2_value );

				for( int i = 0; i < filledLayers.Length; ++i )
					filledLayers[i].TruncateRight( step_2_value );
			}
			break;
		default:
			{
				Debug.Log( "Invalid step");
			}
			break;
		}
	}

	private void UpdateDecreaseProgress()
	{
		switch( step)
		{
		case PROGRESS_STEP.STP_STEP1:
			{
				if( null == emptySprite )
					break;

				step_2_value -= ( Time.deltaTime * speed );
				if( m_value > step_2_value )
				{
					step_2_value = m_value;
					step = PROGRESS_STEP.STP_STEP2;
				}

				this.TruncateRight( step_2_value );

				for( int i = 0; i < filledLayers.Length; ++i )
					filledLayers[i].TruncateRight( step_2_value );
			}
			break;
		case PROGRESS_STEP.STP_STEP2:
			{
				if( null == afterImageSprite )
					break;

				step_1_value -= ( Time.deltaTime * speed );

				if( m_value > step_1_value )
				{
					step_1_value = m_value;
					step = PROGRESS_STEP.STP_INVALID;
				}

				afterImageSprite.TruncateRight( step_1_value );

				for( int i = 0; i < afterImageLayers.Length; ++i )
					afterImageLayers[i].TruncateRight( step_1_value );
			}
			break;
		default:
			{
				Debug.Log( "Invalid state");
			}
			break;
		}
	}

	private void UpdateEqualProgress()
	{
		if( null == afterImageSprite ) 
			return;

		afterImageSprite.TruncateRight( m_value );

		for( int i = 0; i < afterImageLayers.Length; ++i )
			afterImageLayers[i].TruncateRight( m_value );

		this.TruncateRight( m_value );

		for( int i = 0; i < filledLayers.Length; ++i )
			filledLayers[i].TruncateRight( m_value );
	}

	protected void UpdateProgress()
	{
		switch( state)
		{
		case PROGRESS_STATE.PS_INCREASE:
			{
				UpdateIncreaseProgress();
			}
			break;
		case PROGRESS_STATE.PS_DECREASE:
			{
				UpdateDecreaseProgress();
			}
			break;
		case PROGRESS_STATE.PS_EQUAL:
			{
				UpdateEqualProgress();
			}
			break;
		default:
			{
				Debug.Log( "Invalid progress state");
			}
			break;
		}
	}

	public override IUIContainer Container
	{
		get	{ return base.Container; }

		set
		{
			if( value != container )
			{
				if( container != null )
					container.RemoveChild( emptySprite.gameObject );

				if( value != null )
					if( emptySprite != null )
						value.AddChild( emptySprite.gameObject );
			}

			base.Container = value;
		}
	}

	public override void Unclip()
	{
		if( ignoreClipping )
			return;

		base.Unclip();
		emptySprite.Unclip();
		afterImageSprite.Unclip();
	}

	public override bool Clipped
	{
		get	{ return base.Clipped; }
		set
		{
			if( ignoreClipping )
				return;

			base.Clipped = value;
			emptySprite.Clipped = value;
			afterImageSprite.Clipped = value;
		}
	}

	public override Rect3D ClippingRect
	{
		get	{ return base.ClippingRect; }
		set
		{
			if( ignoreClipping )
				return;

			base.ClippingRect = value;
			emptySprite.ClippingRect = value;
			afterImageSprite.ClippingRect = value;
		}
	}

	static public AsUIProgressBar Create( string name, Vector3 pos )
	{
		GameObject go = new GameObject( name );
		go.transform.position = pos;
		return (AsUIProgressBar)go.AddComponent( typeof( AsUIProgressBar ) );
	}

	static public AsUIProgressBar Create( string name, Vector3 pos, Quaternion rotation )
	{
		GameObject go = new GameObject( name );
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (AsUIProgressBar)go.AddComponent( typeof( AsUIProgressBar ) );
	}

	public override void Hide( bool tf )
	{
		base.Hide( tf );

		if( emptySprite != null )
			emptySprite.Hide( tf );

		if( null != afterImageSprite)
			afterImageSprite.Hide( tf);
	}

	public override void SetColor( Color c )
	{
		base.SetColor( c );

		if( emptySprite != null )
			emptySprite.SetColor( c );

		if( null != afterImageSprite)
			afterImageSprite.SetColor( c);
	}
	
//	public void Init()
//	{
//		step_1_value = step_2_value = 1.0f;
//	}
	
	public void Init( float value)
	{
		m_value = Mathf.Clamp01( value);
		step_1_value = value;
		step_2_value = value;
		state = PROGRESS_STATE.PS_EQUAL;
		
		UpdateProgress();
	}
}
