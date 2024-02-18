using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assessment.DTO
{
    public class LeaseDto
    {
        public int id { get; set; }
        public DateTime move_in { get; set; }
        public DateTime? move_out { get; set; }
        public int resident_id { get; set; }
        public string resident_name { get; set; }
        public int unit_id { get; set; }
    }
}
