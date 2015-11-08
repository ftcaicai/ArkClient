using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WmWemeSDK.JSON {

	public static class Extensions {
		public static T Pop<T>(this List<T> list) {
			var result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}
	}

	public enum JSONValueType {
		String,
		Number,
		Object,
		Array,
		Boolean,
		Null
	}
	
	public class JSONValue {

		public JSONValue(JSONValueType type) {
			Type = type;
		}

		public JSONValue(string str) {
			Type = JSONValueType.String;
			Str = UnEscape(str);
			
		}
			
		
		public JSONValue(double number) {
			Type = JSONValueType.Number;
			Number = number;
		}

		public JSONValue(JSONObject obj) {
			if (obj == null) {
				Type = JSONValueType.Null;
			} else {
				Type = JSONValueType.Object;
				Obj = obj;
			}
		}

		public JSONValue(JSONArray array) {
			Type = JSONValueType.Array;
			Array = array;
		}

		public JSONValue(bool boolean) {
			Type = JSONValueType.Boolean;
			Boolean = boolean;
		}

		/// <summary>
		/// Construct a copy of the JSONValue given as a parameter
		/// </summary>
		/// <param name="value"></param>
		public JSONValue(JSONValue value) {
			Type = value.Type;
			switch (Type) {
				case JSONValueType.String:
					Str = value.Str;
					break;

				case JSONValueType.Boolean:
					Boolean = value.Boolean;
					break;

				case JSONValueType.Number:
					Number = value.Number;
					break;

				case JSONValueType.Object:
					if (value.Obj != null) {
						Obj = new JSONObject(value.Obj);
					}
					break;

				case JSONValueType.Array:
					Array = new JSONArray(value.Array);
					break;
			}
		}
		internal static string Escape(string aText)
        {
            string result = "";
            foreach(char c in aText)
            {
                switch(c)
                {
                    case '\\' : result += "\\\\"; break;
                    case '\"' : result += "\\\""; break;
                    case '\n' : result += "\\n" ; break;
                    case '\r' : result += "\\r" ; break;
                    case '\t' : result += "\\t" ; break;
                    case '\b' : result += "\\b" ; break;
                    case '\f' : result += "\\f" ; break;
                    default   : result += c     ; break;
                }
            }
            return result;
        }
		internal static string UnEscape(string aText)
        {
            string result = "";
			int i = 0;
          	while (i < aText.Length)
            {
				string Token = "";
                switch (aText[i])
                {
				case '\\':
                        ++i;
                        char C = aText[i];
                        switch (C)
                            {
                                case 't' : Token += '\t'; break;
                                case 'r' : Token += '\r'; break;
                                case 'n' : Token += '\n'; break;
                                case 'b' : Token += '\b'; break;
                                case 'f' : Token += '\f'; break;
                                case 'u':
                                {
                                    string s = aText.Substring(i+1,4);
                                    Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                    i += 4;
                                    break;
                                }
                                default  : Token += C; break;
                        }
					result += Token;
                    break;

                    default:
                        result += aText[i];
                    break;
				}
			 i++;
			}
            return result;
        }
		public JSONValueType Type { get; private set; }
		public string Str { get; set; }
		public double Number { get; set; }
		public JSONObject Obj { get; set; }
		public JSONArray Array { get; set; }
		public bool Boolean { get; set; }
		public JSONValue Parent { get; set; }

		public static implicit operator JSONValue(string str) {
			return new JSONValue(str);
		}

		public static implicit operator JSONValue(double number) {
			return new JSONValue(number);
		}

		public static implicit operator JSONValue(JSONObject obj) {
			return new JSONValue(obj);
		}

		public static implicit operator JSONValue(JSONArray array) {
			return new JSONValue(array);
		}

		public static implicit operator JSONValue(bool boolean) {
			return new JSONValue(boolean);
		}

		/// <returns>String representation of this JSONValue</returns>
		public override string ToString() {
			switch (Type) {
				case JSONValueType.Object:
					return Obj.ToString();

				case JSONValueType.Array:
					return Array.ToString();

				case JSONValueType.Boolean:
					return Boolean ? "true" : "false";

				case JSONValueType.Number:
					return Number.ToString();

				case JSONValueType.String:
					return "\"" + Escape(Str) + "\"";

				case JSONValueType.Null:
					return "null";
			}
			return "null";
		}

	}

	public class JSONArray : IEnumerable<JSONValue> {

		private readonly List<JSONValue> values = new List<JSONValue>();

		public JSONArray() {
		}

		/// <summary>
		/// Construct a new array and copy each value from the given array into the new one
		/// </summary>
		/// <param name="array"></param>
		public JSONArray(JSONArray array) {
			values = new List<JSONValue>();
			foreach (var v in array.values) {
				values.Add(new JSONValue(v));
			}
		}

		/// <summary>
		/// Add a JSONValue to this array
		/// </summary>
		/// <param name="value"></param>
		public void Add(JSONValue value) {
			values.Add(value);
		}

		public JSONValue this[int index] {
			get { return values[index]; }
			set { values[index] = value; }
		}

		/// <returns>
		/// Return the length of the array
		/// </returns>
		public int Length {
			get { return values.Count; }
		}

		/// <returns>String representation of this JSONArray</returns>
		public override string ToString() {
			var stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (var value in values) {
				stringBuilder.Append(value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0) {
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public IEnumerator<JSONValue> GetEnumerator() {
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

		/// <summary>
		/// Attempt to parse a string as a JSON array.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONArray object if successful, null otherwise.</returns>
		public static JSONArray Parse(string jsonString) {
			var tempObject = JSONObject.Parse("{ \"array\" :" + jsonString + '}');
			return tempObject == null ? null : tempObject.GetValue("array").Array;
		}

		/// <summary>
		/// Empty the array of all values.
		/// </summary>
		public void Clear() {
			values.Clear();
		}

		/// <summary>
		/// Remove the value at the given index, if it exists.
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index) {
			if (index >= 0 && index < values.Count) {
				values.RemoveAt(index);
			} else {
				Debug.LogWarning("index out of range: " + index + " (Expected 0 <= index < " + values.Count + ")");
			}
		}

		/// <summary>
		/// Concatenate two JSONArrays
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns>A new JSONArray that is the result of adding all of the right-hand side array's values to the left-hand side array.</returns>
		public static JSONArray operator +(JSONArray lhs, JSONArray rhs) {
			var result = new JSONArray(lhs);
			foreach (var value in rhs.values) {
				result.Add(value);
			}
			return result;
		}

	}

	public class JSONObject : IEnumerable<KeyValuePair<string, JSONValue>> {

		private enum JSONParsingState {
			Object,
			Array,
			EndObject,
			EndArray,
			Key,
			Value,
			KeyValueSeparator,
			ValueSeparator,
			String,
			Number,
			Boolean,
			Null
		}

		private readonly IDictionary<string, JSONValue> values = new Dictionary<string, JSONValue>();

		public JSONObject() {
		}

		/// <summary>
		/// Construct a copy of the given JSONObject.
		/// </summary>
		/// <param name="other"></param>
		public JSONObject(JSONObject other) {
			values = new Dictionary<string, JSONValue>();

			if (other != null) {
				foreach (var keyValuePair in other.values) {
					values[keyValuePair.Key] = new JSONValue(keyValuePair.Value);
				}
			}
		}
		internal static string Escape(string aText)
        {
            string result = "";
            
            foreach(char c in aText)
            {
                switch(c)
                {
                    case '\\' : result += "\\\\"; break;
                    case '\"' : result += "\\\""; break;
                    case '\n' : result += "\\n" ; break;
                    case '\r' : result += "\\r" ; break;
                    case '\t' : result += "\\t" ; break;
                    case '\b' : result += "\\b" ; break;
                    case '\f' : result += "\\f" ; break;
                    default   : result += c     ; break;
                }
            }
            return result;
        }
		
		/// <param name="key"></param>
		/// <returns>Does 'key' exist in this object.</returns>
		public bool ContainsKey(string key) {
			return values.ContainsKey(key);
		}

		public JSONValue GetValue(string key) {
			JSONValue value;
			values.TryGetValue(key, out value);
			return value;
		}

		public string GetString(string key) {
			var value = GetValue(key);
			if (value == null) {
				Debug.LogWarning(key + "(string) == null");
				return string.Empty;
			}
			
			return value.Str;
		}

		public double GetNumber(string key) {
			var value = GetValue(key);
			if (value == null) {
				Debug.LogWarning(key + " == null");
				return double.NaN;
			}
			return value.Number;
		}

		public JSONObject GetObject(string key) {
			var value = GetValue(key);
			if (value == null) {
				Debug.LogWarning(key + " == null");
				return null;
			}
			return value.Obj;
		}

		public bool GetBoolean(string key) {
			var value = GetValue(key);
			if (value == null) {
				Debug.LogWarning(key + " == null");
				return false;
			}
			return value.Boolean;
		}

		public JSONArray GetArray(string key) {
			var value = GetValue(key);
			if (value == null) {
				Debug.LogWarning(key + " == null");
				return null;
			}
			return value.Array;
		}

		public JSONValue this[string key] {
			get { return GetValue(key); }
			set { values[key] = value; }
		}

		public void Add(string key, JSONValue value) {
			values[key] = value;
		}

		public void Add(KeyValuePair<string, JSONValue> pair) {
			values[pair.Key] = pair.Value;
		}
		
		
		/// <summary>
		/// Attempt to parse a string into a JSONObject.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONObject or null if parsing fails.</returns>
		public static JSONObject Parse(string jsonString) {
			if (string.IsNullOrEmpty(jsonString)) {
				return null;
			}			
			JSONValue currentValue = null;

			var keyList = new List<string>();

			var state = JSONParsingState.Object;

			for (var startPosition = 0; startPosition < jsonString.Length; ++startPosition) {
				startPosition = SkipWhitespace(jsonString, startPosition);

				switch (state) {
					case JSONParsingState.Object:
						if (jsonString[startPosition] != '{') {
							return Fail('{', startPosition);
						}

						JSONValue newObj = new JSONObject();
						if (currentValue != null) {
							newObj.Parent = currentValue;
						}
						currentValue = newObj;

						state = JSONParsingState.Key;
						break;

					case JSONParsingState.EndObject:
						if (jsonString[startPosition] != '}') {
							return Fail('}', startPosition);
						}

						if (currentValue.Parent == null) {
							return currentValue.Obj;
						}

						switch (currentValue.Parent.Type) {

							case JSONValueType.Object:
								currentValue.Parent.Obj.values[keyList.Pop()] = new JSONValue(currentValue.Obj);
								break;

							case JSONValueType.Array:
								currentValue.Parent.Array.Add(new JSONValue(currentValue.Obj));
								break;

							default:
								return Fail("valid object", startPosition);

						}
						currentValue = currentValue.Parent;

						state = JSONParsingState.ValueSeparator;
						break;

					case JSONParsingState.Key:
						if (jsonString[startPosition] == '}') {
							--startPosition;
							state = JSONParsingState.EndObject;
							break;
						}

						var key = ParseString(jsonString, ref startPosition);
						if (key == null) {
							return Fail("key string", startPosition);
						}
						keyList.Add(key);
						state = JSONParsingState.KeyValueSeparator;
						break;

					case JSONParsingState.KeyValueSeparator:
						if (jsonString[startPosition] != ':') {
							return Fail(':', startPosition);
						}
						state = JSONParsingState.Value;
						break;

					case JSONParsingState.ValueSeparator:
						switch (jsonString[startPosition]) {

							case ',':
								state = currentValue.Type == JSONValueType.Object ? JSONParsingState.Key : JSONParsingState.Value;
								break;

							case '}':
								state = JSONParsingState.EndObject;
								--startPosition;
								break;

							case ']':
								state = JSONParsingState.EndArray;
								--startPosition;
								break;
						case '\\':
									Debug.Log("sadiokasjdlkahsjdkjasd");
									break;
							default:
								return Fail(", } ]", startPosition);
						}
						break;

					case JSONParsingState.Value: {
						var c = jsonString[startPosition];
						if (c == '"') {							
							state = JSONParsingState.String;
						} else if (char.IsDigit(c) || c == '-') {
							state = JSONParsingState.Number;
						} else
							switch (c) {

								case '{':
									state = JSONParsingState.Object;
									break;

								case '[':
									state = JSONParsingState.Array;
									break;

								case ']':
									if (currentValue.Type == JSONValueType.Array) {
										state = JSONParsingState.EndArray;
									} else {
										return Fail("valid array", startPosition);
									}
									break;

								case 'f':
								case 't':
									state = JSONParsingState.Boolean;
									break;
								case 'n':
									state = JSONParsingState.Null;
									break;
								default:
									return Fail("beginning of value", startPosition);
							}

						--startPosition; //To re-evaluate this char in the newly selected state
						break;
					}

					case JSONParsingState.String:
						var str = ParseString(jsonString, ref startPosition);
						
						if (str == null) {
							return Fail("string value", startPosition);
						}

						switch (currentValue.Type) {

							case JSONValueType.Object:
								currentValue.Obj.values[keyList.Pop()] = new JSONValue(str);
								break;

							case JSONValueType.Array:
								currentValue.Array.Add(str);
								break;

							default:
								Debug.LogWarning("Fatal error, current JSON value not valid");
								return null;
						}

						state = JSONParsingState.ValueSeparator;
						break;

					case JSONParsingState.Number:
						var number = ParseNumber(jsonString, ref startPosition);
						if (double.IsNaN(number)) {
							return Fail("valid number", startPosition);
						}

						switch (currentValue.Type) {

							case JSONValueType.Object:
								currentValue.Obj.values[keyList.Pop()] = new JSONValue(number);
								break;

							case JSONValueType.Array:
								currentValue.Array.Add(number);
								break;

							default:
								Debug.LogWarning("Fatal error, current JSON value not valid");
								return null;
						}

						state = JSONParsingState.ValueSeparator;

						break;

					case JSONParsingState.Boolean:
						if (jsonString[startPosition] == 't') {
							if (jsonString.Length < startPosition + 4 ||
							    jsonString[startPosition + 1] != 'r' ||
							    jsonString[startPosition + 2] != 'u' ||
							    jsonString[startPosition + 3] != 'e') {
								return Fail("true", startPosition);
							}

							switch (currentValue.Type) {

								case JSONValueType.Object:
									currentValue.Obj.values[keyList.Pop()] = new JSONValue(true);
									break;

								case JSONValueType.Array:
									currentValue.Array.Add(new JSONValue(true));
									break;

								default:
									Debug.LogWarning("Fatal error, current JSON value not valid");
									return null;
							}

							startPosition += 3;
						} else {
							if (jsonString.Length < startPosition + 5 ||
							    jsonString[startPosition + 1] != 'a' ||
							    jsonString[startPosition + 2] != 'l' ||
							    jsonString[startPosition + 3] != 's' ||
							    jsonString[startPosition + 4] != 'e') {
								return Fail("false", startPosition);
							}

							switch (currentValue.Type) {

								case JSONValueType.Object:
									currentValue.Obj.values[keyList.Pop()] = new JSONValue(false);
									break;

								case JSONValueType.Array:
									currentValue.Array.Add(new JSONValue(false));
									break;

								default:
									Debug.LogWarning("Fatal error, current JSON value not valid");
									return null;
							}

							startPosition += 4;
						}

						state = JSONParsingState.ValueSeparator;
						break;

					case JSONParsingState.Array:
						if (jsonString[startPosition] != '[') {
							return Fail('[', startPosition);
						}

						JSONValue newArray = new JSONArray();
						if (currentValue != null) {
							newArray.Parent = currentValue;
						}
						currentValue = newArray;

						state = JSONParsingState.Value;
						break;

					case JSONParsingState.EndArray:
						if (jsonString[startPosition] != ']') {
							return Fail(']', startPosition);
						}

						if (currentValue.Parent == null) {
							return currentValue.Obj;
						}

						switch (currentValue.Parent.Type) {

							case JSONValueType.Object:
								currentValue.Parent.Obj.values[keyList.Pop()] = new JSONValue(currentValue.Array);
								break;

							case JSONValueType.Array:
								currentValue.Parent.Array.Add(new JSONValue(currentValue.Array));
								break;

							default:
								return Fail("valid object", startPosition);
						}
						currentValue = currentValue.Parent;

						state = JSONParsingState.ValueSeparator;
						break;

					case JSONParsingState.Null:
						if (jsonString[startPosition] == 'n') {
							if (jsonString.Length < startPosition + 4 ||
							    jsonString[startPosition + 1] != 'u' ||
							    jsonString[startPosition + 2] != 'l' ||
							    jsonString[startPosition + 3] != 'l') {
								return Fail("null", startPosition);
							}

							switch (currentValue.Type) {

								case JSONValueType.Object:
									currentValue.Obj.values[keyList.Pop()] = new JSONValue(JSONValueType.Null);
									break;

								case JSONValueType.Array:
									currentValue.Array.Add(new JSONValue(JSONValueType.Null));
									break;

								default:
									Debug.LogWarning("Fatal error, current JSON value not valid");
									return null;
							}

							startPosition += 3;
						}
						state = JSONParsingState.ValueSeparator;
						break;

				}
			}
			Debug.LogWarning("Unexpected end of string");
			return null;
		}

		private static int SkipWhitespace(string str, int pos) {
			for (; pos < str.Length && char.IsWhiteSpace(str[pos]); ++pos) ;
			return pos;
		}

		private static string ParseString(string str, ref int startPosition) {
			if (str[startPosition] != '"' || startPosition + 1 >= str.Length) {
				Fail('"', startPosition);
				return null;
			}

			var endPosition = str.IndexOf('"', startPosition + 1);
			if (endPosition <= startPosition) {
				Fail('"', startPosition + 1);
				return null;
			}

			while (str[endPosition - 1] == '\\') {
				endPosition = str.IndexOf('"', endPosition + 1);
				if (endPosition <= startPosition) {
					Fail('"', startPosition + 1);
					return null;
				}
			}

			var result = string.Empty;

			if (endPosition > startPosition + 1) {
				result = str.Substring(startPosition + 1, endPosition - startPosition - 1);
			}

			startPosition = endPosition;
			//result = result.Replace("\\/","/");
			return result;
		}

		private static double ParseNumber(string str, ref int startPosition) {
			if (startPosition >= str.Length || (!char.IsDigit(str[startPosition]) && str[startPosition] != '-')) {
				return double.NaN;
			}

			var endPosition = startPosition + 1;

			for (;
				endPosition < str.Length && str[endPosition] != ',' && str[endPosition] != ']' && str[endPosition] != '}';
				++endPosition) ;

			double result;
			if (
				!double.TryParse(str.Substring(startPosition, endPosition - startPosition), System.Globalization.NumberStyles.Float,
				                 System.Globalization.CultureInfo.InvariantCulture, out result)) {
				return double.NaN;
			}
			startPosition = endPosition - 1;
			return result;
		}

		private static JSONObject Fail(char expected, int position) {
			return Fail(new string(expected, 1), position);
		}

		private static JSONObject Fail(string expected, int position) {
			Debug.LogWarning("Invalid json string, expecting " + expected + " at " + position);
			return null;
		}

		/// <returns>String representation of this JSONObject</returns>
		public override string ToString() {
			var stringBuilder = new StringBuilder();
			stringBuilder.Append('{');

			foreach (var pair in values) {
				stringBuilder.Append("\"" + Escape(pair.Key) + "\"");
				stringBuilder.Append(':');
				stringBuilder.Append(pair.Value.ToString());
				stringBuilder.Append(',');
				
			}
			if (values.Count > 0) {
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public IEnumerator<KeyValuePair<string, JSONValue>> GetEnumerator() {
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

		/// <summary>
		/// Empty this JSONObject of all values.
		/// </summary>
		public void Clear() {
			values.Clear();
		}

		/// <summary>
		/// Remove the JSONValue attached to the given key.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key) {
			if (values.ContainsKey(key)) {
				values.Remove(key);
			}
		}
		public static string MyDictionaryToJson(Dictionary<string, string> dict)
		{
			List<string> items = new List<string>();
			foreach (KeyValuePair<string, string> item in dict) {
 	 			items.Add(string.Format("\"{0}\":\"{1}\"",item.Key,item.Value));
			}
			string[] entries = items.ToArray();
    		return "{" + string.Join(",", entries) + "}";
		}
	}
}

/*
 * //
		string jsonString = "{}";
		JSONObject json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 1 : " + json.ToString() );
		//
		jsonString = "{ \"v\":\"1\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 2 : " + json.ToString()+ " : " + json.GetString("v"));
		
		
		jsonString = "{	\"v\":\"1\\r\\n\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 3 : " + json.ToString()+ " : " + json.GetString("v"));
		
		jsonString = "{ \"v\":1}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 4 : " + json.ToString()+ " : " + json.GetNumber("v"));
		
		jsonString = "{ \"v\":\"ab'c\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 5 : " + json.ToString()+ " : " + json.GetString("v"));
		
		jsonString = "{ \"v\":3.141E-10}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 6 : " + json.ToString()+ " : " + json.GetNumber("v"));
		
		jsonString = "{ \"v\":3.141e-10}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 7 : " + json.ToString()+ " : " + json.GetNumber("v"));
		
		jsonString = "{ \"v\":12345123456789}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 8 : " + json.ToString()+ " : " + json.GetNumber("v"));
		
		jsonString = "{ \"v\":123456789123456789123456789}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 9 : " + json.ToString());
		
		jsonString = "{ \"v\":[ 1,2,3,4]}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 10 : " + json.ToString());
		
		jsonString = "{ \"v\":[ \"1\",\"2\",\"3\",\"4\"]}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 11 : " + json.ToString());
		
		jsonString = "{ \"v\":[ { }, { },[]]}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 12 : " + json.ToString());
		
		jsonString = "{ \"v\":\"\u2000\u20ff\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 13 : " + json.ToString()+ " : " + json.GetString("v"));
		

		jsonString = "{ \"v\":\"\u2000\u20FF\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 14 : " + json.ToString());

		jsonString = "{ \"v\":\"hp://foo\"}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 15 : " + json.ToString());

		jsonString = "{ \"v\":null}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 16 : " + json.ToString());


				jsonString = "{ \"v\":true}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 17 : " + json.ToString());

				jsonString = "{ \"v\": true }";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 18 : " + json.ToString());
		
		jsonString = "{ \"v\":1.7976931348623157E308}";
		json = JSONObject.Parse(jsonString);
		Debug.Log("json tc 19 : " + json.ToString());
*/