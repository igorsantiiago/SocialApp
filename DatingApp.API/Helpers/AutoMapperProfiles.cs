using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.DTOs.EntitiesDTO;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;

namespace DatingApp.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, AppUserDTO>()
            .ForMember(destination => destination.PhotoUrl,
                        options => options.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsProfile)!.Url))
            .ForMember(destination => destination.Age,
                        options => options.MapFrom(src => src.BirthDate.CalculateAge()));

        CreateMap<Photo, PhotoDTO>();

        CreateMap<AppUserUpdateDTO, AppUser>();

        CreateMap<RegisterAccountDTO, AppUser>();

        CreateMap<Message, MessageDTO>()
            .ForMember(s => s.SenderPhotoUrl, opt => opt.MapFrom(
                src => src.Sender.Photos.FirstOrDefault(x => x.IsProfile)!.Url))
            .ForMember(s => s.RecipientPhotoUrl, opt => opt.MapFrom(
                src => src.Recipient.Photos.FirstOrDefault(x => x.IsProfile)!.Url));
    }
}
