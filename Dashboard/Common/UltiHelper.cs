using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Globalization;

namespace Dashboard.Common
{
    public static class UltiHelper
    {
        public static int TinhTuoiDang(DateTime? ngayVaoDang, DateTime? ngayHienTai = null)
        {
            if (ngayVaoDang == null) return 0;
            if (ngayHienTai == null)
            {
                ngayHienTai = DateTime.Today;
            }

            var a = (ngayHienTai.GetValueOrDefault().Year * 100 + ngayHienTai.GetValueOrDefault().Month) * 100 + ngayHienTai.GetValueOrDefault().Day;
            var b = (ngayVaoDang.GetValueOrDefault().Year * 100 + ngayVaoDang.GetValueOrDefault().Month) * 100 + ngayVaoDang.GetValueOrDefault().Day;

            var c = (a - b) / 10000;
            return c;
        }

        public static string TrimText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Trim();
            }

            return "";
        }

        public static string RemoveVietnameseAccent(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string CreateTenTrim(string hoTen)
        {
            if (string.IsNullOrEmpty(hoTen))
            {
                return string.Empty;
            }

            var tenTrim = RemoveVietnameseAccent(hoTen);
            tenTrim = Regex.Replace(tenTrim, @"\s+", "");
            return tenTrim.ToLower();
        }

        public static string GenerateMa(int index)
        {
            var str = $"000{index}";
            return str.Substring(str.Length - 4, 4);
        }

        public static bool IsInteger(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            try
            {
                var number = Convert.ToInt32(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetFileNameAndId(string fileName)
        {
            var fileInfo = new Dictionary<string, string>();
            var fileNameSplit = fileName.Split('|');
            if (fileNameSplit.Length != 2) return fileInfo;

            fileInfo["FileName"] = fileNameSplit[0];
            fileInfo["Id"] = fileNameSplit[1];

            return fileInfo;
        }

        public static string ConvertToRoman(int convertThis)
        {
            string romanNumeral = "";
            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"M", 1000},// 1000 = M
                {"CM", 900},// 900 = CM
                {"D", 500}, // 500 = D
                {"CD", 400},// 400 = CD
                {"C", 100}, // 100 = C
                {"XC", 90}, // 90 = XC
                {"L", 50},  // 50 = L
                {"XL", 40}, // 40 = XL
                {"X", 10},  // 10 = X
                {"IX", 9},  // 9 = IX
                {"V", 5},   // 5 = V
                {"IV", 4},  // 4 = IV
                {"I", 1},   // 1 = I
            };
            foreach (KeyValuePair<string, int> pair in dict)
            {
                if (convertThis >= pair.Value)
                {
                    var leftovers = convertThis % pair.Value;
                    int remainder = (convertThis - leftovers) / pair.Value;
                    convertThis = leftovers;
                    while (remainder > 0)
                    {
                        romanNumeral += pair.Key; remainder--;
                    }
                }
            }
            return romanNumeral;
        }

        public static string SaveFile(this HttpPostedFileBase file)
        {
            var fileName = string.Empty;

            try
            {
                if (file == null)
                {
                    throw new ArgumentException("File không được để trống.");
                }

                FileHelper.ValidateFileExtension(file);

                fileName = Path.GetFileNameWithoutExtension(file.FileName);

                //To Get File Extension  
                string FileExtension = Path.GetExtension(file.FileName);

                //Add Current Date To Attached File Name  
                fileName = DateTime.Now.ToString("yyyyMMdd") + "-" + fileName.Trim() + FileExtension;

                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = ConfigurationManager.AppSettings["FilePathVBQPPL"].ToString();

                //To copy and save file into server.  
                file.SaveAs(UploadPath + fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu file: {ex.Message}", ex);
            }

            return fileName;
        }

        public static string RemoveNumberInString(string str)
        {
            return Regex.Replace(str, "[0-9]", "");
        }

        public static int? ConvertStringToInteger(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static decimal? ConvertStringToDecimal(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return Convert.ToDecimal(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool? ConvertStringToBool(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                if (str.ToLower().Trim() == "có")
                {
                    return true;
                }
                else if (str.ToLower().Trim() == "không")
                {
                    return false;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GeneratePreviewFileUrl(string path)
        {
            var urlFile = ConfigurationManager.AppSettings["UrlRoot"] + "/"
                    + path;

            return urlFile.Replace(@"//FileTemp\", @"/FileTemp/");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string CheckIntEmpty(int? num)
        {
            if (num.GetValueOrDefault() > 0)
            {
                return num.GetValueOrDefault().ToString();
            }

            return "";
        }

        public static string MappingValueReturnRadio(this string valueCurrent, string value1Return, string value2Return)
        {
            return valueCurrent == "true" ? value1Return : value2Return;
        }

        public static List<FileModel> GetFiles(this List<HttpPostedFileBase> fileDinhKems)
        {
            var results = new List<FileModel>();

            if (fileDinhKems != null)
            {
                foreach (var item in fileDinhKems)
                {
                    var target = new FileModel();
                    target.FileName = item.FileName;
                    target.File = item;
                    results.Add(target);
                }
            }

            return results;
        }


        public static ValidateError KiemTraDinhKemFile(this List<FileModel> fileModels, string id)
        {
            var error = new ValidateError();

            if (!fileModels.Any())
            {
                error.Id = id;
                error.Description = "Vui lòng chọn file đính kèm.";

                return error;
            }

            return null;
        }
        public static JsonResult SendMessageTelegram(this string message, string chatId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
            string apiToken = WebConfigurationManager.AppSettings["TOKEN_BOT_TELEGRAM"];

            urlString = String.Format(urlString, apiToken, chatId, message);
            WebRequest request = WebRequest.Create(urlString);
            Stream rs = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(rs);
            string line = "";
            StringBuilder sb = new StringBuilder();

            while (line != null)
            {
                line = reader.ReadLine();
                if (line != null)
                    sb.Append(line);
            }

            string response = sb.ToString();
            // Do what you want with response

            return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static string ChuanHoaSoDienThoai(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt))
                return string.Empty;

            var phone = sdt.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");
            if (phone.StartsWith("+84"))
            {
                phone = "0" + phone.Substring(3);
            }
            else if (phone.StartsWith("84") && phone.Length == 11)
            {
                phone = "0" + phone.Substring(2);
            }

            return phone;
        }

        public static bool PhoneValidVietNam(string sdt)
        {
            var phone = ChuanHoaSoDienThoai(sdt);
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var listDauSo = ConfigurationManager.AppSettings["DauSoDiDong"];
            string pattern = $"^({listDauSo})\\d{{8}}$";

            return Regex.IsMatch(phone, pattern);
        }

        public static string GetQuyText(int quy)
        {
            switch (quy)
            {
                case 1: return "I";
                case 2: return "II";
                case 3: return "III";
                case 4: return "IV";
                default:
                    return "";
            }
        }

        public static T DeepCopy<T>(T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static string GetMoTaQuyNam(int quy, int nam)
        {
            if (quy > 0 && nam > 0)
            {
                return $"Quý {UltiHelper.GetQuyText(quy)}/ {nam}";
            }
            return "";
        }

        public static string DinhDangSelect2ListJs(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return "";
            }
            var item = ids.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim());
            return string.Join(",", item);
        }

        public static List<int> ConvertIdStringToListInt(string ids)
        {
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    return ids.Split(',').Where(o => !string.IsNullOrEmpty(o))
                        .Select(o => Convert.ToInt32(o.Trim())).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new List<int>();
        }

        public static MenuGenerateViewModel TaoMenuQuyenMoRongTuBienDungChung<T>(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return new MenuGenerateViewModel();
            }

            var field = typeof(T).GetField(fieldName);
            if (field == null) return null;

            var key = field?.GetValue(null)?.ToString();

            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return new MenuGenerateViewModel
            {
                Key = key,
                Ten = attr.Description
            };
        }

        public static string TaoTenNguoiDungTuIds(string ids, List<UserGridVo> users)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var idsList = UltiHelper.DinhDangSelect2ListJs(ids);

                var ctName = new List<string>();
                var ctris = ids.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => Convert.ToInt32(o.Trim())).ToList();
                foreach (var ct in ctris)
                {
                    var userCt = users.FirstOrDefault(o => o.Id == ct);
                    if (userCt != null)
                    {
                        ctName.Add(userCt.Name);
                    }
                }
                return string.Join(", ", ctName);
            }

            return "";
        }

        public static List<LookupViewModel> TrangThaiHoatDongEnum(bool isHideLabelChon = false)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };


            var result = EnumUltiHelper.EnumToLookupList<TrangThaiHoatDong>();

            if (isHideLabelChon)
            {
                return result;
            }

            return data.Concat(result).ToList();
        }

        public static List<LookupViewModel> QuanLyBaoCaoChuyenNganhEnumLookup(bool isHideLabelChon = false)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var result = EnumUltiHelper.EnumToLookupList<QuanLyBaoCaoChuyenNganhEnum>();

            if (isHideLabelChon)
            {
                return result;
            }

            return data.Concat(result).ToList();
        }
        public static string TaoNoiDungCuocHop(CuocHopGridVo cuocHop)
        {
            var phongHops = new List<string>();
            if (cuocHop.PhongHopId.GetValueOrDefault() != 0)
            {
                var phongHop = CacheManagerHelper.GetPhongHopByIdFromCache(cuocHop.PhongHopId.Value);
                if (phongHop != null && phongHop.Id != 0)
                {
                    phongHops.Add(phongHop.TenPhong);
                }
            }

            if (!string.IsNullOrEmpty(cuocHop.PhongHopTxt))
            {
                phongHops.Add(cuocHop.PhongHopTxt);
            }

            return cuocHop.TenCuocHop + $" (Thời gian: {cuocHop.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")}) - Địa điểm: {string.Join(" - ", phongHops)}";
        }

        public static List<int> GetPhongBanChildrenIds(int phongBanId)
        {
            var result = new List<int>();
            var phongBans = CacheManagerHelper.GetPhongBanCache();
            var phongBanCon = phongBans
                .Where(o => o.PhongBanChaId == phongBanId && o.IsDeleted != true)
                .OrderBy(o => o.ThuTu ?? int.MaxValue)
                .ThenBy(o => o.Ten)
                .ToList();

            foreach (var phongBan in phongBanCon)
            {
                result.Add(phongBan.Id);
                result.AddRange(GetPhongBanChildrenIds(phongBan.Id));
            }

            return result;
        }

        public static List<LookupViewModel> GetPhongBanChildrenSorted(int phongBanId)
        {
            var result = new List<LookupViewModel>();
            var phongBans = CacheManagerHelper.GetPhongBanCache();
            var phongBanCon = phongBans
                .Where(o => o.PhongBanChaId == phongBanId && o.IsDeleted != true)
                .OrderBy(o => o.ThuTu ?? int.MaxValue)
                .ThenBy(o => o.Ten)
                .ToList();

            foreach (var phongBan in phongBanCon)
            {
                result.Add(new LookupViewModel
                {
                    id = phongBan.Id,
                    text = phongBan.Ten,
                    phongBanId = phongBan.PhongBanChaId
                });
                
                var children = GetPhongBanChildrenSorted(phongBan.Id);
                result.AddRange(children);
            }

            return result;
        }

        public static List<PhongBanGridVo> GetPhongBanTreeSorted(int currentPhongBanId = 0, bool isPhongNhanViecOnly = false)
        {
            var allPhongBans = isPhongNhanViecOnly ? CacheManagerHelper.GetPhongBanNhanViecCacheNotDeleted() : CacheManagerHelper.GetPhongBanCacheNotDeleted();
            var result = new List<PhongBanGridVo>();

            if (currentPhongBanId != 0)
            {
                var root = allPhongBans.FirstOrDefault(pb => pb.Id == currentPhongBanId);
                if (root != null)
                {
                    root.Level = 0;
                    result.Add(root);
                    AddPhongBanChildrenRecursive(root.Id, allPhongBans, result, 1);
                }
            }
            else
            {
                var roots = allPhongBans
                    .Where(pb => !pb.PhongBanChaId.HasValue || pb.PhongBanChaId.Value == 0 || !allPhongBans.Any(p => p.Id == pb.PhongBanChaId.Value))
                    .OrderBy(pb => pb.ThuTu ?? int.MaxValue)
                    .ThenBy(pb => pb.Ten)
                    .ToList();

                foreach (var root in roots)
                {
                    root.Level = 0;
                    result.Add(root);
                    AddPhongBanChildrenRecursive(root.Id, allPhongBans, result, 1);
                }
            }

            return result;
        }

        private static void AddPhongBanChildrenRecursive(int parentId, List<PhongBanGridVo> allPhongBans, List<PhongBanGridVo> result, int currentLevel)
        {
            var children = allPhongBans
                .Where(pb => pb.PhongBanChaId == parentId)
                .OrderBy(pb => pb.ThuTu ?? int.MaxValue)
                .ThenBy(pb => pb.Ten)
                .ToList();

            foreach (var child in children)
            {
                child.Level = currentLevel;
                result.Add(child);
                AddPhongBanChildrenRecursive(child.Id, allPhongBans, result, currentLevel + 1);
            }
        }
    }
}