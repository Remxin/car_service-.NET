using Grpc.Core;
using WorkshopService;
using Shared.Grpc.Messages;
using WorkshopService.Data;
using WorkshopService.Entities;
using WorkshopService.Mappers;

namespace WorkshopService.Services;

public class WorkshopServiceImplementation(
    Shared.Grpc.Services.AuthService.AuthServiceClient authService,
    AppDbContext dbContext,
    IWorkshopEventPublisher eventPublisher)
    : Shared.Grpc.Services.WorkshopService.WorkshopServiceBase {
    
    private readonly Shared.Grpc.Services.AuthService.AuthServiceClient _authService = authService;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IWorkshopEventPublisher _eventPublisher = eventPublisher;
    
    public override async Task<AddOrderResponse> AddOrder(AddOrderRequest request, ServerCallContext context) {
        var response = new AddOrderResponse {
            Success = false,
            Message = "",
            ServiceOrder = null,
        };

        var valiationResponse = await _authService.VerifyActionAsync(new VerifyActionRequest {
            Token = request.Token,
            Action = "create_service_order",
        });
        if (!valiationResponse.Allowed) {
            response.Message = valiationResponse.Message;
            return response;
        }

        var order = new ServiceOrderEntity {
            VehicleId = request.VehicleId,
            Status = request.Status,
            AssignedMechanicId = request.AssignedMechanicId,
        };
        
        var createdOrder = _dbContext.ServiceOrders.Add(order);
        await _dbContext.SaveChangesAsync();
        _eventPublisher.PublishEvent("workshop.service.order.created", createdOrder.Entity);
        
        response.Success = true;
        response.Message = "Success";
        response.ServiceOrder = createdOrder.Entity.ToProto();
        
        return response;
    }
}