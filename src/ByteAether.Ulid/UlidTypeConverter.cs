using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ByteAether.Ulid;

/// <summary>
/// A type converter for the <see cref="Ulid"/> type.
/// </summary>
public class UlidTypeConverter : TypeConverter
{
	private static readonly Type[] _convertibleTypes = [typeof(string), typeof(byte[]), typeof(Guid)];

	/// <summary>
	/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
	/// </summary>
	/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
	/// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
	/// <returns><see langword="true"/> if this converter can perform the conversion; otherwise, <see langword="false"/>.</returns>
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
		=> _convertibleTypes.Contains(sourceType)
			|| base.CanConvertFrom(context, sourceType);

	/// <summary>
	/// Converts the given object to the type of this converter, using the specified context and culture information.
	/// </summary>
	/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
	/// <param name="culture">A <see cref="CultureInfo"/>. If <see langword="null"/> is passed, the current culture is assumed.</param>
	/// <param name="value">The <see cref="object"/> to convert.</param>
	/// <returns>An <see cref="object"/> that represents the converted value.</returns>
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
		=> value switch
		{
			string s => Ulid.Parse(s),
			byte[] b => Ulid.New(b),
			Guid guid => Ulid.New(guid),
			_ => base.ConvertFrom(context, culture, value),
		};

	/// <summary>
	/// Returns whether this converter can convert the object to the specified type, using the specified context.
	/// </summary>
	/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
	/// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
	/// <returns><see langword="true"/> if this converter can perform the conversion; otherwise, <see langword="false"/>.</returns>
	public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
		=> _convertibleTypes.Contains(destinationType)
			|| base.CanConvertTo(context, destinationType);

	/// <summary>
	/// Converts the given value object to the specified type, using the specified context and culture information.
	/// </summary>
	/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
	/// <param name="culture">A <see cref="CultureInfo"/>. If <see langword="null"/> is passed, the current culture is assumed.</param>
	/// <param name="value">The <see cref="object"/> to convert.</param>
	/// <param name="destinationType">The <see cref="Type"/> to convert the value to.</param>
	/// <returns>An <see cref="object"/> that represents the converted value.</returns>
	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
		=> value is Ulid ulid
			? destinationType == typeof(string) ? ulid.ToString()
			: destinationType == typeof(byte[]) ? ulid.ToByteArray()
			: destinationType == typeof(Guid) ? ulid.ToGuid()
			: base.ConvertTo(context, culture, value, destinationType)
		: base.ConvertTo(context, culture, value, destinationType);
}
