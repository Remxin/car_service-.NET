using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class ServiceTaskMapper {
    public static ServiceTask ToProto(this ServiceTaskEntity entity)
    {
        return new ServiceTask
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Description = entity.Description,
            LaborCost = (double)entity.LaborCost,
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime())
        };
    }
}