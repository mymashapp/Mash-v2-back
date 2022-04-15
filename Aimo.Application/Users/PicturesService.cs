using Aimo.Core;
using Aimo.Core.Infrastructure;
using Aimo.Domain;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

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
                var image = new Base64Image(dto.PictureBase64);
                var entity = dto.Map<Picture>();
                entity.Url="NoImage.png";
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

        public static string GetFileExtension(string base64String)
        {
            var data = base64String[..5];

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
    }

    public partial interface IPicturesService
    {
        Task<Result<Picture>> InsertPictureAsync(PictureDto dto);
    }
}