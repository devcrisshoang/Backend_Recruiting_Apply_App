using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticle()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpGet("recruiter/{recruiterId}")]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticlesByRecruiterId(int recruiterId)
        {
            var articles = await _articleService.GetArticlesByRecruiterIdAsync(recruiterId);
            if (articles == null || !articles.Any())
            {
                return NotFound();
            }
            return Ok(articles);
        }

        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle(Article article)
        {
            var createdArticle = await _articleService.CreateArticleAsync(article);
            return CreatedAtAction(nameof(GetArticle), new { id = createdArticle.ID }, createdArticle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Article article)
        {
            var success = await _articleService.UpdateArticleAsync(id, article);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var success = await _articleService.DeleteArticleAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}