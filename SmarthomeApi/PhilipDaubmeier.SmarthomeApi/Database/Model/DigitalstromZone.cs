using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.SmarthomeApi.Database.Model
{
    public class DigitalstromZone
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(40)]
        public int Name { get; set; }
    }
}
