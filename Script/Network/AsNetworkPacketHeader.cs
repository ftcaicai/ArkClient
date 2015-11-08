using System;
using System.Reflection;
using System.Text;
using System.IO;
using System.Net;
using UnityEngine;

public class AsBaseClass
{
	public enum ePackingLength
	{
		PL_MAX = 4096,
	};

	public static byte[] StringToByteArray( string str, int length)
	{
		return Encoding.UTF8.GetBytes( str.PadRight( length, '\0'));
//		return Encoding.ASCII.GetBytes( str.PadRight( length, '\0'));
	}

	public static char[] StringToCharArray( string str, int length)
	{
		return Encoding.UTF8.GetChars( StringToByteArray( str, length));
//		return Encoding.ASCII.GetChars( StringToByteArray( str, length));
	}

	#region new
	public byte[] ClassToByteArray()
	{
		byte[] data = new byte[(int)ePackingLength.PL_MAX];
		byte[] tmpData;
		int size = 0;
		MemoryStream ms = new MemoryStream( data);
		BinaryWriter bw = new BinaryWriter( ms);

		Type infotype = this.GetType();
		FieldInfo[] infolist = infotype.GetFields( BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly);
		foreach( FieldInfo info in infolist)
		{
			if( info.FieldType.IsArray == false)
			{
				switch( Convert.ToString( info.FieldType))
				{
				case "System.Boolean":	bw.Write( (Boolean)info.GetValue( this));	break;
				case "System.Byte":	bw.Write( (Byte)info.GetValue( this));	break;
				case "System.SByte":	bw.Write( (SByte)info.GetValue( this));	break;
				case "System.Char":	bw.Write( (Char)info.GetValue( this));	break;
				case "System.Int16":	bw.Write( (Int16)info.GetValue( this));	break;
				case "System.UInt16":	bw.Write( (UInt16)info.GetValue( this));	break;
				case "System.Int32":	bw.Write( (Int32)info.GetValue( this));	break;
				case "System.UInt32":	bw.Write( (UInt32)info.GetValue( this));	break;
				case "System.Int64":	bw.Write( (Int64)info.GetValue( this));	break;
				case "System.UInt64":	bw.Write( (UInt64)info.GetValue( this));	break;
				case "System.Single":	bw.Write( (Single)info.GetValue( this));	break;
				case "System.Double":	bw.Write( (double)info.GetValue( this));	break;
				case "UnityEngine.Vector3":
					Vector3 v3 = (Vector3)info.GetValue( this);
					bw.Write( v3.x);
					bw.Write( v3.y);
					bw.Write( v3.z);
					break;
				case "eGENDER": bw.Write( (Int32)info.GetValue( this));	break;
				case "eCLASS": bw.Write( (Int32)info.GetValue( this));	break;
				case "eRACE": bw.Write( (Int32)info.GetValue( this));	break;
				case "sSKILL":
					sSKILL sSkill = (sSKILL)info.GetValue( this);
					bw.Write( sSkill.nSkillTableIdx);
					bw.Write( sSkill.nSkillLevel);
					break;
				case "sPOSTITEM":
					sPOSTITEM postItem = (sPOSTITEM)info.GetValue( this);
					bw.Write( postItem.nInentorySlot);
					bw.Write( postItem.nOverlapped);
					break;
				case "eCHATTYPE": bw.Write( (Int32)info.GetValue( this)); break;
				case "eCONFUSIONTYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eGUILDJOINTYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eGUILDMEMBER_DELETE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eGUILDPERMISSION":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eGUILD_UI_SCROLL":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eSTORAGE_MOVE_TYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eOSTYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eCOUPON_TYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eCONDITION_TYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eEVENT_TYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "AsEmotionManager+eCHAT_FILTER": bw.Write( (Int32)info.GetValue( this));	break;
				case "ePOST_ADDRESS_BOOK_TYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "eEQUIPITEM_GRADE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "ePRIVATESHOPSEARCHTYPE":	bw.Write( (Int32)info.GetValue( this));	break;
				case "ePET_SKILL_TYPE": bw.Write( (Int32)info.GetValue( this));	break;
				case "ePET_HUNGRY_STATE": bw.Write( (Int32)info.GetValue( this));	break;
				default:
					if( Convert.ToString( info.FieldType.BaseType) == "BaseClass")
					{
						tmpData = ((AsBaseClass)info.GetValue( this)).ClassToByteArray();
						bw.Write( tmpData);
					}
					else
					{
						Debug.LogWarning( "AsNteworkPacketHeader::ClassToByteArray: " + Convert.ToString( info.FieldType));
					}
					break;
				}
			}
			else
			{
				switch( Convert.ToString( info.FieldType))
				{
				case "System.Boolean[]":
					{
						bw.Write( (Boolean)info.GetValue( this));
					}
					break;
				case "System.Byte[]":
					{
						size = ((Byte[])info.GetValue( this)).GetLength(0);
						Byte[] tmpByte = (Byte[])info.GetValue( this);
						bw.Write( tmpByte);
					}
					break;
				case "System.Char[]":
					{
						size = ((Char[])info.GetValue( this)).GetLength(0);
						Char[] tmpChar = (Char[])info.GetValue( this);
						tmpData = new byte[ 2 * size];
						Buffer.BlockCopy( tmpChar, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.Int16[]":
					{
						size = ((Int16[])info.GetValue( this)).GetLength(0);
						Int16[] tmpInt16 = (Int16[])info.GetValue( this);
						tmpData = new byte[ 4 * size];
						Buffer.BlockCopy( tmpInt16, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.UInt16[]":
					{
						size = ((UInt16[])info.GetValue( this)).GetLength(0);
						UInt16[] tmpUInt16 = (UInt16[])info.GetValue( this);
						tmpData = new byte[ 4 * size];
						Buffer.BlockCopy( tmpUInt16, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.Int32[]":
					{
						size = ((Int32[])info.GetValue( this)).GetLength(0);
						Int32[] tmpInt32 = (Int32[])info.GetValue( this);
						tmpData = new byte[ 4 * size];
						Buffer.BlockCopy( tmpInt32, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.UInt32[]":
					{
						size = ((UInt32[])info.GetValue( this)).GetLength(0);
						UInt32[] tmpUInt32 = (UInt32[])info.GetValue( this);
						tmpData = new byte[ 4 * size];
						Buffer.BlockCopy( tmpUInt32, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.Int64[]":
					{
						size = ((Int64[])info.GetValue( this)).GetLength(0);
						Int64[] tmpInt64 = (Int64[])info.GetValue( this);
						tmpData = new byte[ 8 * size];
						Buffer.BlockCopy( tmpInt64, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.Single[]":
					{
						size = ((Single[])info.GetValue( this)).GetLength(0);
						Single[] tmpSingle = (Single[])info.GetValue( this);
						tmpData = new byte[ 4 * size];
						Buffer.BlockCopy( tmpSingle, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "System.Double[]":
					{
						size = ((Double[])info.GetValue( this)).GetLength(0);
						Double[] tmpDouble = (Double[])info.GetValue( this);
						tmpData = new byte[ 8 * size];
						Buffer.BlockCopy( tmpDouble, 0, tmpData, 0, tmpData.Length);
						bw.Write( tmpData);
					}
					break;
				case "sPOSTITEM[]":
					size = ((sPOSTITEM[])info.GetValue( this)).GetLength(0);
					sPOSTITEM[] temp = (sPOSTITEM[])info.GetValue( this);
					tmpData = new byte[ size * 8];
					Buffer.BlockCopy( temp, 0, tmpData, 0, tmpData.Length);
					bw.Write( tmpData);
					break;
				default:
					{
						if( Convert.ToString( info.FieldType.BaseType) == "System.Array")
						{
							AsBaseClass[] tmpClass = (AsBaseClass[])info.GetValue( this);
							int i = 0;
							foreach( AsBaseClass it in tmpClass)
							{
								++i;
								tmpData = it.ClassToByteArray();
								bw.Write( tmpData);
							}
						}
					}
					break;
				}
			}
		}

		tmpData = new byte[(int)ms.Position];
		Buffer.BlockCopy( data, 0, tmpData, 0, tmpData.Length);

		bw.Close();
		ms.Close();

		return tmpData;
	}

	public int ByteArrayToClass( byte[] data)
	{
		byte[] tmpData;
		int size = 0;
		MemoryStream ms = new MemoryStream( data);
		BinaryReader br = new BinaryReader( ms);
		Type infotype = this.GetType();
		FieldInfo[] infolist = infotype.GetFields( BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.DeclaredOnly);
		foreach( FieldInfo info in infolist)
		{
			if( info.FieldType.IsArray == false)
			{
				string type = Convert.ToString( info.FieldType);

				switch( type)
				{
				case "System.Boolean":	info.SetValue( this, br.ReadBoolean());	break;
				case "System.Byte":	info.SetValue( this, br.ReadByte());	break;
				case "System.SByte":	info.SetValue( this, br.ReadSByte());	break;
				case "System.Char":	info.SetValue( this, br.ReadChar());	break;
				case "System.Int16":	info.SetValue( this, br.ReadInt16());	break;
				case "System.UInt16":	info.SetValue( this, br.ReadUInt16());	break;
				case "System.Int32":	info.SetValue( this, br.ReadInt32());	break;
				case "System.UInt32":	info.SetValue( this, br.ReadUInt32());	break;
				case "System.Int64":	info.SetValue( this, br.ReadInt64());	break;
				case "System.UInt64":	info.SetValue( this, br.ReadUInt64());	break;
				case "System.Single":	info.SetValue( this, br.ReadSingle());	break;
				case "System.Double":	info.SetValue( this, br.ReadDouble());	break;
				case "eBUFFTYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eRESULTCODE":	info.SetValue( this, br.ReadInt32());	break;
				case "eQUICKSLOTCATEGORY":	info.SetValue( this, br.ReadInt32());	break;
				case "eATTRCHANGECONTENTS":	info.SetValue( this, br.ReadInt32());	break;
				case "eGENDER":	info.SetValue( this, br.ReadInt32());	break;
				case "eRACE":	info.SetValue( this, br.ReadInt32());	break;
				case "eCLASS":	info.SetValue( this, br.ReadInt32());	break;
				case "eCHANGE_INFO_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eNPCSTATUS":	info.SetValue( this, br.ReadInt32());	break;
				case "eITEM_STRENGTHEN_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eCONFUSIONTYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eGUILDJOINTYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eGUILDMEMBER_DELETE":	info.SetValue( this, br.ReadInt32());	break;
				case "eGUILDPERMISSION":	info.SetValue( this, br.ReadInt32());	break;
				case "eGUILD_UI_SCROLL":	info.SetValue( this, br.ReadInt32());	break;
				case "eOSTYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eCOUPON_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eCONDITION_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eEVENT_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "AsEmotionManager+eCHAT_FILTER":	info.SetValue( this, br.ReadInt32());	break;
				case "ePOST_ADDRESS_BOOK_TYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "eEQUIPITEM_GRADE":	info.SetValue( this, br.ReadInt32());	break;
				case "ePRIVATESHOPSEARCHTYPE":	info.SetValue( this, br.ReadInt32());	break;
				case "ePET_SKILL_TYPE": info.SetValue( this, br.ReadInt32());	break;
				case "ePET_HUNGRY_STATE": info.SetValue( this, br.ReadInt32());	break;
				case "UnityEngine.Vector3":
					{
						tmpData = new byte[ sizeof( float) * 3];
						br.Read( tmpData, 0, tmpData.Length);
						Single[] tmpSingle = new Single[3];
						Buffer.BlockCopy( tmpData, 0, tmpSingle, 0, tmpData.Length);
						Vector3 v = new Vector3( tmpSingle[0], tmpSingle[1], tmpSingle[2]);
						info.SetValue( this, v);
					}
					break;
				case "sCLIENTSTATUS":
					sCLIENTSTATUS clientStatus = new sCLIENTSTATUS();
					tmpData = new byte[ sCLIENTSTATUS.size];
					br.Read( tmpData, 0, tmpData.Length);
					clientStatus.ByteArrayToClass( tmpData);
					info.SetValue( this, clientStatus);
					break;
				case "sITEMVIEW":
					sITEMVIEW itemview = new sITEMVIEW();
					tmpData = new byte[ sITEMVIEW.size];
					br.Read( tmpData, 0, tmpData.Length);
					itemview.ByteArrayToClass( tmpData);
					info.SetValue( this, itemview);
					break;
				case "sITEM":
					sITEM sitem = new sITEM();
					tmpData = new byte[ sITEM.size];
					br.Read( tmpData, 0, tmpData.Length);
					sitem.ByteArrayToClass( tmpData);
					info.SetValue( this, sitem);
					break;
				case "sPARTYOPTION":
					sPARTYOPTION sPartyOption = new sPARTYOPTION();
					tmpData = new byte[ sPARTYOPTION.size];
					br.Read( tmpData, 0, tmpData.Length);
					sPartyOption.ByteArrayToClass( tmpData);
					info.SetValue( this, sPartyOption);
					break;
				case "sPARTYDETAILINFO":
					sPARTYDETAILINFO sPartyDetailInfo = new sPARTYDETAILINFO();
					tmpData = new byte[ sPARTYDETAILINFO.size];
					br.Read( tmpData, 0, tmpData.Length);
					sPartyDetailInfo.ByteArrayToClass( tmpData);
					info.SetValue( this, sPartyDetailInfo);
					break;
				case "eCHATTYPE":
					info.SetValue( this, br.ReadInt32());
					break;
				case "sSKILL":
					sSKILL skill = new sSKILL();
					tmpData = new byte[ sSKILL.size];
					br.Read( tmpData, 0, tmpData.Length);
					skill.ByteArrayToClass( tmpData);
					info.SetValue( this, skill);
					break;
				case "sPRODUCT_SLOT":
					sPRODUCT_SLOT sProductSlot = new sPRODUCT_SLOT();
					tmpData = new byte[ sPRODUCT_SLOT.size];
					br.Read( tmpData, 0, tmpData.Length);
					sProductSlot.ByteArrayToClass( tmpData);
					info.SetValue( this, sProductSlot);
					break;
				case "sPRODUCT_INFO":
					sPRODUCT_INFO sProductInfo = new sPRODUCT_INFO();
					tmpData = new byte[ sPRODUCT_INFO.size];
					br.Read( tmpData, 0, tmpData.Length);
					sProductInfo.ByteArrayToClass( tmpData);
					info.SetValue( this, sProductInfo);
					break;
				case "sPETSKILL":
					sPETSKILL sPetSkill = new sPETSKILL();
					tmpData = new byte[ sPETSKILL.size];
					br.Read( tmpData, 0, tmpData.Length);
					sPetSkill.ByteArrayToClass( tmpData);
					info.SetValue( this, sPetSkill);
					break;
				default:
					{
						if( Convert.ToString( info.FieldType.BaseType) == "BaseClass")
						{
							tmpData = new byte[ data.Length - (int)ms.Position];
							Buffer.BlockCopy( data,(int)ms.Position, tmpData, 0, tmpData.Length);
							ms.Position += ((AsBaseClass)info.GetValue( this)).ByteArrayToClass( tmpData);
						}
					}
					break;
				}
			}
			else
			{
				switch( Convert.ToString( info.FieldType))
				{
				case "System.Boolean[]":
					{
						size = ((Boolean[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 4 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Boolean[] tmpBoolean = new Boolean[ size];
						Buffer.BlockCopy( tmpData, 0, tmpBoolean, 0, tmpData.Length);
						info.SetValue( this, tmpBoolean);
					}
					break;
				case "System.Byte[]":
					{
						size = ((Byte[])info.GetValue( this)).GetLength(0);
						info.SetValue( this, br.ReadBytes( size));
					}
					break;
				case "System.Char[]":
					{
						size = ((Char[])info.GetValue( this)).GetLength(0);
						info.SetValue( this, br.ReadChars( size * 2));
					}
					break;
				case "System.Int16[]":
					{
						size = ((Int16[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 2 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Int16[] tmpInt16 = new Int16[ size];
						Buffer.BlockCopy( tmpData, 0, tmpInt16, 0, tmpData.Length);
						info.SetValue( this, tmpInt16);
					}
					break;
				case "System.UInt16[]":
					{
						size = ((UInt16[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 2 * size];
						br.Read( tmpData, 0, tmpData.Length);
						UInt16[] tmpUInt16 = new UInt16[ size];
						Buffer.BlockCopy( tmpData, 0, tmpUInt16, 0, tmpData.Length);
						info.SetValue( this, tmpUInt16);
					}
					break;
				case "System.Int32[]":
					{
						size = ((Int32[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 4 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Int32[] tmpInt32 = new Int32[ size];
						Buffer.BlockCopy( tmpData, 0, tmpInt32, 0, tmpData.Length);
						info.SetValue( this, tmpInt32);
					}
					break;
				case "System.UInt32[]":
					{
						size = ((UInt32[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 4 * size];
						br.Read( tmpData, 0, size);
						UInt32[] tmpUInt32 = new UInt32[ size];
						Buffer.BlockCopy( tmpData, 0, tmpUInt32, 0, tmpData.Length);
						info.SetValue( this, tmpUInt32);
					}
					break;
				case "System.Int64[]":
					{
						size = ((Int64[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 8 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Int64[] tmpInt64 = new Int64[ size];
						Buffer.BlockCopy( tmpData, 0, tmpInt64, 0, tmpData.Length);
						info.SetValue( this, tmpInt64);
					}
					break;
				case "System.Single[]":
					{
						size = ((Single[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 4 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Single[] tmpSingle = new Single[ size];
						Buffer.BlockCopy( tmpData, 0, tmpSingle, 0, tmpData.Length);
						info.SetValue( this, tmpSingle);
					}
					break;
				case "System.Double[]":
					{
						size = ((Double[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ 8 * size];
						br.Read( tmpData, 0, tmpData.Length);
						Double[] tmpDouble = new Double[ size];
						Buffer.BlockCopy( tmpData, 0, tmpDouble, 0, tmpData.Length);
						info.SetValue( this, tmpDouble);
					}
					break;
				case "sPETSKILL[]":
					{
						sPETSKILL[] destAr = new sPETSKILL[(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX];
						size = ((sPETSKILL[])info.GetValue( this)).GetLength(0);
						tmpData = new byte[ sPETSKILL.size * size];
						br.Read( tmpData, 0, tmpData.Length);
					
						int idx = 0;
						for( int i=0; i<(int)ePET_SKILL_TYPE.ePET_SKILL_TYPE_MAX; ++i)
						{
							sPETSKILL sPetSkill = new sPETSKILL();
							byte[] unit = new byte[ sPETSKILL.size];
							Buffer.BlockCopy( tmpData, idx, unit, 0, sPETSKILL.size);
							sPetSkill.ByteArrayToClass( unit);
							destAr[i] = sPetSkill;
						
							idx += sPETSKILL.size;
						}
					
						info.SetValue( this, destAr);
					}
					break;
				default:
					{
						if( Convert.ToString( info.FieldType.BaseType) == "System.Array")
						{
							AsBaseClass[] tmpClass = (AsBaseClass[])info.GetValue( this);
							foreach( AsBaseClass it in tmpClass)
							{
								if( data.Length - (int)ms.Position <= 0)
									break;

								tmpData = new byte[ data.Length - (int)ms.Position];
								Buffer.BlockCopy( data, (int)ms.Position, tmpData, 0, tmpData.Length);
								ms.Position += it.ByteArrayToClass( tmpData);
							}
						}
					}
					break;
				}
			}
		}

		int index = (int)ms.Position;
		br.Close();
		ms.Close();

		return index;
	}
	#endregion

	public byte[] ClassToPacketBytes()
	{
		byte[] data = new byte[ (int)ePackingLength.PL_MAX];
		byte[] tmpData;
		int index = 2;

		Type infotype = this.GetType();

		// 서버와의 문제로 순서 바꿔서 집어 넣음( Protocol, Category -> Category, Protocol)
		FieldInfo headerinfo = infotype.GetField( "Protocol", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);
		data[ index++] = (byte)headerinfo.GetValue( this);

		headerinfo = infotype.GetField( "Category", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField);
		data[ index++] = (byte)headerinfo.GetValue( this);

		tmpData = ClassToByteArray();

		Buffer.BlockCopy( tmpData, 0, data, index, tmpData.Length);
		index += tmpData.Length;

		int dataLength = tmpData.Length;

		tmpData = new byte[2];
		//tmpData = BitConverter.GetBytes( ( ushort)index);
		// 패킷의 길이는 순수하게 데이터의 길이만 집어 넣음
		tmpData = BitConverter.GetBytes( (ushort)dataLength);
		Buffer.BlockCopy( tmpData, 0, data, 0, 2);

		tmpData = new byte[ index];
		Array.Copy( data, tmpData, index);

		return tmpData;
	}
}

public class AsPacketHeader : AsBaseClass
{
	public ushort Size = 0;
	public byte Protocol = 0;
	public byte Category = 0;
	public const BindingFlags BINDING_FLAGS_PIG = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField;

	public int ParsePacketHeader( byte[] data)
	{
		Type infotype = this.GetType();

		int index = 0;

		// Size
		byte[] packetSize = new byte[ sizeof( ushort)];
		Buffer.BlockCopy( data, index, packetSize, 0, sizeof( ushort));
		FieldInfo headerinfo = infotype.GetField( "Size", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, BitConverter.ToUInt16( packetSize, 0));
		index += sizeof( ushort);

		// Protocol
		headerinfo = infotype.GetField( "Protocol", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		// Category
		headerinfo = infotype.GetField( "Category", BINDING_FLAGS_PIG);
		headerinfo.SetValue( this, data[ index++]);

		return index;
	}

	public void PacketBytesToClass( byte[] data)
	{
		int index = ParsePacketHeader( data);

		byte[] packetBody = new byte[ data.Length - index];
		Buffer.BlockCopy( data, index, packetBody, 0, packetBody.Length);
		ByteArrayToClass( packetBody);
	}

	//$yde
	public byte[] ParseValueToByte(byte[] _ar, ref int _idx, int _size)
	{
		byte[] tmpData = new byte[_size];
		Buffer.BlockCopy(_ar, _idx, tmpData, 0, _size);
		_idx += _size;

		return tmpData;
	}
	public T[] ParsePacketToBody<T>(byte[] _ar, ref int _idx, int _cnt) where T : AsPacketHeader, IContainSize, new()
	{
		T[] body = new T[ _cnt];
		for( int i = 0; i < _cnt; i++)
		{
			body[i] = new T();
			byte[] tmpData = new byte[ body[i].Size()];
			Buffer.BlockCopy( _ar, _idx, tmpData, 0, tmpData.Length);
			body[i].ByteArrayToClass( tmpData);
			_idx += body[i].Size();
		}

		return body;
	}
}

#region - misc -
public interface IContainSize
{
//	IContainSize();

	int Size();
}
#endregion
