using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareSpaceApi.Data.Models
{
    [Table("interests")]
    public class Interest
    {
        [Key]
        [Column("interest_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("interest_name")]
        public string InterestName { get; set; } = string.Empty;
    }
}