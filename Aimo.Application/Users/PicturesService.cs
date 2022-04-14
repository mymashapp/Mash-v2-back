using Aimo.Core;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Users
{
    public partial class PicturesService : IPicturesService
    {
        private readonly IRepository<Picture> _picturesRepository;

        public PicturesService(IRepository<Picture> picturesRepository)
        {
            _picturesRepository = picturesRepository;
        }

        public async Task<Result<PictureDto>> InsertPictureAsync(PictureDto dto)
        {
            var entity = dto.Map<Picture>();
            await _picturesRepository.AddAsync(entity);
            await _picturesRepository.CommitAsync();
            return Result.Create(entity.MapTo(dto)).Success();
        }

        public async Task<Result<PictureDto>> UpdatePictureAsync(PictureDto dto)
        {
            var result = Result.Create(dto);
            var entity = dto.Map<Picture>();
            _picturesRepository.Update(entity);
            var affected = await _picturesRepository.CommitAsync();

            return result.SetData(entity.MapTo(dto), affected).Success();
        }
    }

    public partial interface IPicturesService
    {
        Task<Result<PictureDto>> InsertPictureAsync(PictureDto dto);
        Task<Result<PictureDto>> UpdatePictureAsync(PictureDto dto);
    }
}