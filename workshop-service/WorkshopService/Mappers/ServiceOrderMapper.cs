using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class ServiceOrderMapper
{
    public static ServiceOrder ToProto(this ServiceOrderEntity entity)
    {
        return new ServiceOrder
        {
            Id = entity.Id,
            VehicleId = entity.VehicleId,
            Status = entity.Status,
            AssignedMechanicId = entity.AssignedMechanicId ?? 0,
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime())
        };
    }
}