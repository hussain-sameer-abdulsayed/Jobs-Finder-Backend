using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MB_Project.Migrations
{
    /// <inheritdoc />
    public partial class postcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_Posts_PostsId",
                table: "PostCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories");

            migrationBuilder.DropIndex(
                name: "IX_PostCategories_PostsId",
                table: "PostCategories");

            migrationBuilder.DropColumn(
                name: "PostsId",
                table: "PostCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories",
                columns: new[] { "CategoryId", "WorkId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_WorkId",
                table: "PostCategories",
                column: "WorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_Posts_WorkId",
                table: "PostCategories",
                column: "WorkId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategories_Posts_WorkId",
                table: "PostCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories");

            migrationBuilder.DropIndex(
                name: "IX_PostCategories_WorkId",
                table: "PostCategories");

            migrationBuilder.AddColumn<int>(
                name: "PostsId",
                table: "PostCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostCategories",
                table: "PostCategories",
                columns: new[] { "CategoryId", "PostsId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_PostsId",
                table: "PostCategories",
                column: "PostsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategories_Posts_PostsId",
                table: "PostCategories",
                column: "PostsId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
