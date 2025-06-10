using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WorkshopService;
using Shared.Grpc.Messages;
using WorkshopService.Data;
using WorkshopService.Entities;
using WorkshopService.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Models;


namespace WorkshopService.Services;

public class WorkshopServiceImplementation(
    Shared.Grpc.Services.AuthService.AuthServiceClient authService,
    AppDbContext dbContext,
    IWorkshopEventPublisher eventPublisher,
    ILogger<WorkshopServiceImplementation> logger,
    Shared.Grpc.Services.AuthService.AuthServiceClient authClient
)
    : Shared.Grpc.Services.WorkshopService.WorkshopServiceBase {

    private readonly Shared.Grpc.Services.AuthService.AuthServiceClient _authService = authService;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IWorkshopEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<WorkshopServiceImplementation> _logger = logger;
    private readonly Shared.Grpc.Services.AuthService.AuthServiceClient _authClient = authClient;

    public override async Task<AddOrderResponse> AddOrder(AddOrderRequest request,
        ServerCallContext context) {
        var response = new AddOrderResponse {
            Success = false,
            Message = "",
            ServiceOrder = null,
        };

        try {

            var order = new ServiceOrderEntity
            {
                VehicleId = request.VehicleId,
                Status = request.Status,
                AssignedMechanicId = request.AssignedMechanicId,
            };

            var createdOrder = _dbContext.ServiceOrders.Add(order);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(order).Reference(o => o.Vehicle).LoadAsync();

            _logger.LogInformation("ServiceOrder created with Id={OrderId}",
                createdOrder.Entity.Id);

            response.Success = true;
            response.Message = "Success";
            response.ServiceOrder = createdOrder.Entity.ToProto();
            _eventPublisher.PublishEvent("workshop.service.order.created", new {
                Id = order.Id,
                Vehicle = order.Vehicle,
                Status = order.Status,
                UserId = order.AssignedMechanicId,
                CreatedAt = order.CreatedAt,
            });
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service order");
        }

        return response;
    }

    public override async Task<AddVehicleResponse> AddVehicle(AddVehicleRequest request,
        ServerCallContext context) {
        var response = new AddVehicleResponse {
            Success = false,
            Message = "",
        };

        try {
            var entity = new VehicleEntity {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year,
                Vin = request.Vin,
                PhotoUrl = request.CarImageUrl,
            };

            var created = _dbContext.Vehicles.Add(entity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Vehicle created with Id={OrderId}", created.Entity.Id);


            response.Success = true;
            response.Message = "Vehicle added successfully";
            response.Vehicle = VehicleMapper.ToProto(created.Entity);
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add vehicle");
        }

        return response;

    }

    public override async Task<AddVehiclePartResponse> AddVehiclePart(AddVehiclePartRequest request,
        ServerCallContext context) {
        var response = new AddVehiclePartResponse {
            Success = false,
            Message = ""
        };
        try {
            var entity = new VehiclePartEntity {
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
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add vehicle part");
        }

        return response;
    }

    public override async Task<AddServiceTaskResponse> AddServiceTask(AddServiceTaskRequest request,
        ServerCallContext context) {
        var response = new AddServiceTaskResponse {
            Success = false,
            Message = ""
        };

        try {
            var entity = new ServiceTaskEntity {
                OrderId = request.OrderId,
                Description = request.Description,
                LaborCost = request.LaborCost
            };

            var created = _dbContext.ServiceTasks.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Service task added successfully";
            response.ServiceTask = ServiceTaskMapper.ToProto(created.Entity);
            _eventPublisher.PublishEvent("workshop.service.task.added", new {
                Id = created.Entity.Id,
                OrderId = created.Entity.OrderId,
                Description = created.Entity.Description,
                LaborCost = created.Entity.LaborCost,
                CreatedAt = created.Entity.CreatedAt,
            });
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service task");
        }

        return response;
    }

    public override async Task<AddServicePartResponse> AddServicePart(AddServicePartRequest request,
        ServerCallContext context) {
        var response = new AddServicePartResponse {
            Success = false,
            Message = ""
        };

        try {
            var entity = new ServicePartEntity {
                OrderId = request.OrderId,
                VehiclePartId = request.VehiclePartId,
                Quantity = request.Quantity
            };

            var created = _dbContext.ServiceParts.Add(entity);
            await _dbContext.SaveChangesAsync();
            var vehiclePart = await _dbContext.VehicleParts
                .FirstOrDefaultAsync(vp => vp.Id == created.Entity.VehiclePartId);

            response.Success = true;
            response.Message = "Service part added successfully";
            response.ServicePart = ServicePartMapper.ToProto(created.Entity);
            
            _eventPublisher.PublishEvent("workshop.service.part.added", new {
                Id = created.Entity.Id,
                OrderId = created.Entity.OrderId,
                VehiclePart = vehiclePart,
                Quantity = created.Entity.Quantity,
            });
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service part");
        }

        return response;
    }

    public override async Task<AddServiceCommentResponse> AddServiceComment(
        AddServiceCommentRequest request, ServerCallContext context) {
        var response = new AddServiceCommentResponse {
            Success = false,
            Message = ""
        };

        try {
            var entity = new ServiceCommentEntity {
                OrderId = request.OrderId,
                Content = request.Content,
            };

            var created = _dbContext.ServiceComments.Add(entity);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Message = "Service comment added successfully";
            response.ServiceComment = ServiceCommentMapper.ToProto(created.Entity);
            
            _eventPublisher.PublishEvent("workshop.service.comment.added", new {
                Id = created.Entity.Id,
                OrderId = created.Entity.OrderId,
                Content = request.Content,
                CreateAt = created.Entity.CreatedAt,
            });
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to add service comment");
        }

        return response;
    }

    public override async Task<GetVehicleResponse> GetVehicle(GetVehicleRequest request,
        ServerCallContext context) {
        var response = new GetVehicleResponse {
            Success = false,
            Message = "",
        };

        try {
            var entity = await _dbContext.Vehicles.FindAsync(request.VehicleId);

            if (entity == null) {
                response.Message = "Vehicle not found";
                _logger.LogWarning("Vehicle with Id={VehicleId} not found", request.VehicleId);
                return response;
            }

            response.Success = true;
            response.Message = "OK";
            response.Vehicle = VehicleMapper.ToProto(entity);

        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to get vehicle");
            return response;
        }

        return response;
    }

    public override async Task<GetVehiclePartResponse> GetVehiclePart(GetVehiclePartRequest request,
        ServerCallContext context) {
        var response = new GetVehiclePartResponse {
            Success = false,
            Message = ""
        };

        try {
            var entity = await _dbContext.VehicleParts.FindAsync(request.VehiclePartId);
            if (entity == null)
            {
                response.Message = $"Vehicle part with ID={request.VehiclePartId} not found";
                return response;
            }

            response.Success = true;
            response.Message = "OK";
            response.VehiclePart = VehiclePartMapper.ToProto(entity);

        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to get vehicle part");
        }

        return response;
    }

    public override async Task<SearchVehiclesResponse> SearchVehicles(SearchVehiclesRequest request,
        ServerCallContext context) {
        var response = new SearchVehiclesResponse {
            Success = false,
            Message = "",
        };

        try {
            var query = _dbContext.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(request.Brand))
                query = query.Where(y => y.Brand.Contains(request.Brand));

            if (!string.IsNullOrEmpty(request.Model))
                query = query.Where(v => v.Model.Contains(request.Model));

            if (request.Year != 0)
                query = query.Where(v => v.Year == request.Year);

            if (!string.IsNullOrEmpty(request.Vin))
                query = query.Where(v => v.Vin.Contains(request.Vin));

            var totalCount = await query.CountAsync();

            var entities = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            response.Success = true;
            response.Message = "OK";
            response.TotalCount = totalCount;
            response.Vehicles.AddRange(entities.Select(VehicleMapper.ToProto));


        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to search vehicles");
        }

        return response;

    }

    public override async Task<SearchVehiclePartsResponse> SearchVehicleParts(
        SearchVehiclePartsRequest request, ServerCallContext context) {
        var response = new SearchVehiclePartsResponse {
            Success = false,
            Message = ""
        };

        try {
            var query = _dbContext.VehicleParts.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(p => p.Name.Contains(request.Name));

            if (!string.IsNullOrEmpty(request.PartNumber))
                query = query.Where(p => p.PartNumber.Contains(request.PartNumber));

            if (!string.IsNullOrEmpty(request.Description))
                query = query.Where(p => p.Description.Contains(request.Description));

            if (request.Price != 0)
                query = query.Where(p => p.Price == request.Price);

            if (request.AvailableQuantity != 0)
                query = query.Where(p => p.AvailableQuantity == request.AvailableQuantity);

            var totalCount = await query.CountAsync();

            var entities = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            response.Success = true;
            response.Message = "OK";
            response.TotalCount = totalCount;
            response.VehiclePart.AddRange(entities.Select(VehiclePartMapper.ToProto));
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to search vehicle parts");
        }

        return response;
    }

    public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context) {
        _logger.LogInformation($"GetOrder called with ID={request.ServiceOrderId}");
        var response = new GetOrderResponse {
            Success = false,
            Message = ""
        };
        try {
            var entity = await _dbContext.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.ServiceTasks)
                .Include(o => o.ServiceParts)
                .Include(o => o.ServiceComments)
                .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId);
        
            if (entity == null) {
                response.Message = $"Order with ID={request.ServiceOrderId} not found";
                return response;
            }

            UserDto? mechanic = null;

            _logger.LogInformation($"Get order called with Mechanic_ID={entity.AssignedMechanicId}");
            if (entity.AssignedMechanicId != null) {
                var getUserResponse = await _authClient.GetUserAsync(new GetUserRequest {
                    UserId = entity.AssignedMechanicId.Value
                });
            
                mechanic = getUserResponse.User;
                if (mechanic == null) {
                    response.Message = $"User with ID={request.ServiceOrderId} not found";
                    return response;
                }
            }
            
            response.Success = true;
            response.Message = "OK";
            response.ServiceCompleteOrder = ServiceCompleteOrderMapper.ToProto(entity, mechanic);
        }
        catch (Exception ex) {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to get order");
        }
        
        return response;
    }

    public override async Task<SearchOrdersResponse> SearchOrders(SearchOrdersRequest request,
        ServerCallContext context)
    {
        var response = new SearchOrdersResponse
        {
            Success = false,
            Message = ""
        };

        var query = _dbContext.ServiceOrders
            .Include(o => o.Vehicle)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.VehicleBrand))
            query = query.Where(o => o.Vehicle.Brand.Contains(request.VehicleBrand));

        if (!string.IsNullOrEmpty(request.VehicleModel))
            query = query.Where(o => o.Vehicle.Model.Contains(request.VehicleModel));

        if (request.VehicleYear != 0)
            query = query.Where(o => o.Vehicle.Year == request.VehicleYear);

        if (!string.IsNullOrEmpty(request.VehicleVin))
            query = query.Where(o => o.Vehicle.Vin.Contains(request.VehicleVin));

        if (request.CreatedAfter != null) {
            var after = request.CreatedAfter.ToDateTime();
            query = query.Where(o => o.CreatedAt >= after);
        }

        if (request.CreatedBefore != null) {
            var before = request.CreatedBefore.ToDateTime();
            query = query.Where(o => o.CreatedAt <= before);
        }

        var totalCount = await query.CountAsync();

        var entities = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        response.Success = true;
        response.Message = "OK";
        response.TotalCount = totalCount;
        response.ServiceOrders.AddRange(entities.Select(ServiceOrderMapper.ToProtoWithVehicle));

        return response;
    }
    
    public override async Task<UpdateVehicleResponse> UpdateVehicle(UpdateVehicleRequest request, ServerCallContext context)
    {
        var vehicle = await _dbContext.Vehicles.FindAsync(request.VehicleId);
        if (vehicle is null)
            return new UpdateVehicleResponse {
                Message = $"Vehicle with id={request.VehicleId} not found",
                Success = false
            };

        if (!string.IsNullOrWhiteSpace(request.Brand)) vehicle.Brand = request.Brand;
        if (!string.IsNullOrWhiteSpace(request.Model)) vehicle.Model = request.Model;
        if (!string.IsNullOrWhiteSpace(request.CarImageUrl)) vehicle.PhotoUrl = request.CarImageUrl;
        if (!string.IsNullOrWhiteSpace(request.Vin)) vehicle.Vin = request.Vin;
        if (request.Year > 0) vehicle.Year = request.Year;

        await _dbContext.SaveChangesAsync();
        return new UpdateVehicleResponse {
            Message = "Ok",
            Success = true,
            Vehicle = vehicle.ToProto()
        };
    }

    public override async Task<UpdateVehiclePartResponse> UpdateVehiclePart(UpdateVehiclePartRequest request, ServerCallContext context)
    {
        var part = await _dbContext.VehicleParts.FindAsync(request.VehiclePartId);
        if (part is null)
            return new UpdateVehiclePartResponse {
               Message = $"Part with id={request.VehiclePartId} not found",
               Success = false
            };

        if (!string.IsNullOrWhiteSpace(request.Name)) part.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.PartNumber)) part.PartNumber = request.PartNumber;
        if (!string.IsNullOrWhiteSpace(request.Description)) part.Description = request.Description;
        part.AvailableQuantity = request.AvailableQuantity;

        await _dbContext.SaveChangesAsync();
        return new UpdateVehiclePartResponse {
            Message = "Ok",
            Success = true,
            VehiclePart = part.ToProto()
        };
    }

    public override async Task<UpdateOrderResponse> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
    {
        var order = await _dbContext.ServiceOrders
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId);
        if (order is null)
            return new UpdateOrderResponse {
                Message = $"Order with id={request.ServiceOrderId} not found",
                Success = false
            };

        if (order.AssignedMechanicId == null) {
            return new UpdateOrderResponse {
                Message = $"Order with id={request.ServiceOrderId} mechanic id not found",
                Success = false
            };
        }
   
        if (!string.IsNullOrWhiteSpace(request.Status)) order.Status = request.Status;
        order.AssignedMechanicId = request.AssignedMechanicId;
        order.VehicleId = request.VehicleId;

        await _dbContext.SaveChangesAsync();
        var mechanic = await _authClient.GetUserAsync(new GetUserRequest {
            UserId = order.AssignedMechanicId.Value
        });
        var orderu = await _dbContext.ServiceOrders
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceOrderId);
        
        _eventPublisher.PublishEvent("workshop.service.order.updated", new {
            Vehicle = orderu.Vehicle,
            OrderId = order.Id,
            Status = order.Status,
            User = mechanic
        });
        return new UpdateOrderResponse {
            Message = "Ok",
            Success = true,
            ServiceOrder = order.ToProto()
        };
    }

