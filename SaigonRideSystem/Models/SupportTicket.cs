using System.ComponentModel.DataAnnotations;

namespace SaigonRideSystem.Models
{
    public class SupportTicket
    {
        public int SupportTicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        [Display(Name = "Issue Type")]
        public SupportIssueType IssueType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Submitted;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? AdminResponse { get; set; }

        public DateTime? RespondedAt { get; set; }

        public bool IsResponseReadByUser { get; set; } = false;
    }
}