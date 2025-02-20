using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    public class SaleData
    {
        public DateTime Date { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    static void Main(string[] args)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "SalesData.txt");
        var sales = ReadSalesData(filePath);
        decimal totalSales = 0;
        foreach (var sale in sales)
        {
            totalSales += sale.TotalPrice;
        }
        Console.WriteLine($"Total Sales of the Store: {totalSales:C2}");

        var monthSales = new Dictionary<string, decimal>();
        var monthItems = new Dictionary<string, Dictionary<string, int>>();
        var monthRevenue = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var sale in sales)
        {
            string monthYear = sale.Date.ToString("yyyy-MM");
            if (!monthSales.ContainsKey(monthYear))
            {
                monthSales[monthYear] = 0;
                monthItems[monthYear] = new Dictionary<string, int>();
                monthRevenue[monthYear] = new Dictionary<string, decimal>();
            }

            monthSales[monthYear] += sale.TotalPrice;

            if (!monthItems[monthYear].ContainsKey(sale.SKU))
            {
                monthItems[monthYear][sale.SKU] = 0;
            }
            monthItems[monthYear][sale.SKU] += sale.Quantity;

            if (!monthRevenue[monthYear].ContainsKey(sale.SKU))
            {
                monthRevenue[monthYear][sale.SKU] = 0;
            }
            monthRevenue[monthYear][sale.SKU] += sale.TotalPrice;
        }

        foreach (var month in monthSales)
        {
            Console.WriteLine($"\nSales for {month.Key}: {month.Value:C2}");
        }

        foreach (var month in monthItems)
        {
            string monthKey = month.Key;
            string mostPopularItem = null;
            int mostPopularQuantity = 0;

            foreach (var item in month.Value)
            {
                if (item.Value > mostPopularQuantity)
                {
                    mostPopularItem = item.Key;
                    mostPopularQuantity = item.Value;
                }
            }

            string highestRevenueItem = null;
            decimal highestRevenue = 0;

            foreach (var item in monthRevenue[monthKey])
            {
                if (item.Value > highestRevenue)
                {
                    highestRevenueItem = item.Key;
                    highestRevenue = item.Value;
                }
            }

            Console.WriteLine($"\nFor {monthKey}:");
            Console.WriteLine($"Most Popular Item: {mostPopularItem} with {mostPopularQuantity} units sold.");
            Console.WriteLine($"Item with Most Revenue: {highestRevenueItem} with {highestRevenue:C2} revenue.");

            int minOrders = int.MaxValue, maxOrders = int.MinValue, totalOrders = 0, totalMonths = 0;

            foreach (var sale in sales)
            {
                if (sale.SKU == mostPopularItem && sale.Date.ToString("yyyy-MM") == monthKey)
                {
                    totalOrders++;
                    if (sale.Quantity < minOrders) minOrders = sale.Quantity;
                    if (sale.Quantity > maxOrders) maxOrders = sale.Quantity;
                    totalMonths++;
                }
            }

            decimal avgOrders = totalMonths > 0 ? (decimal)totalOrders / totalMonths : 0;
            Console.WriteLine($"Min Orders for {mostPopularItem}: {minOrders}");
            Console.WriteLine($"Max Orders for {mostPopularItem}: {maxOrders}");
            Console.WriteLine($"Average Orders for {mostPopularItem}: {avgOrders:F2}");
        }
    }

    static List<SaleData> ReadSalesData(string filePath)
    {
        var sales = new List<SaleData>();
        var lines = File.ReadAllLines(filePath, Encoding.UTF8);

        foreach (var line in lines)
        {
            var columns = line.Split(',');

            string dateStr = columns[0].Trim('"');
            string sku = columns[1].Trim('"');
            decimal unitPrice = decimal.Parse(columns[2]);
            int quantity = int.Parse(columns[3]);
            decimal totalPrice = decimal.Parse(columns[4]);

            var sale = new SaleData
            {
                Date = DateTime.Parse(dateStr),
                SKU = sku,
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = totalPrice
            };
            sales.Add(sale);
        }

        return sales;
    }

}
