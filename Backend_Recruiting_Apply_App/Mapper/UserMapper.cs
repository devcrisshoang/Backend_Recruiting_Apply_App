using Backend_Recruiting_Apply_App.Data.DTOs;
using Backend_Recruiting_Apply_App.Data.Entities;

namespace Backend_Recruiting_Apply_App.Data.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToDTO(User user)
        {
            return new UserDTO
            {
                ID = user.ID,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Image = user.Image,
                Type = user.Type
            };
        }

        public static List<UserDTO> ToDTOList(IEnumerable<User> users)
        {
            return users.Select(ToDTO).ToList();
        }
    }
}
