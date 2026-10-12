using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Dashboard.Common
{
    public class EnumUltiHelper
    {
        public static List<LookupViewModel> EnumToLookupList<TEnum>() where TEnum : Enum
        {
            var item = Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new LookupViewModel
                       {
                           id = Convert.ToInt32(e),
                           text = GetEnumDescription(e)
                       }).OrderBy(o => o.text).ToList();
            return item;
        }

        private static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}