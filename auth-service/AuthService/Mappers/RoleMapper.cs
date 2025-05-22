using AuthService.Entities;
using Shared.Grpc.Models;

namespace AuthService.Mappers;

public static class RoleMapper
{
    public static RoleEntity ToEntity(this Role proto)
    {
        return new RoleEntity
        {
            Id = proto.Id,
            Name = proto.Name,
            Description = proto.Description
        };
    }

    public static Role ToProto(this RoleEntity entity)
    {
        return new Role
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description ?? ""
        };
    }
}