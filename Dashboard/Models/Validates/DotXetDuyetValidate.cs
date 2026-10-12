using Dashboard.Models.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dashboard.Models.Validates
{
    public interface IDotXetDuyetValidate
    {
        List<ValidateError> Validate(DotXetDuyetViewModel DotXetDuyet);
    }

    public class DotXetDuyetValidate : IDotXetDuyetValidate
    {
        private readonly IContext _context;
        public DotXetDuyetValidate(IContext context)
        {
            _context = context;
        }

        public List<ValidateError> Validate(DotXetDuyetViewModel DotXetDuyet)
        {
            var errors = new List<ValidateError>();

            if (string.IsNullOrEmpty(DotXetDuyet.TenDotXetDuyet) || string.IsNullOrEmpty(DotXetDuyet.TenDotXetDuyet.Trim()))
            {
                errors.Add(new ValidateError
                {
                    Id = "TenDotXetDuyet",
                    Description = "Tên đợt xét duyệt không được trống."
                });
            }
            else
            {
                var checkExist = _context.DotXetDuyets.AsNoTracking().FirstOrDefault(o => o.TenDotXetDuyet.ToLower().Trim()
                                                == DotXetDuyet.TenDotXetDuyet.ToLower().Trim());
                if (checkExist != null && checkExist.Id != DotXetDuyet.Id)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "TenDotXetDuyet",
                        Description = "Tên đợt xét duyệt đã tồn tại."
                    });
                }
            }

            return errors;
        }
    }
}

