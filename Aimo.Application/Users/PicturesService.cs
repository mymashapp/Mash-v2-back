using Aimo.Core;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Users
{
    public partial class PicturesService: IPicturesService
    {
        private readonly IRepository<Pictures> _picturesRepository;

        public PicturesService(IRepository<Pictures> picturesRepository)
        {
            _picturesRepository = picturesRepository;
        }
        public async Task<Result<PictureDto>> InsertPictureAsync(PictureDto dto)
        {
            var result = Result.Create(dto);

            var entity = dto.Map<Pictures>();
            await _picturesRepository.AddAsync(entity);
            var affected = await _picturesRepository.CommitAsync();
            result.SetData(entity.MapTo(dto), affected).Success();

            return result;
        }

        public async Task<Result<PictureDto>> UpdatePictureAsync(PictureDto dto)
        {
            var result = Result.Create(dto);
            var entity = dto.Map<Pictures>();
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
