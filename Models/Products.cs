using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace acme.Models
{

    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pack")]
        public string Pack { get; set; }

        [JsonProperty("uom")]
        public string Uom { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("department_name")]
        public string DepartmentName { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("date_active")]
        public DateTime? DateActive { get; set; }

        [JsonProperty("date_deactive")]
        public DateTime? DateDeactive { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }
    }

    public class OrderLine
    {
        public int OrderLineId { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("product_id")]
        public Product Product { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price_per_unit")]
        public double PricePerUnit { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }
    public class Order
    {
        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        //[JsonProperty("Order")]
        public List<OrderLine> OrderLine { get; set; }
    }





    /// <summary>
    /// IMPORTED ORDER START
    /// </summary>
    public class ImportedOrder
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string ShippingMethod { get; set; }
        public DateTime RequestedDeliveryDate { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingHandling { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string PO { get; set; }
        public List<ImportedOrderItem> Items { get; set; } = new List<ImportedOrderItem>();

        public ImportedOrder()
        {
            ShippingAddress = new Address();
            BillingAddress = new Address();

        }
    }

    public class ImportedOrderItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string CustomerNumber { get; set; }
        public string DistributionNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitType { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class Address
    {
        public string Id { get; set; }
        public string FullAddress { get; set; }
    }
    /// <summary>
    /// IMPORTED ORDER FINISH
    /// </summary>
}

