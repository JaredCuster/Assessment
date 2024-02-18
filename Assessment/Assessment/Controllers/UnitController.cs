using Assessment.Data;
using Assessment.DTO;
using Assessment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("units")]
    public class UnitController : Controller
    {
        private AssessmentContext _dbContext;

        public UnitController(AssessmentContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            var unitDtos = new List<UnitDto>();
            var units = await _dbContext.Units.ToListAsync();
            foreach (var unit in units)
            {
                unitDtos.Add(new UnitDto()
                {
                    id = unit.Id,
                    max_capacity = unit.MaxCapacity
                });
            }
            return Ok(unitDtos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUnit([FromRoute] int id)
        {
            var unit = await _dbContext.Units.FindAsync(id);

            if (unit == null)
            {
                return NotFound();
            }

            var unitDto = new UnitDto()
            {
                id = unit.Id,
                max_capacity = unit.MaxCapacity
            };

            return Ok(unitDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddUnit(UnitDto unitDto)
        {
            var unit = new Unit()
            {
                MaxCapacity = unitDto.max_capacity
            };

            await _dbContext.Units.AddAsync(unit);
            await _dbContext.SaveChangesAsync();

            unitDto.id = unit.Id;

            return Ok(unit);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateUnit([FromRoute] int id, UnitDto unitDto)
        {
            var unit = await _dbContext.Units.FindAsync(id);

            if (unit == null)
            {
                return NotFound();
            }

            unit.MaxCapacity = unitDto.max_capacity;

            await _dbContext.SaveChangesAsync();

            return Ok(unitDto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUnit([FromRoute] int id)
        {
            var unit = await _dbContext.Units.FindAsync(id);

            if (unit != null)
            {
                _dbContext.Remove(unit);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
