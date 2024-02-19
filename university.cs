using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Enrollment> Enrollments { get; set; }
}

public class Course
{
    public int CourseId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Enrollment> Enrollments { get; set; }
    public List<Instructor> Instructors { get; set; }
}

public class Enrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
}

public class Instructor
{
    public int InstructorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Course> Courses { get; set; }
}

public class UniversityManager
{
    private UniversityDbContext _dbContext;

    public UniversityManager(UniversityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // 1) Получить список студентов, зачисленных на определенный курс.
    public IQueryable<Student> GetStudentsEnrolledInCourse(int courseId)
    {
        return _dbContext.Enrollments.Where(e => e.CourseId == courseId).Select(e => e.Student);
    }

    // 2) Получить список курсов, на которых учит определенный преподаватель.
    public IQueryable<Course> GetCoursesTaughtByInstructor(int instructorId)
    {
        return _dbContext.Courses.Where(c => c.Instructors.Any(i => i.InstructorId == instructorId));
    }

    // 3) Получить список курсов, на которых учит определенный преподаватель, вместе с именами студентов, зачисленных на каждый курс.
    public IQueryable<CourseWithStudents> GetCoursesWithStudentsTaughtByInstructor(int instructorId)
    {
        return _dbContext.Courses.Where(c => c.Instructors.Any(i => i.InstructorId == instructorId))
                                 .Select(c => new CourseWithStudents 
                                 {
                                     Course = c,
                                     Students = c.Enrollments.Select(e => e.Student)
                                 });
    }

    // 4) Получить список курсов, на которые зачислено более 10 студентов.
    public IQueryable<Course> GetCoursesWithMoreThan10Students()
    {
        return _dbContext.Courses.Where(c => c.Enrollments.Count() > 10);
    }
}

public class CourseWithStudents
{
    public Course Course { get; set; }
    public IQueryable<Student> Students { get; set; }
}

public class UniversityManager
{
  private UniversityDbContext _dbContext;

  public UniversityManager(UniversityDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // 5) Получить список студентов, старше 25 лет.
  public IQueryable<Student> GetStudentsOlderThan25()
  {
    var cutoffDate = DateTime.Today.AddYears(-25);
    return _dbContext.Students.Where(s => s.DateOfBirth < cutoffDate);
  }

  // 6) Получить средний возраст всех студентов.
  public double GetAverageStudentAge()
  {
    return _dbContext.Students.Average(s => (DateTime.Today - s.DateOfBirth).TotalDays / 365);
  }

  // 7) Получить самого молодого студента.
  public Student GetYoungestStudent()
  {
    return _dbContext.Students.OrderBy(s => s.DateOfBirth).FirstOrDefault();
  }

  // 8) Получить средний возраст студентов.
  public double GetAverageStudentAge()
  {
    return _dbContext.Students.Average(s => (DateTime.Today - s.DateOfBirth).TotalDays / 365);
  }

  // 9) Получить количество курсов, на которых учится студент с определенным Id.
  public int GetCourseCountForStudent(int studentId)
  {
    return _dbContext.Enrollments.Where(e => e.StudentId == studentId).Select(e => e.CourseId).Distinct().Count();
  }

  // 10) Получить список имен всех студентов.
  public IQueryable<string> GetAllStudentNames()
  {
    return _dbContext.Students.Select(s => $"{s.FirstName} {s.LastName}");
  }

  // 11) Сгруппировать студентов по возрасту.
  public IQueryable<StudentAgeGroup> GroupStudentsByAge()
  {
    return _dbContext.Students.GroupBy(s => (DateTime.Today - s.DateOfBirth).TotalDays / 365)
                              .Select(g => new StudentAgeGroup
                              {
                                Age = (int)g.Key,
                                Count = g.Count()
                              });
  }

  // 12) Получить список студентов, отсортированных по фамилии в алфавитном порядке.
  public IQueryable<Student> GetStudentsOrderedByLastName()
  {
    return _dbContext.Students.OrderBy(s => s.LastName);
  }

  // 13) Получить список студентов вместе с информацией о зачислениях на курсы.
  public IQueryable<StudentWithEnrollments> GetStudentsWithEnrollments()
  {
    return _dbContext.Students.Select(s => new StudentWithEnrollments
    {
      Student = s,
      Enrollments = s.Enrollments
    });
  }

  // 14) Получить список студентов, не зачисленных на определенный курс.
  public IQueryable<Student> GetStudentsNotEnrolledInCourse(int courseId)
  {
    return _dbContext.Students.Where(s => !s.Enrollments.Any(e => e.CourseId == courseId));
  }

  // 15) Получить список студентов, зачисленных одновременно на два определенных курса.
  public IQueryable<Student> GetStudentsEnrolledInBothCourses(int courseId1, int courseId2)
  {
    var studentsCourse1 = _dbContext.Enrollments.Where(e => e.CourseId == courseId1).Select(e => e.StudentId);
    var studentsCourse2 = _dbContext.Enrollments.Where(e => e.CourseId == courseId2).Select(e => e.StudentId);
    return _dbContext.Students.Where(s => studentsCourse1.Contains(s.StudentId) && studentsCourse2.Contains(s.StudentId));
  }

  // 16) Получить количество студентов на каждом курсе.
  public IQueryable<CourseStudentCount> GetCourseStudentCounts()
  {
    return _dbContext.Enrollments.GroupBy(e => e.CourseId)
                                 .Select(g => new CourseStudentCount
                                 {
                                   CourseId = g.Key,
                                   StudentCount = g.Count()
                                 });
  }
}

public class StudentAgeGroup
{
  public int Age { get; set; }
  public int Count { get; set; }
}

public class StudentWithEnrollments
{
  public Student Student { get; set; }
  public IQueryable<Enrollment> Enrollments { get; set; }
}

public class CourseStudentCount
{
  public int CourseId { get; set; }
  public int StudentCount { get; set; }
}

