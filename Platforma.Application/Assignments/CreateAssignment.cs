﻿using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class CreateAssignment
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required AssignmentDTORequest AssignmentDTO { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.AssignmentDTO.CourseId);
                if (course == null) return Result<Unit?>.Failure("Course not found.");
                if (request.AssignmentDTO.CourseId != request.AssignmentDTO.CourseId) return Result<Unit?>.Failure("Incorrect courseId.");
                if (request.AssignmentDTO.OpenDate.HasValue && request.AssignmentDTO.Deadline.HasValue)
                {
                    if (request.AssignmentDTO.OpenDate > request.AssignmentDTO.Deadline)
                        return Result<Unit?>.Failure("Open date cannot be after deadline.");
                }

                var assignment = new Assignment
                {
                    CourseId = request.AssignmentDTO.CourseId,
                    Name = request.AssignmentDTO.AssignmentName ?? "New assignment",
                    Content = request.AssignmentDTO.AssignmentContent ?? "Assigment description",
                    OpenDate = request.AssignmentDTO.OpenDate,
                    Deadline = request.AssignmentDTO.Deadline,
                    FileName = "",
                    FilePath = "",
                    AcceptedFileTypes = request.AssignmentDTO.AcceptedFileTypes ?? ".txt;.docx;.pdf;.zip",
                    AnswerRequired = request.AssignmentDTO.AnswerRequired ?? false
                };
                _context.Assignments.Add(assignment);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to create assignment.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
