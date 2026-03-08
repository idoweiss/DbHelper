// מחלקות בדיקה (DTOs)
public class Student
{
    public int Id;
    public string Name;
    public double Average;
    public DateTime JoinedDate;
    public bool IsActive;
}

public class Report
{
    public string StudentName;
    public string CourseName;
}


static class DbHeloperTests
{

    static void Assert(bool condition, string msg)
    {
        if (!condition)
        {
            Console.WriteLine($"\n[X] FAILURE: {msg}");
            Environment.Exit(1);
        }
    }

    static void InitializeDatabase()
    {
        // שימוש בפרמטר יחיד - מחרוזת SQL מלאה
        string createTable = @"
            CREATE TABLE Students (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Average REAL,
                JoinedDate TEXT,
                IsActive INTEGER
            );
            CREATE TABLE Courses (
                Id INTEGER PRIMARY KEY,
                Title TEXT
            );
            INSERT INTO Courses VALUES (101, 'Math');
        ";
        DbHelper.RunSqlChange(createTable);
        Console.WriteLine("[v] Database Initialized.");
    }

    public static void runDbHelperTests()
    {
        Console.WriteLine("--- Starting DbHelper Automated Tests ---\n");

        string testDbFile = "test_database.db";
        if (File.Exists(testDbFile)) File.Delete(testDbFile);

        DbHelper.ConnectionString = $"Data Source={testDbFile}";

        InitializeDatabase();


        // בדיקות בשיטה הפשוטה (פרמטר אחד - מחרוזת מלאה)
        Test_SimpleStringInterpolation();
        Test_SimpleLiteralString();


        // בדיקות בשיטה המוגנת (מספר פרמטרים - תבנית + ערכים)
        Test_InsertAndSelect();
        Test_SqlInjectionProtection();
        Test_UpdateAndDelete();
        Test_DataTypesAndNulls();
        Test_JoinQuery();
        Test_FieldCaseInsensitivity();

        Console.WriteLine("\n--- All Tests Completed Successfully ---");
        Console.WriteLine($"Test database created at: {Path.GetFullPath(testDbFile)}");
    }

    static void Test_DataTypesAndNulls()
    {
        Console.Write("Test 4: Null Handling & Types... ");

        DbHelper.RunSqlChange("INSERT INTO Students (Name, Average) VALUES ({}, {})", "NullGuy", null);
        var list = DbHelper.RunSelect<Student>("SELECT * FROM Students WHERE Name = 'NullGuy'");

        Assert(list[0].Name == "NullGuy", "Name match");
        Assert(list[0].Average == 0, "Double default should be 0");

        Console.WriteLine("PASSED");
    }

    static void Test_FieldCaseInsensitivity()
    {
        Console.Write("Test 6: Case Insensitivity... ");

        var list = DbHelper.RunSelect<Student>("SELECT Name as name FROM Students LIMIT 1");

        Assert(list.Count > 0, "Got result");
        Assert(!string.IsNullOrEmpty(list[0].Name), "Mapped 'name' to 'Name'");

        Console.WriteLine("PASSED");
    }

    static void Test_InsertAndSelect()
    {
        Console.Write("Test 1: Protected Insert & Select (Multiple Parameters)... ");

        DateTime now = DateTime.Now;
        // שימוש בפורמט המוגן עם סוגריים מסולסלים כפי שהגדרנו קודם
        DbHelper.RunSqlChange("INSERT INTO Students (Name, Average, JoinedDate, IsActive) VALUES ({}, {}, {}, {})",
            "Alice", 95.5, now, true);

        var list = DbHelper.RunSelect<Student>("SELECT * FROM Students WHERE Name = {}", "Alice");

        Assert(list.Count == 1, "Count should be 1");
        Assert(list[0].Name == "Alice", "Name match");
        Console.WriteLine("PASSED");
    }

    static void Test_JoinQuery()
    {
        Console.Write("Test 5: JOIN Mapping... ");

        string sql = @"
            SELECT 'Joiner' as StudentName, Title as CourseName 
            FROM Courses 
            WHERE Id = 101";

        var reports = DbHelper.RunSelect<Report>(sql);

        Assert(reports.Count > 0, "Got result");
        Assert(reports[0].StudentName == "Joiner", "Mapped StudentName");
        Console.WriteLine("PASSED");
    }

    static void Test_SimpleLiteralString()
    {
        Console.Write("Test 0b: Simple Literal String (Single Parameter)... ");

        // שליפה פשוטה ללא שום משתנים
        var list = DbHelper.RunSelect<Student>("SELECT * FROM Students");

        Assert(list.Count > 0, "Should have students in the list");
        Console.WriteLine("PASSED");
    }

    static void Test_SimpleStringInterpolation()
    {
        Console.Write("Test 0a: Simple String Interpolation (Single Parameter)... ");

        string studentName = "SimpleStudent";
        double average = 88.8;

        // התלמיד שולח פרמטר אחד בלבד - מחרוזת משובצת (Interpolated String)
        DbHelper.RunSqlChange($"INSERT INTO Students (Name, Average) VALUES ('{studentName}', {average})");

        // שליפה עם פרמטר אחד
        var list = DbHelper.RunSelect<Student>($"SELECT * FROM Students WHERE Name = '{studentName}'");

        Assert(list.Count == 1, "Should find the student");
        Assert(list[0].Average == 88.8, "Average should match");
        Console.WriteLine("PASSED");
    }

    static void Test_SqlInjectionProtection()
    {
        Console.Write("Test 2: SQL Injection Protection... ");

        string badName = "Robert'); DROP TABLE Students; --";
        DbHelper.RunSqlChange("INSERT INTO Students (Name) VALUES ({})", badName);

        var list = DbHelper.RunSelect<Student>("SELECT * FROM Students WHERE Name = {}", badName);
        Assert(list.Count == 1, "Should handle dangerous string as plain text");

        try
        {
            DbHelper.RunSelect<Student>("SELECT count(*) FROM Students");
            Console.WriteLine("PASSED");
        }
        catch
        {
            Console.WriteLine("FAILED (Table was deleted!)");
        }
    }

    static void Test_UpdateAndDelete()
    {
        Console.Write("Test 3: Update & Delete... ");

        DbHelper.RunSqlChange("INSERT INTO Students (Name, Average) VALUES ({}, {})", "Bob", 80);
        var students = DbHelper.RunSelect<Student>("SELECT Id FROM Students WHERE Name = 'Bob'");
        int id = students[0].Id;

        DbHelper.RunSqlChange("UPDATE Students SET Average = {} WHERE Id = {}", 100, id);
        var bob = DbHelper.RunSelect<Student>("SELECT * FROM Students WHERE Id = {}", id)[0];
        Assert(bob.Average == 100, "Grade updated");

        int affected = DbHelper.RunSqlChange("DELETE FROM Students WHERE Id = {}", id);
        Assert(affected == 1, "Delete affected 1 row");

        Console.WriteLine("PASSED");
    }
}