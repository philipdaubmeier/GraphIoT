using SmarthomeApi.FormatParsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class DigitalstromZone
    {
        [Key, Required]
        public int Id { get; set; }

        [MaxLength(40)]
        public int Name { get; set; }
    }
}
