using APILojaEstoque.Models;
using AutoMapper;

namespace APILojaEstoque.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
       public MappingProfile()
        {
            CreateMap<Produtos, ProdutoReadDTO>().ReverseMap();
            CreateMap<ProdutoCreateDTO, Produtos>().ReverseMap();
            CreateMap<ProdutoUpdateDTO, Produtos>().ReverseMap();

            CreateMap<Estoque, EstoqueReadDTO>()
           .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome)).ReverseMap();
            CreateMap<EstoqueCreateDTO, Estoque>().ReverseMap();
            CreateMap<EstoqueUpdateDTO, Estoque>().ReverseMap();

        }
    }
}
