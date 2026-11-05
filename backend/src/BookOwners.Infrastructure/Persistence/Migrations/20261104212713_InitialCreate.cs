using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookOwners.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookOwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_BookOwners_BookOwnerId",
                        column: x => x.BookOwnerId,
                        principalTable: "BookOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BookOwners",
                columns: new[] { "Id", "Age", "Name" },
                values: new object[,]
                {
                    { 1, 23, "Jane" },
                    { 2, 14, "Charlotte" },
                    { 3, 25, "Max" },
                    { 4, 15, "William" },
                    { 5, 17, "Charles" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "BookOwnerId", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 1, "Hamlet", "Hardcover" },
                    { 2, 1, "Wuthering Heights", "Paperback" },
                    { 3, 2, "Hamlet", "Paperback" },
                    { 4, 3, "React: The Ultimate Guide", "Hardcover" },
                    { 5, 3, "Gulliver's Travels", "Hardcover" },
                    { 6, 3, "Jane Eyre", "Paperback" },
                    { 7, 3, "Great Expectations", "Hardcover" },
                    { 8, 4, "Great Expectations", "Hardcover" },
                    { 9, 5, "Little Red Riding Hood", "Hardcover" },
                    { 10, 5, "The Hobbit", "Ebook" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookOwnerId",
                table: "Books",
                column: "BookOwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "BookOwners");
        }
    }
}
