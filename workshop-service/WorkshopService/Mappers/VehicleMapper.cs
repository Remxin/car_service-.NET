using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class VehicleMapper {
    public static Vehicle ToProto(this VehicleEntity entity)
    {
        return new Vehicle
        {
            Id = entity.Id,
            Brand = entity.Brand,
            Model = entity.Model,
            Year = entity.Year,
            Vin = entity.Vin,
            PhotoUrl = entity.PhotoUrl ?? "",
            CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime())
        };
    }
}