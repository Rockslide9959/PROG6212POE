using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace PROG6212POE.Models
{
    public class Claim : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string ClaimId { get; set; }
        public string LecturerName { get; set; }
        public string SalaryMonth { get; set; }
        public string HoursWorked { get; set; }
    }
}
