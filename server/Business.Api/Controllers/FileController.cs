using Business.Domain.Interfaces.Services;
using Business.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/file")]
    public class FileControler : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileControler(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("verify-if-bucket-exists")]
        public async Task<IActionResult> VerifyIfBucketExists([FromBody] FileDTO dto)
            => Ok(await _fileService.VerifyIfBucketExists(dto.Bucket));


        [HttpPut("update-file")]
        public async Task<IActionResult> UpdateFile([FromBody] FileDTO dto)
        {
            await _fileService.UpdateFile(dto.Bucket, dto.FilePath, dto.Obj);
            return Ok();
        }

        [HttpPut("copy-file")]
        public async Task<IActionResult> CopyFile([FromBody] FileDTO dto)
        {
            await _fileService.CopyFile(dto.FromBucket, dto.FromObj, dto.ToBucket, dto.ToObj);
            return Ok();
        }

        [HttpPost("create-bucket")]
        public async Task<IActionResult> CreateBucket([FromBody] FileDTO dto)
        {
            await _fileService.CreateBucket(dto.Bucket);
            return Ok();
        }

        [HttpDelete("delete-bucket")]
        public async Task<IActionResult> DeleteBucket([FromBody] FileDTO dto)
        {
            await _fileService.DeleteBucket(dto.Bucket);
            return Ok();
        }

        [HttpGet("get-files-in-bucket-by-prefix")]
        public IActionResult GetFilesInBucketByPrefix([FromBody] FileDTO dto)
        {
            _fileService.GetFilesInBucketByPrefix(dto.Bucket, dto.Prefix);
            return Ok();
        }

        [HttpGet("get-bucket-notifications")]
        public async Task<IActionResult> GetBucketNotifications([FromBody] FileDTO dto)
            => Ok(await _fileService.GetBucketNotifications(dto.Bucket));

        [HttpGet("get-all-buckets")]
        public async Task<IActionResult> GetAllBuckets([FromBody] FileDTO dto)
            => Ok(await _fileService.GetAllBuckets());

        [HttpGet("listen-incomplete-uploads")]
        public IActionResult ListenIncompleteUploads([FromBody] FileDTO dto)
        {
            _fileService.ListenIncompleteUploads(dto.Bucket, dto.Prefix);
            return Ok();
        }

        [HttpGet("listen-bucket-notifications")]
        public IActionResult ListenBucketNotifications([FromBody] FileDTO dto)
        {
            _fileService.ListenBucketNotifications(dto.Bucket, dto.Events, dto.Prefix, dto.Suffix, dto.Recursive);
            return Ok();
        }

        [HttpDelete("remove-all-bucket-notifications")]
        public async Task<IActionResult> RemoveAllBucketNotifications([FromBody] FileDTO dto)
        {
            await _fileService.RemoveAllBucketNotifications(dto.Bucket);
            return Ok();
        }

        [HttpDelete("remove-incomplete-notifications")]
        public async Task<IActionResult> RemoveIncompleteUpload([FromBody] FileDTO dto)
        {
            await _fileService.RemoveAllBucketNotifications(dto.Bucket);
            return Ok();
        }

        [HttpDelete("remove-object")]
        public async Task<IActionResult> RemoveObject([FromBody] FileDTO dto)
        {
            await _fileService.RemoveObject(dto.Bucket, dto.Obj, dto.VersionId);
            return Ok();
        }

        [HttpDelete("remove-objects")]
        public async Task<IActionResult> RemoveObjects([FromBody] FileDTO dto)
        {
            await _fileService.RemoveObjects(dto.Bucket, dto.Objs);
            return Ok();
        }
    }
}
