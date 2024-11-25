using Application.PlantPosts.Command.CreatePost;
using Application.Users.Querry.Login;
using AutoMapper;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Mapping
{
    public class PlantPostProfile : Profile
    {
        public PlantPostProfile()
        {
            CreateMap<CreatePlantPostRequest, CreatePlantPostCommand>().ReverseMap();
        }
    }
}
