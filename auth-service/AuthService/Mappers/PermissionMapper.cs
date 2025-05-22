using AuthService.Entities;
using Shared.Grpc.Models;

namespace AuthService.Mappers;

public static class PermissionMapper
{
    public static PermissionEntity ToEntity(this Permission proto)
    {
        return new PermissionEntity
        {
            Id = proto.Id,
            Name = proto.Name,
            Description = proto.Description
        };
    }

    public static Permission ToProto(this PermissionEntity entity)
    {
        return new Permission
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description ?? ""
        };
    }
}