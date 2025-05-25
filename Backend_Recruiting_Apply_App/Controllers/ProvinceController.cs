using Microsoft.AspNetCore.Mvc;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        // GET: api/Province
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetProvince()
        {
            var provinces = await _provinceService.GetAllProvincesAsync();
            return Ok(provinces);
        }

        // GET: api/Province/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Province>> GetProvince(int id)
        {
            var province = await _provinceService.GetProvinceByIdAsync(id);

            if (province == null)
                return NotFound();

            return Ok(province);
        }

        // POST: api/Province
        [HttpPost]
        public async Task<ActionResult<Province>> CreateProvince(Province province)
        {
            var createdProvince = await _provinceService.CreateProvinceAsync(province);
            return CreatedAtAction(nameof(GetProvince), new { id = createdProvince.ID }, createdProvince);
        }

        // PUT: api/Province/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvince(int id, Province province)
        {
            var result = await _provinceService.UpdateProvinceAsync(id, province);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Province/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
            var result = await _provinceService.DeleteProvinceAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}