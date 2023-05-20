using Business.Domain.Interfaces.Repositories;
using Business.Domain.Model;

namespace Business.Background.Tasks
{
    public class RedisConsumerTask
    {
        private readonly IRedisRepository _redisRepository;

        public RedisConsumerTask(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task RedisConsumer()
        {
            List<Image> keys = await _redisRepository.GetAllKeysWithValue<Image>();

            try
            {
                if (keys is not null && keys.Count > 0)
                {
                    string disk = await _redisRepository.Get("defaultdisk");
                    (string nsfw, string sfw) = GenerateFolders(disk);

                    await Parallel.ForEachAsync(keys, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (key, cancellationToken) =>
                    {
                        if (key is not null)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                throw new TaskCanceledException(nameof(RedisConsumer));

                            if (key.is_nsfw)
                                await DownloadImageAsync(nsfw, key.image_id.ToString(), new Uri(key.url));
                            else
                                await DownloadImageAsync(sfw, key.image_id.ToString(), new Uri(key.url));

                            await _redisRepository.Delete(key.image_id.ToString());
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(keys);
                Console.WriteLine(ex.ToString());
            }

            static (string, string) GenerateFolders(string disk)
            {
                string nsfw = $@"{disk}:\NSFW";
                string sfw = $@"{disk}:\SFW";

                if (!Directory.Exists(nsfw))
                    Directory.CreateDirectory(nsfw);

                if (!Directory.Exists(sfw))
                    Directory.CreateDirectory(sfw);

                return (nsfw, sfw);
            }
        }

        private static async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                string fileExtension = Path.GetExtension(uriWithoutQuery);

                string path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");

                if (!File.Exists(path))
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(uri);
                    await File.WriteAllBytesAsync(path, imageBytes);
                }
            }
        }
    }
}
