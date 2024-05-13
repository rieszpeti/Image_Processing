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

        [Fact]
        public async Task ImageProcessEndpoint_TestShouldFail_SendBadImage()
        {
            // Arrange
            var cancellationToken = new CancellationToken(canceled: true);

            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
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

        //[Fact]
        //public async Task MapImageProcessingEndpoints_UnhandledException_ReturnsProblemResultWithTitle()
        //{
        //    // Arrange
        //    __mockLogger = new Mock<ILogger>();
        //    var _mockImageProcessingService = new Mock<IImageProcessingService>();
        //    _mockImageProcessingService.Setup(x => x.ProcessImage(It.IsAny<IFormFile>(), It.IsAny<System.Threading.CancellationToken>())).Throws(new Exception("Something went wrong"));

        //    var mockFormFile = new Mock<IFormFile>();

        //    var endpoint = new ImageProcessEndpoint();

        //    // Act
        //    var result = await ExecuteRequest(endpoint, _mockLogger.Object, mockFormFile.Object, _mockImageProcessingService.Object);

        //    // Assert
        //    Assert.IsType<ObjectResult>(result);
        //    var problemResult = result as ObjectResult;
        //    Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        //    Assert.Equal("An error occurred while processing the image.", problemResult.Value);
        //}

        //private async Task<IActionResult> ExecuteRequest(ImageProcessEndpoint endpoint, ILogger logger, IFormFile file, IImageProcessingService service, System.Threading.CancellationToken ct = default)
        //{
        //    IActionResult result = null;
        //    var context = new DefaultHttpContext();
        //    var responseStream = new System.IO.MemoryStream();
        //    context.Response.Body = responseStream;

        //    var endpointRouteBuilder = Microsoft.AspNetCore.Routing.EndpointRouteBuilderExtensions.New();
        //    endpoint.MapImageProcessingEndpoints(endpointRouteBuilder);
        //    var requestDelegate = endpointRouteBuilder.Build();

        //    context.RequestServices = new ServiceCollection()
        //        .AddSingleton(logger)
        //        .AddSingleton(service)
        //        .BuildServiceProvider();

        //    context.Request.Path = "/imageProcess";
        //    context.Request.Method = "POST";
        //    context.Request.Form = new FormCollection(new FormFileCollection { file });

        //    context.RequestAborted = ct;

        //    await requestDelegate(context);

        //    responseStream.Seek(0, System.IO.SeekOrigin.Begin);
        //    using (var reader = new System.IO.StreamReader(responseStream))
        //    {
        //        var responseBody = await reader.ReadToEndAsync();
        //        if (!string.IsNullOrEmpty(responseBody))
        //        {
        //            result = new ContentResult
        //            {
        //                Content = responseBody,
        //                ContentType = "text/plain",
        //                StatusCode = context.Response.StatusCode
        //            };
        //        }
        //    }

        //    return result;
        //}
    }
}