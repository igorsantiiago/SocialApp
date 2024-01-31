using DatingApp.API.DTOs;
using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
