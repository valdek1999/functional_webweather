using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebWeather.Migrations
{
    public partial class InitWeatherDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AirTemperature = table.Column<float>(type: "real", nullable: false),
                    AirHumidity = table.Column<int>(type: "integer", nullable: false),
                    DewPointTemperature = table.Column<float>(type: "real", nullable: false),
                    AtmosphericPressure = table.Column<int>(type: "integer", nullable: false),
                    WindDirection = table.Column<string>(type: "text", nullable: true),
                    WindSpeed = table.Column<int>(type: "integer", nullable: true),
                    Cloudiness = table.Column<int>(type: "integer", nullable: true),
                    LowerCloudinessLimit = table.Column<float>(type: "real", nullable: true),
                    HorizontalVisibility = table.Column<string>(type: "text", nullable: true),
                    WeatherEvent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weather");
        }
    }
}
