using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp13
{
    class Program
    {
        static List<Student> students = new List<Student>();
        static List<Course> courses = new List<Course>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n=== MENU ===");
                Console.WriteLine("1. Dodaj studenta");
                Console.WriteLine("2. Dodaj kurs");
                Console.WriteLine("3. Zapisz studenta na kurs");
                Console.WriteLine("4. Wystaw ocenę");
                Console.WriteLine("5. Dodaj frekwencję");
                Console.WriteLine("6. Pokaż raport frekwencji");
                Console.WriteLine("7. Pokaż oceny studenta");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybierz opcję: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        AddCourse();
                        break;
                    case "3":
                        EnrollStudent();
                        break;
                    case "4":
                        AssignGrade();
                        break;
                    case "5":
                        AddAttendance();
                        break;
                    case "6":
                        ShowAttendance();
                        break;
                    case "7":
                        ShowGrades();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Nieznana opcja.");
                        break;
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Podaj imię studenta: ");
            var name = Console.ReadLine();
            var student = new Student(name);
            students.Add(student);
            Console.WriteLine($"Dodano studenta: {student.Name} (ID: {student.Id})");
        }

        static void AddCourse()
        {
            Console.Write("Podaj nazwę kursu: ");
            var title = Console.ReadLine();
            var course = new Course(title);
            courses.Add(course);
            Console.WriteLine($"Dodano kurs: {course.Title} (ID: {course.Id})");
        }

        static Student SelectStudent()
        {
            if (!students.Any())
            {
                Console.WriteLine("Brak studentów.");
                return null;
            }
            for (int i = 0; i < students.Count; i++)
                Console.WriteLine($"{i + 1}. {students[i].Name} (ID: {students[i].Id})");

            Console.Write("Wybierz numer studenta: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= students.Count)
                return students[index - 1];

            Console.WriteLine("Nieprawidłowy wybór.");
            return null;
        }

        static Course SelectCourse()
        {
            if (!courses.Any())
            {
                Console.WriteLine("Brak kursów.");
                return null;
            }
            for (int i = 0; i < courses.Count; i++)
                Console.WriteLine($"{i + 1}. {courses[i].Title} (ID: {courses[i].Id})");

            Console.Write("Wybierz numer kursu: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= courses.Count)
                return courses[index - 1];

            Console.WriteLine("Nieprawidłowy wybór.");
            return null;
        }

        static void EnrollStudent()
        {
            var student = SelectStudent();
            var course = SelectCourse();
            if (student != null && course != null)
            {
                student.Enroll(course);
                Console.WriteLine("Zapisano studenta na kurs.");
            }
        }

        static void AssignGrade()
        {
            var student = SelectStudent();
            var course = SelectCourse();
            if (student != null && course != null)
            {
                Console.Write("Podaj ocenę: ");
                if (double.TryParse(Console.ReadLine(), out double value))
                {
                    course.AssignGrade(student, value);
                    Console.WriteLine("Wystawiono ocenę.");
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa ocena.");
                }
            }
        }

        static void AddAttendance()
        {
            var student = SelectStudent();
            var course = SelectCourse();
            if (student != null && course != null)
            {
                Console.Write("Podaj datę (YYYY-MM-DD): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    Console.Write("Obecny? (t/n): ");
                    bool isPresent = Console.ReadLine().Trim().ToLower() == "t";
                    course.MarkAttendance(student, date, isPresent);
                    Console.WriteLine("Dodano frekwencję.");
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa data.");
                }
            }
        }

        static void ShowAttendance()
        {
            var student = SelectStudent();
            if (student != null)
                student.ShowAttendanceReport();
        }

        static void ShowGrades()
        {
            var student = SelectStudent();
            if (student != null)
                student.ShowGrades();
        }
    }

    class Student
    {
        private static int _idCounter = 1;
        public int Id { get; }
        public string Name { get; }
        public List<Course> Courses { get; }

        public Student(string name)
        {
            Id = _idCounter++;
            Name = name;
            Courses = new List<Course>();
        }

        public void Enroll(Course course)
        {
            if (!Courses.Contains(course))
            {
                Courses.Add(course);
                course.AddStudent(this);
            }
        }

        public void ShowAttendanceReport()
        {
            Console.WriteLine($"\n[Frekwencja] {Name} (ID: {Id})");
            foreach (var course in Courses)
            {
                var attendanceRecords = course.Attendance.Where(a => a.Student == this).ToList();
                int present = attendanceRecords.Count(a => a.IsPresent);
                int total = attendanceRecords.Count;
                Console.WriteLine($"- {course.Title} (ID: {course.Id}): {present}/{total} obecności");
            }
        }

        public void ShowGrades()
        {
            Console.WriteLine($"\n[Oceny] {Name} (ID: {Id})");
            foreach (var course in Courses)
            {
                var grade = course.Grades.FirstOrDefault(g => g.Student == this);
                Console.WriteLine($"- {course.Title} (ID: {course.Id}): {(grade != null ? grade.Value.ToString() : "brak")}");
            }
        }
    }

    class Course
    {
        private static int _idCounter = 1;
        public int Id { get; }
        public string Title { get; }
        public List<Student> Students { get; }
        public List<Grade> Grades { get; }
        public List<Attendance> Attendance { get; }

        public Course(string title)
        {
            Id = _idCounter++;
            Title = title;
            Students = new List<Student>();
            Grades = new List<Grade>();
            Attendance = new List<Attendance>();
        }

        public void AddStudent(Student student)
        {
            if (!Students.Contains(student))
            {
                Students.Add(student);
            }
        }

        public void AssignGrade(Student student, double value)
        {
            Grades.Add(new Grade(student, this, value));
        }

        public void MarkAttendance(Student student, DateTime date, bool isPresent)
        {
            Attendance.Add(new Attendance(student, this, date, isPresent));
        }
    }

    class Grade
    {
        public Student Student { get; }
        public Course Course { get; }
        public double Value { get; }

        public Grade(Student student, Course course, double value)
        {
            Student = student;
            Course = course;
            Value = value;
        }
    }

    class Attendance
    {
        public Student Student { get; }
        public Course Course { get; }
        public DateTime Date { get; }
        public bool IsPresent { get; }

        public Attendance(Student student, Course course, DateTime date, bool isPresent)
        {
            Student = student;
            Course = course;
            Date = date;
            IsPresent = isPresent;
        }
    }
}
