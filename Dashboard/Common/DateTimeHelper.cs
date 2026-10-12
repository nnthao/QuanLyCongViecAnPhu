using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dashboard.Common
{
    public static class DateTimeHelper
    {
        public static DateTime BeginningOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        public static string BeginningOfDayForSqlQuery(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd 00:00:00");
        }

        public static DateTime EndOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }

        public static string EndOfDayForSqlQuery(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd 23:59:59");
        }

        public static string DateTimeFormatViOnlyDate(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("dd/MM/yyyy");

            }
            return "";
        }

        public static string DateTimeFormatVi(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm");

            }
            return "";
        }

        public static string TimeFormatVi(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");

            }
            return "";
        }
        public static string TimeStampCurrent()
        {
            return DateTime.Now.ToFileTime().ToString();
        }
        public static string DateEmptyExport()
        {
            return $"{new string('.', 5)}/{new string('.', 5)}/{new string('.', 5)}";
        }

        public static DateTime? ConvertStrToDate(string dt)
        {
            if (string.IsNullOrEmpty(dt))
            {
                return null;
            }

            try
            {

                return DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static DateTime? ConvertStrToDateTime(string dt)
        {
            if (string.IsNullOrEmpty(dt))
            {
                return null;
            }

            try
            {

                return DateTime.ParseExact(dt, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static string ConvertDateTimeToDateTime(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm");
            }

            return null;
        }

        public static string ConvertDateTimeToDateOnly(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("dd/MM/yyyy");
            }

            return null;
        }

        public static string ConvertDateTimeToTimeOnly(DateTime? dt)
        {
            if (dt != null && dt.GetValueOrDefault().Year > 1900)
            {
                return dt.GetValueOrDefault().ToString("HH:mm");
            }

            return null;
        }

        public static string GetThuTiengViet(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: return "Thứ 2";
                case DayOfWeek.Tuesday: return "Thứ 3";
                case DayOfWeek.Wednesday: return "Thứ 4";
                case DayOfWeek.Thursday: return "Thứ 5";
                case DayOfWeek.Friday: return "Thứ 6";
                case DayOfWeek.Saturday: return "Thứ 7";
                case DayOfWeek.Sunday: return "Chủ nhật";
                default: return "";
            }
        }

        public static List<DateTime> LayTuanTuNgay(DateTime dt)
        {
            // Tìm Thứ 2 gần nhất (đầu tuần)
            int daysToMonday = ((int)dt.DayOfWeek + 6) % 7;  // vì DayOfWeek: Sunday = 0, Monday = 1, ..., Saturday = 6
            DateTime monday = dt.AddDays(-daysToMonday);

            var ketQua = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                ketQua.Add(monday.AddDays(i));
            }

            return ketQua;
        }

        public static int TuanTrongNam(DateTime dt)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            Calendar cal = cul.Calendar;

            // Xác định quy tắc tính tuần và ngày bắt đầu tuần
            CalendarWeekRule rule = cul.DateTimeFormat.CalendarWeekRule; // FirstDay, FirstFullWeek, FirstFourDayWeek
            DayOfWeek firstDay = cul.DateTimeFormat.FirstDayOfWeek;       // Thường là Monday ở VN

            int weekNumber = cal.GetWeekOfYear(dt, rule, firstDay);

            return weekNumber;
        }

        public static string GetThoiGianTuongDoi(DateTime thoiGian)
        {
            var now = DateTime.Now;
            var diff = now - thoiGian;

            if (diff.TotalSeconds < 60)
                return $"{Math.Floor(diff.TotalSeconds)} giây trước";

            if (diff.TotalMinutes < 60)
                return $"{Math.Floor(diff.TotalMinutes)} phút trước";

            if (diff.TotalHours < 24)
                return $"{Math.Floor(diff.TotalHours)} giờ trước";

            if (diff.TotalDays < 2)
                return "Hôm qua";

            if (diff.TotalDays < 7)
                return $"{Math.Floor(diff.TotalDays)} ngày trước";

            // Nếu lớn hơn 7 ngày thì hiện ngày cụ thể
            return thoiGian.ToString("dd/MM/yyyy");
        }

        public static DateTime ConvertStrToBeginOfDay(string dt)
        {
            if (!string.IsNullOrEmpty(dt))
            {
                var thoiGain = ConvertStrToDate(dt);
                if (thoiGain != null)
                {
                    return thoiGain.GetValueOrDefault().BeginningOfDay();
                }
            }


            return DateTime.Now.AddDays(-7);
        }

        public static DateTime NgayDauThang(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
        }

        public static DateTime NgayCuoiThang(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
        }
    }
}