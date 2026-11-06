# Task Tracker (C#)

A simple console-based Task Tracker application built with C# and .NET 8.

## Features

- **Add Tasks:** Create tasks with a title, description, due date, and priority.
- **Update Tasks:** Change a task's status to Pending, In Progress, or Completed.
- **View Tasks:** Display all tasks or filter them by Active, Overdue, and Completed.
- **Delete Tasks:** Remove tasks after confirmation.
- **Safe Exit:** Exit the application through the main menu.
- **Input Validation:** Validate required text, dates, numeric choices, and task status.

## Requirements

- .NET 8 SDK or later
- Visual Studio, Visual Studio Code, or any C# compatible IDE

## How to Run

1. Clone or download this repository.
2. Open a terminal in the project folder.
3. Run:

```bash
dotnet run
```

## Notes

- Tasks are stored in memory while the application is running. They are cleared when the program exits.
- Enter due dates using the `yyyy-MM-dd` format, for example `2026-12-31`.

## Technologies

- C#
- .NET 8
- Object-Oriented Programming
- Collections and LINQ
- Enums
- Exception-safe input handling with `TryParse`

## Author

Ahmed Reda
