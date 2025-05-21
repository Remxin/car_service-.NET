using AuthService.Entities;
using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Models;

namespace AuthService.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt.ToDateTime().ToUniversalTime()
        };
    }

    public static User ToProto(this UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc))
        };
    }

    public static UserDto ToProtoDto(this UserEntity entity) {
        return new UserDto {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc))
        };
    }
}
