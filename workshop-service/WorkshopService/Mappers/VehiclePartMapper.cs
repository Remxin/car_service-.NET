using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class VehiclePartMapper {
    public static VehiclePart ToProto(this VehiclePartEntity entity)
    {
        return new VehiclePart
        {
            Id = entity.Id,
            Name = entity.Name,
            PartNumber = entity.PartNumber,
            Description = entity.Description ?? "",
            Price = (double)(entity.Price ?? 0),
            AvailableQuantity = entity.AvailableQuantity ?? 0
        };
    }
}