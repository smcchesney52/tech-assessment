using Microsoft.EntityFrameworkCore;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Domain.Enums;
using Sogeti.TechAssessment.Orders.Models;
using Sogeti.TechAssessment.Orders.Services;

namespace Sogeti.TechAssessment.Orders.UnitTests
{
    public class OrderServiceTests
    {
        private readonly OrderService _systemUnderTest;
        private readonly Guid _johnSmithId = Guid.Parse("f8919bcb-8cd8-4afc-ba31-444790484e0e"); 
        private readonly Guid _janeDoeId = Guid.Parse("2e046c10-06de-498c-b8c0-70d2f5ab8b54"); 
        private readonly Guid _princeId = Guid.Parse("6324a19b-89ab-467e-82a3-4aeb99bb5a9b"); 
        private readonly Guid _productId = Guid.NewGuid();
        private readonly DateTimeOffset _orderDate = DateTimeOffset.Now;
        private const string AddUser = "TEST";
        private const int Quantity = 2;
        private const decimal UnitPrice = 2.5m;

        public OrderServiceTests()
        {
            var logger = new TestLogger<OrderService>();
            _systemUnderTest = new OrderService(logger, CreateTestContext());
        }

        [Fact]
        public async Task CreateAsync_WithNullItems_ReturnsNull()
        {
            var model = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items = null
            };

            var newOrder = await _systemUnderTest.CreateAsync(model);
            
            Assert.Null(newOrder);
        }

        [Fact]
        public async Task CreateAsync_WithItems_ReturnsOrder()
        {
            var model = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items =
                [
                    new CreateOrderOrderItemModel { ProductId = _productId, UnitPrice = UnitPrice, Quantity = Quantity }
                ]
            };

            var expected = new Order(_princeId, _orderDate, AddUser);
            expected.AddItem(_productId, Quantity, UnitPrice, AddUser);

            var actual = await _systemUnderTest.CreateAsync(model);
            
            Assert.NotNull(actual);
            Assert.Equivalent(expected.CustomerId, actual.CustomerId);
            Assert.Equivalent(expected.OrderDate, actual.OrderDate);
            Assert.Equivalent(expected.AddUser, actual.AddUser);
            Assert.Equivalent(expected.OrderTotal, actual.OrderTotal);

