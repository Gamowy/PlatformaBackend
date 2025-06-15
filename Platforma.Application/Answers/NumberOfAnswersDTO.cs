namespace Platforma.Application.Answers
{
    public class NumberOfAnswersDTO
    {
        public Guid AssignmentId { get; set; }
        public int UnmarkedAnswers { get; set; }
        public int MarkedAnswers { get; set; }
    }
}
