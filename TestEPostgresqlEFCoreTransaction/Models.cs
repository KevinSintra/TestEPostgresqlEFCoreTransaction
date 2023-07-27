using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestEPostgresqlEFCoreTransaction
{
    public class pg_stat_statements_record
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 serialid { get; set; }
        [Required]
        public Int64 queryid { get; set; }
        [Required]
        public DateTime query_date { get; set; }
        [Required] public Int64 calls { get; set; }
        [Required] public double total_exec_time { get; set; }
        [Required] public double min_exec_time { get; set; }
        [Required] public double max_exec_time { get; set; }
        [Required] public Int64 rows { get; set; }
        [Required] public string query { get; set; } = null!;
    }
}
