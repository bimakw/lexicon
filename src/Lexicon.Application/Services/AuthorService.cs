using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lexicon.Application.Services;

<<<<<<< Updated upstream
public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);
        return authors.Select(MapToDto);
=======
public class AuthorService(
    IUnitOfWork unitOfWork,
    ILogger<AuthorService> logger) : IAuthorService
{
    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await unitOfWork.Authors.GetAllAsync(cancellationToken);
        return Result<IEnumerable<AuthorDto>>.Success(authors.Select(MapToDto));
>>>>>>> Stashed changes
    }

    public async Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        return author != null ? MapToDto(author) : null;
=======
        var author = await unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        
        if (author == null)
            return Result<AuthorDto>.Failure("Author tidak ditemukan.");

        return Result<AuthorDto>.Success(MapToDto(author));
>>>>>>> Stashed changes
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        try 
        {
            // cek email duplikat
            var existing = await unitOfWork.Authors.GetByEmailAsync(dto.Email, cancellationToken);
            if (existing != null)
                return Result<AuthorDto>.Failure("Alamat email sudah terdaftar.");

<<<<<<< Updated upstream
        return MapToDto(author);
=======
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Bio = dto.Bio,
                AvatarUrl = dto.AvatarUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Authors.AddAsync(author, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDto>.Success(MapToDto(author));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas register author baru: {Email}", dto.Email);
            return Result<AuthorDto>.Failure("Gagal memproses pendaftaran author baru.");
        }
>>>>>>> Stashed changes
    }

    public async Task<AuthorDto?> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null) return null;
=======
        try 
        {
            var author = await unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
            if (author == null) 
                return Result<AuthorDto>.Failure("Data author tidak ditemukan dalam sistem.");
>>>>>>> Stashed changes

            // pastikan email baru tidak digunakan orang lain
            if (author.Email != dto.Email)
            {
                var existing = await unitOfWork.Authors.GetByEmailAsync(dto.Email, cancellationToken);
                if (existing != null && existing.Id != id)
                    return Result<AuthorDto>.Failure("Alamat email tersebut sudah digunakan oleh author lain.");
            }

            author.Name = dto.Name;
            author.Email = dto.Email;
            author.Bio = dto.Bio;
            author.AvatarUrl = dto.AvatarUrl;
            author.UpdatedAt = DateTime.UtcNow;

<<<<<<< Updated upstream
        return MapToDto(author);
=======
            await unitOfWork.Authors.UpdateAsync(author, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthorDto>.Success(MapToDto(author));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal update author {AuthorId}", id);
            return Result<AuthorDto>.Failure("Gagal memperbarui data author karena kendala teknis.");
        }
>>>>>>> Stashed changes
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null) return false;

        await _unitOfWork.Authors.DeleteAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
=======
        try 
        {
            var author = await unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
            if (author == null)
                return Result<bool>.Failure("Author yang ingin dihapus tidak ditemukan.");

            await unitOfWork.Authors.DeleteAsync(author, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas hapus author {AuthorId}", id);
            return Result<bool>.Failure("Gagal menghapus author. Pastikan tidak ada dependensi data.");
        }
>>>>>>> Stashed changes
    }

    private static AuthorDto MapToDto(Author author) => new(
        author.Id,
        author.Name,
        author.Email,
        author.Bio,
        author.AvatarUrl,
        author.Posts?.Count ?? 0,
        author.CreatedAt,
        author.UpdatedAt
    );
}

