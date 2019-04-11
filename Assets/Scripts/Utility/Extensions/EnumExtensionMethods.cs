using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace GLEAMoscopeVR.Utility.Extensions
{
    /// <summary>
    /// Extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> value as a string for the enum value <param name="e"></param> passed in.
        /// This method assists in removing the need for string references when working with enumerations.
        /// </summary>
        /// <typeparam name="T">Attribute type to be retrieved - in this case <see cref="DescriptionAttribute"/></typeparam>
        /// <param name="e">Value for which you want to retrieve the <see cref="DescriptionAttribute"/> value.</param>
        /// <returns>
        /// The string representation of the <see cref="DescriptionAttribute"/> associated with the enum value <param name="e"></param>.
        /// If no <see cref="DescriptionAttribute"/> exists, an empty string is returned.</returns>
        /// See: https://www.codementor.io/cerkit/giving-an-enum-a-string-value-using-the-description-attribute-6b4fwdle0
        /// for additional information.
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                var type = e.GetType();
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memberInfo = type.GetMember(type.GetEnumName(val));

                        if (memberInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}