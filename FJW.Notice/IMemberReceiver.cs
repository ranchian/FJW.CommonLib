

namespace FJW.Notice
{
    public interface IMemberReceiver : IReceiver
    {
        MemberNotice Receive();
    }
}
