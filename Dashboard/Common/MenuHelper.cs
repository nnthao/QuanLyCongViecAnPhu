using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Common
{
    public static class MenuHelper
    {
        public static bool CheckPermissionMenu(string key)
        {
            try
            {
                var isSystemUser = (bool)HttpContext.Current.Session["IsSystemUser"];
                if (isSystemUser == true)
                {
                    return isSystemUser;
                }

                var phanQuyenArr = HttpContext.Current.Session["PhanQuyen"] as List<string>;

                if (phanQuyenArr?.FirstOrDefault(o => o == key) != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public static int CurrentUserId()
        {
            return (int)HttpContext.Current.Session["UserId"];
        }

        public static int? CurrentPhongBanId()
        {
            return (int)HttpContext.Current.Session["PhongBanId"];
        }

        public static List<MenuGenerateViewModel> ReadMenuJsonFile(string path)
        {
            var menuInit = new List<MenuGenerateViewModel>();

            try
            {
                if (System.IO.File.Exists(path))
                {
                    var dataStr = System.IO.File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(dataStr))
                    {
                        menuInit = JsonConvert.DeserializeObject<List<MenuGenerateViewModel>>(dataStr);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return menuInit;
        }

        public static MenuGenerateViewModel DanhSachQuyenMoRong()
        {
            var dataExts = new MenuGenerateViewModel
            {
                Ten = "Quyền mở rộng",
                Key = "QuyenMoRong"
            };

            // Lấy tất cả các field kiểu const string trong class QuyenMoRong
            var fields = typeof(QuyenMoRong).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (var field in fields)
            {
                // Lấy tên constant (vd: QUYEN_XEM_LICH_HOP_TAT_CA_DON_VI)
                string name = field.Name;

                // Gọi helper để tạo menu (dùng nameof thì ở đây name đã chính là nameof)
                dataExts.Datas.Add(UltiHelper.TaoMenuQuyenMoRongTuBienDungChung<QuyenMoRong>(name));
            }

            return dataExts;
        }
    }
}