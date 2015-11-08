using UnityEngine;
using System.Collections;

public class AsMath 
{
	const float ROUNDING_ERROR_F32 = 0.000001f;
	const double ROUNDING_ERROR_F64 = 0.00000001;
	
	static public bool Equals( float a, float b, float tolerance = ROUNDING_ERROR_F32 )
	{
		return (a + tolerance >= b) && (a - tolerance <= b);
	}
	
	static public string GetDateConvertRemainTime( int _remain, string strh, string strm, string strs )
	{	
		int hr = _remain / 3600;
		int min = _remain % 3600 / 60;
		int sec = _remain % 3600 % 60;
		
		return string.Format( "{0:D2}{1}{2:D2}{3}{4:D2}{5}", hr, strh, min, strm, sec, strs);	
	}
	
	static public string GetCoolTimeRemainTime( float _remain  )
	{			
		if( 1.0f > _remain )
		{
			return string.Format( "{0:F1}{1}", _remain, AsTableManager.Instance.GetTbl_String(90) );	
		}
		int iRemain = (int)_remain;
		int day = iRemain / 86400;
		int hr = iRemain / 3600;
		int min = iRemain % 3600 / 60;
		int sec = iRemain % 3600 % 60;
		
		if( 0 < day )
		{
			return string.Format( "{0:0}{1}", (day+1), AsTableManager.Instance.GetTbl_String(91) );	
		}
		else if( 0 < hr )
		{
			return string.Format( "{0:0}{1}", (hr+1), AsTableManager.Instance.GetTbl_String(88) );	
		}
		else if( 0 < min )
		{
			
			return string.Format( "{0:0}{1}", (min+1), AsTableManager.Instance.GetTbl_String(89) );
		}
		
		
		return string.Format( "{0:0}{1}", sec, AsTableManager.Instance.GetTbl_String(90) );			
	}
}
