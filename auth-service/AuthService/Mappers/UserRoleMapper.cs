using AuthService.Entities;
using Shared.Grpc.Models;

namespace AuthService.Mappers;

public static class UserRoleMapper
{
    public static UserRoleEntity ToEntity(this UserRole proto)
    {
        return new UserRoleEntity
        {
            UserId = proto.UserId,
            RoleId = proto.RoleId
        };
    }

    public static UserRole ToProto(this UserRoleEntity entity)
    {
        return new UserRole
        {
            UserId = entity.UserId,
            RoleId = entity.RoleId
        };
    }
}