using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sogeti.TechAssessment.Orders.Domain;
using Sogeti.TechAssessment.Orders.Interfaces.Services;
using Sogeti.TechAssessment.Orders.Models;
using Sogeti.TechAssessment.Orders.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderContext>(o => o.UseInMemoryDatabase("Orders"));
builder.Services.AddTransient<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/orders/{customerId}", async (Guid customerId, IOrderService orderService) => await orderService.GetForCustomerAsync(customerId))
    .WithName("ListOrdersByCustomer")
    .WithOpenApi();

app.MapPost("/order", async (CreateOrderModel createOrderModel, IOrderService orderService) =>
    {
        var newOrder = await orderService.CreateAsync(createOrderModel);
        return newOrder is null ? Results.BadRequest() : Results.Created();
    })
    .WithName("CreateOrder")
    .WithOpenApi();

app.MapPut("/order", async (UpdateOrderModel updateOrderModel, IOrderService orderService) =>
    {
        var updatedOrder = await orderService.UpdateAsync(updateOrderModel);
        return updatedOrder is null ? Results.BadRequest() : Results.NoContent();
    })
    .WithName("UpdateOrder")
    .WithOpenApi();

app.MapPost("/orders/cancel", async (CancelOrderModel cancelOrderModel, IOrderService orderService) =>
    {
        var cancelledOrder = await orderService.CancelAsync(cancelOrderModel);
        return cancelledOrder is null ? Results.BadRequest() : Results.NoContent();
    })
    .WithName("CancelOrder")
    .WithOpenApi();

app.Run();