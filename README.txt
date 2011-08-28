ProcrastinHater is a task management application being created in C# and WPF (.NET 4.0) using the MVVM design pattern and some other basic software patterns. 
It uses SQL Server CE to store its data.

It's mostly just a practice project.

Important:
The App.config file points to C:\ProcrastinHater.Data.sdf. So copy said file from the ProcrastinHater.DAL project folder to that location for development purposes.
For release builds, uncomment the line in the App.config file that points to |DataDirectory|.