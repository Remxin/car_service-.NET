using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;
using WorkshopService.Entities;

namespace WorkshopService.Mappers;

public static class ServiceCommentMapper {
    public static ServiceComment ToProto(this ServiceCommentEntity entity)
    {
        return new ServiceComment
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Content = entity.Content ?? "",
            CreatedAt = Timestamp.FromDateTime(entity.CreatedAt.ToUniversalTime())
        };
    }
}