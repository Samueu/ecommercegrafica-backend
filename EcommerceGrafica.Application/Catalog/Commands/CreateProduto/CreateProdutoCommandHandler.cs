using EcommerceGrafica.Application.Catalog.DTOs;
using EcommerceGrafica.Application.Catalog.Mappings;
using EcommerceGrafica.Application.Common.Interfaces;
using EcommerceGrafica.Application.Common.Models;
using EcommerceGrafica.Domain.Catalog.Entities;
using EcommerceGrafica.Domain.Catalog.Repositories;
using EcommerceGrafica.Domain.Catalog.ValueObjects;
using EcommerceGrafica.Domain.Shared;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Commands.CreateProduto;

public sealed class CreateProdutoCommandHandler : IRequestHandler<CreateProdutoCommand, Result<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProdutoCommandHandler(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProdutoDto>> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var produto = Produto.Criar(
                request.Nome,
                request.Descricao,
                new Preco(request.Preco),
                request.Tipo);

            await _produtoRepository.AddAsync(produto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProdutoDto>.Success(produto.ToDto());
        }
        catch (DomainException ex)
        {
            return Result<ProdutoDto>.Failure(ex.Message);
        }
    }
}
