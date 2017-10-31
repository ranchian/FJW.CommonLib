

namespace FJW.Notice
{

    public interface IMemberSender : ISender
    {
        /// <summary>
        /// 注册成功
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="phone">手机号</param>
        /// <param name="invitationCode">邀请码</param>
        void Reg(long memberId, string phone, string invitationCode);

    }
}
