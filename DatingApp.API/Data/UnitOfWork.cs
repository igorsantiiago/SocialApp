using AutoMapper;
using DatingApp.API.Data.Repositories;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UnitOfWork(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IUserRepository UserRepository
        => new UserRepository(_context, _mapper);

    public IMessageRepository MessageRepository
        => new MessageRepository(_context, _mapper);

    public ILikesRepository LikesRepository
        => new LikesRepository(_context);

    public IPhotoRepository PhotoRepository
        => new PhotoRepository(_context);

    public async Task<bool> Complete()
        => await _context.SaveChangesAsync() > 0;

    public bool HasChanges()
        => _context.ChangeTracker.HasChanges();
}
