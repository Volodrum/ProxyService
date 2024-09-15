using ProxyService.Interfaces;

namespace MicroServices1.Models
{
    public class ReqresUserResponse : IReqresUserResponse
    {
        public User Data { get; set; }
    }
}
