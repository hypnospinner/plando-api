using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Plando.Commands.Orders;
using Plando.DTOs;
using Plando.Models.Orders;
using Plando.Models.Users;
using Plando.Queries.Orders;
using Plando.Utils;

namespace Plando.Router
{
    internal static class OrdersRouter
    {
        public static IDispatcherEndpointsBuilder AddOrdersRouter(this IDispatcherEndpointsBuilder endpoints)
            => endpoints
                .Post<CreateOrder>(
                    path: "order/create",
                    auth: true)
                .Post<CancelOrder>(
                    path: "order/cancel",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ClientId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Client.ToString())
                .Post<PutOrderInProgress>(
                    path: "order/progress",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ManagerId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Manager.ToString())
                .Post<FinishOrder>(
                    path: "order/finish",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ManagerId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Manager.ToString())
                .Post<PassOrder>(
                    path: "order/pass",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ClientId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Client.ToString())
                .Post<AddServiceToOrder>(
                    path: "order/service/add",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ClientId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Client.ToString())
                .Post<RemoveServiceFromOrder>(
                    path: "order/service/remove",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ClientId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Client.ToString())
                .Post<CompleteServiceInOrder>(
                    path: "order/service/complete",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.ManagerId = id;

                        return Task.CompletedTask;
                    },
                    auth: true,
                    roles: UserRole.Manager.ToString())
                .Get<GetOrder, Order>(
                    path: "order/{orderId}",
                    beforeDispatch: (command, context) =>
                    {
                        var id = context.GetUserId();
                        command.UserId = id;

                        return Task.CompletedTask;
                    },
                    auth: true);
    }
}