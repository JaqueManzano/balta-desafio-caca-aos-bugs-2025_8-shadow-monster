using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tabelas
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    BirthDate TEXT NOT NULL
                );
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS Products (
                    Id TEXT PRIMARY KEY,
                    Title TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Slug TEXT NOT NULL,
                    Price NUMERIC NOT NULL
                );
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id TEXT PRIMARY KEY,
                    CustomerId TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
                );
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS OrderLines (
                    Id TEXT PRIMARY KEY,
                    OrderId TEXT NOT NULL,
                    ProductId TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Total NUMERIC NOT NULL,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
                );
            ");

            // View do report
            migrationBuilder.Sql(@"
               CREATE VIEW IF NOT EXISTS vw_report_best_customers AS
                SELECT 
                    c.Name AS CustomerName,
                    c.Email AS CustomerEmail,
                    COUNT(o.Id) AS TotalOrders,
                    SUM(CAST(REPLACE(ol.Total, '.', '') AS REAL)) AS SpentAmount
                FROM Customers c
                INNER JOIN Orders o ON c.Id = o.CustomerId
                INNER JOIN OrderLines ol ON o.Id = ol.OrderId
                GROUP BY c.Id, c.Name, c.Email
                ORDER BY SpentAmoOrdersunt DESC;
            ");

            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS vw_report_revenue_by_period AS
            SELECT
                 strftime('%Y', o.CreatedAt) AS Year,
                 strftime('%m', o.CreatedAt) AS Month,
                 COUNT(o.Id) AS TotalOrders,
                 SUM(CAST(REPLACE(ol.Total, '.', '') AS REAL)) AS TotalRevenue
             FROM Orders o
             INNER JOIN OrderLines ol
                 ON o.Id = ol.OrderId
             GROUP BY
                 strftime('%Y', o.CreatedAt),
                 strftime('%m', o.CreatedAt)
             ORDER BY
                Year, Month;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_report_best_customers;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS OrderLines;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Orders;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Products;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Customers;");
        }
    }
}
