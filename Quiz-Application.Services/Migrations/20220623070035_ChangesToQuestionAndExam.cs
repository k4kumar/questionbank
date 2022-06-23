using Microsoft.EntityFrameworkCore.Migrations;

namespace Quiz_Application.Services.Migrations
{
    public partial class ChangesToQuestionAndExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PDFLink",
                table: "Exam",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentationLink",
                table: "Exam",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeTutorialLink",
                table: "Exam",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "PDFLink",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "PresentationLink",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "YoutubeTutorialLink",
                table: "Exam");

            migrationBuilder.CreateTable(
                name: "QuizAttempt",
                columns: table => new
                {
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sl_No = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "QuizReport",
                columns: table => new
                {
                    CandidateID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });
        }
    }
}
