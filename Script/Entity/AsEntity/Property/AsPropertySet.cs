using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsPropertySet
{
	Dictionary<eComponentProperty, AsProperty> properties_ = new Dictionary<eComponentProperty, AsProperty>();

	public AsPropertySet Clone()
	{
		AsPropertySet clone = new AsPropertySet();

		foreach( KeyValuePair<eComponentProperty, AsProperty> container in properties_)
				clone.properties_.Add( container.Key, ( AsProperty)container.Value.Clone());

		return clone;
	}

	public AsProperty GetProperty( eComponentProperty id)
	{
		if( properties_.ContainsKey( id))
			return properties_[id];

		return null;
	}

	public bool ContainProperty( eComponentProperty id)
	{
		return properties_.ContainsKey( id);
	}

	public AsProperty InitProperty( string id, string type)
	{
		eComponentProperty propEnum = AsProperty.GetPropertyEnum( id);
		if( ContainProperty( propEnum) == false)
		{
			AsProperty prop = new AsProperty( id, type);
			properties_.Add( propEnum, prop);
			return prop;
		}
		else
			return null;
	}

	public void RemoveProperty( eComponentProperty id)
	{
		if( properties_.ContainsKey( id))
		{
			properties_[id] = null;
			properties_.Remove( id);
		}
	}

	public void SetDefaultValue( eComponentProperty id,  System.Object v)
	{
		AsProperty prop = GetProperty( id);
		if( prop == null)
			Debug.LogError( "[AsPropertySet]SetDefaultValue: Invalid id -" + id + "," + v.GetType());
		else
			prop.SetDefaultValue( v);
	}

	public System.Object GetDefaultValue( eComponentProperty id)
	{
		AsProperty prop = GetProperty( id);
		if( prop == null)
		{
			Debug.LogError( "[AsPropertySet]SetDefaultValue: Invalid id -" + id);
			return null;
		}
		else
			return prop.GetDefaultValue();
	}

	public void SetValue( eComponentProperty id,  System.Object v)
	{
		AsProperty prop = GetProperty( id);
		if( prop == null)
			Debug.LogError( "[AsPropertySet]SetDefaultValue: Invalid id - " + id + ", " + v.GetType());
		else
			prop.SetValue( v);
	}

	public System.Object GetValue( eComponentProperty id)
	{
		AsProperty prop = GetProperty( id);
		if( prop == null)
		{
			Debug.LogError( "[AsPropertySet]SetDefaultValue: Invalid id -" + id);
			return null;
		}

		return prop.GetValue();
	}
}