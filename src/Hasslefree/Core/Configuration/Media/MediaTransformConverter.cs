using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hasslefree.Core.Configuration.Media
{
	internal class MediaTransformConverter : TypeConverter
	{
		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(String) || base.CanConvertFrom(context, sourceType);
		}

		public override Boolean IsValid(ITypeDescriptorContext context, Object value)
		{
			return (value is String) || base.IsValid(context, value);
		}

		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is String)) return base.ConvertFrom(context, culture, value);
			
			var s = value.ToString();
			var f = JsonConvert.DeserializeObject<MediaTransforms>(s);
			return f;
		}
	}

	internal class NoTypeConverterJsonConverter<T> : JsonConverter
	{
		private static readonly IContractResolver Resolver = new NoTypeConverterContractResolver();

		private class NoTypeConverterContractResolver : DefaultContractResolver
		{
			protected override JsonContract CreateContract(Type objectType)
			{
				if (!typeof(T).IsAssignableFrom(objectType)) return base.CreateContract(objectType);

				var contract = CreateObjectContract(objectType);
				contract.Converter = null; // Also null out the converter to prevent infinite recursion.
				return contract;
			}
		}

		public override Boolean CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
		{
			return JsonSerializer.CreateDefault(new JsonSerializerSettings { ContractResolver = Resolver }).Deserialize(reader, objectType);
		}

		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			JsonSerializer.CreateDefault(new JsonSerializerSettings { ContractResolver = Resolver }).Serialize(writer, value);
		}
	}
}
