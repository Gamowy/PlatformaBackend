namespace Platforma.Application.Assignments
{
    public class AssignmentDTORequest
    {
        public Guid CourseId { get; set; }
        public string? AssignmentName { get; set; }
        public string? AssignmentContent { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AcceptedFileTypes { get; set; }
        public bool? AnswerRequired { get; set; } = true;
    }
}
