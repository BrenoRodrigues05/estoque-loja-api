using APILojaEstoque.Models;
using AutoMapper;

namespace APILojaEstoque.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
       public MappingProfile()
        {
            CreateMap<Produtos, ProdutoReadDTO>();
            CreateMap<ProdutoCreateDTO, Produtos>();
            CreateMap<ProdutoUpdateDTO, Produtos>();

            CreateMap<Estoque, EstoqueReadDTO>()
           .ForMember(dest => dest.ProdutoNome, opt => opt.MapFrom(src => src.Produto.Nome));
            CreateMap<EstoqueCreateDTO, Estoque>();
            CreateMap<EstoqueUpdateDTO, Estoque>();

        }
    }
}
