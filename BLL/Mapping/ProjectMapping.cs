using AutoMapper;
using BLL.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BLL.Mapping
{
    public class ProjectMapping : Profile
    {
        public ProjectMapping()
        {
            EntitiesToDTOs();
            DTOsToEntities();
        }

        private void DTOsToEntities()
        {
            CreateMap<BoardModel, BoardEntity>()
                .ForMember(x => x.ID, opt => opt.MapFrom(x => x.ID))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.Content, opt => opt.MapFrom(x => x.Content));

            CreateMap<UserModel, UserEntity>()
                .ForMember(x => x.ID, opt => opt.MapFrom(x => x.ID))
                .ForMember(x => x.Username, opt => opt.MapFrom(x => x.Username))
                .ForMember(x => x.Password, opt => opt.MapFrom(x => x.Password))
                .ForMember(x => x.Boards, opt => opt.MapFrom(x=>x.Boards));
        }

        private void EntitiesToDTOs()
        {
            CreateMap<BoardEntity, BoardModel>()
                .ForMember(x => x.ID, opt => opt.MapFrom(x => x.ID))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.Content, opt => opt.MapFrom(x => x.Content));

            CreateMap<UserEntity, UserModel>()
                .ForMember(x => x.ID, opt => opt.MapFrom(x => x.ID))
                .ForMember(x => x.Username, opt => opt.MapFrom(x => x.Username))
                .ForMember(x => x.Password, opt => opt.MapFrom(x => x.Password))
                .ForMember(x => x.Boards, opt => opt.MapFrom(x => x.Boards));
        }
    }
}
