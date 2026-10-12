using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using log4net;
using System;

namespace Dashboard.Services
{
    public interface IZaloService
    {
        void ReplyMessageZalo(ZaloLienKetRequest request);
        void ReplyMessageZalo(string text, string message_id, string user_id);
        string GetAccessTokenFromRefreshToken(string refreshtoken = null);
        bool ThemOrCapNhat(string AccessToken, string RefreshToken);
        bool GuiTinNhanQuaZaloOA(string text, string message_id, string user_id);
        dynamic ThongBaoQLCV(ZaloTestMessageRequest request);
    }

    public class ZaloService : IZaloService
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(typeof(ZaloService));
        private readonly IZaloRepository _zaloRepository;
        private readonly IZaloValidate _zaloValidate;

        public ZaloService(IZaloRepository zaloRepository, IZaloValidate zaloValidate)
        {
            _zaloRepository = zaloRepository;
            _zaloValidate = zaloValidate;
        }

        public void ReplyMessageZalo(ZaloLienKetRequest request)
        {
            try
            {
                if (request == null)
                {
                    return;
                }

                log.Info($"ReplyMessageZalo nhan duoc: text={request.SoDienThoai}, message_id={request.MessageId}, user_id={request.UserZaloId}");
                string phoneChuanHoa = UltiHelper.ChuanHoaSoDienThoai(request.SoDienThoai);
                if (!UltiHelper.PhoneValidVietNam(phoneChuanHoa))
                {
                    log.Info($"ReplyMessageZalo: text '{request.SoDienThoai}' (chuan hoa: '{phoneChuanHoa}') khong phai so dien thoai hop le.");
                    return;
                }

                request.SoDienThoai = phoneChuanHoa;
                var lienKetResult = _zaloRepository.LienKetSoDienThoai(request);
                string textPhanHoi = lienKetResult.Message;

                GuiTinNhanQuaZaloOA(textPhanHoi, request.MessageId, request.UserZaloId);
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi ReplyMessageZalo: {ex.Message}", ex);
            }
        }

        public void ReplyMessageZalo(string text, string message_id, string user_id)
        {
            ReplyMessageZalo(new ZaloLienKetRequest
            {
                SoDienThoai = text,
                MessageId = message_id,
                UserZaloId = user_id
            });
        }

        public dynamic ThongBaoQLCV(ZaloTestMessageRequest request)
        {
            try
            {
                var errors = _zaloValidate.Validate(request);
                if (errors.Count > 0)
                {
                    return new { Errors = errors, Success = false };
                }

                string soDienThoai = request.SoDienThoai != null ? request.SoDienThoai.Trim() : string.Empty;
                string noiDung = request.NoiDung != null ? request.NoiDung.Trim() : string.Empty;

                User check = _zaloRepository.LayUserTheoSoDienThoaiDaLienKetZalo(soDienThoai);
                if (check != null)
                {
                    if (!GuiTinNhanQuaZaloOA(noiDung, null, check.UserZaloId))
                    {
                        errors.Add(new ValidateError
                        {
                            Id = "NoiDung",
                            Description = "Có lỗi khi gửi tin nhắn, vui lòng liên hệ quản trị để được hỗ trợ"
                        });
                        return new { Errors = errors, Success = false };
                    }

                    return new { Success = true, Message = "Gửi tin nhắn Zalo thành công." };
                }

                errors.Add(new ValidateError
                {
                    Id = "SoDienThoai",
                    Description = "Số điện thoại chưa được liên kết"
                });
                return new { Errors = errors, Success = false };
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi ThongBaoQLCV: {ex.Message}", ex);
                return new { Success = false, Message = "Có lỗi khi gửi tin nhắn, vui lòng liên hệ quản trị để được hỗ trợ" };
            }
        }

        public bool GuiTinNhanQuaZaloOA(string text, string message_id, string user_id)
        {
            return ZaloHelper.GuiTinNhanQuaZaloOA(text, user_id, message_id);
        }

        public string GetAccessTokenFromRefreshToken(string refreshtoken = null)
        {
            return ZaloHelper.GetAccessTokenFromRefreshToken(refreshtoken);
        }

        public bool ThemOrCapNhat(string AccessToken, string RefreshToken)
        {
            return _zaloRepository.ThemOrCapNhat(AccessToken, RefreshToken);
        }
    }
}
