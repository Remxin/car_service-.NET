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

        try
        {
            var order = new ServiceOrderEntity
            {
                VehicleId = request.VehicleId,
                Status = request.Status,
                AssignedMechanicId = request.AssignedMechanicId,
            };

            var createdOrder = _dbContext.ServiceOrders.Add(order);
            await _dbContext.SaveChangesAsync();
            // _eventPublisher.PublishEvent("workshop.service.order.created", createdOrder.Entity);
            
            response.Success = true;
            response.Message = "Success";
            response.ServiceOrder = createdOrder.Entity.ToProto();
        }
        catch(Exception ex)
        {
            response.Success = false;
            response.Message = $"Error: {ex.Message}";    
        }
        
        return response;
    }

    public override async Task<AddVehicleResponse> AddVehicle(AddVehicleRequest request,
        ServerCallContext context)
    {
        var response = new AddVehicleResponse
        {
            Success = false,
            Message = "",
        };

        try
        {
            var entity = new VehicleEntity
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year,
                Vin = request.Vin,
                PhotoUrl = request.CarImageUrl,
                CreatedAt = DateTime.UtcNow,
            };

            var created = _dbContext.Vehicles.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Vehicle added successfully";
            response.Vehicle = VehicleMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error: {ex.Message}";    

        }
        return response;
        
    }
}