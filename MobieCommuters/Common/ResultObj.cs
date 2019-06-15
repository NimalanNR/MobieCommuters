using System;
using System.Runtime.Serialization;

namespace MobieCommuters.Common
{
    [DataContract]
    public class ResultObj<T>
    {
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public int ResponseCode { get; set; }

        public ResultObj(bool status, String message, T data, int totalCount, int ResponseCode)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
            this.TotalCount = totalCount;
            this.ResponseCode = ResponseCode;
        }

    }

}