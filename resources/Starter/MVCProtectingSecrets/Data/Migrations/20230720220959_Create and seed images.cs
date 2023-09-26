using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCProtectingSecrets.Data.Migrations
{
    public partial class Createandseedimages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageDetails", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ImageDetails",
                columns: new[] { "Id", "AltText", "Description", "FileName" },
                values: new object[] { 1, "Two fawns under a tree in the shade near a firepit", "Two fawns enjoying the shade during the heat of summer", "fawns.jpg" });

            migrationBuilder.InsertData(
                table: "ImageDetails",
                columns: new[] { "Id", "AltText", "Description", "FileName" },
                values: new object[] { 2, "Todd Stashwick portraying Captain Liam Shaw with an expression of pain on his face and the subtitle for the spoken line: I'm just a dipshit from Chicago", "Todd Stashwick gives an emmy worthy performance of PTSD in Star Trek Picard Season three episode four", "chicago.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //roll-forward only
            //migrationBuilder.DropTable(
            //    name: "ImageDetails");
        }
    }
}
