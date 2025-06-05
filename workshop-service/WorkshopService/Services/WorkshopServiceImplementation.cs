using Grpc.Core;
using WorkshopService;
using Shared.Grpc.Messages;
using WorkshopService.Data;
using WorkshopService.Entities;
using WorkshopService.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace WorkshopService.Services;

public class WorkshopServiceImplementation(
    Shared.Grpc.Services.AuthService.AuthServiceClient authService,
    AppDbContext dbContext,
    IWorkshopEventPublisher eventPublisher,
    ILogger<WorkshopServiceImplementation> logger
    )
    : Shared.Grpc.Services.WorkshopService.WorkshopServiceBase {
    
    private readonly Shared.Grpc.Services.AuthService.AuthServiceClient _authService = authService;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IWorkshopEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<WorkshopServiceImplementation> _logger = logger;
    
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
            
            _logger.LogInformation("ServiceOrder created with Id={OrderId}", createdOrder.Entity.Id);
            
            response.Success = true;
            response.Message = "Success";
            response.ServiceOrder = createdOrder.Entity.ToProto();
        }
        catch(Exception ex)
        {
            response.Message = $"Error: {ex.Message}";    
            _logger.LogError(ex, "Failed to add service order");
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
            
            _logger.LogInformation("Vehicle created with Id={OrderId}", created.Entity.Id);


            response.Success = true;
            response.Message = "Vehicle added successfully";
            response.Vehicle = VehicleMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}"; 
            _logger.LogError(ex, "Failed to add vehicle");
        }
        return response;
        
    }

    public override async Task<AddVehiclePartResponse> AddVehiclePart(AddVehiclePartRequest request, ServerCallContext context)
    {
        var response = new AddVehiclePartResponse
        {
            Success = false,
            Message = ""
        };
        try
        {
            var entity = new VehiclePartEntity
            {
                Name = request.Name,
                PartNumber = request.PartNumber,
                Description = request.Description,
                Price = request.Price,
                AvailableQuantity = request.AvailableQuantity
            };

            var created = _dbContext.VehicleParts.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Vehicle part added successfully";
            response.VehiclePart = VehiclePartMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add vehicle part");
        }
        
        return response;
    }
    
    public override async Task<AddServiceTaskResponse> AddServiceTask(AddServiceTaskRequest request, ServerCallContext context)
    {
        var response = new AddServiceTaskResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
            var entity = new ServiceTaskEntity
            {
                OrderId     = request.OrderId,
                Description = request.Description,
                LaborCost   = request.LaborCost
            };

            var created = _dbContext.ServiceTasks.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Service task added successfully";
            response.ServiceTask = ServiceTaskMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service task");
        }

        return response;
    }
    
    public override async Task<AddServicePartResponse> AddServicePart(AddServicePartRequest request, ServerCallContext context)
    {
        var response = new AddServicePartResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
            var entity = new ServicePartEntity
            {
                OrderId         = request.OrderId,
                VehiclePartId   = request.VehiclePartId,
                Quantity        = request.Quantity
            };

            var created = _dbContext.ServiceParts.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Service part added successfully";
            response.ServicePart = ServicePartMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service part");
        }

        return response;
    }
    
    public override async Task<AddServiceCommentResponse> AddServiceComment(AddServiceCommentRequest request, ServerCallContext context)
    {
        var response = new AddServiceCommentResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
            var entity = new ServiceCommentEntity
            {
                OrderId     = request.OrderId,
                Content     = request.Content,
                CreatedAt   = DateTime.UtcNow
            };

            var created = _dbContext.ServiceComments.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Service comment added successfully";
            response.ServiceComment = ServiceCommentMapper.ToProto(created.Entity);
        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service comment");
        }

        return response;
    }
    
    



}