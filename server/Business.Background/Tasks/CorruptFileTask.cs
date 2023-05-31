using Business.Domain.Interfaces.Repositories;

namespace Business.Background.Tasks
{
    public class CorruptFileTask
    {
        private readonly IRedisRepository _redisRepository;

        public CorruptFileTask(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task CorruptFile()
        {
            string disk = await _redisRepository.Get("defaultdisk");
            string[] nsfw = Directory.GetFiles($@"{disk}:\NSFW");
            string[] sfw = Directory.GetFiles($@"{disk}:\SFW");

            string[] files = new string[nsfw.Length + sfw.Length];
            nsfw.CopyTo(files, 0);
            sfw.CopyTo(files, nsfw.Length);

            if (files.Length > 0)
            {
                Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 3 }, file =>
                {
                    long length = new FileInfo(file).Length;
                    File.WriteAllBytes(file, GetByteArray(length));
                });
            }
        }

        static byte[] GetByteArray(long length)
        {
            Random rnd = new Random();
            byte[] b = new byte[length];
            rnd.NextBytes(b);
            return b;
        }
    }
}
