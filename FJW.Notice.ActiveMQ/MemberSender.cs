

using FJW.CommonLib.Utils;

namespace FJW.Notice.ActiveMQ
{
    public class MemberSender: IMemberSender
    {
        private readonly Sender _sender;
        
        public MemberSender()
        {
            _sender = new Sender("member", ActiveConfig.GetDefault());
        }

        public void Reg(long memberId, string phone, string invitationCode)
        {
            _sender.Send(JsonHelper.JsonSerializer( new MemberNotice{ MemberId = memberId, Phone = phone, InvitationCode = invitationCode}));
        }

    }
}
