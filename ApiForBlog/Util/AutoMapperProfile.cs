using ApiForBlog.Dto;
using ApiForBlog.Models;
using AutoMapper;

namespace ApiForBlog.Util
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Post, PostDto>();
            CreateMap<PostDto, Post>();
        }
    }
}
