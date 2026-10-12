using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Dashboard.Models;
using Dashboard.Models.Entities;

namespace Dashboard.Repository
{
    public interface ISangKienCNTTFileRepository
    {
        List<SangKienCNTTFile> GetBySangKienCNTTId(int sangKienCNTTId);
        SangKienCNTTFile GetById(int id);
        int Them(SangKienCNTTFile entity);
        void Xoa(int id);
    }

    public class SangKienCNTTFileRepository : ISangKienCNTTFileRepository
    {
        private readonly IContext _context;

        public SangKienCNTTFileRepository(IContext context)
        {
            _context = context;
        }

        public List<SangKienCNTTFile> GetBySangKienCNTTId(int sangKienCNTTId)
        {
            return _context.SangKienCNTTFiles.AsNoTracking()
                .Where(o => o.SangKienCNTTId == sangKienCNTTId && (o.IsDeleted == null || o.IsDeleted != true))
                .ToList();
        }

        public SangKienCNTTFile GetById(int id)
        {
            return _context.SangKienCNTTFiles.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }

        public int Them(SangKienCNTTFile entity)
        {
            entity.NgayTao = DateTime.Now;
            entity.IsDeleted = false;
            _context.SangKienCNTTFiles.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public void Xoa(int id)
        {
            var entity = _context.SangKienCNTTFiles.FirstOrDefault(o => o.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.NgayXoa = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}


