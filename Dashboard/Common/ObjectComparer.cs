using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace Dashboard.Common
{
    public class ObjectComparer
    {
        public static DataChange GetChangedValues(object oldObject, object newObject,
            List<string> propertyAllows)
        {
            if (oldObject == null)
            {
                oldObject = new ExpandoObject();
            }
            if (newObject == null)
            {
                newObject = new ExpandoObject();
            }

            var data = new DataChange();

            foreach (var propName in propertyAllows)
            {
                var sOldValue = GetPropValueFormPropName(oldObject, propName);
                var sNewValue = GetPropValueFormPropName(newObject, propName);

                if (Equals(sOldValue, sNewValue) && propName != "Id") continue;

                data.OldValue[propName] = sOldValue;
                data.NewValue[propName] = sNewValue;
            }

            return data;
        }

        public static object GetPropValueFormPropName(object src, string propName)
        {
            if (src.GetType() != null && src.GetType().GetProperty(propName) != null)
            {
                return src.GetType().GetProperty(propName).GetValue(src, null);
            }
            return "";
        }

        public static void CopyFields<T>(T source, T destination, List<string> propertyAllows)
        {
            var oType = source.GetType();
            foreach (var oProperty in oType.GetProperties())
            {
                if (propertyAllows.Contains(oProperty.Name))
                {
                    oProperty.SetValue(destination, oProperty.GetValue(source));
                }
            }
        }
    }

    public class DataChange
    {
        public DataChange()
        {
            OldValue = new Dictionary<string, object>();
            NewValue = new Dictionary<string, object>();
        }

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public Dictionary<string, object> OldValue { get; set; }
        public string OldValueSerializeObject
        {
            get
            {
                return JsonConvert.SerializeObject(OldValue, jsonSettings);
            }
        }
        public Dictionary<string, object> NewValue { get; set; }
        public string NewValueSerializeObject
        {
            get
            {
                return JsonConvert.SerializeObject(NewValue, jsonSettings);
            }
        }
    }

}