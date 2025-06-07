using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class ServiceCompleteOrderMapper {
    public static ServiceCompleteOrder ToProto(this ServiceOrderEntity entity, UserDto user) {
        return new ServiceCompleteOrder {
            Id = entity.Id,
            Vehicle = VehicleMapper.ToProto(entity.Vehicle),
            Status = entity.Status,
            Mechanic = user,
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime()),
            ServiceParts = { entity.ServiceParts.Select(ServicePartMapper.ToProto) },
            ServiceComment = { entity.ServiceComments.Select(ServiceCommentMapper.ToProto) },
            ServiceTasks = {  entity.ServiceTasks.Select(ServiceTaskMapper.ToProto) }
        };
    }
}