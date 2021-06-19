using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Get the bytes from a stream (max 2GB)
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static Byte[] GetBytes(this Stream stream)
		{
			Int32 length = (Int32)stream.Length;
			Byte[] bytes = new Byte[length];
			stream.Read(bytes, 0, length);
			return bytes;
		}

		/// <summary>
		/// Deserialize XML from a Stream
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this"></param>
		/// <returns></returns>
		public static T ParseXML<T>(this Stream @this) where T : class
		{
			var reader = XmlReader.Create(@this, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
			return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
		}

		public static StreamReader ToStream(this Byte[] bytes)
		{
			return new StreamReader(new MemoryStream(bytes));
		}
	}
}
