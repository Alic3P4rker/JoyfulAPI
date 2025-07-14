using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JoyfulAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddChatParticipantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_Chats_ChatEntityId",
                table: "UserFriends");

            migrationBuilder.DropIndex(
                name: "IX_UserFriends_ChatEntityId",
                table: "UserFriends");

            migrationBuilder.DropColumn(
                name: "ChatEntityId",
                table: "UserFriends");

            migrationBuilder.CreateTable(
                name: "ChatParticipants",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipants", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatParticipants_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_UserId",
                table: "ChatParticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatParticipants");

            migrationBuilder.AddColumn<int>(
                name: "ChatEntityId",
                table: "UserFriends",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_ChatEntityId",
                table: "UserFriends",
                column: "ChatEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_Chats_ChatEntityId",
                table: "UserFriends",
                column: "ChatEntityId",
                principalTable: "Chats",
                principalColumn: "Id");
        }
    }
}
