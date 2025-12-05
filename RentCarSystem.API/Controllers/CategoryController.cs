using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Tüm kategorileri listeler
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();  
            return Ok(categories);
        }

        /// <summary>
        /// ID'ye göre kategori getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)  // ✅ Null kontrolü ekle
                return NotFound(new { message = $"Category with id {id} not found" });

            return Ok(category);
        }

        /// <summary>
        /// Aktif kategorileri listeler
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<CategoryDTO>>> GetActive()
        {
            var categories = await _categoryService.GetActiveCategoriesAsync(); 
            return Ok(categories);
        }

        /// <summary>
        /// Yeni kategori ekler
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDTO dto)
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);  
        }

        /// <summary>
        /// Kategori günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> Update(int id, [FromBody] CreateCategoryDTO dto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }

        /// <summary>
        /// Kategori siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
                return NotFound(new { message = $"Category with id {id} not found" });

            return NoContent();
        }
    }
}