using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class _0001_InitialSchemaDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                table: "TicketAttachments");

            migrationBuilder.EnsureSchema(
                name: "bug");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Tickets",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "TicketComments",
                newName: "TicketComments",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "TicketAttachments",
                newName: "TicketAttachments",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "Invites",
                newName: "Invites",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "Files",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Companies",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "bug");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject",
                newName: "ApplicationUserProject",
                newSchema: "bug");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadId",
                schema: "bug",
                table: "TicketAttachments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                schema: "bug",
                table: "TicketAttachments",
                column: "UploadId",
                principalSchema: "bug",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                schema: "bug",
                table: "TicketAttachments");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "bug",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "TicketComments",
                schema: "bug",
                newName: "TicketComments");

            migrationBuilder.RenameTable(
                name: "TicketAttachments",
                schema: "bug",
                newName: "TicketAttachments");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "bug",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "Invites",
                schema: "bug",
                newName: "Invites");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "bug",
                newName: "Files");

            migrationBuilder.RenameTable(
                name: "Companies",
                schema: "bug",
                newName: "Companies");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "bug",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "bug",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "bug",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "bug",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "bug",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "bug",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "bug",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "ApplicationUserProject",
                schema: "bug",
                newName: "ApplicationUserProject");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadId",
                table: "TicketAttachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                table: "TicketAttachments",
                column: "UploadId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
