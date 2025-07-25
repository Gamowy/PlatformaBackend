﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platforma.Application;
using Platforma.Application.Courses;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
using System.Security.Claims;

namespace PlatformaBackend.Controllers
{
    public class CourseController : BaseAPIController
    {
        /// <summary>
        /// Get a list of all courses
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAllCourses(string? searchPhrase)
        {
            var result = await Mediator.Send(new CourseList.Query() { SearchedPhrase = searchPhrase });
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get a list of all courses that user participate in
        /// </summary>
        [HttpGet("myCourses")]
        public async Task<ActionResult<List<Course>>> GetUsersCourses()
        {
            if (HttpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value.Equals(Platforma.Domain.User.Roles.Administrator))
                return await GetAllCourses(null);

            var result = await Mediator.Send(new CourseListForUser.Query 
                { UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value) });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get a list of users for specifed course
        /// </summary>
        [HttpGet("{courseId}/users")]
        public async Task<ActionResult<List<UserCourseDTO>>> GetUsersForCourse(Guid courseId)
        {
            var result = await Mediator.Send(new UserList.Query { CourseId = courseId, onlyAccepted = false });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get a list of accepted users for specifed course
        /// </summary>
        [HttpGet("{courseId}/AcceptedUsers")]
        public async Task<ActionResult<List<UserCourseDTO>>> GetAcceptedUsersForCourse(Guid courseId)
        {
            var result = await Mediator.Send(new UserList.Query { CourseId = courseId, onlyAccepted = true });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get a list of accepted users for specifed course
        /// </summary>
        [HttpGet("{courseId}/UnacceptedUsers")]
        public async Task<ActionResult<List<UserCourseDTO>>> GetUnacceptedUsersForCourse(Guid courseId)
        {
            var result = await Mediator.Send(new UserList.Query { CourseId = courseId, onlyAccepted = false });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                var list = result.Value.Where(u => u.status.Equals(UserStatus.Awaiting)).ToList();
                return Ok(list);
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get details about specified course
        /// </summary>
        [HttpGet("{courseId}")] 
        public async Task<ActionResult<Course>> GetCourse(Guid courseId)
        {
            var result = await Mediator.Send(new Details.Query { id = courseId });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null 
                && await UserParticipateInCourse(courseId))
                return Ok(result.Value);
            return BadRequest(result.Error);

        }

        /// <summary>
        /// Edit course
        /// </summary>
        [HttpPut("{courseId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> EditCourse(Guid courseId, CourseEditDTO course)
        {
            if (!await CheckIfAdminOrOwner(courseId))
                return Forbid();

            var result = await Mediator.Send(new Edit.Command { CourseDTO = course, CourseId = courseId });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> CreateCourse(CourseCreateDTO course)
        {
            var result = await Mediator.Send(new Create.Command { 
                CourseDTO = course,
                UserId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value)
            });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            if (!await CheckIfAdminOrOwner(id))
                return Forbid();

            var result = await Mediator.Send(new Delete.Command { Id = id });

            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        /// <summary>
        /// Get courses with close deadlines
        /// </summary>
        [HttpGet("deadlines")]
        [Authorize(Policy = "NotAdmin")]
        public async Task<ActionResult<List<Course>>> GetCoursesWithCloseDeadlines()
        {
            var userId = Guid.Parse(HttpContextAccessor.HttpContext!.User.FindFirst("UserId")!.Value);

            var result = await Mediator.Send(new GetCoursesWithCloseDeadlines.Query { UserId = userId});
            if (result == null || (result.IsSuccess && result.Value == null))
                return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
    }

}
