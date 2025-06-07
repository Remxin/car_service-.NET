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

    public static ServiceOrderWithVehicle ToProtoWithVehicle(this ServiceOrderEntity entity) {
        return new ServiceOrderWithVehicle {
            Id = entity.Id,
            Vehicle = VehicleMapper.ToProto(entity.Vehicle),
            Status = entity.Status,
            AssignedMechanicId = entity.AssignedMechanicId ?? 0,
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime()),
        };
    }
}