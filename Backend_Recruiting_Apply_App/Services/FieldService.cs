using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IFieldService
    {
        Task<List<Field>> GetAllFieldsAsync();
        Task<Field?> GetFieldByIdAsync(int id);
        Task<Field> CreateFieldAsync(Field field);
        Task<bool> UpdateFieldAsync(int id, Field field);
        Task<bool> DeleteFieldAsync(int id);
    }

    public class FieldService : IFieldService
    {
        private readonly RAADbContext _dbContext;

        public FieldService(RAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Field>> GetAllFieldsAsync()
        {
            return await _dbContext.Field.ToListAsync();
        }

        public async Task<Field?> GetFieldByIdAsync(int id)
        {
            return await _dbContext.Field.FindAsync(id);
        }

        public async Task<Field> CreateFieldAsync(Field field)
        {
            _dbContext.Field.Add(field);
            await _dbContext.SaveChangesAsync();
            return field;
        }

        public async Task<bool> UpdateFieldAsync(int id, Field field)
        {
            if (id != field.ID)
            {
                return false;
            }

            _dbContext.Entry(field).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _dbContext.Field.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteFieldAsync(int id)
        {
            var field = await _dbContext.Field.FindAsync(id);
            if (field == null)
            {
                return false;
            }

            _dbContext.Field.Remove(field);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}