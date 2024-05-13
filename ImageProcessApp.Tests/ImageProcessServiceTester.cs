using Application.CSharp.Interfaces;
using Application.CSharp.Models;
using Application.CSharp.ModelValidation;
using Application.Interfaces;
using Image_Processing_Backend.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Application.Models;
using Application.Services;
using System.Runtime.InteropServices;

namespace ImageProcessApp.Tests
{
    public class ImageProcessServiceTester
    {
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private readonly Mock<IImageValidator> _mockImageValidator = new Mock<IImageValidator>();

        /// <summary>
        /// Copy TestImages folder to debug folder to the Webapi for test
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImageProcessService_TestShouldPass_SendGoodRealImage()
        {
            //Arrange
            var cancellationToken = new CancellationToken(canceled: false);

            var currentDir = Directory.GetCurrentDirectory();
            var dirPath = Path.Combine(currentDir, "TestImages");

            var files = new List<IFormFile>();
            using var stream = new MemoryStream();

            if (Directory.Exists(dirPath))
            {
                var filePaths = Directory.GetFiles(dirPath);


                foreach (var path in filePaths)
                {
                    if (System.Enum.IsDefined(typeof(EncodingType), Path.GetExtension(path)))
                    {
                        stream.ReadAsync(File.ReadAllBytes(path).ToArray());
                        var formFile = new FormFile(stream, 0, stream.Length, "streamFile", Path.GetFileName(path));
                        files.Add(formFile);
                    }
                }

                var service = new ImageProcessingService(_mockLogger.Object, _mockImageValidator.Object);

                foreach (var file in files)
                {
                    // Arrange
                    await file.CopyToAsync(stream);
                    var expected = new ImageProcessResponse { Bytes = stream.ToArray(), FileExtension = Path.GetExtension(file.FileName) };

                    // Act
                    var actual = await service.ProcessImage(file, cancellationToken);

                    // Assert
                    Assert.Equal(expected, actual);
                }
            }
            else
            {
                throw new Exception("Copy TestImages folder to debug folder in the WebApi project!!!");
            }
        }
    }
}
