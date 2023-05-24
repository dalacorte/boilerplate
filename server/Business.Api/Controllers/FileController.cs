using Business.Domain.Interfaces.Services;
using Business.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    ///<Summary>
    /// File Controller :)
    ///</Summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/file")]
    public class FileControler : ControllerBase
    {
        private readonly IFileService _fileService;

        ///<Summary>
        /// Constructor
        ///</Summary>
        public FileControler(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Verifies if a bucket exists.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>True if the bucket exists, otherwise false.</returns>
        [HttpGet("verify-if-bucket-exists")]
        public async Task<IActionResult> VerifyIfBucketExists([FromBody] FileDTO dto)
            => Ok(await _fileService.VerifyIfBucketExists(dto.Bucket));

        /// <summary>
        /// Updates a file in the specified bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the file was updated successfully.</returns>
        [HttpPut("update-file")]
        public async Task<IActionResult> UpdateFile([FromBody] FileDTO dto)
        {
            await _fileService.UpdateFile(dto.Bucket, dto.FilePath, dto.Obj);
            return Ok();
        }

        /// <summary>
        /// Copies a file from one bucket to another.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the file was copied successfully.</returns>
        [HttpPut("copy-file")]
        public async Task<IActionResult> CopyFile([FromBody] FileDTO dto)
        {
            await _fileService.CopyFile(dto.FromBucket, dto.FromObj, dto.ToBucket, dto.ToObj);
            return Ok();
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the bucket was created successfully.</returns>
        [HttpPost("create-bucket")]
        public async Task<IActionResult> CreateBucket([FromBody] FileDTO dto)
        {
            await _fileService.CreateBucket(dto.Bucket);
            return Ok();
        }

        /// <summary>
        /// Deletes a bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the bucket was deleted successfully.</returns>
        [HttpDelete("delete-bucket")]
        public async Task<IActionResult> DeleteBucket([FromBody] FileDTO dto)
        {
            await _fileService.DeleteBucket(dto.Bucket);
            return Ok();
        }

        /// <summary>
        /// Gets a list of files in a bucket with a specific prefix.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the operation was successful.</returns>
        [HttpGet("get-files-in-bucket-by-prefix")]
        public IActionResult GetFilesInBucketByPrefix([FromBody] FileDTO dto)
        {
            _fileService.GetFilesInBucketByPrefix(dto.Bucket, dto.Prefix);
            return Ok();
        }

        /// <summary>
        /// Gets the notifications configured for a bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>The bucket notifications.</returns>
        [HttpGet("get-bucket-notifications")]
        public async Task<IActionResult> GetBucketNotifications([FromBody] FileDTO dto)
            => Ok(await _fileService.GetBucketNotifications(dto.Bucket));

        /// <summary>
        /// Gets all the buckets.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>The list of buckets.</returns>
        [HttpGet("get-all-buckets")]
        public async Task<IActionResult> GetAllBuckets([FromBody] FileDTO dto)
            => Ok(await _fileService.GetAllBuckets());

        /// <summary>
        /// Listens for incomplete uploads in a bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the operation was successful.</returns>
        [HttpGet("listen-incomplete-uploads")]
        public IActionResult ListenIncompleteUploads([FromBody] FileDTO dto)
        {
            _fileService.ListenIncompleteUploads(dto.Bucket, dto.Prefix);
            return Ok();
        }

        /// <summary>
        /// Listens for bucket notifications.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the operation was successful.</returns>
        [HttpGet("listen-bucket-notifications")]
        public IActionResult ListenBucketNotifications([FromBody] FileDTO dto)
        {
            _fileService.ListenBucketNotifications(dto.Bucket, dto.Events, dto.Prefix, dto.Suffix, dto.Recursive);
            return Ok();
        }

        /// <summary>
        /// Removes all bucket notifications.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the operation was successful.</returns>
        [HttpDelete("remove-all-bucket-notifications")]
        public async Task<IActionResult> RemoveAllBucketNotifications([FromBody] FileDTO dto)
        {
            await _fileService.RemoveAllBucketNotifications(dto.Bucket);
            return Ok();
        }

        /// <summary>
        /// Removes an incomplete upload.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the operation was successful.</returns>
        [HttpDelete("remove-incomplete-notifications")]
        public async Task<IActionResult> RemoveIncompleteUpload([FromBody] FileDTO dto)
        {
            await _fileService.RemoveAllBucketNotifications(dto.Bucket);
            return Ok();
        }

        /// <summary>
        /// Removes an object from a bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the object was removed successfully.</returns>
        [HttpDelete("remove-object")]
        public async Task<IActionResult> RemoveObject([FromBody] FileDTO dto)
        {
            await _fileService.RemoveObject(dto.Bucket, dto.Obj, dto.VersionId);
            return Ok();
        }

        /// <summary>
        /// Removes multiple objects from a bucket.
        /// </summary>
        /// <param name="dto">The file data.</param>
        /// <returns>OK if the objects were removed successfully.</returns>
        [HttpDelete("remove-objects")]
        public async Task<IActionResult> RemoveObjects([FromBody] FileDTO dto)
        {
            await _fileService.RemoveObjects(dto.Bucket, dto.Objs);
            return Ok();
        }
    }
}
