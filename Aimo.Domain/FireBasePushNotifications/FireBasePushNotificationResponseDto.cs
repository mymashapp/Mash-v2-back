using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aimo.Domain.FireBasePushNotifications
{
    public partial class FireBasePushNotificationResponseDto : Dto
    {
        public FireBasePushNotificationResponseDto()
        {
            lstDeviceIds = new List<ResponseDevice>();
        }
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        public List<ResponseDevice> lstDeviceIds { get; set; }
    }

    public partial class ResponseDevice
    {
        public string DeviceId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
