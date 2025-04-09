using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Migrations;

/// <inheritdoc />
public partial class AddPhoneNumberToUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameIndex(
            name: "IX_users_Email",
            table: "users",
            newName: "ix_users_email");

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "users",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(512)",
            oldMaxLength: 512);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "users",
            type: "character varying(256)",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(256)",
            oldMaxLength: 256);

        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "users",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_phone_number",
            table: "users",
            column: "PhoneNumber",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_users_phone_number",
            table: "users");

        migrationBuilder.DropColumn(
            name: "PhoneNumber",
            table: "users");

        migrationBuilder.RenameIndex(
            name: "ix_users_email",
            table: "users",
            newName: "IX_users_Email");

        migrationBuilder.AlterColumn<string>(
            name: "PasswordHash",
            table: "users",
            type: "character varying(512)",
            maxLength: 512,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(512)",
            oldMaxLength: 512,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "users",
            type: "character varying(256)",
            maxLength: 256,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(256)",
            oldMaxLength: 256,
            oldNullable: true);
    }
}