public override async Task<UpdateServiceTaskResponse> UpdateServiceTask(UpdateServiceTaskRequest request, ServerCallContext context)
{
    var response = new UpdateServiceTaskResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceTasks.FindAsync(request.ServiceTaskId);
        if (entity == null) {
            response.Message = "Service task not found";
            return response;
        }

        entity.Description = request.Description;
        entity.LaborCost = request.LaborCost;

        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service task updated successfully";
        response.ServiceTask = ServiceTaskMapper.ToProto(entity);
        
        _eventPublisher.PublishEvent("workshop.service.task.updated", new {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Description = entity.Description,
            LaborCost = entity.LaborCost,
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to update service task");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<UpdateServicePartResponse> UpdateServicePart(UpdateServicePartRequest request, ServerCallContext context)
{
    var response = new UpdateServicePartResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceParts
            .Include(sp => sp.VehiclePart)
            .FirstOrDefaultAsync(sp => sp.Id == request.ServicePartId);
        if (entity == null) {
            response.Message = "Service part not found";
            return response;
        }

        entity.Quantity = request.Quantity;

        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service part updated successfully";
        response.ServicePart = ServicePartMapper.ToProto(entity);
        
        _eventPublisher.PublishEvent("workshop.service.part.updated",new {
            Id = entity.Id,
            OrderId = entity.OrderId,
            VehiclePart = entity.VehiclePart,
            Quantity = entity.Quantity,
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to update service part");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<UpdateServiceCommentResponse> UpdateServiceComment(UpdateServiceCommentRequest request, ServerCallContext context)
{
    var response = new UpdateServiceCommentResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceComments.FindAsync(request.ServiceCommentId);
        if (entity == null) {
            response.Message = "Service comment not found";
            return response;
        }

        entity.Content = request.Content;

        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service comment updated successfully";
        response.ServiceComment = entity.ToProto();
        
        _eventPublisher.PublishEvent("workshop.service.comment.updated", new {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Content = entity.Content,
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to update service comment");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}
    
    public override async Task<DeleteVehicleResponse> DeleteVehicle(DeleteVehicleRequest request, ServerCallContext context)
{
    var response = new DeleteVehicleResponse { Success = false };

    try {
        var entity = await _dbContext.Vehicles.FindAsync(request.VehicleId);
        if (entity == null) {
            response.Message = "Vehicle not found";
            return response;
        }

        _dbContext.Vehicles.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Vehicle deleted successfully";
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete vehicle");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<DeleteVehiclePartResponse> DeleteVehiclePart(DeleteVehiclePartRequest request, ServerCallContext context)
{
    var response = new DeleteVehiclePartResponse { Success = false };

    try {
        var entity = await _dbContext.VehicleParts.FindAsync(request.VehiclePartId);
        if (entity == null) {
            response.Message = "Vehicle part not found";
            return response;
        }

        _dbContext.VehicleParts.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Vehicle part deleted successfully";
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete vehicle part");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<DeleteOrderResponse> DeleteOrder(DeleteOrderRequest request, ServerCallContext context)
{
    var response = new DeleteOrderResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceOrders.FindAsync(request.ServiceOrderId);
        if (entity == null) {
            response.Message = "Order not found";
            return response;
        }

        _dbContext.ServiceOrders.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Order deleted successfully";
        _eventPublisher.PublishEvent("workshop.service.order.deleted", new {
            Id = request.ServiceOrderId
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete order");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<DeleteServiceTaskResponse> DeleteServiceTask(DeleteServiceTaskRequest request, ServerCallContext context)
{
    var response = new DeleteServiceTaskResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceTasks.FindAsync(request.ServiceTaskId);
        if (entity == null) {
            response.Message = "Service task not found";
            return response;
        }

        _dbContext.ServiceTasks.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service task deleted successfully";
        
        _eventPublisher.PublishEvent("workshop.service.task.removed", new {
            Id = request.ServiceTaskId,
            OrderId = entity.OrderId
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete service task");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<DeleteServicePartResponse> DeleteServicePart(DeleteServicePartRequest request, ServerCallContext context)
{
    var response = new DeleteServicePartResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceParts.FindAsync(request.ServicePartId);
        if (entity == null) {
            response.Message = "Service part not found";
            return response;
        }

        _dbContext.ServiceParts.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service part deleted successfully";
        
        _eventPublisher.PublishEvent("workshop.service.part.removed", new {
            Id = request.ServicePartId,
            OrderId = entity.OrderId
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete service part");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}

public override async Task<DeleteServiceCommentResponse> DeleteServiceComment(DeleteServiceCommentRequest request, ServerCallContext context)
{
    var response = new DeleteServiceCommentResponse { Success = false };

    try {
        var entity = await _dbContext.ServiceComments.FindAsync(request.ServiceCommentId);
        if (entity == null) {
            response.Message = "Service comment not found";
            return response;
        }

        _dbContext.ServiceComments.Remove(entity);
        await _dbContext.SaveChangesAsync();

        response.Success = true;
        response.Message = "Service comment deleted successfully";
        _eventPublisher.PublishEvent("workshop.service.comment.removed", new {
            Id = request.ServiceCommentId,
            OrderId = entity.OrderId
        });
    } catch (Exception ex) {
        _logger.LogError(ex, "Failed to delete service comment");
        response.Message = $"Error: {ex.Message}";
    }

    return response;
}
}
    





