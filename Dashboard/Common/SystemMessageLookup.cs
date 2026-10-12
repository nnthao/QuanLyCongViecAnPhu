using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Common
{
    public class SystemMessageLookup
    {
        public static string GetMessage(string resourceKey)
        {
            switch (resourceKey)
            {
                case "LoiXayRa":
                    return "Đã có lỗi xảy ra vui lòng tải lại trang.";
                case "FieldRequired":
                    return "{0} không được trống.";
                case "FieldInvalid":
                    return "{0} không hợp lệ. {1}";
                case "FieldExist":
                    return "{0} đã tồn tại.";
                case "FieldNotExist":
                    return "{0} không có.";
            }
            return resourceKey;
        }
    }
}