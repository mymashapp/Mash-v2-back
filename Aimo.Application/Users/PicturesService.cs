using Aimo.Core.Infrastructure;
using Aimo.Domain;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Users
{
    public partial class PicturesService : IPicturesService
    {
        private readonly IRepository<Picture> _picturesRepository;
        private readonly IAppFileProvider _fileProvider;

        public PicturesService(IRepository<Picture> picturesRepository, IAppFileProvider fileProvider)
        {
            _picturesRepository = picturesRepository;
            _fileProvider = fileProvider;
        }

        public async Task<Result<Picture>> InsertPictureAsync(PictureDto dto)
        {
            try
            {
                var image = new Base64Image(dto.PictureUrl);
                var entity = dto.Map<Picture>();
                entity.Url=AppDefaults.NoImageFileName;
                await _picturesRepository.AddAsync(entity);
                await _picturesRepository.CommitAsync();
                
                var fileName = $"{entity.Id}_{Guid.NewGuid()}.{image.Extension}";
                var fileAbsPath = _fileProvider.GetAbsolutePath(AppDefaults.UserPicturePath);
                _fileProvider.CreateDirectory(fileAbsPath);
                await _fileProvider.WriteAllBytesAsync(_fileProvider.Combine(fileAbsPath, fileName), image.Bytes);
                entity.Url = $"{AppDefaults.UserPicturePath}{fileName}";
                _picturesRepository.Update(entity);
                await _picturesRepository.CommitAsync();
                return Result.Create(entity).Success();
            }
            catch (Exception e)
            {
                return Result.Create<Picture>().Failure(e.Message);
            }
        }

    }

    public partial interface IPicturesService
    {
        Task<Result<Picture>> InsertPictureAsync(PictureDto dto);
    }
}