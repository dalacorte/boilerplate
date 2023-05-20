using Business.Domain.Enums;
using Business.Domain.Helpers;
using Business.Domain.Interfaces.Repositories;
using Business.Domain.Model;
using Newtonsoft.Json;

namespace Business.Background.Tasks
{
    public class RandomImageTask
    {
        private readonly IRedisRepository _redisRepository;

        public RandomImageTask(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task GetImage()
        {
            Random rnd = new Random();

            switch ((ImageEnum)rnd.Next(1, 3))
            {
                case ImageEnum.Waifu:
                    await Waifu();
                    break;
                case ImageEnum.Hentai:
                    await Hentai();
                    break;
            }
        }

        private async Task Waifu()
        {
            string[] tags = { "maid", "waifu", "marin-kitagawa", "mori-calliope", "raiden-shogun", "oppai", "selfies", "uniform" };

            await Parallel.ForEachAsync(tags, new ParallelOptions { MaxDegreeOfParallelism = 8 }, async (tag, cancellationToken) =>
            {
                HttpResponseMessage response = await HttpHelpers.SendRequestRaw($"https://api.waifu.im/search/?included_tags={tag}&many=true", HttpMethod.Get);
                WaifuIm images = JsonConvert.DeserializeObject<WaifuIm>(await response.Content.ReadAsStringAsync());

                await Parallel.ForEachAsync(images.images, new ParallelOptions { MaxDegreeOfParallelism = 3 }, async (image, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException(nameof(Waifu));

                    await _redisRepository.Set(image.image_id.ToString(), JsonConvert.SerializeObject(image));
                });
            });
        }

        private async Task Hentai()
        {
            string[] tags = { "ass", "hentai", "milf", "oral", "paizuri", "ecchi", "ero" };

            await Parallel.ForEachAsync(tags, new ParallelOptions { MaxDegreeOfParallelism = 7 }, async (tag, cancellationToken) =>
            {
                HttpResponseMessage response = await HttpHelpers.SendRequestRaw($"https://api.waifu.im/search/?included_tags={tag}&many=true", HttpMethod.Get);
                WaifuIm images = JsonConvert.DeserializeObject<WaifuIm>(await response.Content.ReadAsStringAsync());

                await Parallel.ForEachAsync(images.images, new ParallelOptions { MaxDegreeOfParallelism = 3 }, async (image, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException(nameof(Waifu));

                    await _redisRepository.Set(image.image_id.ToString(), JsonConvert.SerializeObject(image));
                });
            });
        }
    }
}