            var expectedItem = expected.Items.First();
            var actualItem = actual.Items.First();
            Assert.Equivalent(expectedItem.ProductId, actualItem.ProductId);
            Assert.Equivalent(expectedItem.UnitPrice, actualItem.UnitPrice);
            Assert.Equivalent(expectedItem.Quantity, actualItem.Quantity);
            Assert.Equivalent(expectedItem.AddUser, actualItem.AddUser);
        }

        [Fact]
        public async Task GetForCustomer_WithNoOrders_ReturnsEmptyList()
        {
            var orders = await _systemUnderTest.GetForCustomerAsync(_princeId);
            
            Assert.Empty(orders);
        }

        [Fact]
        public async Task GetForCustomer_WithOneOrder_ReturnsList()
        {
            var model = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items =
                [
                    new CreateOrderOrderItemModel { ProductId = _productId, UnitPrice = UnitPrice, Quantity = Quantity }
                ]
            };

            var newOrder = await _systemUnderTest.CreateAsync(model);

            var expected = new[]
            {
                new ListOrderModel
                {
                    Id = newOrder!.Id, OrderDate = newOrder.OrderDate, CurrentStatus = newOrder.Status.ToString(),
                    CustomerName = "Prince", OrderTotal = newOrder.OrderTotal
                }
            };

            var actual = await _systemUnderTest.GetForCustomerAsync(_princeId);
            
            Assert.Single(actual);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public async Task Update_NoOrderToUpdate_ReturnsNull()
        {
            var model = new UpdateOrderModel
            {
                OrderId = Guid.NewGuid(),
                UpdateUser = AddUser,
                Items =
                [
                    new UpdateOrderOrderItemModel
                        { OrderItemId = Guid.NewGuid(), Quantity = Quantity, UnitPrice = UnitPrice }
                ]
            };

            var actual = await _systemUnderTest.UpdateAsync(model);
            
            Assert.Null(actual);
        }

        [Fact]
        public async Task Update_OrderToUpdate_WithUpdateToItem_ReturnsUpdatedOrder()
        {
            var createModel = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items =
                [
                    new CreateOrderOrderItemModel { ProductId = _productId, UnitPrice = UnitPrice, Quantity = Quantity }
                ]
            };

            var newOrder = await _systemUnderTest.CreateAsync(createModel);

            var newQuantity = Quantity * 2;
            var updateModel = new UpdateOrderModel
            {
                OrderId = newOrder!.Id,
                UpdateUser = AddUser,
                Items =
                [
                    new UpdateOrderOrderItemModel
                        { OrderItemId = newOrder.Items.First().Id, Quantity = newQuantity, UnitPrice = UnitPrice }
                ]
            };

            var actual = await _systemUnderTest.UpdateAsync(updateModel);
            
            Assert.NotNull(actual);
            Assert.Equal(newQuantity, actual.Items.First().Quantity);
        }

        [Fact]
        public async Task Update_OrderToUpdate_WithDeleteOfItem_ReturnsUpdatedOrder()
        {
            var createModel = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items =
                [
                    new CreateOrderOrderItemModel { ProductId = _productId, UnitPrice = UnitPrice, Quantity = Quantity }
                ]
            };

            var newOrder = await _systemUnderTest.CreateAsync(createModel);

            var newQuantity = Quantity * 2;
            var updateModel = new UpdateOrderModel
            {
                OrderId = newOrder!.Id,
                UpdateUser = AddUser,
                Items =
                [
                    new UpdateOrderOrderItemModel
                        { OrderItemId = newOrder.Items.First().Id, Delete = true }
                ]
            };

            var actual = await _systemUnderTest.UpdateAsync(updateModel);
            
            Assert.NotNull(actual);
            Assert.Empty(actual.Items);
        }

        [Fact]
        public async Task CancelOrder_NoOrder_ReturnsNull()
        {
            var model = new CancelOrderModel { OrderId = Guid.NewGuid(), CancelUser = AddUser };

            var actual = await _systemUnderTest.CancelAsync(model);
            
            Assert.Null(actual);
        }

        [Fact]
        public async Task CancelOrder_OrderExists_ReturnsCanceledOrder()
        {
            var createModel = new CreateOrderModel
            {
                CustomerId = _princeId,
                OrderDate = _orderDate,
                AddUser = AddUser,
                Items =
                [
                    new CreateOrderOrderItemModel { ProductId = _productId, UnitPrice = UnitPrice, Quantity = Quantity }
                ]
            };

            var newOrder = await _systemUnderTest.CreateAsync(createModel);
            
            var cancelModel = new CancelOrderModel { OrderId = newOrder!.Id, CancelUser = AddUser };

            var actual = await _systemUnderTest.CancelAsync(cancelModel);
            
            Assert.NotNull(actual);
            Assert.Equal(OrderStatus.Cancelled, actual.Status);
        }

        private OrderContext CreateTestContext()
        {
            var options = new DbContextOptionsBuilder<OrderContext>();
            options.UseInMemoryDatabase("Orders");
            var context = new OrderContext(options.Options);

            if (context.Orders.Any())
            {
                foreach (var order in context.Orders)
                {
                    context.Orders.Remove(order);
                }
                context.SaveChanges();
            }

            if (!context.Customers.Any())
            {
                context.Customers.AddRange(
                    new Customer(_johnSmithId, "John", "Smith", "SYSTEM"),
                    new Customer(_janeDoeId, "Jane", "Doe", "SYSTEM"),
                    new Customer(_princeId, "Prince", "SYSTEM")
                );
                context.SaveChanges();
            }

            return context;
        }
    }
}