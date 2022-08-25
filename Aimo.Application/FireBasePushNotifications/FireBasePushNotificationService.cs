using Aimo.Domain;
using Aimo.Domain.FireBasePushNotifications;
using CorePush.Google;
using System.Net.Http.Headers;

namespace Aimo.Application.FireBasePushNotifications
{
    internal partial class FireBasePushNotificationService : IFireBasePushNotificationService
    {
        private readonly AppSetting _appSetting;
        private readonly HttpClient _httpClient;

        public FireBasePushNotificationService(AppSetting appSetting, HttpClient httpClient)
        {
            _appSetting = appSetting;
            _httpClient = httpClient;
        }

        public async ResultTask SendPushNotification(FireBasePushNotificationDto dto)
        {
            var response = new FireBasePushNotificationResponseDto();

            foreach (var device in dto.lstDevices)
            {
                // FCM Sender(Android Device) 
                var settings = new FcmSettings()
                {
                    SenderId = _appSetting.FcmSenderId,
                    ServerKey = _appSetting.FcmServerKey
                };

                var authorizationKey = $"keyy={settings.ServerKey}";
                var deviceToken = device.DeviceId;
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                _httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var dataPayload = new GoogleNotification.DataPayload
                {
                    Title = dto.Title,
                    Body = dto.Body
                };

                var notification = new GoogleNotification
                {
                    Data = dataPayload,
                    Notification = dataPayload
                };

                var fcm = new FcmSender(settings, _httpClient);
                var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);
                if (fcmSendResponse.IsSuccess())
                {
                    response.IsSuccess = true;
                    response.Message = "Notification sent successfully";
                    response.lstDeviceIds.Add(new ResponseDevice
                    {
                        DeviceId = device.DeviceId,
                        IsSuccess = true,
                        Message = "Notification sent successfully"
                    });
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Notification sent failed";
                    response.lstDeviceIds.Add(new ResponseDevice
                    {
                        DeviceId = device.DeviceId,
                        IsSuccess = false,
                        Message = fcmSendResponse.Results[0].Error
                    });
                }
            }

            return Result.Create(response);
        }
    }

    public partial interface IFireBasePushNotificationService
    {
        ResultTask SendPushNotification(FireBasePushNotificationDto dto);
    }
}