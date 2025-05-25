using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IProvinceService
    {
        Task<IEnumerable<Province>> GetAllProvincesAsync();
        Task<Province> GetProvinceByIdAsync(int id);
        Task<Province> CreateProvinceAsync(Province province);
        Task<bool> UpdateProvinceAsync(int id, Province province);
        Task<bool> DeleteProvinceAsync(int id);
        Task<bool> ProvinceExistsAsync(int id);
    }

    public class ProvinceService : IProvinceService
    {
        private readonly RAADbContext _context;

        public ProvinceService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {
            return await _context.Province.ToListAsync();
        }

        public async Task<Province> GetProvinceByIdAsync(int id)
        {
            return await _context.Province.FindAsync(id);
        }

        public async Task<Province> CreateProvinceAsync(Province province)
        {
            _context.Province.Add(province);
            await _context.SaveChangesAsync();
            return province;
        }

        public async Task<bool> UpdateProvinceAsync(int id, Province province)
        {
            if (id != province.ID)
                return false;

            _context.Entry(province).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProvinceExistsAsync(id))
                    return false;
                throw;
            }
        }

        public async Task<bool> DeleteProvinceAsync(int id)
        {
            var province = await _context.Province.FindAsync(id);
            if (province == null)
                return false;

            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProvinceExistsAsync(int id)
        {
            return await _context.Province.AnyAsync(e => e.ID == id);
        }
    }
}