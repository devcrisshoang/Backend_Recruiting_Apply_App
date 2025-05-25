using Microsoft.EntityFrameworkCore;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IArticleService
    {
        Task<List<Article>> GetAllArticlesAsync();
        Task<Article?> GetArticleByIdAsync(int id);
        Task<List<Article>> GetArticlesByRecruiterIdAsync(int recruiterId);
        Task<Article> CreateArticleAsync(Article article);
        Task<bool> UpdateArticleAsync(int id, Article article);
        Task<bool> DeleteArticleAsync(int id);
    }

    public class ArticleService : IArticleService
    {
        private readonly RAADbContext _context;

        public ArticleService(RAADbContext context)
        {
            _context = context;
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Article.ToListAsync();
        }

        public async Task<Article?> GetArticleByIdAsync(int id)
        {
            return await _context.Article.FindAsync(id);
        }

        public async Task<List<Article>> GetArticlesByRecruiterIdAsync(int recruiterId)
        {
            return await _context.Article
                .Where(a => a.Recruiter_ID == recruiterId)
                .ToListAsync();
        }

        public async Task<Article> CreateArticleAsync(Article article)
        {
            _context.Article.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> UpdateArticleAsync(int id, Article article)
        {
            if (id != article.ID)
            {
                return false;
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return !await _context.Article.AnyAsync(e => e.ID == id);
            }
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return false;
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}