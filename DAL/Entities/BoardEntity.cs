using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    [Table("tbl_boards")]
    public class BoardEntity
    {
        [Key]
        public int ID { get; set; } = -1;
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        public string Content { get; set; }

        public virtual List<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
