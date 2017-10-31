using FJW.CommonLib.Entity;
using FJW.CommonLib.XService;

namespace FJW.CommonLib.Validation
{
    public abstract class ValidateBase
    {
        public virtual ServiceResultStatus Validate<T>( T parameter,  out string msg,  bool isValidateLogin = false ) where T : BaseParameter, new()
        {
        
            if (!ValidateHelper.CheckEntity(ref parameter, out msg))
                return ServiceResultStatus.InvalidParameter;


            if (isValidateLogin && parameter.MemberId <= 0)
                return ServiceResultStatus.InvalidToken;

            msg = string.Empty;
            return ServiceResultStatus.Ok ;
        }
    }
}