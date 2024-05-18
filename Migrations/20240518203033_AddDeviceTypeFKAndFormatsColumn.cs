using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alexandria.api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceTypeFKAndFormatsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeviceTypeId",
                table: "KnownDevices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Formats",
                table: "KnownDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Formats",
                table: "DeviceTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnownDevices_DeviceTypeId",
                table: "KnownDevices",
                column: "DeviceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnownDevices_DeviceTypes_DeviceTypeId",
                table: "KnownDevices",
                column: "DeviceTypeId",
                principalTable: "DeviceTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnownDevices_DeviceTypes_DeviceTypeId",
                table: "KnownDevices");

            migrationBuilder.DropIndex(
                name: "IX_KnownDevices_DeviceTypeId",
                table: "KnownDevices");

            migrationBuilder.DropColumn(
                name: "DeviceTypeId",
                table: "KnownDevices");

            migrationBuilder.DropColumn(
                name: "Formats",
                table: "KnownDevices");

            migrationBuilder.DropColumn(
                name: "Formats",
                table: "DeviceTypes");
        }
    }
}
