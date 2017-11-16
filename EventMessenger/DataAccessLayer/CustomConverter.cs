using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace EventMessenger
{
	/// <summary>
	/// class for Hexidecimal to Byte and Byte to Hexidecimal conversion
	/// </summary>
	public static class CustomConverter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns></returns>
		public static int GetByteCount(string hexString)
		{
			int numHexChars = 0;
			char c;
			// remove all none A-F, 0-9, characters
			for (int i = 0; i < hexString.Length; i++) {
				c = hexString[i];
				if (IsHexDigit(c))
					numHexChars++;
			}
			// if odd number of characters, discard last character
			if (numHexChars % 2 != 0) {
				numHexChars--;
			}
			return numHexChars / 2; // 2 characters per byte
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexString"></param>
		/// <param name="discarded"></param>
		/// <returns></returns>
		public static byte[] GetBytes(string hexString, out int discarded)
		{
			discarded = 0;
			string newString = "";
			char c;
			// remove all none A-F, 0-9, characters
			for (int i = 0; i < hexString.Length; i++) {
				c = hexString[i];
				if (IsHexDigit(c))
					newString += c;
				else
					discarded++;
			}
			// if odd number of characters, discard last character
			if (newString.Length % 2 != 0) {
				discarded++;
				newString = newString.Substring(0, newString.Length - 1);
			}

			int byteLength = newString.Length / 2;
			byte[] bytes = new byte[byteLength];
			string hex;
			int j = 0;
			for (int i = 0; i < bytes.Length; i++) {
				hex = new String(new Char[] { newString[j], newString[j + 1] });
				bytes[i] = HexToByte(hex);
				j = j + 2;
			}
			return bytes;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string HexToString(byte[] bytes)
		{
			string hexString = "";
			for (int i = 0; i < bytes.Length; i++) {
				hexString += bytes[i].ToString("X2");
			}
			return hexString;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string HexToString(byte bytes)
		{
			string hexString = "";
			{
				hexString += bytes.ToString("X2");
			}
			return hexString;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexString"></param>
		/// <returns>true if 'hexString' is in hexadecimal format, false otherwise</returns>
		public static bool InHexFormat(string hexString)
		{
			bool hexFormat = true;

			foreach (char digit in hexString) {
				if (!IsHexDigit(digit)) {
					hexFormat = false;
					break;
				}
			}
			return hexFormat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c">the char value to check if it is a hex</param>
		/// <returns>true if 'c' is NOT a hex digit, false otherwise</returns>
		public static bool IsHexDigit(Char c)
		{
			int numChar;
			int numA = Convert.ToInt32('A');
			int num1 = Convert.ToInt32('0');
			c = Char.ToUpper(c);
			numChar = Convert.ToInt32(c);
			if (numChar >= numA && numChar < (numA + 6))
				return true;
			if (numChar >= num1 && numChar < (num1 + 10))
				return true;
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string EncodeNonAsciiCharacters( string value ) {
			StringBuilder sb = new StringBuilder();
			foreach( char c in value ) {
				if( c > 127 ) {
					// This character is too big for ASCII
					string encodedValue = "\\u" + ((int) c).ToString( "x4" );
					sb.Append( encodedValue );
				}
				else {
					sb.Append( c );
				}
			}
			return sb.ToString();
		}
		
		
		private static byte HexToByte(string hex)
		{
			if (hex.Length > 2 || hex.Length <= 0)
				return 0;
			byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return newByte;
		}

	}
}
