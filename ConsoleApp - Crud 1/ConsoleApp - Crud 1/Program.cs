

using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Spectre.Console;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ConsoleApp_CRUD1
{
    public class Program
    {
        SqlConnection sqlConnection = new SqlConnection(connectionString);
        SqlCommand insertCommand;
        const string connectionString = "Data Source=OsiaTech\\SQLEXPRESS;Initial Catalog=db_consoleTest;Integrated Security=True;Trust Server Certificate=True";
        string insertQuery;
        Table table = new Table();

        public static void Main()
        {
            Program program = new Program();
            program.MainMenu();
        }

        public void InsertData()
        {
            AnsiConsole.Clear();
            try
            {
                AnsiConsole.MarkupLine("\n[bold white]Console App - Crud Operation[/] \n");
                sqlConnection.Open();
                AnsiConsole.MarkupLine("[bold green1]Database connection established successfully.[/] \n");

                var fullName = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("[bold white]Full Name:[/]")
                        .PromptStyle("yellow")
                    );
                AnsiConsole.WriteLine();
                var gender = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("[bold white]Gender:[/]\n ")
                        .PromptStyle("yellow")
                    );
                AnsiConsole.WriteLine();
                var email = AnsiConsole.Prompt
                    (
                        new TextPrompt<string>("[bold white]Email:[/]\n")
                        .PromptStyle("yellow")
                    );
                AnsiConsole.WriteLine();

                insertQuery = $"INSERT INTO studentTest(fullName, gender, email) VALUES('{fullName}', '{gender}', '{email}')";
                insertCommand = new SqlCommand(insertQuery, sqlConnection);
                insertCommand.ExecuteNonQuery();

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[bold green1]Record inserted successfully.[/]");
                AnsiConsole.WriteLine();
            }
            catch (Exception exception)
            {
                AnsiConsole.Clear();
                Type exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"[bold red1]Something went wrong. \n Error Description:{exceptionType.Name} {exception.Message}[/]");
                Console.WriteLine();
            }
            finally
            {
                sqlConnection.Close();
                AnsiConsole.MarkupLine("[bold green1]Database connection closed successfully.[/]");
            }
        }

        public void GetRecord()
        {
            AnsiConsole.Clear();

            try
            {
                table.AddColumns("[bold orange1]ID[/]", "[bold orange1]Full Name[/]", "[bold orange1]Gender[/]", "[bold orange1]Email[/]").Centered();
                sqlConnection.Open();
                AnsiConsole.MarkupLine("[bold green1]Database connection established successfully.[/] \n");

                AnsiConsole.MarkupLine(" [bold white]Console App - Get a Record by ID[/] \n");
                int id = AnsiConsole.Prompt
                    (
                        new TextPrompt<int>("[bold white]Insert the ID of the record: [/]")
                        .PromptStyle("yellow")
                    );

                string selectWhereQuery = $"SELECT id, fullName, gender, email FROM studentTest WHERE id = {id}";
                string selectWhereQuery22 = $"SELECT * FROM studentTest";

                SqlCommand selectWhereCommand = new SqlCommand(selectWhereQuery, sqlConnection);

                using (SqlDataReader dataReader = selectWhereCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.HasRows)
                        {
                            /* int _id = dataReader.GetInt32(0);
                             string fullName = dataReader.GetString(1);
                             string gender = dataReader.GetString(2);
                             string email = dataReader.GetString(3); */

                            //better and safer alternative: using the column's names

                            int _id = Convert.ToInt32(dataReader["id"]);
                            string fullName = dataReader["fullName"].ToString();
                            string gender = dataReader["gender"].ToString();
                            string email = dataReader["email"].ToString();

                            AnsiConsole.WriteLine("");
                            AnsiConsole.MarkupLine($"[bold darkslategray2]|------------------------------------------------|[/] \n" +
                                $"[bold white]ID:[/] [bold yellow]{_id}[/] \n" +
                                $"[bold white]Full Name:[/] [bold yellow]{fullName}[/] \n" +
                                $"[bold white]Gender:[/] [bold yellow]{gender}[/] \n" +
                                $"[bold white]Email: [/] [bold yellow]{email}[/] \n" +
                                $"[bold darkslategray2]|------------------------------------------------|[/]");

                            table.AddRow(_id.ToString(), fullName, gender, email);
                            table.Border(TableBorder.Square);
                            AnsiConsole.Write(table);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"No record was found with the id: '{id}'");
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                AnsiConsole.Clear();
                var exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"[bold red1] Something went wrong. \n Error Description: {exceptionType.Name} {exception.Message}[/]");
            }
            finally
            {
                sqlConnection.Close();
                AnsiConsole.MarkupLine("[bold green1]Database connection closed successfully.[/]");
            }
        }

        public void GetAllRecords()
        {
            try
            {
                table.AddColumns("[bold orange1]ID[/]", "[bold orange1]Full Name[/]", "[bold orange1]Gender[/]", "[bold orange1]Email[/]").Centered();
                AnsiConsole.Clear();
                sqlConnection.Open();
                AnsiConsole.MarkupLine("[bold green1]Database connection established successfully.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine(" [bold white] Console App - Get All Records[/] \n");

                string selectFromQuery = "SELECT * FROM studentTest";
                SqlCommand selectFromCommand = new SqlCommand(selectFromQuery, sqlConnection);

                using (SqlDataReader dataReader = selectFromCommand.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int id = Convert.ToInt32(dataReader["id"]);
                            string fullName = dataReader["fullName"].ToString();
                            string gender = dataReader["gender"].ToString();
                            string email = dataReader["email"].ToString();

                            AnsiConsole.MarkupLine($"[bold darkslategray2]|--------------------------------------|[/] \n" +
                                $"[bold white]ID:[/] [bold yellow]{id}[/] \n" +
                                $"[bold white]Full Name: [/][bold yellow]{fullName}[/] \n" +
                                $"[bold white]Gender: [/] [bold yellow]{gender}[/] \n" +
                                $"[bold yellow]Email: [/] [bold yellow]{email}[/] \n" +
                                $"[bold darkslategray2]|--------------------------------------|[/]\n");


                            table.AddRow(id.ToString(), fullName, gender, email);
                            table.Border(TableBorder.Square);
                        }
                        AnsiConsole.Write(table);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("No Records was found.");
                    }
                }

            }
            catch (Exception exception)
            {
                AnsiConsole.Clear();
                var exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"[bold red1]Something went wrong. \n Error Description: {exceptionType.Name} {exception.Message}[/]");
            }
            finally
            {
                AnsiConsole.MarkupLine("");
                sqlConnection.Close();
                AnsiConsole.MarkupLine("[bold green1]Database connection closed successfully.[/]");
            }
        }

        public void DeleteRecord()
        {
            try
            {
                AnsiConsole.Clear();
                table.AddColumns("[bold orange1]ID[/]", "[bold orange1]Full Name[/]", "[bold orange1]Gender[/]", "[bold orange1]Email[/]").Centered();
                sqlConnection.Open();
                AnsiConsole.MarkupLine($"[bold green1]Database connection established successfully.[/]");
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine("[bold white]Console App - Delete Record[/] \n");

                var id = AnsiConsole.Prompt
                    (
                        new TextPrompt<int>("[bold white]Insert the id of the record to get deleted:[/] ")
                        .PromptStyle("bold yellow")
                    );

                string selectQuery = $"SELECT id, fullName, gender, email FROM studentTest WHERE id = {id}";
                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                AnsiConsole.WriteLine("");

                using (SqlDataReader dataReader = selectCommand.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int _id = Convert.ToInt32(dataReader["id"]);
                            string fullName = dataReader["fullName"].ToString();
                            string gender = dataReader["gender"].ToString();
                            string email = dataReader["email"].ToString();

                            AnsiConsole.MarkupLine($"[bold darkslategray2]|------------------------------------|[/]\n" +
                                $"[bold white]ID:[/] [bold yellow]{id}[/]\n" +
                                $"[bold white]Full Name:[/] [bold yellow]{fullName}[/] \n" +
                                $"[bold white]Gender:[/] [bold yellow]{gender}[/]\n" +
                                $"[bold white]Email:[/] [bold yellow]{email}[/]\n");

                            table.AddRow(_id.ToString(), fullName, gender, email);
                            AnsiConsole.Write(table);
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"No record was found with the id: '{id}'");
                    }
                }

                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine("[bold orange1]Are you sure you want to delete this record?[/]");
                var yesNo = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<string>()
                        .PageSize(3)
                        .HighlightStyle("cyan1")
                        .AddChoices(["Yes", "No", "Go Back", "Exit Program"])
                    );
                if (yesNo == "Yes")
                {
                    string deleteQuery = $"DELETE FROM studentTest WHERE id = {id}";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);
                    deleteCommand.ExecuteNonQuery();
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("The record was deleted succesfully.");
                }

                AnsiConsole.WriteLine("");


            }
            catch (Exception exception)
            {
                AnsiConsole.Clear();
                var exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"[bold red1]Something went wrong. \n Error Description: {exceptionType.Name} {exception.Message}[/]");
            }
            finally
            {
                AnsiConsole.MarkupLine("");
                sqlConnection.Close();
                AnsiConsole.MarkupLine("[bold green1]Database connection closed successfully.[/]");
            }
        }

        public void UpdateRecord()
        {
            try
            {
                AnsiConsole.Clear();
                sqlConnection.Open();
                table.AddColumns("[bold orange1]ID[/]", "[bold orange1]Full Name[/]", "[bold orange1]Gender[/]", "[bold orange1]Email[/]").Centered();
                AnsiConsole.MarkupLine("[bold green1]Database connection established succesfully.[/]");

                AnsiConsole.WriteLine("\n");
                AnsiConsole.MarkupLine("[bold white]Console App - Update Record[/] \n");


                int id = AnsiConsole.Prompt
                    (
                        new TextPrompt<int>("[bold white]Insert the id of the record to get update: [/]")
                        .PromptStyle("bold yellow")
                    );

                string selectQuery = $"SELECT id, fullName, gender, email FROM studentTest WHERE id = {id}";
                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                using (SqlDataReader dataReader = selectCommand.ExecuteReader())
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int _id = Convert.ToInt32(dataReader["id"]);
                            string fullName = dataReader["fullName"].ToString();
                            string gender = dataReader["gender"].ToString();
                            string email = dataReader["email"].ToString();

                            AnsiConsole.MarkupLine($"[bold darkslategray2]|---------------------------------|[/] \n" +
                                $"[bold white]ID:[/] [bold yellow]{_id}[/] \n" +
                                $"[bold white]Full Name: [/][bold yellow]{fullName}[/] \n" +
                                $"[bold white]Gender: [/][bold yellow]{gender}[/]\n" +
                                $"[bold white]Email:[/] [bold yellow]{email}[/]\n" +
                                $"[bold darkslategray2]|---------------------------------|[/]\n");

                            table.AddRow(_id.ToString(), fullName, gender, email);
                            AnsiConsole.Write(table);
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold yellow]No record was found with the id: '{id}'[/]");
                    }
                }

                AnsiConsole.MarkupLine(" [bold white]Which information would like to update?[/] \n");

                var choice = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<string>()
                        .HighlightStyle("aqua")
                        .PageSize(6)
                        .AddChoices(["Full Name", "Gender", "Email", "All of them", "Go back", "Exit Program"])
                    );

                if (choice == "All of them")
                {
                    AnsiConsole.Clear();

                    string fullName = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Full Name: ")
                            .PromptStyle("bold yellow")
                        );
                    string gender = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Gender: ")
                            .PromptStyle("bold yellow")
                        );
                    string email = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Email: ")
                            .PromptStyle("bold yellow")
                        );

                    string updateQuery = $"UPDATE studentTest SET fullName = '{fullName}', gender = '{gender}', email = '{email}' WHERE id = {id}";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                    updateCommand.ExecuteNonQuery();
                    AnsiConsole.WriteLine("\n");
                    AnsiConsole.MarkupLine("[bold green1]The record was updated succesfully[/]");
                }
                else if (choice == "Full Name")
                {
                    string fullName = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Full Name: ")
                            .PromptStyle("bold yellow")
                        );
                    string updateQuery = $"UPDATE studentTest SET fullName = '{fullName}' WHERE id = {id}";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                    updateCommand.ExecuteNonQuery();
                    AnsiConsole.WriteLine("\n");
                    AnsiConsole.MarkupLine("[bold green1]The record was updated succesfully[/]");
                }
                else if (choice == "Gender")
                {
                    string gender = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Gender: ")
                            .PromptStyle("bold yellow")
                        );
                    string updateQuery = $"UPDATE studentTest SET gender = '{gender}' WHERE id = {id}";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                    updateCommand.ExecuteNonQuery();
                    AnsiConsole.WriteLine("\n");
                    AnsiConsole.MarkupLine("[bold green1]The record was updated succesfully[/]");
                }
                else if (choice == "Email")
                {
                    string email = AnsiConsole.Prompt
                        (
                            new TextPrompt<string>("Email: ")
                            .PromptStyle("bold yellow")
                        );
                    string updateQuery = $"UPDATE studentTest SET email = '{email}' WHERE id = {id}";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection);
                    updateCommand.ExecuteNonQuery();
                    AnsiConsole.WriteLine("\n");
                    AnsiConsole.MarkupLine("[bold green1]The record was updated succesfully[/]");
                }

            }
            catch (Exception exception)
            {
                AnsiConsole.WriteLine("\n");
                var exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"Something went wrong. \n Error Description: {exceptionType.Name} {exception.Message}");
            }
            finally
            {
                AnsiConsole.WriteLine("\n");
                sqlConnection.Close();
                AnsiConsole.MarkupLine("[bold green1]Database connection closed succesfully.[/]");
            }
        }

        public void MainMenu()
        {
            try
            {
                AnsiConsole.MarkupLine(" [bold white]Welcome to Console App - CRUD 1![/] \n");
                AnsiConsole.MarkupLine("What would you like to do? \n");
                var choice = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<string>()
                        .HighlightStyle("cyan1")
                        .PageSize(6)
                        .AddChoices(new[]
                        {
                        "Add Record",
                        "Get a Record",
                        "Get All Records",
                        "Update Record",
                        "Delete Record",
                        "Exit Program"
                        })
                    );
                if (choice == "Add Record") { InsertData(); }
                else if (choice == "Get a Record") { GetRecord(); }
                else if (choice == "Get All Records") { GetAllRecords(); }
                else if (choice == "Delete Record") { DeleteRecord(); }
                else if (choice == "Update Record") { UpdateRecord(); }
                else { Environment.Exit(1); }

            }
            catch (Exception exception)
            {
                AnsiConsole.Clear();
                var exceptionType = exception.GetType();
                AnsiConsole.MarkupLine($"[bold red1] Something went wrong. \n Error Description {exceptionType.Name} {exception.Message}[/]");
            }

        }
    }
}