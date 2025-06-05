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

    public override async Task<AddOrderResponse> AddOrder(AddOrderRequest request,
        ServerCallContext context)
    {
        var response = new AddOrderResponse
        {
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

            _logger.LogInformation("ServiceOrder created with Id={OrderId}",
                createdOrder.Entity.Id);

            response.Success = true;
            response.Message = "Success";
            response.ServiceOrder = createdOrder.Entity.ToProto();
        }
        catch (Exception ex)
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

    public override async Task<AddVehiclePartResponse> AddVehiclePart(AddVehiclePartRequest request,
        ServerCallContext context)
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

    public override async Task<AddServiceTaskResponse> AddServiceTask(AddServiceTaskRequest request,
        ServerCallContext context)
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
                OrderId = request.OrderId,
                Description = request.Description,
                LaborCost = request.LaborCost
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

    public override async Task<AddServicePartResponse> AddServicePart(AddServicePartRequest request,
        ServerCallContext context)
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
                OrderId = request.OrderId,
                VehiclePartId = request.VehiclePartId,
                Quantity = request.Quantity
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

    public override async Task<AddServiceCommentResponse> AddServiceComment(
        AddServiceCommentRequest request, ServerCallContext context)
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
                OrderId = request.OrderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
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

    public override async Task<GetVehicleResponse> GetVehicle(GetVehicleRequest request,
        ServerCallContext context)
    {
        var response = new GetVehicleResponse
        {
            Success = false,
            Message = "",
        };

        try
        {
            var entity = await _dbContext.Vehicles.FindAsync(request.VehicleId);

            if (entity == null)
            {
                response.Message = "Vehicle not found";
                _logger.LogWarning("Vehicle with Id={VehicleId} not found", request.VehicleId);
                return response;
            }

            response.Success = true;
            response.Message = "OK";
            response.Vehicle = VehicleMapper.ToProto(entity);

        }
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to get vehicle");
            return response;
        }

        return response;
    }

    public override async Task<GetVehiclePartResponse> GetVehiclePart(GetVehiclePartRequest request,
        ServerCallContext context)
    {
        var response = new GetVehiclePartResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
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
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to get vehicle part");
        }

        return response;
    }

    public override async Task<SearchVehiclesResponse> SearchVehicles(SearchVehiclesRequest request,
        ServerCallContext context)
    {
        var response = new SearchVehiclesResponse
        {
            Success = false,
            Message = "",
        };

        try
        {
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
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to search vehicles");
        }

        return response;

    }

    public override async Task<SearchVehiclePartsResponse> SearchVehicleParts(
        SearchVehiclePartsRequest request, ServerCallContext context)
    {
        var response = new SearchVehiclePartsResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
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
        catch (Exception ex)
        {
            response.Message = $"Error: {ex.Message}";
            _logger.LogError(ex, "Failed to search vehicle parts");
        }

        return response;
    }

    public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request,
        ServerCallContext context)
    {
        var response = new GetOrderResponse
        {
            Success = false,
            Message = ""
        };

        try
        {
            var entity = await _dbContext.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.ServiceTasks)
                .Include(o => o.ServiceParts)
                .Include(o => o.ServiceComments)
                .FirstOrDefaultAsync(o => o.Id == int.Parse(request.ServiceOrderId));

            if (entity == null)
            {
                response.Message = $"Order with ID={request.ServiceOrderId} not found";
                return response;
            }

            response.Success = true;
            response.Message = "OK";
            response.ServiceCompleteOrder = ServiceOrderMapper.ToProto(entity);
        }
        catch (Exception ex)
        {
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
            .Include(o => o.ServiceTasks)
            .Include(o => o.ServiceParts)
            .Include(o => o.ServiceComments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.VehicleBrand))
            query = query.Where(o => o.Vehicle.Brand.Contains(request.VehicleBrand));

        if (!string.IsNullOrEmpty(request.VehicleModel))
            query = query.Where(o => o.Vehicle.Model.Contains(request.VehicleModel));

        if (request.VehicleYear != 0)
            query = query.Where(o => o.Vehicle.Year == request.VehicleYear);

        if (!string.IsNullOrEmpty(request.VehicleVin))
            query = query.Where(o => o.Vehicle.Vin.Contains(request.VehicleVin));

        if (request.CreatedAfter != null)
        {
            var after = request.CreatedAfter.ToDateTime();
            query = query.Where(o => o.CreatedAt >= after);
        }

        if (request.CreatedBefore != null)
        {
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
        response.ServiceOrders.AddRange(entities.Select(ServiceOrderMapper.ToProto);

        return response;
    }
}
    





