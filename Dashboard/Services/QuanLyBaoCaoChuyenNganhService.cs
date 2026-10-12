using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System;
using System.ComponentModel;
using System.Linq;

namespace Dashboard.Services
{
    public interface IQuanLyBaoCaoChuyenNganhService
    {
        DataTableResult<QuanLyBaoCaoChuyenNganhGridVo> GetAll(BaoCaoChuyenNganhQueryViewModel queryParams);
        bool Xoa(int id);
        int TaoBaoCaoLog(QuanLyBaoCaoChuyenNganhViewModel viewModel);
    }

    public class QuanLyBaoCaoChuyenNganhService : IQuanLyBaoCaoChuyenNganhService
    {
        private readonly IQuanLyBaoCaoChuyenNganhRepository _repository;
        public QuanLyBaoCaoChuyenNganhService(IQuanLyBaoCaoChuyenNganhRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<QuanLyBaoCaoChuyenNganhGridVo> GetAll(BaoCaoChuyenNganhQueryViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            foreach (var item in data.rows)
            {
                var users = CacheManagerHelper.GetUserCache();
                var user = users.FirstOrDefault(o => o.Id == item.NguoiTaoId);
                if (user != null)
                {
                    item.NguoiTao = user.Name;
                }

                // Gán tên danh mục báo cáo từ Description của enum
                if (item.DanhMucBaoCaoId.HasValue)
                {
                    item.DanhMucBaoCao = GetEnumDescription((DanhMucBaoCaoEnum)item.DanhMucBaoCaoId.Value);
                }

                // Gán tên báo cáo chuyên ngành từ Description của enum
                if (item.QuanLyBaoCaoChuyenNganhEnumId.HasValue)
                {
                    item.QuanLyBaoCaoChuyenNganh = GetEnumDescription((QuanLyBaoCaoChuyenNganhEnum)item.QuanLyBaoCaoChuyenNganhEnumId.Value);
                }
            }
            return data;
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public int TaoBaoCaoLog(QuanLyBaoCaoChuyenNganhViewModel viewModel)
        {
            var data = _repository.TaoBaoCaoLog(viewModel);
            return data;
        }

        private string GetEnumDescription<TEnum>(TEnum value) where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);
            var name = Enum.GetName(type, value);
            if (name == null) return string.Empty;
            var field = type.GetField(name);
            if (field == null) return name;
            var attr = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attr != null && attr.Length > 0 ? attr[0].Description : name;
        }
    }
}
