using AuthService.Entities;
using Shared.Grpc.Models;

namespace AuthService.Mappers;

public static class RolePermissionMapper
{
    public static RolePermissionEntity ToEntity(this RolePermission proto)
    {
        return new RolePermissionEntity
        {
            RoleId = proto.RoleId,
            PermissionId = proto.PermissionId
        };
    }

    public static RolePermission ToProto(this RolePermissionEntity entity)
    {
        return new RolePermission
        {
            RoleId = entity.RoleId,
            PermissionId = entity.PermissionId
        };
    }
}