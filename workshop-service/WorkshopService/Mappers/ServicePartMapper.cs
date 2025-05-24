using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class ServicePartMapper {
    public static ServicePart ToProto(this ServicePartEntity entity)
    {
        return new ServicePart
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            VehiclePartId = entity.VehiclePartId,
            Quantity = entity.Quantity
        };
    }
}