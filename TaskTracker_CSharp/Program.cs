using System.Globalization;

namespace TaskTracker;

public enum TaskStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3
}

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public class TodoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
}

internal class Program
{
    private static readonly List<TodoTask> Tasks = new();
    private static int nextId = 1;

    private static void Main()
    {
        Console.Title = "Task Tracker";
        bool running = true;

        while (running)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("1. Add a new task");
            Console.WriteLine("2. Update task status");
            Console.WriteLine("3. View tasks");
            Console.WriteLine("4. Delete a task");
            Console.WriteLine("5. Exit");
            Console.Write("\nChoose an option (1-5): ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    UpdateTaskStatus();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("\nThank you for using Task Tracker. Goodbye!");
                    break;
                default:
                    ShowMessage("Invalid choice. Please enter a number from 1 to 5.");
                    break;
            }

            if (running)
                Pause();
        }
    }

    private static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================");
        Console.WriteLine("          TASK TRACKER          ");
        Console.WriteLine("================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void AddTask()
    {
        Console.Clear();
        PrintSection("ADD NEW TASK");

        string title = ReadRequiredText("Title: ");
        string description = ReadRequiredText("Description: ");
        DateTime dueDate = ReadDate("Due date (yyyy-MM-dd): ");
        TaskPriority priority = ReadPriority();

        Tasks.Add(new TodoTask
        {
            Id = nextId++,
            Title = title,
            Description = description,
            DueDate = dueDate,
            Priority = priority,
            Status = TaskStatus.Pending
        });

        ShowMessage("Task added successfully!", ConsoleColor.Green);
    }

    private static void UpdateTaskStatus()
    {
        Console.Clear();
        PrintSection("UPDATE TASK STATUS");
        if (!EnsureTasksExist()) return;

        DisplayTasks(Tasks);
        int id = ReadInt("\nEnter task ID to update: ");
        TodoTask? task = Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            ShowMessage("Task not found.", ConsoleColor.Red);
            return;
        }

        Console.WriteLine("\nSelect new status:");
        Console.WriteLine("1. Pending");
        Console.WriteLine("2. In Progress");
        Console.WriteLine("3. Completed");

        int choice = ReadInt("Your choice: ");
        if (choice < 1 || choice > 3)
        {
            ShowMessage("Invalid status choice.", ConsoleColor.Red);
            return;
        }

        task.Status = (TaskStatus)choice;
        ShowMessage("Task status updated successfully!", ConsoleColor.Green);
    }

    private static void ViewTasks()
    {
        Console.Clear();
        PrintSection("VIEW TASKS");
        if (!EnsureTasksExist()) return;

        Console.WriteLine("1. All tasks");
        Console.WriteLine("2. Active tasks");
        Console.WriteLine("3. Overdue tasks");
        Console.WriteLine("4. Completed tasks");

        int choice = ReadInt("\nChoose a category: ");
        IEnumerable<TodoTask> selectedTasks = choice switch
        {
            1 => Tasks,
            2 => Tasks.Where(t => t.Status != TaskStatus.Completed),
            3 => Tasks.Where(t => t.Status != TaskStatus.Completed && t.DueDate.Date < DateTime.Today),
            4 => Tasks.Where(t => t.Status == TaskStatus.Completed),
            _ => Enumerable.Empty<TodoTask>()
        };

        if (choice < 1 || choice > 4)
        {
            ShowMessage("Invalid category choice.", ConsoleColor.Red);
            return;
        }

        Console.WriteLine();
        DisplayTasks(selectedTasks);
    }

    private static void DeleteTask()
    {
        Console.Clear();
        PrintSection("DELETE TASK");
        if (!EnsureTasksExist()) return;

        DisplayTasks(Tasks);
        int id = ReadInt("\nEnter task ID to delete: ");
        TodoTask? task = Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            ShowMessage("Task not found.", ConsoleColor.Red);
            return;
        }

        Console.Write($"Are you sure you want to delete \"{task.Title}\"? (y/n): ");
        string? confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirmation == "y" || confirmation == "yes")
        {
            Tasks.Remove(task);
            ShowMessage("Task deleted successfully!", ConsoleColor.Green);
        }
        else
        {
            Console.WriteLine("Delete operation cancelled.");
        }
    }

    private static void DisplayTasks(IEnumerable<TodoTask> tasks)
    {
        List<TodoTask> list = tasks.OrderBy(t => t.DueDate).ToList();

        if (list.Count == 0)
        {
            Console.WriteLine("No tasks found in this category.");
            return;
        }

        Console.WriteLine(
            "{0,-5} {1,-22} {2,-12} {3,-12} {4,-12}",
            "ID", "TITLE", "DUE DATE", "PRIORITY", "STATUS");
        Console.WriteLine(new string('-', 70));

        foreach (TodoTask task in list)
        {
            string title = task.Title.Length > 20
                ? task.Title[..17] + "..."
                : task.Title;

            Console.WriteLine(
                "{0,-5} {1,-22} {2,-12:yyyy-MM-dd} {3,-12} {4,-12}",
                task.Id, title, task.DueDate, task.Priority, task.Status);

            Console.WriteLine($"      Description: {task.Description}");
            if (task.Status != TaskStatus.Completed && task.DueDate.Date < DateTime.Today)
                Console.WriteLine("      ** OVERDUE **");
        }
    }

    private static bool EnsureTasksExist()
    {
        if (Tasks.Count > 0) return true;

        Console.WriteLine("There are no tasks yet. Add a task first.");
        return false;
    }

    private static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            Console.WriteLine("This field cannot be empty. Please try again.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            if (DateTime.TryParseExact(
                input,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
                return date;

            Console.WriteLine("Invalid date. Use the format yyyy-MM-dd (example: 2026-12-31).");
        }
    }

    private static TaskPriority ReadPriority()
    {
        while (true)
        {
            Console.WriteLine("\nSelect priority:");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. High");

            int choice = ReadInt("Priority: ");
            if (choice >= 1 && choice <= 3)
                return (TaskPriority)choice;

            Console.WriteLine("Please choose 1, 2, or 3.");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;

            Console.WriteLine("Please enter a valid number.");
        }
    }

    private static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"--- {title} ---\n");
        Console.ResetColor();
    }

    private static void ShowMessage(string message, ConsoleColor color = ConsoleColor.Yellow)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"\n{message}");
        Console.ResetColor();
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress Enter to return to the main menu...");
        Console.ReadLine();
    }
}
