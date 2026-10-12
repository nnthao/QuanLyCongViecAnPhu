using System.Collections.Generic;
using Dashboard.Models.Entities;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface ISangKienCNTTFileService
    {
        List<SangKienCNTTFile> GetBySangKienCNTTId(int sangKienCNTTId);
        SangKienCNTTFile GetById(int id);
        int Them(SangKienCNTTFile entity);
        void Xoa(int id);
    }

    public class SangKienCNTTFileService : ISangKienCNTTFileService
    {
        private readonly ISangKienCNTTFileRepository _repository;

        public SangKienCNTTFileService(ISangKienCNTTFileRepository repository)
        {
            _repository = repository;
        }

        public List<SangKienCNTTFile> GetBySangKienCNTTId(int sangKienCNTTId)
        {
            return _repository.GetBySangKienCNTTId(sangKienCNTTId);
        }

        public SangKienCNTTFile GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int Them(SangKienCNTTFile entity)
        {
            return _repository.Them(entity);
        }

        public void Xoa(int id)
        {
            _repository.Xoa(id);
        }
    }
}


