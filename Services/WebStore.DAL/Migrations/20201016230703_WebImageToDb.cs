using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebStore.DAL.Migrations
{
    public partial class WebImageToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Picture = table.Column<byte[]>(nullable: false),
                    BrandId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebImages_ProductBrands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "ProductBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WebImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebImages_BrandId",
                table: "WebImages",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_WebImages_ProductId",
                table: "WebImages",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebImages");
        }
    }
}
