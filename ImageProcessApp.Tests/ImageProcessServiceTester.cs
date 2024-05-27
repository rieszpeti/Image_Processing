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
        /// Tests the ProcessImage method with valid image files.
        /// You have to add both of the images to the InputTestImages and OutputTestImages folder with the same name
        /// Must contain at least 1-1 pictures in the folders
        /// </summary>
        /// <exception cref="Exception">Thrown if the InputTestImages and OutputTestImages folder is not found.</exception>
        [Fact]
        public async Task ImageProcessService_TestShouldPass_SendGoodRealImage()
        {
            //Arrange 
            //copy images to directory to pass images to image processing
            var cancellationToken = new CancellationToken(canceled: false);

            var currentDir = Directory.GetCurrentDirectory();
            var inputDirPath = Path.Combine(currentDir, "InputTestImages");
            var outputDirPath = Path.Combine(currentDir, "OutputTestImages");

            //folder check and file count check
            if (CheckDirectories(inputDirPath, outputDirPath))
            {
                //Get service
                var service = new ImageProcessingService(_mockLogger.Object, _mockImageValidator.Object);

                var stream = new MemoryStream();

                var inputFilePaths = Directory.GetFiles(inputDirPath);
                var outputFilePaths = Directory.GetFiles(outputDirPath);

                if (CheckIfAtLeastOneFileNameMatch(inputFilePaths, outputFilePaths))
                {
                    throw new Exception("Must contain at least 1-1 pictures in the folders with the same name");
                }

                foreach (var path in inputFilePaths)
                {
                    var inputFileName = Path.GetFileName(path);

                    var file = outputFilePaths.FirstOrDefault(f => Path.GetFileName(f) == inputFileName);

                    byte[] outputFile;

                    if (file is null)
                    {
                        break;
                        //imagename has to match
                    }
                    else
                    {
                        outputFile = File.ReadAllBytes(file);
                    }
                    
                    var extension = Path.GetExtension(path);

                    if (Enum.TryParse(typeof(EncodingType), extension.TrimStart('.').ToUpper(), true, out var _))
                    {
                        var fileBytes = File.ReadAllBytes(path);

                        stream = new MemoryStream(fileBytes);
                        stream.Position = 0;

                        var formFile = new FormFile(stream, 0, stream.Length, "file", inputFileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = $"image/{extension.Trim('.')}",
                            ContentDisposition = $"""form-data; name=\"file\";filename=\"{inputFileName}\""",
                        };
                       
                        var expected = new ImageProcessResponse { Bytes = outputFile, FileExtension = extension.Replace(".", "") };

                        // Act
                        var actual = await service.ProcessImage(formFile, cancellationToken);

                        // Assert
                        Assert.Equal(expected.Bytes, actual.Bytes);
                        Assert.Equal(expected.FileExtension, actual.FileExtension);
                    }
                }

                stream.Close();
                stream.Dispose();
            }
            else
            {
                throw new Exception("Copy TestImages folder to debug folder in the WebApi project!!!");
            }
        }

        private bool CheckDirectories(string inputDirPath, string outputDirPath)
        {
            return Directory.Exists(inputDirPath) &&
                Directory.Exists(outputDirPath) &&
                Directory.GetFiles(inputDirPath).Length != 0 &&
                Directory.GetFiles(outputDirPath).Length != 0;
        }

        private bool CheckIfAtLeastOneFileNameMatch(string[] inputFilePaths, string[] outputFilePaths)
        {
            foreach (var inputFile in inputFilePaths)
            {
                var inputFileName = Path.GetFileName(inputFile);

                foreach (var outputFile in outputFilePaths)
                {
                    var outputFileName = Path.GetFileName(outputFile);

                    if (string.Equals(inputFileName, outputFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
