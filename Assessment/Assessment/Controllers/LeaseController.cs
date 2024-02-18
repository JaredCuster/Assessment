using Assessment.Data;
using Assessment.DTO;
using Assessment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("leases")]
    public class LeaseController : Controller
    {
        private AssessmentContext _dbContext;

        public LeaseController(AssessmentContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeases()
        {
            var leaseDtos = new List<LeaseDto>();
            var leases = await _dbContext.Leases.ToListAsync();
            foreach (var lease in leases)
            {
                leaseDtos.Add(new LeaseDto()
                {
                    id = lease.Id,
                    move_in = lease.MoveIn,
                    move_out = lease.MoveOut,
                    resident_name = lease.ResidentName,
                    unit_id = lease.UnitId
                });
            }
            return Ok(leaseDtos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetLease([FromRoute] int id)
        {
            var lease = await _dbContext.Leases.FindAsync(id);

            if (lease == null)
            {
                return NotFound();
            }

            var leaseDto = new LeaseDto()
            {
                id = lease.Id,
                move_in = lease.MoveIn,
                move_out = lease.MoveOut,
                resident_name = lease.ResidentName,
                unit_id = lease.UnitId
            };

            return Ok(leaseDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddLease(LeaseDto leaseDto)
        {
            if (await IsValidLease(leaseDto))
            {
                var lease = new Lease()
                {
                    MoveIn = leaseDto.move_in,
                    MoveOut = leaseDto.move_out,
                    ResidentName = leaseDto.resident_name,
                    UnitId = leaseDto.unit_id
                };

                await _dbContext.Leases.AddAsync(lease);
                await _dbContext.SaveChangesAsync();

                leaseDto.id = lease.Id;
                return new ObjectResult(leaseDto) { StatusCode = StatusCodes.Status201Created };
            }

            return new ObjectResult(leaseDto) { StatusCode = StatusCodes.Status422UnprocessableEntity };
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateLease([FromRoute] int id, LeaseDto leaseDto)
        {
            var lease = await _dbContext.Leases.FindAsync(id);

            if (lease == null)
            {
                return NotFound();
            }

            lease.ResidentName = leaseDto.resident_name;

            await _dbContext.SaveChangesAsync();

            return Ok(leaseDto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteLease([FromRoute] int id)
        {
            var leaseModel = await _dbContext.Leases.FindAsync(id);

            if (leaseModel != null)
            {
                _dbContext.Remove(leaseModel);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        private async Task<bool> IsValidLease(LeaseDto leaseDto)
        {
            var unit = await _dbContext.Units.FindAsync(leaseDto.unit_id);
            if (unit != null)
            {
                var activeLeaseCount = _dbContext.Leases
                    .Where(l => l.UnitId == unit.Id)
                    .Where(l => (l.MoveIn <= leaseDto.move_in && l.MoveOut >= leaseDto.move_in) || 
                                (l.MoveIn <= leaseDto.move_out && l.MoveOut >= leaseDto.move_out))
                    .Count();

                if (activeLeaseCount < unit.MaxCapacity) {
                    return true; 
                }
            }

            return false;
        }
    }
}
