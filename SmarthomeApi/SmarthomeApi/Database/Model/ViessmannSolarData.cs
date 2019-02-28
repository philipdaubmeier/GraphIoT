using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class ViessmannSolarData
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Id { get; set; }

        [Required]
        public DateTime Day { get; set; }

        public int? SolarWhTotal { get; set; }

        [MaxLength(400)]
        public string SolarWhCurve { get; set; }

        [MaxLength(400)]
        public string SolarCollectorTempCurve { get; set; }

        [MaxLength(400)]
        public string SolarHotwaterTempCurve { get; set; }

        [MaxLength(50)]
        public string SolarPumpStateCurve { get; set; }

        [MaxLength(50)]
        public string SolarSuppressionCurve { get; set; }
    }
}
