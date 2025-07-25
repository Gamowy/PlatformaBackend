﻿using System.Text.Json.Serialization;

namespace Platforma.Domain
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string FileName { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public bool AnswerRequired { get; set; } = true;
        public string AcceptedFileTypes { get; set; }
        [JsonIgnore]
        public List<Answer> Answers { get; set; }
    }
}
