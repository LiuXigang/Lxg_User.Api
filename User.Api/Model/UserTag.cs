using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Model
{
    public class UserTag
    {
        [Key]
        public int AppUserId { get; set; }
        [Key]
        [MaxLength(100)]
        public int Tag { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationTime { get; set; }
    }
}
