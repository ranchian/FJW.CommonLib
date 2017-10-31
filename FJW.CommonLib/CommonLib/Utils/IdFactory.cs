namespace FJW.CommonLib.Utils
{
    public class IdFactory: Singleton<IdFactory>
    {
        public IdWorker RecordIdWorker { get; set; }

        public IdFactory()
        {
            RecordIdWorker = new IdWorker(1,1);
        }
    }
}