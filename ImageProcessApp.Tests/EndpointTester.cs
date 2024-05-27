using Application.CSharp.Models;
using Application.Interfaces;
using Application.Models;
using Google.Protobuf.WellKnownTypes;
using Image_Processing_Backend.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Reflection;

namespace ImageProcessApp.Tests
{
    public class EndpointTester
    {
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private readonly Mock<IImageProcessingService> _mockImageProcessingService = new Mock<IImageProcessingService>();


        /// <summary>
        /// Tests the ProcessImage method of the ImageProcessEndpoint to ensure it fails correctly
        /// when an invalid image file is provided.
        /// </summary>
        /// <remarks>
        /// This test simulates a scenario where a non-image file (e.g., a PDF) is sent to the image processing
        /// endpoint. It verifies that the endpoint returns a ProblemHttpResult, indicating an error response.
        /// </remarks>
        [Fact]
        public async Task ImageProcessEndpoint_TestShouldFail_SendBadImage()
        {
            // Arrange
            var cancellationToken = new CancellationToken(canceled: true);

            //Setup mock file using a memory stream
            var content = "Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            // Act
            var result = await ImageProcessEndpoint.ProcessImage(_mockLogger.Object, file, _mockImageProcessingService.Object, cancellationToken);

            // Assert
            Assert.IsType<ProblemHttpResult>(result);
        }

        /// <summary>
        /// Tests the ProcessImage method of the ImageProcessEndpoint to ensure it processes a valid image file
        /// correctly and returns the expected response.
        /// </summary>
        /// <remarks>
        /// This test simulates a scenario where a valid image file is sent to the image processing
        /// endpoint. It verifies that the endpoint returns a FileContentHttpResult, indicating a successful
        /// processing of the image.
        /// </remarks>
        [Fact]
        public async Task ImageProcessEndpoint_TestShouldPass_SendImage()
        {
            // Arrange
            var cancellationToken = new CancellationToken(canceled: false);

            var stream = new MemoryStream();
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", "fileName.png");
            var fileBytes = stream.ToArray();

            _mockImageProcessingService
                .Setup(x => x.ProcessImage(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ImageProcessResponse { Bytes = new byte[1], FileExtension = ".png" }));

            // Act
            var result = await ImageProcessEndpoint.ProcessImage(_mockLogger.Object, file, _mockImageProcessingService.Object);

            // Assert
            Assert.IsType<FileContentHttpResult>(result);
        }
    }
}