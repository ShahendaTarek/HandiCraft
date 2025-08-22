using AutoMapper;
using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Specificatoins;
using HandiCraft.Domain.ProductList;
using HandiCraft.Infrastructure.Specification;
using HandiCraft.Presentation;
using HandiCraft.Presistance.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandiCraft.Infrastructure.Services.ProductList
{
    public class ProductListServices : IProductListServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly IMapper _mapper;

        public ProductListServices(HandiCraftDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async  Task<ProductResponseDto> AddProductAsync(string UserId, AddProductDto productdto)
        {

            var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == productdto.CategoryName.ToLower())
                        ?? throw new Exception($"Category '{productdto.CategoryName}' not found.");

            var product = _mapper.Map<Product>(productdto);
            product.CategoryId = category.Id;
            product.UserId = UserId;

            
            if (productdto.ProductImageUrl?.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(productdto.ProductImageUrl.FileName)}";
                var filePath = Path.Combine("wwwroot/images/products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productdto.ProductImageUrl.CopyToAsync(stream);
                }

                product.ProductImageUrl = $"/images/products/{fileName}";
            }

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Saving product failed: " + ex.InnerException?.Message ?? ex.Message);
            }

           
            var savedProduct = await _context.Products
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (savedProduct == null)
                throw new Exception("Could not retrieve saved product.");

            return _mapper.Map<ProductResponseDto>(savedProduct);


        }
        
        public async Task<Pagination<ProductResponseDto>> GetAllProductsAsync(ProductSpecParams productParams)
        {
            if (productParams.PageSize <= 0) productParams.PageSize = 10;
            if (productParams.PageIndex <= 0) productParams.PageIndex = 1;

            var spec = new ProductsWithSpecifications(productParams);

            var countSpec = new ProdcutWithFilterationCountAsync(productParams);

            var totalItems = await _context.Products
                .Where(countSpec.Criteria)
                .CountAsync();

            var products = await SpecificationEvaluator<Product>
                .GetQuery(_context.Products.AsQueryable(), spec)
                .ToListAsync();

             

            var data = _mapper.Map<IReadOnlyList<ProductResponseDto>>(products);

            return new Pagination<ProductResponseDto>(productParams.PageIndex, productParams.PageSize, totalItems, data);
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products
               .Include(p => p.Category)
               .Include(p => p.User)
               .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> UpdateProductAsync(string userId,Guid id, UpdateProductDto dto)
        {
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id );

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found or you are not the owner.");

            _mapper.Map(dto, product);

            if (userId != product.UserId)
            {
                throw new Exception(" You are unauthorized .");
            }
            var UpdatedProduct = await _context.Products
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (UpdatedProduct == null)
                throw new Exception("Could not retrieve saved product.");

            return _mapper.Map<ProductResponseDto>(UpdatedProduct);
        }
        public async Task<bool> DeleteProductAsync(string userId, Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            if (userId != product.UserId)
            {
                throw new Exception(" You are unauthorized .");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
