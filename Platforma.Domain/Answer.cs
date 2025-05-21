using System.Text.Json.Serialization;

namespace Platforma.Domain
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        [JsonIgnore]
        public Assignment Assignment { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string? Comment { get; set; }
        public float? Mark { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
    }
}
