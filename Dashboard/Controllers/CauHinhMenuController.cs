using Dashboard.Common;
using Dashboard.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class CauHinhMenuController : Controller
    {
        [JwtAuthorize("Public")]
        public ActionResult MenuQuanTriPreview()
        {
            return View();
        }

        [JwtAuthorize("Public")]
        public ActionResult LayDanhSachMenuTheoQuyen()
        {
            var menuInit = CacheManagerHelper.GetDanhSachMenuCache();

            var isSystemUser = UserClaimHelper.GetIsSystemUser((ClaimsIdentity)User.Identity);
            if (isSystemUser)
            {
                return Json(menuInit, JsonRequestBehavior.AllowGet);
            }

            var vaiTro = UserClaimHelper.GetCurrentQuyenOfUser((ClaimsIdentity)User.Identity);
            if (string.IsNullOrEmpty(vaiTro))
            {
                return Json(new List<MenuGenerateViewModel>(), JsonRequestBehavior.AllowGet);
            }

            var filteredMenus = new List<MenuGenerateViewModel>();
            var quyenArr = vaiTro.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).ToList();

            filteredMenus = FilterMenusByPermissions(menuInit, quyenArr);

            return Json(filteredMenus, JsonRequestBehavior.AllowGet);
        }

        public List<MenuGenerateViewModel> FilterMenusByPermissions(List<MenuGenerateViewModel> menus, List<string> userPermissions)
        {
            var result = new List<MenuGenerateViewModel>();
            bool isInHeader = false;
            bool headerAdded = false;

            for (int i = 0; i < menus.Count; i++)
            {
                var menu = menus[i];

                // Nếu là header
                if (!string.IsNullOrEmpty(menu.Type) && menu.Type.ToLower() == "header")
                {
                    isInHeader = false;
                    headerAdded = false;

                    // Kiểm tra user có quyền trong permissions của header không
                    bool hasHeaderPermission = menu.Permissions != null && menu.Permissions.Any(p => userPermissions.Contains(p));
                    if (!hasHeaderPermission)
                    {
                        continue;
                    }

                    // Kiểm tra xem có menu nào sau header này có quyền không
                    bool hasMenuWithPermission = false;
                    for (int j = i + 1; j < menus.Count; j++)
                    {
                        var nextMenu = menus[j];
                        // Nếu gặp header mới thì dừng
                        if (!string.IsNullOrEmpty(nextMenu.Type) && nextMenu.Type.ToLower() == "header")
                        {
                            break;
                        }

                        // Kiểm tra quyền của menu này
                        bool menuHasPermission = false;
                        if (nextMenu.Datas != null && nextMenu.Datas.Any())
                        {
                            menuHasPermission = nextMenu.Datas.Any(child => userPermissions.Contains(child.Key));
                        }
                        else if (!string.IsNullOrEmpty(nextMenu.Key))
                        {
                            menuHasPermission = userPermissions.Contains(nextMenu.Key);
                        }

                        if (menuHasPermission)
                        {
                            hasMenuWithPermission = true;
                            break;
                        }
                    }

                    // Nếu có cả quyền header và có menu có quyền → thêm header vào kết quả
                    if (hasMenuWithPermission)
                    {
                        result.Add(new MenuGenerateViewModel
                        {
                            Type = menu.Type,
                            Title = menu.Title,
                            Permissions = menu.Permissions
                        });
                        isInHeader = true;
                        headerAdded = true;
                    }
                }
                else
                {
                    // Nếu là menu item bình thường
                    bool shouldAdd = false;
                    MenuGenerateViewModel menuToAdd = null;

                    // Nếu có menu con (Datas)
                    if (menu.Datas != null && menu.Datas.Any())
                    {
                        // Lọc menu con theo quyền
                        var filteredChildren = menu.Datas
                            .Where(child => userPermissions.Contains(child.Key))
                            .ToList();

                        // Nếu còn menu con sau khi lọc → thêm menu cha và các menu con đó
                        if (filteredChildren.Any())
                        {
                            shouldAdd = true;
                            menuToAdd = new MenuGenerateViewModel
                            {
                                Ten = menu.Ten,
                                Key = menu.Key,
                                Icon = menu.Icon,
                                Datas = filteredChildren,
                                SpanCount = menu.SpanCount
                            };
                        }
                    }
                    else
                    {
                        // Nếu là menu không có con → kiểm tra quyền trực tiếp
                        if (!string.IsNullOrEmpty(menu.Key) && userPermissions.Contains(menu.Key))
                        {
                            shouldAdd = true;
                            menuToAdd = new MenuGenerateViewModel
                            {
                                Ten = menu.Ten,
                                Key = menu.Key,
                                Icon = menu.Icon,
                                Url = menu.Url,
                                SpanCount = menu.SpanCount
                            };
                        }
                    }

                    // Nếu menu có quyền
                    if (shouldAdd)
                    {
                        // Nếu đang trong header và header đã được thêm → thêm menu vào kết quả
                        if (isInHeader && headerAdded)
                        {
                            result.Add(menuToAdd);
                        }
                        // Nếu không có header → thêm trực tiếp
                        else if (!isInHeader)
                        {
                            result.Add(menuToAdd);
                        }
                    }
                }
            }

            return result;
        }
    }
}